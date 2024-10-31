using System;
using System.Collections;
using Bytom.Tools;


namespace Bytom.Hardware
{
    public class Memory : SingleAddressDevice
    {
        public uint capacity_bytes { get; }
        public uint write_latency_cycles { get; }
        public uint read_latency_cycles { get; }
        public byte[] memory { get; }

        public Memory(
            uint capacity_bytes,
            uint write_latency_cycles,
            uint read_latency_cycles,
            uint bandwidth_bytes,
            Clock clock
        ) : base(bandwidth_bytes, clock)
        {
            this.capacity_bytes = capacity_bytes;
            this.write_latency_cycles = write_latency_cycles;
            this.read_latency_cycles = read_latency_cycles;
            memory = new byte[capacity_bytes];
        }

        public override IEnumerable write(WriteMessage message)
        {
            foreach (var _ in Itertools.yieldVoidTimes(write_latency_cycles))
            { yield return null; }

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

        public override IEnumerable read(ReadMessage message)
        {
            foreach (var _ in Itertools.yieldVoidTimes(read_latency_cycles))
            { yield return null; }

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

        public uint getCapacityBytes()
        {
            return capacity_bytes;
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