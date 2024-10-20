

using System.Diagnostics;
using System.Threading.Tasks;

namespace Bytom.Hardware
{
    public class Clock
    {
        public uint frequency_hz { get; set; }
        public uint cycles { get; set; }
        public uint bandwidth_bytes_per_cycle { get; set; }

        public Clock(uint frequency_hz)
        {
            this.frequency_hz = frequency_hz;
        }

        public long getCycleLengthMilliseconds()
        {
            return 1_000 / frequency_hz;
        }

        public long getCycleLengthMicroseconds()
        {
            return 1_000_000 / frequency_hz;
        }

        public async Task waitForCycles(uint cycles)
        {
            if (frequency_hz <= 1000)
            {
                var milliseconds = cycles * getCycleLengthMilliseconds();
                await waitMilliseconds(milliseconds);
            }
            else
            {
                var microseconds = cycles * getCycleLengthMicroseconds();
                await waitMicroseconds(microseconds);
            }
        }

        public async Task waitMilliseconds(long milliseconds)
        {
            await Task.Delay((int)milliseconds);
        }

        public async Task waitMicroseconds(long microseconds)
        {
            var stopwatch = Stopwatch.StartNew();
            long ticksToWait = microseconds * (Stopwatch.Frequency / 1_000_000);

            while (stopwatch.ElapsedTicks < ticksToWait)
            {
                // Busy-wait
            }

            stopwatch.Stop();
            await Task.Delay(0);
        }
    }
}