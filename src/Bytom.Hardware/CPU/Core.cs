using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bytom.Hardware.RAM;
using Bytom.Tools;

namespace Bytom.Hardware.CPU
{
    public enum RegisterID
    {
        NO_REGISTER = 0b00_0000,
        RD0 = 0b00_0001,
        RD1 = 0b00_0010,
        RD2 = 0b00_0011,
        RD3 = 0b00_0100,
        RD4 = 0b00_0101,
        RD5 = 0b00_0110,
        RD6 = 0b00_0111,
        RD7 = 0b00_1000,
        RD8 = 0b00_1001,
        RD9 = 0b00_1010,
        RDA = 0b00_1011,
        RDB = 0b00_1100,
        RDC = 0b00_1101,
        RDD = 0b00_1110,
        RDE = 0b00_1111,
        RDF = 0b01_0000,
        CCR = 0b01_1111,
        CR0 = 0b10_0000,
        STP = 0b10_0100,
        FBP = 0b10_0101,
        VATTA = 0b10_0110,
        IDT = 0b10_0111,
        IRA = 0b10_1000,
        IP = 0b10_1001,
        TRA = 0b10_1010, // Task Descriptor Address (address of currently running task)
        TDTA = 0b10_1011, // Task Descriptor Table Address
        KERNEL_STP = 0b11_1010,
        KERNEL_FBP = 0b11_1011,
        KERNEL_IP = 0b11_1100,
    }

    public class Core
    {
        public uint core_id { get; }
        public uint clock_speed_hz { get; }
        public PowerStatus power_status;
        public bool requested_power_off;
        public Clock clock { get; }
        public Package? package;
        private Thread? thread;

        public Register32 RD0;
        public Register32 RD1;
        public Register32 RD2;
        public Register32 RD3;
        public Register32 RD4;
        public Register32 RD5;
        public Register32 RD6;
        public Register32 RD7;
        public Register32 RD8;
        public Register32 RD9;
        public Register32 RDA;
        public Register32 RDB;
        public Register32 RDC;
        public Register32 RDD;
        public Register32 RDE;
        public Register32 RDF;

        public ConditionCodeRegister CCR;
        public Register32 CR0;
        public Register32 STP;
        public Register32 FBP;
        public Register32 VATTA;
        public Register32 IDT;
        public Register32 IRA;
        public Register32 IP;
        public Register32 TRA;
        public Register32 TDTA;
        public Register32 KERNEL_STP;
        public Register32 KERNEL_FBP;
        public Register32 KERNEL_IP;

        public Dictionary<RegisterID, Register32> registers;

        public Core(
            uint core_id,
            uint clock_speed_hz
        )
        {
            this.core_id = core_id;
            this.clock_speed_hz = clock_speed_hz;

            power_status = PowerStatus.OFF;
            clock = new Clock(clock_speed_hz);

            RD0 = new Register32(0);
            RD1 = new Register32(0);
            RD2 = new Register32(0);
            RD3 = new Register32(0);
            RD4 = new Register32(0);
            RD5 = new Register32(0);
            RD6 = new Register32(0);
            RD7 = new Register32(0);
            RD8 = new Register32(0);
            RD9 = new Register32(0);
            RDA = new Register32(0);
            RDB = new Register32(0);
            RDC = new Register32(0);
            RDD = new Register32(0);
            RDE = new Register32(0);
            RDF = new Register32(0);

            CCR = new ConditionCodeRegister();
            CR0 = new Register32(0b10000000_00000000_00000000_00000000);
            STP = new Register32(0);
            FBP = new Register32(0);
            VATTA = new Register32(0);
            IDT = new Register32(0);
            IRA = new Register32(0);
            IP = new Register32(0);
            TRA = new Register32(0);
            TDTA = new Register32(0);
            KERNEL_STP = new Register32(0);
            KERNEL_FBP = new Register32(0);
            KERNEL_IP = new Register32(0);


            registers = new Dictionary<RegisterID, Register32>{
                { RegisterID.RD0, RD0 },
                { RegisterID.RD1, RD1 },
                { RegisterID.RD2, RD2 },
                { RegisterID.RD3, RD3 },
                { RegisterID.RD4, RD4 },
                { RegisterID.RD5, RD5 },
                { RegisterID.RD6, RD6 },
                { RegisterID.RD7, RD7 },
                { RegisterID.RD8, RD8 },
                { RegisterID.RD9, RD9 },
                { RegisterID.RDA, RDA },
                { RegisterID.RDB, RDB },
                { RegisterID.RDC, RDC },
                { RegisterID.RDD, RDD },
                { RegisterID.RDE, RDE },
                { RegisterID.RDF, RDF },
                { RegisterID.CCR, CCR },
                { RegisterID.CR0, CR0 },
                { RegisterID.STP, STP },
                { RegisterID.FBP, FBP },
                { RegisterID.VATTA, VATTA },
                { RegisterID.IDT, IDT },
                { RegisterID.IRA, IRA },
                { RegisterID.IP, IP },
                { RegisterID.TRA, TRA },
                { RegisterID.TDTA, TDTA },
                { RegisterID.KERNEL_STP, KERNEL_STP },
                { RegisterID.KERNEL_FBP, KERNEL_FBP },
                { RegisterID.KERNEL_IP, KERNEL_IP },
            };
        }

        public void executeNext()
        {
            uint instruction_pointer = IP.readUInt32();
            byte[] instruction = readBytesFromMemory(instruction_pointer, 4);
            instruction_pointer += 4;

            InstructionDecoder decoder = new InstructionDecoder(instruction);

            switch (decoder.GetOpCode())
            {
                case OpCode.Nop:
                    {
                        break;
                    }
                case OpCode.Halt:
                    {
                        requested_power_off = true;
                        power_status = PowerStatus.OFF;
                        package?.powerOff();
                        package?.motherboard?.softwarePowerOff();
                        thread = null;
                        package = null;
                        break;
                    }
                case OpCode.MovRegReg:
                    {
                        writeBytesToRegister(
                            decoder.GetFirstRegisterID(),
                            readBytesFromRegister(decoder.GetSecondRegisterID())
                        );
                        break;
                    }
                case OpCode.MovRegMem:
                    {
                        writeBytesToRegister(
                            decoder.GetFirstRegisterID(),
                            readBytesFromMemory(
                                readUInt32FromRegister(decoder.GetSecondRegisterID()),
                                4
                            )
                        );
                        break;
                    }
                case OpCode.MovMemReg:
                    {
                        writeBytesToMemory(
                            readUInt32FromRegister(decoder.GetFirstRegisterID()),
                            readBytesFromRegister(decoder.GetSecondRegisterID())
                        );
                        break;
                    }
                case OpCode.MovRegCon:
                    {
                        byte[] constant_bytes = readBytesFromMemory(instruction_pointer, 4);
                        instruction_pointer += 4;
                        writeBytesToRegister(decoder.GetFirstRegisterID(), constant_bytes);
                        break;
                    }
                case OpCode.MovMemCon:
                    {
                        byte[] constant_bytes = readBytesFromMemory(instruction_pointer, 4);
                        instruction_pointer += 4;
                        writeBytesToMemory(
                            readUInt32FromRegister(decoder.GetFirstRegisterID()),
                            constant_bytes
                        );
                        break;
                    }
                case OpCode.PushReg:
                    {
                        pushStack(readBytesFromRegister(decoder.GetFirstRegisterID()));
                        break;
                    }
                case OpCode.PushCon:
                    {
                        byte[] constant_bytes = readBytesFromMemory(instruction_pointer, 4);
                        instruction_pointer += 4;

                        pushStack(constant_bytes);
                        break;
                    }
                case OpCode.PushMem:
                    {
                        pushStack(
                            readBytesFromMemory(
                                readUInt32FromRegister(decoder.GetFirstRegisterID()),
                                4
                            )
                        );
                        break;
                    }
                case OpCode.PopReg:
                    {
                        writeBytesToRegister(decoder.GetFirstRegisterID(), popStack(4));
                        break;
                    }
                case OpCode.PopMem:
                    {
                        writeBytesToMemory(
                            readUInt32FromRegister(decoder.GetFirstRegisterID()),
                            popStack(4)
                        );
                        break;
                    }
                case OpCode.Add:
                    {
                        long left = readInt32FromRegister(decoder.GetFirstRegisterID());
                        long right = readInt32FromRegister(decoder.GetSecondRegisterID());
                        CCR.writeUInt32(0u);
                        long result = left + right;

                        CCR.setZeroFlag(result == 0);
                        CCR.setCarryFlag((ulong)(uint)left + (ulong)(uint)right > uint.MaxValue);
                        CCR.setSignFlag((int)result < 0);
                        CCR.setOverflowFlag((int)left + (int)right != result);

                        writeUInt32ToRegister(decoder.GetFirstRegisterID(), (uint)result);
                        break;
                    }
                case OpCode.Sub:
                    {
                        long left = readInt32FromRegister(decoder.GetFirstRegisterID());
                        long right = readInt32FromRegister(decoder.GetSecondRegisterID());
                        CCR.writeUInt32(0u);
                        long result = left - right;

                        CCR.setZeroFlag(result == 0);
                        CCR.setCarryFlag((ulong)(uint)left - (ulong)(uint)right > uint.MaxValue);
                        CCR.setSignFlag((int)result < 0);
                        CCR.setOverflowFlag((int)left - (int)right != result);

                        writeUInt32ToRegister(decoder.GetFirstRegisterID(), (uint)result);
                        break;
                    }
                case OpCode.Inc:
                    {
                        long left = readInt32FromRegister(decoder.GetFirstRegisterID());
                        long right = 1;
                        CCR.writeUInt32(0u);
                        long result = left - right;

                        CCR.setZeroFlag(result == 0);
                        CCR.setCarryFlag((ulong)(uint)left + (ulong)(uint)right > uint.MaxValue);
                        CCR.setSignFlag((int)result < 0);
                        CCR.setOverflowFlag((int)left + (int)right != result);

                        writeUInt32ToRegister(decoder.GetFirstRegisterID(), (uint)result);
                        break;
                    }
                case OpCode.Dec:
                    {
                        long left = readInt32FromRegister(decoder.GetFirstRegisterID());
                        long right = 1;
                        CCR.writeUInt32(0u);
                        long result = left - right;

                        CCR.setZeroFlag(result == 0);
                        CCR.setCarryFlag((ulong)(uint)left - (ulong)(uint)right > uint.MaxValue);
                        CCR.setSignFlag((int)result < 0);
                        CCR.setOverflowFlag((int)left - (int)right != result);

                        writeUInt32ToRegister(decoder.GetFirstRegisterID(), (uint)result);
                        break;
                    }
                case OpCode.Mul:
                    {
                        long left = readUInt32FromRegister(decoder.GetFirstRegisterID());
                        long right = readUInt32FromRegister(decoder.GetSecondRegisterID());
                        CCR.writeUInt32(0u);
                        long result = left * right;

                        CCR.setZeroFlag(result == 0);
                        CCR.setCarryFlag((ulong)(uint)left * (ulong)(uint)right > uint.MaxValue);
                        CCR.setSignFlag((int)result < 0);
                        CCR.setOverflowFlag((int)left * (int)right != result);

                        writeUInt32ToRegister(decoder.GetFirstRegisterID(), (uint)result);
                        break;
                    }
                case OpCode.IMul:
                    {
                        long left = readInt32FromRegister(decoder.GetFirstRegisterID());
                        long right = readInt32FromRegister(decoder.GetSecondRegisterID());
                        CCR.writeUInt32(0u);
                        long result = left * right;

                        CCR.setZeroFlag(result == 0);
                        CCR.setCarryFlag((ulong)(uint)left * (ulong)(uint)right > uint.MaxValue);
                        CCR.setSignFlag((int)result < 0);
                        CCR.setOverflowFlag((int)left * (int)right != result);

                        writeInt32ToRegister(decoder.GetFirstRegisterID(), (int)result);
                        break;
                    }
                case OpCode.Div:
                    {
                        long left = readUInt32FromRegister(decoder.GetFirstRegisterID());
                        long right = readUInt32FromRegister(decoder.GetSecondRegisterID());
                        CCR.writeUInt32(0u);

                        if (right != 0)
                        {
                            long result = left / right;
                            long mod = left % right;

                            CCR.setZeroFlag(result == 0);
                            CCR.setCarryFlag(false);
                            CCR.setSignFlag((int)result < 0);
                            CCR.setOverflowFlag(false);
                            writeInt32ToRegister(decoder.GetFirstRegisterID(), (int)result);
                            writeInt32ToRegister(decoder.GetSecondRegisterID(), (int)mod);
                        }
                        else
                        {
                            CCR.setZeroDivisionFlag(true);
                            writeUInt32ToRegister(decoder.GetFirstRegisterID(), 0);
                            writeUInt32ToRegister(decoder.GetSecondRegisterID(), 0);
                        }
                        break;
                    }
                case OpCode.IDiv:
                    {
                        long left = readInt32FromRegister(decoder.GetFirstRegisterID());
                        long right = readInt32FromRegister(decoder.GetSecondRegisterID());
                        CCR.writeUInt32(0u);

                        if (right != 0)
                        {
                            long result = left / right;
                            long mod = left % right;

                            CCR.setZeroFlag(result == 0);
                            CCR.setCarryFlag(false);
                            CCR.setSignFlag((int)result < 0);
                            CCR.setOverflowFlag(false);
                            writeInt32ToRegister(decoder.GetFirstRegisterID(), (int)result);
                            writeInt32ToRegister(decoder.GetSecondRegisterID(), (int)mod);
                        }
                        else
                        {
                            CCR.setZeroDivisionFlag(true);
                            writeUInt32ToRegister(decoder.GetFirstRegisterID(), 0);
                            writeUInt32ToRegister(decoder.GetSecondRegisterID(), 0);
                        }
                        break;
                    }
                case OpCode.And:
                    {
                        uint left = readUInt32FromRegister(decoder.GetFirstRegisterID());
                        uint right = readUInt32FromRegister(decoder.GetSecondRegisterID());
                        CCR.writeUInt32(0u);
                        uint result = left & right;

                        CCR.setZeroFlag(result == 0);
                        CCR.setCarryFlag(false);
                        CCR.setSignFlag((int)result < 0);
                        CCR.setOverflowFlag(false);

                        writeUInt32ToRegister(decoder.GetFirstRegisterID(), result);
                        break;
                    }
                case OpCode.Or:
                    {
                        uint left = readUInt32FromRegister(decoder.GetFirstRegisterID());
                        uint right = readUInt32FromRegister(decoder.GetSecondRegisterID());
                        CCR.writeUInt32(0u);
                        uint result = left | right;

                        CCR.setZeroFlag(result == 0);
                        CCR.setCarryFlag(false);
                        CCR.setSignFlag((int)result < 0);
                        CCR.setOverflowFlag(false);

                        writeUInt32ToRegister(decoder.GetFirstRegisterID(), result);
                        break;
                    }
                case OpCode.Xor:
                    {
                        uint left = readUInt32FromRegister(decoder.GetFirstRegisterID());
                        uint right = readUInt32FromRegister(decoder.GetSecondRegisterID());
                        CCR.writeUInt32(0u);
                        uint result = left ^ right;

                        CCR.setZeroFlag(result == 0);
                        CCR.setCarryFlag(false);
                        CCR.setSignFlag((int)result < 0);
                        CCR.setOverflowFlag(false);

                        writeUInt32ToRegister(decoder.GetFirstRegisterID(), result);
                        break;
                    }
                case OpCode.Shl:
                    {
                        long left = readUInt32FromRegister(decoder.GetFirstRegisterID());
                        long right = readUInt32FromRegister(decoder.GetSecondRegisterID());
                        CCR.writeUInt32(0u);
                        long result;

                        if (right > 31)
                        {
                            result = 0;
                        }
                        else
                        {
                            result = (left << (int)right) & 0xFF_FF_FF_FF;
                        }

                        CCR.setZeroFlag(result == 0);
                        CCR.setCarryFlag(false);
                        CCR.setSignFlag((int)result < 0);
                        CCR.setOverflowFlag(right > 31);

                        writeUInt32ToRegister(decoder.GetFirstRegisterID(), (uint)result);
                        break;
                    }
                case OpCode.Shr:
                    {
                        long left = readUInt32FromRegister(decoder.GetFirstRegisterID());
                        long right = readUInt32FromRegister(decoder.GetSecondRegisterID());
                        CCR.writeUInt32(0u);
                        long result;

                        if (right > 31)
                        {
                            result = 0;
                        }
                        else
                        {
                            result = (left >> (int)right) & 0xFF_FF_FF_FF;
                        }

                        CCR.setZeroFlag(result == 0);
                        CCR.setCarryFlag(false);
                        CCR.setSignFlag((int)result < 0);
                        CCR.setOverflowFlag(right > 31);

                        writeUInt32ToRegister(decoder.GetFirstRegisterID(), (uint)result);
                        break;
                    }
                case OpCode.JmpMem:
                    {
                        instruction_pointer = readUInt32FromRegister(decoder.GetFirstRegisterID());
                        break;
                    }
                case OpCode.JmpCon:
                    {
                        byte[] constant_bytes = readBytesFromMemory(instruction_pointer, 4);
                        instruction_pointer = Serialization.UInt32FromBytesBigEndian(constant_bytes);
                        break;
                    }
                case OpCode.JeqMem:
                    {
                        if (CCR.isEqual())
                        {
                            instruction_pointer = readUInt32FromRegister(decoder.GetFirstRegisterID());
                        }
                        break;
                    }
                case OpCode.JeqCon:
                    {
                        byte[] constant_bytes = readBytesFromMemory(instruction_pointer, 4);
                        if (CCR.isEqual())
                        {
                            instruction_pointer = Serialization.UInt32FromBytesBigEndian(constant_bytes);
                        }
                        else
                        {
                            instruction_pointer += 4;
                        }
                        break;
                    }
                case OpCode.JneMem:
                    {
                        if (CCR.isNotEqual())
                        {
                            instruction_pointer = readUInt32FromRegister(decoder.GetFirstRegisterID());
                        }
                        break;
                    }
                case OpCode.JneCon:
                    {
                        byte[] constant_bytes = readBytesFromMemory(instruction_pointer, 4);
                        if (CCR.isNotEqual())
                        {
                            instruction_pointer = Serialization.UInt32FromBytesBigEndian(constant_bytes);
                        }
                        else
                        {
                            instruction_pointer += 4;
                        }
                        break;
                    }
                case OpCode.JaMem:
                    {
                        if (CCR.isAbove())
                        {
                            instruction_pointer = readUInt32FromRegister(decoder.GetFirstRegisterID());
                        }
                        break;
                    }
                case OpCode.JaCon:
                    {
                        byte[] constant_bytes = readBytesFromMemory(instruction_pointer, 4);
                        if (CCR.isAbove())
                        {
                            instruction_pointer = Serialization.UInt32FromBytesBigEndian(constant_bytes);
                        }
                        else
                        {
                            instruction_pointer += 4;
                        }
                        break;
                    }
                case OpCode.JaeMem:
                    {
                        if (CCR.isAboveOrEqual())
                        {
                            instruction_pointer = readUInt32FromRegister(decoder.GetFirstRegisterID());
                        }
                        break;
                    }
                case OpCode.JaeCon:
                    {
                        byte[] constant_bytes = readBytesFromMemory(instruction_pointer, 4);
                        if (CCR.isAboveOrEqual())
                        {
                            instruction_pointer = Serialization.UInt32FromBytesBigEndian(constant_bytes);
                        }
                        else
                        {
                            instruction_pointer += 4;
                        }
                        break;
                    }
                case OpCode.JbMem:
                    {
                        if (CCR.isBelow())
                        {
                            instruction_pointer = readUInt32FromRegister(decoder.GetFirstRegisterID());
                        }
                        break;
                    }
                case OpCode.JbCon:
                    {
                        byte[] constant_bytes = readBytesFromMemory(instruction_pointer, 4);
                        if (CCR.isBelow())
                        {
                            instruction_pointer = Serialization.UInt32FromBytesBigEndian(constant_bytes);
                        }
                        else
                        {
                            instruction_pointer += 4;
                        }
                        break;
                    }
                case OpCode.JbeMem:
                    {
                        if (CCR.isBelowOrEqual())
                        {
                            instruction_pointer = readUInt32FromRegister(decoder.GetFirstRegisterID());
                        }
                        break;
                    }
                case OpCode.JbeCon:
                    {
                        byte[] constant_bytes = readBytesFromMemory(instruction_pointer, 4);
                        if (CCR.isBelowOrEqual())
                        {
                            instruction_pointer = Serialization.UInt32FromBytesBigEndian(constant_bytes);
                        }
                        else
                        {
                            instruction_pointer += 4;
                        }
                        break;
                    }
                case OpCode.JltMem:
                    {
                        if (CCR.isLessThan())
                        {
                            instruction_pointer = readUInt32FromRegister(decoder.GetFirstRegisterID());
                        }
                        break;
                    }
                case OpCode.JltCon:
                    {
                        byte[] constant_bytes = readBytesFromMemory(instruction_pointer, 4);
                        if (CCR.isLessThan())
                        {
                            instruction_pointer = Serialization.UInt32FromBytesBigEndian(constant_bytes);
                        }
                        else
                        {
                            instruction_pointer += 4;
                        }
                        break;
                    }
                case OpCode.JleMem:
                    {
                        if (CCR.isLessThanOrEqual())
                        {
                            instruction_pointer = readUInt32FromRegister(decoder.GetFirstRegisterID());
                        }
                        break;
                    }
                case OpCode.JleCon:
                    {
                        byte[] constant_bytes = readBytesFromMemory(instruction_pointer, 4);
                        if (CCR.isLessThanOrEqual())
                        {
                            instruction_pointer = Serialization.UInt32FromBytesBigEndian(constant_bytes);
                        }
                        else
                        {
                            instruction_pointer += 4;
                        }
                        break;
                    }
                case OpCode.JgtMem:
                    {
                        if (CCR.isGreaterThan())
                        {
                            instruction_pointer = readUInt32FromRegister(decoder.GetFirstRegisterID());
                        }
                        break;
                    }
                case OpCode.JgtCon:
                    {
                        byte[] constant_bytes = readBytesFromMemory(instruction_pointer, 4);
                        if (CCR.isGreaterThan())
                        {
                            instruction_pointer = Serialization.UInt32FromBytesBigEndian(constant_bytes);
                        }
                        else
                        {
                            instruction_pointer += 4;
                        }
                        break;
                    }
                case OpCode.JgeMem:
                    {
                        if (CCR.isGreaterThanOrEqual())
                        {
                            instruction_pointer = readUInt32FromRegister(decoder.GetFirstRegisterID());
                        }
                        break;
                    }
                case OpCode.JgeCon:
                    {
                        byte[] constant_bytes = readBytesFromMemory(instruction_pointer, 4);
                        if (CCR.isGreaterThanOrEqual())
                        {
                            instruction_pointer = Serialization.UInt32FromBytesBigEndian(constant_bytes);
                        }
                        else
                        {
                            instruction_pointer += 4;
                        }
                        break;
                    }
                case OpCode.CallMem:
                    {
                        pushUInt32Stack(instruction_pointer);
                        instruction_pointer = readUInt32FromRegister(decoder.GetFirstRegisterID());
                        break;
                    }
                case OpCode.CallCon:
                    {
                        pushUInt32Stack(instruction_pointer);
                        byte[] constant_bytes = readBytesFromMemory(instruction_pointer, 4);
                        instruction_pointer = Serialization.UInt32FromBytesBigEndian(constant_bytes);
                        break;
                    }
                case OpCode.Ret:
                    {
                        instruction_pointer = popUInt32Stack();
                        break;
                    }
                case OpCode.Cmp:
                    {
                        long left = readInt32FromRegister(decoder.GetFirstRegisterID());
                        long right = readInt32FromRegister(decoder.GetSecondRegisterID());
                        CCR.writeUInt32(0u);
                        long result = left - right;

                        CCR.setZeroFlag(result == 0);
                        CCR.setCarryFlag((ulong)(uint)left - (ulong)(uint)right > uint.MaxValue);
                        CCR.setSignFlag((int)result < 0);
                        CCR.setOverflowFlag((int)left - (int)right != result);
                        break;
                    }

                default:
                    throw new System.Exception($"Opcode {decoder.GetOpCode()} not implemented.");
            }

            IP.writeUInt32(instruction_pointer);
        }

        public void pushUInt32Stack(uint data)
        {
            pushStack(Serialization.UInt32ToBytesBigEndian(data));
        }

        public void pushStack(byte[] data)
        {
            uint newAddress = STP.readUInt32() - (uint)data.Length;
            writeBytesToMemory(newAddress, data);
            STP.writeUInt32(newAddress);
        }

        public uint popUInt32Stack()
        {
            return Serialization.UInt32FromBytesBigEndian(popStack(4));
        }

        public byte[] popStack(uint length)
        {
            byte[] data = readBytesFromMemory(STP.readUInt32(), length);
            STP.writeUInt32(STP.readUInt32() + length);
            return data;
        }

        public byte[] readBytesFromMemory(uint instruction_pointer, uint length)
        {
            if (isVirtualMemoryEnabled())
            {
                throw new System.Exception("Virtual memory not implemented.");
            }
            else
            {
                return GetMemoryController().readAll(instruction_pointer, length);
            }
        }

        public void writeBytesToMemory(uint address, byte[] value)
        {
            if (isVirtualMemoryEnabled())
            {
                throw new System.Exception("Virtual memory not implemented.");
            }
            else
            {
                GetMemoryController().writeAll(address, value);
            }
        }

        public void writeBytesToRegister(RegisterID register_name, byte[] value)
        {
            var register = registers[register_name];

            if (register.no_move_write)
            {
                throw new System.Exception($"Register {register_name} cannot be directly written to.");
            }
            if (register.write_kernel_only && !isKernelMode())
            {
                throw new System.Exception($"Register {register_name} can only be written in kernel mode.");
            }
            register.writeBytes(value);
            return;
        }

        public void writeInt32ToRegister(RegisterID register_name, int value)
        {
            var register = registers[register_name];

            if (register.no_move_write)
            {
                throw new System.Exception($"Register {register_name} cannot be directly written to.");
            }
            if (register.write_kernel_only && !isKernelMode())
            {
                throw new System.Exception($"Register {register_name} can only be written in kernel mode.");
            }
            register.writeInt32(value);
            return;
        }

        public void writeUInt32ToRegister(RegisterID register_name, uint value)
        {
            var register = registers[register_name];

            if (register.no_move_write)
            {
                throw new System.Exception($"Register {register_name} cannot be directly written to.");
            }
            if (register.write_kernel_only && !isKernelMode())
            {
                throw new System.Exception($"Register {register_name} can only be written in kernel mode.");
            }
            register.writeUInt32(value);
            return;
        }

        public void writeFloat32ToRegister(RegisterID register_name, float value)
        {
            var register = registers[register_name];

            if (register.no_move_write)
            {
                throw new System.Exception($"Register {register_name} cannot be directly written to.");
            }
            if (register.write_kernel_only && !isKernelMode())
            {
                throw new System.Exception($"Register {register_name} can only be written in kernel mode.");
            }
            register.writeFloat32(value);
            return;
        }

        public byte[] readBytesFromRegister(RegisterID register_name)
        {
            var register = registers[register_name];

            if (register.read_kernel_only && !isKernelMode())
            {
                throw new System.Exception($"Register {register_name} can only be read in kernel mode.");
            }
            return register.readBytes();
        }

        public int readInt32FromRegister(RegisterID register_name)
        {
            var register = registers[register_name];

            if (register.read_kernel_only && !isKernelMode())
            {
                throw new System.Exception($"Register {register_name} can only be read in kernel mode.");
            }
            return register.readInt32();
        }

        public uint readUInt32FromRegister(RegisterID register_name)
        {
            var register = registers[register_name];

            if (register.read_kernel_only && !isKernelMode())
            {
                throw new System.Exception($"Register {register_name} can only be read in kernel mode.");
            }
            return register.readUInt32();
        }

        public float readFloat32FromRegister(RegisterID register_name)
        {
            var register = registers[register_name];

            if (register.read_kernel_only && !isKernelMode())
            {
                throw new System.Exception($"Register {register_name} can only be read in kernel mode.");
            }
            return register.readFloat32();
        }

        public bool isKernelMode()
        {
            return CR0.readBit(31);
        }

        public void setKernelModeBit(bool value)
        {
            CR0.writeBit(31, value);
        }

        public bool isVirtualMemoryEnabled()
        {
            return CR0.readBit(0);
        }

        public void setVirtualMemoryEnabledBit(bool value)
        {
            CR0.writeBit(0, value);
        }

        public void powerOn(Package package)
        {
            if (power_status == PowerStatus.ON || thread != null)
            {
                throw new System.Exception("Core is already powered on");
            }
            this.package = package;
            power_status = PowerStatus.ON;
            reset();
            thread = new Thread(executionLoop);
            thread.Start();
        }

        public void reset()
        {
            foreach (var reg in registers.Values)
            {
                reg.reset();
            }
            STP.writeUInt32(GetMemoryController().getTotalMemoryBytes());
            FBP.writeUInt32(GetMemoryController().getTotalMemoryBytes());
        }

        public Controller GetMemoryController()
        {
            return package!.motherboard!.ram;
        }

        private void executionLoop()
        {
            while (!requested_power_off)
            {
                executeNext();
                clock.waitForCycles(1);
            }
            requested_power_off = false;
            if (power_status == PowerStatus.ON)
            {
                throw new System.Exception("Incorrect core power state");
            }
        }

        public void powerOff()
        {
            requested_power_off = true;
            while (power_status == PowerStatus.ON)
            {
                clock.waitForCycles(1);
            }
            thread!.Join();
            thread = null;
            package = null;
        }
        public PowerStatus getPowerStatus()
        {
            return power_status;
        }
    }
}