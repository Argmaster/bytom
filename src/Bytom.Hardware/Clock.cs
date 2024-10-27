using System.Diagnostics;


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
}