

using System;
using System.Collections;
using System.Collections.Concurrent;
using Bytom.Tools;

namespace Bytom.Hardware.CPU
{
    public class MicroOp
    {
        public virtual IEnumerable execute(Core core)
        {
            yield return null;
        }
    }

    public class MemoryIO : MicroOp
    {
        public Register storage;
        public Register32 address;
        public MemoryIO(Register storage, Register32 address)
        {
            this.storage = storage;
            this.address = address;
        }
    }
    public class ReadMemory : MemoryIO
    {
        public ReadMemory(Register destination, Register32 address) : base(destination, address)
        { }
        public override IEnumerable execute(Core core)
        {
            var address_value = address.readAddress();
            var register_size = storage.getSizeBytes();
            byte[] buffer = new byte[register_size];

            ConcurrentQueue<WriteMessage> writeBackQueue = new ConcurrentQueue<WriteMessage>();

            for (int i = 0; i < register_size; i++)
            {
                core.GetMemoryController().pushIoMessage(new ReadMessage(address_value + i, writeBackQueue));
            }
            var bytes_read = 0;

            while (bytes_read < register_size)
            {
                if (writeBackQueue.TryDequeue(out var message))
                {
                    var offset = message.address - address_value;
                    buffer[offset.ToLong()] = message.data;
                    bytes_read++;
                }
                yield return null;
            }
            yield break;
        }
    }

    public class WriteMemory : MemoryIO
    {
        public WriteMemory(Register32 address, Register source) : base(source, address)
        { }
        public override IEnumerable execute(Core core)
        {
            var address_value = address.readAddress();
            var register_size = storage.getSizeBytes();
            byte[] buffer = storage.readBytes();

            for (int i = 0; i < register_size; i++)
            {
                core.GetMemoryController().pushIoMessage(new WriteMessage(address_value + i, buffer[i]));
            }
            yield break;
        }
    }

    public class InstructionDecode : MicroOp
    {
        public Register32 source;
        public InstructionDecode(Register32 source)
        {
            this.source = source;
        }
        public override IEnumerable execute(Core core)
        {
            var decoder = new InstructionDecoder(source.readBytes());
            updateInstructionPointer(core, 4u);

            Register? first = null;
            Register? second = null;

            if (decoder.GetFirstRegisterID() != RegisterID.NO_REGISTER)
                first = core.registers[decoder.GetFirstRegisterID()];

            if (decoder.GetSecondRegisterID() != RegisterID.NO_REGISTER)
                second = core.registers[decoder.GetSecondRegisterID()];

            switch (decoder.GetOpCode())
            {
                case OpCode.Nop:
                    pushContinueExecution(core);
                    break;

                case OpCode.Halt:
                    core.requested_power_off = true;
                    core.power_status = PowerStatus.STOPPING;
                    break;

                case OpCode.MovRegReg:
                    core.pushMicroOp(
                        new CopyRegister(
                            destination: first!,
                            source: second!
                        )
                    );
                    pushContinueExecution(core);
                    break;

                case OpCode.MovRegMem:
                    core.pushMicroOp(
                        new ReadMemory(
                            destination: first!,
                            address: (Register32)second!
                        )
                    );
                    pushContinueExecution(core);
                    break;

                case OpCode.MovMemReg:
                    core.pushMicroOp(
                        new WriteMemory(
                            address: (Register32)first!,
                            source: second!
                        )
                    );
                    pushContinueExecution(core);
                    break;

                case OpCode.MovRegCon:
                    readInstructionConstant(core, core.vRD0);
                    core.pushMicroOp(
                        new CopyRegister(
                            destination: first!,
                            source: core.vRD0
                        )
                    );
                    pushContinueExecution(core);
                    break;

                case OpCode.MovMemCon:
                    readInstructionConstant(core, core.vRD0);
                    core.pushMicroOp(
                        new WriteMemory(
                            address: (Register32)first!,
                            source: core.vRD0
                        )
                    );
                    pushContinueExecution(core);
                    break;

                case OpCode.PushCon:
                    readInstructionConstant(core, core.vRD0);
                    pushStackValue(core, core.vRD0);
                    pushContinueExecution(core);
                    break;

                case OpCode.PushReg:
                    pushStackValue(core, first!);
                    pushContinueExecution(core);
                    break;

                case OpCode.PushMem:
                    core.pushMicroOp(
                        new ReadMemory(
                            destination: core.vRD0,
                            address: (Register32)first!
                        )
                    );
                    pushStackValue(core, core.vRD0);
                    pushContinueExecution(core);
                    break;

                case OpCode.PopReg:
                    popStackValue(core, first!);
                    pushContinueExecution(core);
                    break;

                case OpCode.PopMem:
                    popStackValue(core, core.vRD0!);
                    core.pushMicroOp(
                        new WriteMemory(
                            address: (Register32)first!,
                            source: core.vRD0
                        )
                    );
                    pushContinueExecution(core);
                    break;

                case OpCode.Add:
                    ALUOperationType alu_operation = ALUOperationType.ADD;
                    goto ALU;
                case OpCode.Sub:
                    alu_operation = ALUOperationType.SUB;
                    goto ALU;
                case OpCode.Mul:
                    alu_operation = ALUOperationType.UNSIGNED_MUL;
                    goto ALU;
                case OpCode.IMul:
                    alu_operation = ALUOperationType.SIGNED_MUL;
                    goto ALU;
                case OpCode.Div:
                    alu_operation = ALUOperationType.UNSIGNED_DIV;
                    goto ALU;
                case OpCode.IDiv:
                    alu_operation = ALUOperationType.SIGNED_DIV;
                    goto ALU;
                case OpCode.And:
                    alu_operation = ALUOperationType.AND;
                    goto ALU;
                case OpCode.Or:
                    alu_operation = ALUOperationType.OR;
                    goto ALU;
                case OpCode.Xor:
                    alu_operation = ALUOperationType.XOR;
                    goto ALU;
                case OpCode.Shl:
                    alu_operation = ALUOperationType.SHL;
                    goto ALU;
                case OpCode.Shr:
                    alu_operation = ALUOperationType.SHR;
                    goto ALU;
                ALU:
                    core.pushMicroOp(
                        new ALUOperation32(
                            operation: alu_operation,
                            left: (Register32)first!,
                            right: (Register32)second!,
                            ccr: core.CCR
                        )
                    );
                    pushContinueExecution(core);
                    break;

                case OpCode.Cmp:
                    core.pushMicroOp(
                        new CopyRegister(
                            destination: core.vRD0,
                            source: first!
                        )
                    );
                    core.pushMicroOp(
                        new ALUOperation32(
                            operation: ALUOperationType.SUB,
                            left: core.vRD0!,
                            right: (Register32)second!,
                            ccr: core.CCR
                        )
                    );
                    pushContinueExecution(core);
                    break;

                case OpCode.JmpMem:
                    updateInstructionPointer(core, (Register32)first!);
                    pushContinueExecution(core);
                    break;

                case OpCode.JmpCon:
                    readInstructionConstant(core, core.vRD0);
                    updateInstructionPointer(core, core.vRD0);
                    pushContinueExecution(core);
                    break;

                case OpCode.JeqMem:
                    Func<ConditionCodeRegister, bool> condition = (ccr) => ccr.isEqual();
                    goto JMP_MEM;
                case OpCode.JneMem:
                    condition = (ccr) => ccr.isNotEqual();
                    goto JMP_MEM;
                case OpCode.JaMem:
                    condition = (ccr) => ccr.isAbove();
                    goto JMP_MEM;
                case OpCode.JaeMem:
                    condition = (ccr) => ccr.isAboveOrEqual();
                    goto JMP_MEM;
                case OpCode.JbMem:
                    condition = (ccr) => ccr.isBelow();
                    goto JMP_MEM;
                case OpCode.JbeMem:
                    condition = (ccr) => ccr.isBelowOrEqual();
                    goto JMP_MEM;
                case OpCode.JltMem:
                    condition = (ccr) => ccr.isLessThan();
                    goto JMP_MEM;
                case OpCode.JleMem:
                    condition = (ccr) => ccr.isLessThanOrEqual();
                    goto JMP_MEM;
                case OpCode.JgtMem:
                    condition = (ccr) => ccr.isGreaterThan();
                    goto JMP_MEM;
                case OpCode.JgeMem:
                    condition = (ccr) => ccr.isGreaterThanOrEqual();
                    goto JMP_MEM;
                JMP_MEM:
                    core.pushMicroOp(
                        new JmpIfCondition(
                            address: (Register32)first!,
                            ccr: core.CCR,
                            condition: (ccr) => ccr.isEqual()
                        )
                    );
                    pushContinueExecution(core);
                    break;

                case OpCode.JeqCon:
                    condition = (ccr) => ccr.isEqual();
                    goto JMP_CON;
                case OpCode.JneCon:
                    condition = (ccr) => ccr.isNotEqual();
                    goto JMP_CON;
                case OpCode.JaCon:
                    condition = (ccr) => ccr.isAbove();
                    goto JMP_CON;
                case OpCode.JaeCon:
                    condition = (ccr) => ccr.isAboveOrEqual();
                    goto JMP_CON;
                case OpCode.JbCon:
                    condition = (ccr) => ccr.isBelow();
                    goto JMP_CON;
                case OpCode.JbeCon:
                    condition = (ccr) => ccr.isBelowOrEqual();
                    goto JMP_CON;
                case OpCode.JltCon:
                    condition = (ccr) => ccr.isLessThan();
                    goto JMP_CON;
                case OpCode.JleCon:
                    condition = (ccr) => ccr.isLessThanOrEqual();
                    goto JMP_CON;
                case OpCode.JgtCon:
                    condition = (ccr) => ccr.isGreaterThan();
                    goto JMP_CON;
                case OpCode.JgeCon:
                    condition = (ccr) => ccr.isGreaterThanOrEqual();
                    goto JMP_CON;
                JMP_CON:
                    readInstructionConstant(core, core.vRD0);
                    core.pushMicroOp(
                        new JmpIfCondition(
                            address: core.vRD0,
                            ccr: core.CCR,
                            condition: (ccr) => ccr.isEqual()
                        )
                    );
                    pushContinueExecution(core);
                    break;

                case OpCode.CallMem:
                    pushStackValue(core, core.IP);
                    updateInstructionPointer(core, (Register32)first!);
                    pushContinueExecution(core);
                    break;

                case OpCode.CallCon:
                    readInstructionConstant(core, core.vRD0);
                    pushStackValue(core, core.IP);
                    updateInstructionPointer(core, core.vRD0);
                    pushContinueExecution(core);
                    break;

                case OpCode.Ret:
                    popStackValue(core, core.IP);
                    pushContinueExecution(core);
                    break;

                default:
                    throw new NotImplementedException(decoder.GetOpCode().ToString());
            }
            yield break;
        }

        private static void pushStackValue(Core core, Register source)
        {
            core.pushMicroOp(
                new WriteMemory(
                    address: core.STP,
                    source: source
                )
            );
            core.pushMicroOp(
                new WriteRegister(
                    destination: core.vRD1,
                    source: Serialization.UInt32ToBytesBigEndian(source.getSizeBytes())
                )
            );
            core.pushMicroOp(
                new ALUOperation32(
                    operation: ALUOperationType.ADD,
                    left: core.STP,
                    right: core.vRD1
                )
            );
        }

        private static void popStackValue(Core core, Register destination)
        {
            core.pushMicroOp(
                new ReadMemory(
                    address: core.STP,
                    destination: destination
                )
            );
            core.pushMicroOp(
                new WriteRegister(
                    destination: core.vRD1,
                    source: Serialization.UInt32ToBytesBigEndian(destination.getSizeBytes())
                )
            );
            core.pushMicroOp(
                new ALUOperation32(
                    operation: ALUOperationType.SUB,
                    left: core.STP,
                    right: core.vRD1
                )
            );
        }
        // Push ReadMemory operation to the core pipeline followed by update of the
        // instruction pointer.
        private static void readInstructionConstant(Core core, Register32 destination)
        {
            core.pushMicroOp(new ReadMemory(destination: destination, address: core.IP));
            updateInstructionPointer(core, 4u);
        }
        // Add `size` to the instruction pointer.
        private static void updateInstructionPointer(Core core, uint size)
        {
            core.pushMicroOp(
                new WriteRegister(
                    destination: core.vInstructionSize,
                    source: Serialization.UInt32ToBytesBigEndian(size)
                )
            );
            core.pushMicroOp(
                new ALUOperation32(
                    operation: ALUOperationType.ADD,
                    left: core.IP,
                    right: core.vInstructionSize
                )
            );
        }

        private static void updateInstructionPointer(Core core, Register32 new_value)
        {
            core.pushMicroOp(
                new CopyRegister(
                    destination: core.IP,
                    source: new_value
                )
            );
        }

        private void pushContinueExecution(Core core)
        {
            core.pushMicroOp(
                new ReadMemory(
                    destination: core.vInstruction,
                    address: core.IP
                )
            );
            core.pushMicroOp(
                new InstructionDecode(core.vInstruction)
            );
        }
    }
    public class CopyRegister : MicroOp
    {
        public Register destination;
        public Register source;
        public CopyRegister(Register destination, Register source)
        {
            this.destination = destination;
            this.source = source;
        }
        public override IEnumerable execute(Core core)
        {
            destination.writeBytes(source.readBytes());
            yield break;
        }
    }
    public class WriteRegister : MicroOp
    {
        public Register destination;
        public byte[] source;
        public WriteRegister(Register destination, byte[] source)
        {
            this.destination = destination;
            this.source = source;
        }
        public override IEnumerable execute(Core core)
        {
            destination.writeBytes(source);
            yield break;
        }
    }
    public class JmpIfCondition : MicroOp
    {
        public Register32 address;
        public ConditionCodeRegister ccr;
        public Func<ConditionCodeRegister, bool> condition;
        public JmpIfCondition(
            Register32 address,
            ConditionCodeRegister ccr,
            Func<ConditionCodeRegister, bool> condition
        )
        {
            this.address = address;
            this.ccr = ccr;
            this.condition = condition;
        }
        public override IEnumerable execute(Core core)
        {
            if (condition(ccr))
            {
                core.pushMicroOp(new CopyRegister(destination: core.IP, source: address));
            }
            yield return null;
        }
    }
}