using System;

namespace Bytom.Hardware
{
    public class FirmwareRom : MessageReceiver
    {
        public Latency read_latency { get; }
        public Latency write_latency { get; }
        public long capacity_bytes { get; }
        private byte[] memory;

        public FirmwareRom(
            long capacity_bytes,
            Latency read_latency,
            Latency write_latency
        ) : base(new Clock(1))
        {
            this.capacity_bytes = capacity_bytes;
            this.read_latency = read_latency;
            this.write_latency = write_latency;
            if (capacity_bytes < 0)
            {
                throw new Exception("capacity_bytes must be greater than 0");
            }
            memory = new byte[this.capacity_bytes];
        }

        public override void write(WriteMessage message)
        {
            clock.waitMicroseconds(write_latency.ToMicroseconds());
            if (isInMyAddressRange(message.address))
            {
                var address = message.address - address_range!.base_address;
                memory[address.ToLong()] = message.data;
            }
            else
            {
                throw new Exception($"Address 0x{message.address.ToLong():X8} out of range");
            }
        }

        public override void read(ReadMessage message)
        {
            clock.waitMicroseconds(read_latency.ToMicroseconds());
            if (isInMyAddressRange(message.address))
            {
                var address = message.address - address_range!.base_address;
                var value = memory[address.ToLong()];
                message.writeBackQueue.Enqueue(new WriteMessage(message.address, value));
            }
            else
            {
                throw new Exception($"Address 0x{message.address.ToLong():X8} out of range");
            }
        }
    }
}