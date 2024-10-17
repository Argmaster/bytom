using System.Collections.Generic;
using Bytom.Hardware.RAM;

namespace Bytom.Hardware.CPU
{
    public class Core
    {
        public uint core_id { get; set; }
        public uint clock_speed_hz { get; set; }
        public List<Cache> caches { get; set; }
        public Controller ram { get; set; }

        public Core(uint core_id, uint clock_speed_hz_, List<Cache> caches_, Controller ram_)
        {
            this.core_id = core_id;
            this.clock_speed_hz = clock_speed_hz_;
            this.caches = caches_;
            this.ram = ram_;
        }
    }
}