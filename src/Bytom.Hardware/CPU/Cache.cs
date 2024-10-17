namespace Bytom.Hardware.CPU
{
    public class Cache
    {
        public uint capacity_bytes { get; set; }
        public uint latency_cycles { get; set; }


        public Cache(uint capacity_bytes_, uint latency_cycles_)
        {
            this.capacity_bytes = capacity_bytes_;
            this.latency_cycles = latency_cycles_;
        }
    }
}