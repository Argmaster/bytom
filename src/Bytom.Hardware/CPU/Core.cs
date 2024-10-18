using System.Collections.Generic;
using System.Threading.Tasks;
using Bytom.Hardware.RAM;

namespace Bytom.Hardware.CPU
{
    public enum RegisterID
    {
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
        public uint core_id { get; set; }
        public uint clock_speed_hz { get; set; }
        public List<Cache> caches { get; set; }
        public Controller ram { get; set; }

        public Dictionary<RegisterID, Register32> registers { get; }

        public Register32 RD0;

        public Core(uint core_id, uint clock_speed_hz, List<Cache> caches, Controller ram)
        {
            this.core_id = core_id;
            this.clock_speed_hz = clock_speed_hz;
            this.caches = caches;
            this.ram = ram;

            RD0 = new Register32();

            this.registers = new Dictionary<RegisterID, Register32>{
                { RegisterID.RD0, RD0 }
            };
        }

        public int GetCycleTimeMilliseconds()
        {
            return (int)(1000 / clock_speed_hz);
        }

        public async Task execute()
        {
            RD0.ReadUInt32();

            await Task.Delay(GetCycleTimeMilliseconds());
        }
    }
}