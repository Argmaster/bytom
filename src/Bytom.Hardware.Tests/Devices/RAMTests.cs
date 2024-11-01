

namespace Bytom.Hardware.Tests
{
    public class TestRAM : RAM
    {
        public TestRAM(
            uint capacity_bytes,
            uint bandwidth_bytes
        ) : base(
            capacity_bytes: capacity_bytes,
            clock_speed_hz: 1000,
            write_latency_cycles: 1,
            read_latency_cycles: 1,
            bandwidth_bytes: bandwidth_bytes
        )
        { }
        public override void beforeThreadStart()
        { }
    }

    public class DebugRAM : RAM
    {
        public DebugRAM(
            uint capacity_bytes,
            uint bandwidth_bytes
        ) : base(
            capacity_bytes: capacity_bytes,
            clock_speed_hz: 1000,
            write_latency_cycles: 1,
            read_latency_cycles: 1,
            bandwidth_bytes: bandwidth_bytes
        )
        { }

        public override void powerOnInit()
        { }
    }

    public class RAMTests : GenericMemoryTests<TestRAM, DebugRAM>
    {
    }
}