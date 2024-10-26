

using System;
using System.Threading.Tasks;

namespace Bytom.Hardware
{
    public class FirmwareRom
    {
        public uint capacity_bytes { get; }
        public uint read_latency_milliseconds { get; }
        public uint write_latency_milliseconds { get; }
        public Clock clock { get; }
        private byte[] memory;

        public FirmwareRom(
            uint capacity_bytes,
            uint read_latency_milliseconds = 0,
            uint write_latency_milliseconds = 0)
        {
            this.capacity_bytes = capacity_bytes;
            this.read_latency_milliseconds = read_latency_milliseconds;
            this.write_latency_milliseconds = write_latency_milliseconds;

            memory = new byte[this.capacity_bytes];
            clock = new Clock(0);
        }

        public void writeAll(long address, byte[] content)
        {
            clock.waitMicroseconds(write_latency_milliseconds * 1000);
            writeAllNoDelay(address, content);
        }

        public void writeAllNoDelay(long address, byte[] content)
        {
            if (address < 0 || address > capacity_bytes)
            {
                throw new Exception($"Writing outside of memory bounds 0x{address:X8}");
            }
            for (int i = 0; i < content.Length; i++)
            {
                memory[address + i] = content[i];
            }
        }

        public byte[] readAll(uint address, uint length)
        {
            clock.waitMicroseconds(read_latency_milliseconds * 1000);
            return readAllNoDelay(address, length);
        }

        public byte[] readAllNoDelay(uint address, uint length)
        {
            if (address < 0 || address > capacity_bytes)
            {
                throw new Exception($"Reading outside of memory bounds 0x{address:X8}");
            }
            return memory[new Index((int)address)..new Index((int)(address + length))];
        }
    }
}