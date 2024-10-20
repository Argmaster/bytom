using System.Collections.Generic;
using System.Threading.Tasks;
using Bytom.Hardware.RAM;

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
        CR0 = 0b10_0000,
        CSTP = 0b10_0100,
        CSBP = 0b10_0101,
        VATTA = 0b10_0110,
        IDT = 0b10_0111,
        IP = 0b10_1001,
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

        public Register32 CR0;
        public Register32 CSTP;
        public Register32 CSBP;
        public Register32 VATTA;
        public Register32 IDT;
        public Register32 IP;

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

            RD0 = new Register32();
            RD1 = new Register32();
            RD2 = new Register32();
            RD3 = new Register32();
            RD4 = new Register32();
            RD5 = new Register32();
            RD6 = new Register32();
            RD7 = new Register32();
            RD8 = new Register32();
            RD9 = new Register32();
            RDA = new Register32();
            RDB = new Register32();
            RDC = new Register32();
            RDD = new Register32();
            RDE = new Register32();
            RDF = new Register32();

            CR0 = new Register32();
            CSTP = new Register32();
            CSBP = new Register32();
            VATTA = new Register32();
            IDT = new Register32();
            IP = new Register32();

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
                { RegisterID.CR0, CR0 },
                { RegisterID.CSTP, CSTP },
                { RegisterID.CSBP, CSBP },
                { RegisterID.VATTA, VATTA },
                { RegisterID.IDT, IDT },
                { RegisterID.IP, IP },
            };
        }

        public async Task executeNext()
        {
            uint instruction_pointer = IP.ReadUInt32();
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
                case OpCode.MovRegCon:
                    {
                        RegisterID register_name = decoder.GetFirstRegisterID();
                        byte[] constant_bytes = await ram.readAll(instruction_pointer, 4);
                        instruction_pointer += 4;

                        registers[register_name].WriteBytes(constant_bytes);
                        break;
                    }
                default:
                    throw new System.Exception($"Opcode {decoder.GetOpCode()} not implemented.");
            }

            IP.WriteUInt32(instruction_pointer);
        }

        public async Task powerOn()
        {
            power_status = PowerStatus.ON;

            RD0.WriteUInt32(0);
            RD1.WriteUInt32(0);
            RD2.WriteUInt32(0);
            RD3.WriteUInt32(0);
            RD4.WriteUInt32(0);
            RD5.WriteUInt32(0);
            RD6.WriteUInt32(0);
            RD7.WriteUInt32(0);
            RD8.WriteUInt32(0);
            RD9.WriteUInt32(0);
            RDA.WriteUInt32(0);
            RDB.WriteUInt32(0);
            RDC.WriteUInt32(0);
            RDD.WriteUInt32(0);
            RDE.WriteUInt32(0);
            RDF.WriteUInt32(0);

            CR0.WriteUInt32(0);
            CSTP.WriteUInt32(0);
            CSBP.WriteUInt32(0);
            VATTA.WriteUInt32(0);
            IDT.WriteUInt32(0);
            IP.WriteUInt32(0);

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