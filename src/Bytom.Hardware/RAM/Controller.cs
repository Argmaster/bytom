using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Bytom.Hardware.RAM
{
    public class Memory
    {
        private MemoryChip stick { get; }
        public long start_address { get; }
        public long end_address { get; }

        public Memory(MemoryChip stick, long start_address, long end_address)
        {
            this.stick = stick;
            this.start_address = start_address;
            this.end_address = end_address;
        }

        public async Task waitUntilReady()
        {
            await stick.waitUntilReady();
        }

        public async Task writeNoDelay(long global_address, byte content)
        {
            var local_address = global_address - start_address;
            if (global_address < start_address || global_address >= end_address)
            {
                throw new Exception(
                    $"Writing outside of memory bounds 0x{global_address:X8} [local: {local_address}]  (0x{start_address:X8} - 0x{end_address:X8})"
                );
            }
            await stick.writeNoDelay(local_address, content);
        }

        public async Task<byte> readNoDelay(long global_address)
        {
            var local_address = global_address - start_address;
            if (global_address < start_address || global_address >= end_address)
            {
                throw new Exception(
                    $"Reading outside of memory bounds 0x{global_address:X8} [local: {local_address}] (0x{start_address:X8} - 0x{end_address:X8})"
                );
            }
            return await stick.readNoDelay(local_address);
        }

        public uint getCapacityBytes()
        {
            return stick.capacity_bytes;
        }
    }

    public class Controller
    {
        private List<Memory> memory_banks { get; }

        public Controller(List<MemoryChip> sticks)
        {
            memory_banks = new List<Memory>();
            long address = 0;

            foreach (var stick in sticks)
            {
                memory_banks.Add(new Memory(stick, address, address + stick.capacity_bytes));
                address += stick.capacity_bytes;
            }
        }

        public async Task writeAll(long address, byte[] content)
        {
            long content_index = 0;
            long current_global_address = address;

            foreach (var bank in memory_banks)
            {
                if (current_global_address >= bank.start_address && current_global_address < bank.end_address)
                {
                    List<Task> tasks = new List<Task>();
                    for (
                        ;
                        current_global_address < bank.end_address && content_index < content.Length;
                        current_global_address++, content_index++
                    )
                    {
                        tasks.Add(bank.writeNoDelay(current_global_address, content[content_index]));
                    }
                    tasks.Add(bank.waitUntilReady());
                    await Task.WhenAll(tasks.ToArray());

                    if (content_index >= content.Length)
                    {
                        break;
                    }
                }
            }
        }

        public uint getTotalMemoryBytes()
        {
            uint total = 0;
            foreach (var bank in memory_banks)
            {
                total += bank.getCapacityBytes();
            }
            return total;
        }

        public async Task<byte[]> readAll(long address, long length)
        {
            long content_index = 0;
            long current_global_address = address;

            byte[] content = new byte[length];

            foreach (var bank in memory_banks)
            {
                if (current_global_address >= bank.start_address && current_global_address < bank.end_address)
                {
                    for (
                        ;
                        current_global_address < bank.end_address && content_index < length;
                        current_global_address++, content_index++)
                    {
                        content[content_index] = await bank.readNoDelay(current_global_address);
                    }
                    await bank.waitUntilReady();

                    if (content_index >= content.Length)
                    {
                        break;
                    }
                }
            }
            return content;
        }
    }
}