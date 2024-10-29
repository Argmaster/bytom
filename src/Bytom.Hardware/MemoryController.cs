using System.Collections.Generic;
using System.Linq;


namespace Bytom.Hardware
{
    public class MemoryController
    {
        protected List<MessageReceiver> devices;
        protected List<AddressRange> ram_address_ranges;
        protected List<AddressRange> mapped_address_ranges;
        protected List<AddressRange> address_ranges;
        protected Motherboard? motherboard;
        protected PowerStatus power_status = PowerStatus.OFF;

        public MemoryController(List<MessageReceiver> devices)
        {
            this.devices = devices;
            ram_address_ranges = new List<AddressRange>();
            mapped_address_ranges = new List<AddressRange>();
            address_ranges = ram_address_ranges;
        }

        public void pushIoMessage(IoMessage message)
        {
            if (power_status == PowerStatus.OFF)
            {
                throw new System.Exception("MemoryController is powered off");
            }
            foreach (var device in devices)
            {
                if (device.isInMyAddressRange(message.address))
                {
                    device.pushIoMessage(message);
                }
            }
        }

        public void powerOn(Motherboard motherboard)
        {
            if (power_status == PowerStatus.ON)
            {
                throw new System.Exception("MemoryController is already powered on");
            }
            power_status = PowerStatus.STARTING;

            this.motherboard = motherboard;

            address_ranges = ram_address_ranges;
            foreach (var device in devices)
            {
                if (device is RAM)
                    device.powerOn(this);
            }

            if (ram_address_ranges.Count == 0)
            {
                throw new System.Exception("No RAM devices found");
            }

            address_ranges = mapped_address_ranges;
            mapped_address_ranges.Add(
                AddressRange.FromSpan(Address.zero, ram_address_ranges.Last().end_address)
            );
            foreach (var device in devices)
            {
                if (!(device is RAM))
                    device.powerOn(this);
            }
            power_status = PowerStatus.ON;
        }

        public AddressRange getRamAddressRange()
        {
            if (power_status == PowerStatus.OFF)
            {
                throw new System.Exception("MemoryController is powered off");
            }
            if (ram_address_ranges.Count == 0)
            {
                throw new System.Exception("No RAM devices found");
            }
            if (mapped_address_ranges.Count == 0)
            {
                throw new System.Exception("No mapped devices found");
            }
            return mapped_address_ranges.First();
        }

        public void powerOff()
        {
            if (power_status == PowerStatus.OFF)
            {
                throw new System.Exception("MemoryController is already powered off");
            }
            foreach (var device in devices)
            {
                device.powerOff();
            }
            ram_address_ranges.Clear();
            mapped_address_ranges.Clear();
            motherboard = null;
            power_status = PowerStatus.OFF;
        }

        public PowerStatus getPowerStatus()
        {
            return power_status;
        }

        public AddressRange allocateAddressRange(long size)
        {
            if (power_status == PowerStatus.OFF)
            {
                throw new System.Exception("MemoryController is powered off");
            }
            if (address_ranges.Count == 0)
            {
                address_ranges.Add(new AddressRange(Address.zero, size));
                return address_ranges.Last();
            }
            else
            {
                var last_address_range = address_ranges.Last();
                var new_address_range = new AddressRange(last_address_range.end_address, size);
                if (new_address_range.end_address.ToLong() > 0xFFFFFFFF)
                {
                    // Maybe at some point it could be useful to actually implement
                    // some algorithm to reuse reclaimed address space.
                    throw new System.Exception("Out of address space");
                }
                address_ranges.Add(new_address_range);
                return new_address_range;
            }
        }

        public void deallocateAddressRange(Address base_address)
        {
            if (power_status == PowerStatus.OFF)
            {
                throw new System.Exception("MemoryController is powered off");
            }
            for (int i = 0; i < address_ranges.Count; i++)
            {
                if (address_ranges[i].base_address == base_address)
                {
                    address_ranges.RemoveAt(i);
                    return;
                }
            }
            throw new System.Exception($"Base address 0x{base_address.ToLong():X8} not found");
        }
    }
}