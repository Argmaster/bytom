using Bytom.Assembler.Nodes;
using Bytom.Assembler.Operands;
using Bytom.Tools;

namespace Bytom.Hardware.CPU.Tests
{
    public class InstructionDecodeTests
    {
        Core? core;

        [SetUp]
        public void Setup()
        {
            core = new Core(0, 1000);
        }

        [Test(Description = "Test how nop instruction is converted into micro ops.")]
        public void TestDecodeNop()
        {
            var machine_code = new Nop().ToMachineCode();
            var instruction = new Register32(Serialization.UInt32FromBytesBigEndian(machine_code));
            var instruction_decode = new InstructionDecode(instruction);

            Itertools.exhaust(instruction_decode.execute(core!));

            var offset = 0;
            offset = checkUpdateInstructionPointer(offset);
            offset = checkContinueExecution(offset);

            Assert.That(core!.getPipeline().Count, Is.EqualTo(offset));
        }

        // Check that instruction at given offset match micro ops necessary to
        // update instruction pointer.
        private int checkUpdateInstructionPointer(int offset)
        {
            Assert.That(core!.getPipeline().microOps[offset], Is.TypeOf<WriteRegister>());
            var write_register = (WriteRegister)core.getPipeline().microOps[offset];
            Assert.That(write_register.destination.name, Is.EqualTo(core.vInstructionSize.name));
            Assert.That(write_register.source, Is.EqualTo(Serialization.UInt32ToBytesBigEndian(4)));

            offset += 1;

            Assert.That(core.getPipeline().microOps[offset], Is.TypeOf<ALUOperation32>());
            var alu_operation = (ALUOperation32)core.getPipeline().microOps[offset];
            Assert.That(alu_operation.operation, Is.EqualTo(ALUOperationType.ADD));
            Assert.That(alu_operation.left.name, Is.EqualTo(core.IP.name));
            Assert.That(alu_operation.right.name, Is.EqualTo(core.vInstructionSize.name));

            offset += 1;

            return offset;
        }

        // Check that instruction at given offset match micro ops necessary to
        // continue instruction execution.
        private int checkContinueExecution(int offset)
        {
            Assert.That(core!.getPipeline().microOps[offset], Is.TypeOf<ReadMemory>());
            var read_memory = (ReadMemory)core.getPipeline().microOps[offset];
            Assert.That(read_memory.storage.name, Is.EqualTo(core.vInstruction.name));
            Assert.That(read_memory.address.name, Is.EqualTo(core.IP.name));

            offset += 1;

            Assert.That(core.getPipeline().microOps[offset], Is.TypeOf<InstructionDecode>());
            var instruction_decode_next = (InstructionDecode)core.getPipeline().microOps[offset];
            Assert.That(instruction_decode_next.source.name, Is.EqualTo(core.vInstruction.name));

            offset += 1;

            return offset;
        }

        [Test(Description = "Test how move register into register is converted into micro ops.")]
        public void TestDecodeMovRegReg()
        {
            var machine_code = new MovRegReg(
                new OpRegister(RegisterID.RD0),
                new OpRegister(RegisterID.RD1)
            ).ToMachineCode();
            var instruction = new Register32(Serialization.UInt32FromBytesBigEndian(machine_code));
            var instruction_decode = new InstructionDecode(instruction);

            Itertools.exhaust(instruction_decode.execute(core!));

            var offset = 0;
            offset = checkUpdateInstructionPointer(offset);
            offset = checkCopyRegister(offset, core!.RD0, core.RD1);
            offset = checkContinueExecution(offset);

            Assert.That(core!.getPipeline().Count, Is.EqualTo(offset));
        }

        private int checkCopyRegister(int offset, Register dest, Register src)
        {
            Assert.That(core!.getPipeline().microOps[offset], Is.TypeOf<CopyRegister>());
            var copy_register = (CopyRegister)core.getPipeline().microOps[offset];
            Assert.That(copy_register.destination.name, Is.EqualTo(dest.name));
            Assert.That(copy_register.source.name, Is.EqualTo(src.name));

            offset += 1;

            return offset;
        }

        [Test(Description = "Test how move memory into register is converted into micro ops.")]
        public void TestMovRegMem()
        {
            var machine_code = new MovRegMem(
                new OpRegister(RegisterID.RD0),
                new OpMemoryAddress(RegisterID.RD1)
            ).ToMachineCode();
            var instruction = new Register32(Serialization.UInt32FromBytesBigEndian(machine_code));
            var instruction_decode = new InstructionDecode(instruction);

            Itertools.exhaust(instruction_decode.execute(core!));

            var offset = 0;
            offset = checkUpdateInstructionPointer(offset);
            offset = checkReadMemory(offset, core!.RD0, core.RD1);
            offset = checkContinueExecution(offset);
            Assert.That(core!.getPipeline().Count, Is.EqualTo(offset));
        }

        private int checkReadMemory(int offset, Register storage, Register address)
        {
            Assert.That(core!.getPipeline().microOps[offset], Is.TypeOf<ReadMemory>());
            var read_memory = (ReadMemory)core.getPipeline().microOps[offset];
            Assert.That(read_memory.storage.name, Is.EqualTo(storage.name));
            Assert.That(read_memory.address.name, Is.EqualTo(address.name));

            offset += 1;
            return offset;
        }

        [Test(Description = "Test how move register into memory is converted into micro ops.")]
        public void TestMovMemReg()
        {
            var machine_code = new MovMemReg(
                new OpMemoryAddress(RegisterID.RD0),
                new OpRegister(RegisterID.RD1)
            ).ToMachineCode();
            var instruction = new Register32(Serialization.UInt32FromBytesBigEndian(machine_code));
            var instruction_decode = new InstructionDecode(instruction);

            Itertools.exhaust(instruction_decode.execute(core!));

            var offset = 0;
            offset = checkUpdateInstructionPointer(offset);
            offset = checkWriteMemory(offset, core!.RD0, core.RD1);
            offset = checkContinueExecution(offset);

            Assert.That(core!.getPipeline().Count, Is.EqualTo(offset));
        }

        private int checkWriteMemory(int offset, Register address, Register storage)
        {
            Assert.That(core!.getPipeline().microOps[offset], Is.TypeOf<WriteMemory>());
            var write_memory = (WriteMemory)core.getPipeline().microOps[offset];
            Assert.That(write_memory.address.name, Is.EqualTo(address.name));
            Assert.That(write_memory.storage.name, Is.EqualTo(storage.name));

            offset += 1;
            return offset;
        }

        [Test(Description = "Test how move constant to register is converted into micro ops.")]
        public void TestMovRegCon()
        {
            var machine_code = new MovRegCon(
                new OpRegister(RegisterID.RD0),
                new OpConstantInt(1)
            ).ToMachineCode();
            var instruction = new Register32(Serialization.UInt32FromBytesBigEndian(
                machine_code.Take(4).ToArray()
            ));
            var instruction_decode = new InstructionDecode(instruction);

            Itertools.exhaust(instruction_decode.execute(core!));

            var offset = 0;
            offset = checkUpdateInstructionPointer(offset);
            offset = checkReadInstructionConstant(offset);

            Assert.That(core!.getPipeline().microOps[5], Is.TypeOf<CopyRegister>());
            var copy_register = (CopyRegister)core.getPipeline().microOps[5];
            Assert.That(copy_register.destination.name, Is.EqualTo(core.RD0.name));
            Assert.That(copy_register.source.name, Is.EqualTo(core.vRD0.name));

            offset += 1;

            offset = checkContinueExecution(offset);
            Assert.That(core!.getPipeline().Count, Is.EqualTo(offset));
        }

        private int checkReadInstructionConstant(int offset)
        {
            Assert.That(core!.getPipeline().microOps[offset], Is.TypeOf<ReadMemory>());
            var read_memory = (ReadMemory)core.getPipeline().microOps[offset];
            Assert.That(read_memory.storage.name, Is.EqualTo(core.vRD0.name));
            Assert.That(read_memory.address.name, Is.EqualTo(core.IP.name));

            offset = checkUpdateInstructionPointer(offset + 1);
            return offset;
        }

        [Test(Description = "Test how move constant to memory is converted into micro ops.")]
        public void TestMovMemCon()
        {
            var machine_code = new MovMemCon(
                new OpMemoryAddress(RegisterID.RD0),
                new OpConstantInt(1)
            ).ToMachineCode();
            var instruction = new Register32(Serialization.UInt32FromBytesBigEndian(
                machine_code.Take(4).ToArray()
            ));
            var instruction_decode = new InstructionDecode(instruction);

            Itertools.exhaust(instruction_decode.execute(core!));

            var offset = 0;
            offset = checkUpdateInstructionPointer(offset);
            offset = checkReadInstructionConstant(offset);

            Assert.That(core!.getPipeline().microOps[offset], Is.TypeOf<WriteMemory>());
            var write_memory = (WriteMemory)core.getPipeline().microOps[offset];
            Assert.That(write_memory.address.name, Is.EqualTo(core.RD0.name));
            Assert.That(write_memory.storage.name, Is.EqualTo(core.vRD0.name));

            offset += 1;

            offset = checkContinueExecution(offset);
            Assert.That(core!.getPipeline().Count, Is.EqualTo(offset));
        }

        [Test(Description = "Test how constant push is converted into micro ops.")]
        public void TestPushCon()
        {
            var machine_code = new PushCon(new OpConstantInt(1)).ToMachineCode();
            var instruction = new Register32(Serialization.UInt32FromBytesBigEndian(
                machine_code.Take(4).ToArray()
            ));
            var instruction_decode = new InstructionDecode(instruction);

            Itertools.exhaust(instruction_decode.execute(core!));

            var offset = 0;
            offset = checkUpdateInstructionPointer(offset);
            offset = checkReadInstructionConstant(offset);
            offset = checkPushStackValue(offset, core!.vRD0);
            offset = checkContinueExecution(offset);

            Assert.That(core!.getPipeline().Count, Is.EqualTo(offset));
        }

        // Check that instruction at given offset match micro ops necessary to
        // push constant value to stack.
        public int checkPushStackValue(int offset, Register write_reg)
        {
            Assert.That(core!.getPipeline().microOps[offset], Is.TypeOf<WriteMemory>());
            var write_memory = (WriteMemory)core.getPipeline().microOps[offset];
            Assert.That(write_memory.address.name, Is.EqualTo(core.STP.name));
            Assert.That(write_memory.storage.name, Is.EqualTo(write_reg.name));

            offset += 1;

            Assert.That(core.getPipeline().microOps[offset], Is.TypeOf<WriteRegister>());
            var write_register = (WriteRegister)core.getPipeline().microOps[offset];
            Assert.That(write_register.destination.name, Is.EqualTo(core.vRD1.name));
            Assert.That(write_register.source, Is.EqualTo(Serialization.Int32ToBytesBigEndian(4)));

            offset += 1;

            Assert.That(core.getPipeline().microOps[offset], Is.TypeOf<ALUOperation32>());
            var alu_operation = (ALUOperation32)core.getPipeline().microOps[offset];
            Assert.That(alu_operation.operation, Is.EqualTo(ALUOperationType.SUB));
            Assert.That(alu_operation.left.name, Is.EqualTo(core.STP.name));
            Assert.That(alu_operation.right.name, Is.EqualTo(core.vRD1.name));

            offset += 1;

            return offset;
        }

        [Test(Description = "Test how register push is converted into micro ops.")]
        public void TestPushReg()
        {
            var machine_code = new PushReg(new OpRegister(RegisterID.RD0)).ToMachineCode();
            var instruction = new Register32(Serialization.UInt32FromBytesBigEndian(machine_code));
            var instruction_decode = new InstructionDecode(instruction);

            Itertools.exhaust(instruction_decode.execute(core!));

            var offset = 0;
            offset = checkUpdateInstructionPointer(offset);
            offset = checkPushStackValue(offset, core!.RD0);
            offset = checkContinueExecution(offset);

            Assert.That(core!.getPipeline().Count, Is.EqualTo(offset));
        }

        [Test(Description = "Test how memory push is converted into micro ops.")]
        public void TestPushMem()
        {
            var machine_code = new PushMem(new OpMemoryAddress(RegisterID.RD0)).ToMachineCode();
            var instruction = new Register32(Serialization.UInt32FromBytesBigEndian(machine_code));
            var instruction_decode = new InstructionDecode(instruction);

            Itertools.exhaust(instruction_decode.execute(core!));

            var offset = 0;
            offset = checkUpdateInstructionPointer(offset);

            Assert.That(core!.getPipeline().microOps[offset], Is.TypeOf<ReadMemory>());
            var read_memory = (ReadMemory)core.getPipeline().microOps[offset];
            Assert.That(read_memory.storage, Is.EqualTo(core.vRD0));
            Assert.That(read_memory.address, Is.EqualTo(core.RD0));

            offset += 1;

            offset = checkPushStackValue(offset, core!.vRD0);
            offset = checkContinueExecution(offset);

            Assert.That(core!.getPipeline().Count, Is.EqualTo(offset));
        }
    }
}