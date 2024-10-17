

namespace Bytom.Hardware.RAM
{
    public class Stick
    {
        public uint capacity_bytes { get; set; }
        public uint clock_speed_hz { get; set; }
        public uint latency_cycles { get; set; }
        public uint bandwidth_bytes_per_cycle { get; set; }

        public Stick(uint capacity_bytes_, uint clock_speed_hz_, uint latency_cycles_, uint bandwidth_bytes_per_cycle_)
        {
            this.capacity_bytes = capacity_bytes_;
            this.clock_speed_hz = clock_speed_hz_;
            this.latency_cycles = latency_cycles_;
            this.bandwidth_bytes_per_cycle = bandwidth_bytes_per_cycle_;
        }
    }
}