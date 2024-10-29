using System;
using System.Diagnostics;
using System.Threading;


namespace Bytom.Hardware
{
    public class Clock
    {
        public uint frequency_hz { get; }
        public Clock(uint frequency_hz)
        {
            this.frequency_hz = frequency_hz;
            if (this.frequency_hz > Stopwatch.Frequency)
            {
                throw new System.Exception("Clock frequency is too high for the system");
            }
        }

        public long getCycleLengthMicroseconds()
        {
            return 1_000_000 / frequency_hz;
        }

        public virtual TickDisposable startTick()
        {
            return new TickDisposable(this);
        }

        public void waitForCycles(uint cycles)
        {
            var microseconds = cycles * getCycleLengthMicroseconds();
            waitMicroseconds(microseconds);
        }

        public void waitMicroseconds(long microseconds)
        {
            if (microseconds >= 1000)
            {
                System.Threading.Thread.Sleep((int)(microseconds / 1000));
                return;
            }
            var stopwatch = Stopwatch.StartNew();
            long ticksToWait = microseconds * (Stopwatch.Frequency / 1_000_000);

            while (stopwatch.ElapsedTicks < ticksToWait)
            {
                // Busy-wait
            }

            stopwatch.Stop();
        }
    }

    public class TickDisposable : IDisposable
    {
        private Clock clock;
        private Stopwatch stopwatch;
        public TickDisposable(Clock clock)
        {
            this.clock = clock;
            this.stopwatch = Stopwatch.StartNew();
        }

        public void Dispose()
        {
            var ticks_per_cycle = Stopwatch.Frequency / clock.frequency_hz;
            while (stopwatch.ElapsedTicks < ticks_per_cycle)
            {
                // Busy-wait
            }
        }
    }

    public class PerformanceClock : Clock
    {
        public PerformanceClock(uint frequency_hz) : base(frequency_hz)
        {
        }
    }
}