using System;
using System.Collections;
using Bytom.Tools;

namespace Bytom.Hardware
{
    public class FirmwareRom : Memory
    {

        public FirmwareRom(
            uint capacity_bytes,
            uint clock_speed_hz,
            uint write_latency_cycles,
            uint read_latency_cycles,
            uint bandwidth_bytes
        ) : base(capacity_bytes, write_latency_cycles, read_latency_cycles, bandwidth_bytes, new Clock(clock_speed_hz))
        { }
    }
}