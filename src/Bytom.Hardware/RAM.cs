using System;


namespace Bytom.Hardware
{
    public class RAM : MessageReceiver
    {
        public uint capacity_bytes { get; }
        public uint write_latency_cycles { get; }
        public uint read_latency_cycles { get; }
        public byte[] memory { get; }

        public RAM(
            uint capacity_bytes,
            uint clock_speed_hz,
            uint write_latency_cycles,
            uint read_latency_cycles
        ) : base(new Clock(clock_speed_hz))
        {
            this.capacity_bytes = capacity_bytes;
            this.write_latency_cycles = write_latency_cycles;
            this.read_latency_cycles = read_latency_cycles;
            memory = new byte[capacity_bytes];
            Array.Fill<byte>(memory, 0);
        }

        public override void beforeThreadStart()
        {
            address_range = memory_controller!.allocateAddressRange(capacity_bytes);
        }

        public override void powerOff()
        {
            base.powerOff();
            Array.Fill<byte>(memory, 0);
        }

        public override void write(WriteMessage message)
        {
            clock.waitForCycles(write_latency_cycles);
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
            clock.waitForCycles(read_latency_cycles);
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

        public void writeDebug(byte[] data, int index)
        {
            data.CopyTo(memory, index);
        }

        public byte[] readDebug()
        {
            var data = new byte[memory.Length];
            memory.CopyTo(data, 0);
            return data;
        }
    }
}