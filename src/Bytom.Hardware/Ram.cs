using System;


namespace Bytom.Hardware
{
    public class Ram
    {
        public UInt32 capacity { get; set; }
        public UInt32 clock_speed_hz { get; set; }
        public UInt32 latency_milliseconds { get; set; }
        public UInt32 bandwidth_bytes_per_cycle { get; set; }

        public Ram(UInt32 capacity_, UInt32 clock_speed_hz_, UInt32 latency_milliseconds_, UInt32 bandwidth_bytes_per_cycle_)
        {
            this.capacity = capacity_;
            this.clock_speed_hz = clock_speed_hz_;
            this.latency_milliseconds = latency_milliseconds_;
            this.bandwidth_bytes_per_cycle = bandwidth_bytes_per_cycle_;
        }
    }
}