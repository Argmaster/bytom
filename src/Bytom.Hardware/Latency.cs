
namespace Bytom.Hardware
{
    public class Latency
    {
        public long microseconds { get; }

        public static Latency zero { get { return new Latency(0); } }

        public Latency(long microseconds)
        {
            this.microseconds = microseconds;
        }

        public Latency FromMicroseconds(long microseconds)
        {
            return new Latency(microseconds);
        }

        public Latency FromMilliseconds(long milliseconds)
        {
            return new Latency(milliseconds * 1000);
        }

        public long ToMicroseconds()
        {
            return microseconds;
        }
    }
}