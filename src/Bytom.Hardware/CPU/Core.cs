using System.Collections.Generic;
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
        public uint core_id;
        public uint clock_speed_hz;
        public PowerStatus power_status;
        public bool requested_power_off;
        public List<Cache> caches;
        public Controller ram;
        public Clock clock;
        public Package? package;

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
            uint clock_speed_hz,
            List<Cache> caches,
            Controller ram
        )
        {
            this.core_id = core_id;
            this.clock_speed_hz = clock_speed_hz;
            this.caches = caches;
            this.ram = ram;

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
            STP = new Register32(ram.getTotalMemoryBytes());
            FBP = new Register32(ram.getTotalMemoryBytes());
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

        public async Task executeNext()
        {
            uint instruction_pointer = IP.readUInt32();
            byte[] instruction = await ram.readAll(instruction_pointer, 4);
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
                        if (package != null)
                        {
                            await package.powerOff();
                            if (package.motherboard != null)
                            {
                                await package.motherboard.softwarePowerOff();
                            }
                        }
                        break;
                    }
                case OpCode.MovRegReg:
                    {
                        await writeBytesToRegister(
                            decoder.GetFirstRegisterID(),
                            await readBytesFromRegister(decoder.GetSecondRegisterID())
                        );
                        break;
                    }
                case OpCode.MovRegMem:
                    {
                        await writeBytesToRegister(
                            decoder.GetFirstRegisterID(),
                            await readBytesFromMemory(
                                await readUInt32FromRegister(decoder.GetSecondRegisterID()),
                                4
                            )
                        );
                        break;
                    }
                case OpCode.MovMemReg:
                    {
                        await writeBytesToMemory(
                            await readUInt32FromRegister(decoder.GetFirstRegisterID()),
                            await readBytesFromRegister(decoder.GetSecondRegisterID())
                        );
                        break;
                    }
                case OpCode.MovRegCon:
                    {
                        byte[] constant_bytes = await readBytesFromMemory(instruction_pointer, 4);
                        instruction_pointer += 4;
                        await writeBytesToRegister(decoder.GetFirstRegisterID(), constant_bytes);
                        break;
                    }
                case OpCode.MovMemCon:
                    {
                        byte[] constant_bytes = await readBytesFromMemory(instruction_pointer, 4);
                        instruction_pointer += 4;
                        await writeBytesToMemory(
                            await readUInt32FromRegister(decoder.GetFirstRegisterID()),
                            constant_bytes
                        );
                        break;
                    }
                case OpCode.PushReg:
                    {
                        uint newAddress = STP.readUInt32() - 4;
                        await writeBytesToMemory(
                            newAddress,
                            await readBytesFromRegister(decoder.GetFirstRegisterID())
                        );
                        STP.writeUInt32(newAddress);
                        break;
                    }
                case OpCode.PushCon:
                    {
                        byte[] constant_bytes = await readBytesFromMemory(instruction_pointer, 4);
                        instruction_pointer += 4;

                        uint newAddress = STP.readUInt32() - 4;
                        await writeBytesToMemory(newAddress, constant_bytes);
                        STP.writeUInt32(newAddress);
                        break;
                    }
                case OpCode.PushMem:
                    {
                        byte[] memory = await readBytesFromMemory(
                            await readUInt32FromRegister(decoder.GetFirstRegisterID()),
                            4
                        );
                        uint newAddress = STP.readUInt32() - 4;
                        await writeBytesToMemory(newAddress, memory);
                        STP.writeUInt32(newAddress);
                        break;
                    }
                case OpCode.PopReg:
                    {
                        byte[] memory = await readBytesFromMemory(STP.readUInt32(), 4);
                        await writeBytesToRegister(decoder.GetFirstRegisterID(), memory);
                        STP.writeUInt32(STP.readUInt32() + 4);
                        break;
                    }
                case OpCode.PopMem:
                    {
                        byte[] memory = await readBytesFromMemory(STP.readUInt32(), 4);
                        await writeBytesToMemory(
                            await readUInt32FromRegister(decoder.GetFirstRegisterID()),
                            memory
                        );
                        STP.writeUInt32(STP.readUInt32() + 4);
                        break;
                    }
                case OpCode.Add:
                    {
                        long left = await readInt32FromRegister(decoder.GetFirstRegisterID());
                        long right = await readInt32FromRegister(decoder.GetSecondRegisterID());
                        CCR.writeUInt32(0u);
                        long result = left + right;

                        CCR.setZeroFlag(result == 0);
                        CCR.setCarryFlag((ulong)(uint)left + (ulong)(uint)right > uint.MaxValue);
                        CCR.setSignFlag((int)result < 0);
                        CCR.setOverflowFlag((int)left + (int)right != result);

                        await writeUInt32ToRegister(decoder.GetFirstRegisterID(), (uint)result);
                        break;
                    }
                case OpCode.Sub:
                    {
                        long left = await readInt32FromRegister(decoder.GetFirstRegisterID());
                        long right = await readInt32FromRegister(decoder.GetSecondRegisterID());
                        CCR.writeUInt32(0u);
                        long result = left - right;

                        CCR.setZeroFlag(result == 0);
                        CCR.setCarryFlag((ulong)(uint)left - (ulong)(uint)right > uint.MaxValue);
                        CCR.setSignFlag((int)result < 0);
                        CCR.setOverflowFlag((int)left - (int)right != result);

                        await writeUInt32ToRegister(decoder.GetFirstRegisterID(), (uint)result);
                        break;
                    }
                case OpCode.Inc:
                    {
                        long left = await readInt32FromRegister(decoder.GetFirstRegisterID());
                        long right = 1;
                        CCR.writeUInt32(0u);
                        long result = left - right;

                        CCR.setZeroFlag(result == 0);
                        CCR.setCarryFlag((ulong)(uint)left + (ulong)(uint)right > uint.MaxValue);
                        CCR.setSignFlag((int)result < 0);
                        CCR.setOverflowFlag((int)left + (int)right != result);

                        await writeUInt32ToRegister(decoder.GetFirstRegisterID(), (uint)result);
                        break;
                    }
                case OpCode.Dec:
                    {
                        long left = await readInt32FromRegister(decoder.GetFirstRegisterID());
                        long right = 1;
                        CCR.writeUInt32(0u);
                        long result = left - right;

                        CCR.setZeroFlag(result == 0);
                        CCR.setCarryFlag((ulong)(uint)left - (ulong)(uint)right > uint.MaxValue);
                        CCR.setSignFlag((int)result < 0);
                        CCR.setOverflowFlag((int)left - (int)right != result);

                        await writeUInt32ToRegister(decoder.GetFirstRegisterID(), (uint)result);
                        break;
                    }
                case OpCode.Mul:
                    {
                        long left = await readUInt32FromRegister(decoder.GetFirstRegisterID());
                        long right = await readUInt32FromRegister(decoder.GetSecondRegisterID());
                        CCR.writeUInt32(0u);
                        long result = left * right;

                        CCR.setZeroFlag(result == 0);
                        CCR.setCarryFlag((ulong)(uint)left * (ulong)(uint)right > uint.MaxValue);
                        CCR.setSignFlag((int)result < 0);
                        CCR.setOverflowFlag((int)left * (int)right != result);

                        await writeUInt32ToRegister(decoder.GetFirstRegisterID(), (uint)result);
                        break;
                    }
                case OpCode.IMul:
                    {
                        long left = await readInt32FromRegister(decoder.GetFirstRegisterID());
                        long right = await readInt32FromRegister(decoder.GetSecondRegisterID());
                        CCR.writeUInt32(0u);
                        long result = left * right;

                        CCR.setZeroFlag(result == 0);
                        CCR.setCarryFlag((ulong)(uint)left * (ulong)(uint)right > uint.MaxValue);
                        CCR.setSignFlag((int)result < 0);
                        CCR.setOverflowFlag((int)left * (int)right != result);

                        await writeInt32ToRegister(decoder.GetFirstRegisterID(), (int)result);
                        break;
                    }
                case OpCode.Div:
                    {
                        long left = await readUInt32FromRegister(decoder.GetFirstRegisterID());
                        long right = await readUInt32FromRegister(decoder.GetSecondRegisterID());
                        CCR.writeUInt32(0u);

                        if (right != 0)
                        {
                            long result = left / right;
                            long mod = left % right;

                            CCR.setZeroFlag(result == 0);
                            CCR.setCarryFlag(false);
                            CCR.setSignFlag((int)result < 0);
                            CCR.setOverflowFlag(false);
                            await writeInt32ToRegister(decoder.GetFirstRegisterID(), (int)result);
                            await writeInt32ToRegister(decoder.GetSecondRegisterID(), (int)mod);
                        }
                        else
                        {
                            CCR.setZeroDivisionFlag(true);
                            await writeUInt32ToRegister(decoder.GetFirstRegisterID(), 0);
                            await writeUInt32ToRegister(decoder.GetSecondRegisterID(), 0);
                        }
                        break;
                    }
                case OpCode.IDiv:
                    {
                        long left = await readInt32FromRegister(decoder.GetFirstRegisterID());
                        long right = await readInt32FromRegister(decoder.GetSecondRegisterID());
                        CCR.writeUInt32(0u);

                        if (right != 0)
                        {
                            long result = left / right;
                            long mod = left % right;

                            CCR.setZeroFlag(result == 0);
                            CCR.setCarryFlag(false);
                            CCR.setSignFlag((int)result < 0);
                            CCR.setOverflowFlag(false);
                            await writeInt32ToRegister(decoder.GetFirstRegisterID(), (int)result);
                            await writeInt32ToRegister(decoder.GetSecondRegisterID(), (int)mod);
                        }
                        else
                        {
                            CCR.setZeroDivisionFlag(true);
                            await writeUInt32ToRegister(decoder.GetFirstRegisterID(), 0);
                            await writeUInt32ToRegister(decoder.GetSecondRegisterID(), 0);
                        }
                        break;
                    }
                case OpCode.And:
                    {
                        uint left = await readUInt32FromRegister(decoder.GetFirstRegisterID());
                        uint right = await readUInt32FromRegister(decoder.GetSecondRegisterID());
                        CCR.writeUInt32(0u);
                        uint result = left & right;

                        CCR.setZeroFlag(result == 0);
                        CCR.setCarryFlag(false);
                        CCR.setSignFlag((int)result < 0);
                        CCR.setOverflowFlag(false);

                        await writeUInt32ToRegister(decoder.GetFirstRegisterID(), result);
                        break;
                    }
                case OpCode.Or:
                    {
                        uint left = await readUInt32FromRegister(decoder.GetFirstRegisterID());
                        uint right = await readUInt32FromRegister(decoder.GetSecondRegisterID());
                        CCR.writeUInt32(0u);
                        uint result = left | right;

                        CCR.setZeroFlag(result == 0);
                        CCR.setCarryFlag(false);
                        CCR.setSignFlag((int)result < 0);
                        CCR.setOverflowFlag(false);

                        await writeUInt32ToRegister(decoder.GetFirstRegisterID(), result);
                        break;
                    }
                case OpCode.Xor:
                    {
                        uint left = await readUInt32FromRegister(decoder.GetFirstRegisterID());
                        uint right = await readUInt32FromRegister(decoder.GetSecondRegisterID());
                        CCR.writeUInt32(0u);
                        uint result = left ^ right;

                        CCR.setZeroFlag(result == 0);
                        CCR.setCarryFlag(false);
                        CCR.setSignFlag((int)result < 0);
                        CCR.setOverflowFlag(false);

                        await writeUInt32ToRegister(decoder.GetFirstRegisterID(), result);
                        break;
                    }
                case OpCode.Shl:
                    {
                        long left = await readUInt32FromRegister(decoder.GetFirstRegisterID());
                        long right = await readUInt32FromRegister(decoder.GetSecondRegisterID());
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

                        await writeUInt32ToRegister(decoder.GetFirstRegisterID(), (uint)result);
                        break;
                    }
                case OpCode.Shr:
                    {
                        long left = await readUInt32FromRegister(decoder.GetFirstRegisterID());
                        long right = await readUInt32FromRegister(decoder.GetSecondRegisterID());
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

                        await writeUInt32ToRegister(decoder.GetFirstRegisterID(), (uint)result);
                        break;
                    }
                case OpCode.JmpMem:
                    {
                        instruction_pointer = await readUInt32FromRegister(decoder.GetFirstRegisterID());
                        break;
                    }
                case OpCode.JmpCon:
                    {
                        byte[] constant_bytes = await readBytesFromMemory(instruction_pointer, 4);
                        instruction_pointer = Serialization.UInt32FromBytesBigEndian(constant_bytes);
                        break;
                    }
                case OpCode.JeqMem:
                    {
                        if (CCR.isEqual())
                        {
                            instruction_pointer = await readUInt32FromRegister(decoder.GetFirstRegisterID());
                        }
                        break;
                    }
                case OpCode.JeqCon:
                    {
                        byte[] constant_bytes = await readBytesFromMemory(instruction_pointer, 4);
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
                        if (!CCR.isNotEqual())
                        {
                            instruction_pointer = await readUInt32FromRegister(decoder.GetFirstRegisterID());
                        }
                        break;
                    }
                case OpCode.JneCon:
                    {
                        byte[] constant_bytes = await readBytesFromMemory(instruction_pointer, 4);
                        if (!CCR.isNotEqual())
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
                            instruction_pointer = await readUInt32FromRegister(decoder.GetFirstRegisterID());
                        }
                        break;
                    }
                case OpCode.JaCon:
                    {
                        byte[] constant_bytes = await readBytesFromMemory(instruction_pointer, 4);
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
                            instruction_pointer = await readUInt32FromRegister(decoder.GetFirstRegisterID());
                        }
                        break;
                    }
                case OpCode.JaeCon:
                    {
                        byte[] constant_bytes = await readBytesFromMemory(instruction_pointer, 4);
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
                case OpCode.Cmp:
                    {
                        long left = await readInt32FromRegister(decoder.GetFirstRegisterID());
                        long right = await readInt32FromRegister(decoder.GetSecondRegisterID());
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

        public async Task<byte[]> readBytesFromMemory(uint instruction_pointer, uint length)
        {
            if (isVirtualMemoryEnabled())
            {
                throw new System.Exception("Virtual memory not implemented.");
            }
            else
            {
                return await ram.readAll(instruction_pointer, length);
            }
        }

        public async Task writeBytesToMemory(uint address, byte[] value)
        {
            if (isVirtualMemoryEnabled())
            {
                throw new System.Exception("Virtual memory not implemented.");
            }
            else
            {
                await ram.writeAll(address, value);
            }
        }

        public async Task writeBytesToRegister(RegisterID register_name, byte[] value)
        {
            await Task.Delay(0);
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

        public async Task writeInt32ToRegister(RegisterID register_name, int value)
        {
            await Task.Delay(0);
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

        public async Task writeUInt32ToRegister(RegisterID register_name, uint value)
        {
            await Task.Delay(0);
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

        public async Task writeFloat32ToRegister(RegisterID register_name, float value)
        {
            await Task.Delay(0);
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

        public async Task<byte[]> readBytesFromRegister(RegisterID register_name)
        {
            await Task.Delay(0);
            var register = registers[register_name];

            if (register.read_kernel_only && !isKernelMode())
            {
                throw new System.Exception($"Register {register_name} can only be read in kernel mode.");
            }
            return register.readBytes();
        }

        public async Task<int> readInt32FromRegister(RegisterID register_name)
        {
            await Task.Delay(0);
            var register = registers[register_name];

            if (register.read_kernel_only && !isKernelMode())
            {
                throw new System.Exception($"Register {register_name} can only be read in kernel mode.");
            }
            return register.readInt32();
        }

        public async Task<uint> readUInt32FromRegister(RegisterID register_name)
        {
            await Task.Delay(0);
            var register = registers[register_name];

            if (register.read_kernel_only && !isKernelMode())
            {
                throw new System.Exception($"Register {register_name} can only be read in kernel mode.");
            }
            return register.readUInt32();
        }

        public async Task<float> readFloat32FromRegister(RegisterID register_name)
        {
            await Task.Delay(0);
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

        public async Task powerOn()
        {
            power_status = PowerStatus.ON;

            foreach (var reg in registers.Values)
            {
                reg.reset();
            }

            while (!requested_power_off)
            {
                await Task.Delay(0);
                await executeNext();
            }

            requested_power_off = false;
            power_status = PowerStatus.OFF;
        }

        public async Task powerOff()
        {
            requested_power_off = true;
            while (power_status == PowerStatus.ON)
            {
                await Task.Delay(10);
            }
        }
        public PowerStatus getPowerStatus()
        {
            return power_status;
        }
    }
}