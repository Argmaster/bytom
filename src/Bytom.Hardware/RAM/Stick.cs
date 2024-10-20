

using System;
using System.Threading.Tasks;

namespace Bytom.Hardware.RAM
{
    public class MemoryChip
    {
        public uint capacity_bytes { get; }
        public uint clock_speed_hz { get; }
        public uint latency_cycles { get; }
        public Clock clock { get; }
        private byte[] memory { get; }

        public MemoryChip(uint capacity_bytes, uint clock_speed_hz, uint latency_cycles)
        {
            this.capacity_bytes = capacity_bytes;
            this.clock_speed_hz = clock_speed_hz;
            this.latency_cycles = latency_cycles;
            clock = new Clock(clock_speed_hz);
            memory = new byte[capacity_bytes];
        }

        public async Task waitUntilReady()
        {
            await clock.waitForCycles(latency_cycles);
        }

        public async Task writeNoDelay(long address, byte content)
        {
            if (address < 0 || address > capacity_bytes)
            {
                throw new Exception($"Writing outside of memory bounds 0x{address:X8}");
            }
            await Task.Delay(0);
            memory[address] = content;
        }

        public async Task<byte> readNoDelay(long address)
        {
            if (address < 0 || address > capacity_bytes)
            {
                throw new Exception($"Reading outside of memory bounds 0x{address:X8}");
            }
            await Task.Delay(0);
            return memory[address];
        }
    }
}