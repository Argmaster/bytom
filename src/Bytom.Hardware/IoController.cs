using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;


namespace Bytom.Hardware
{
    public class IoController
    {
        public List<RAM> ram_slots;
        public List<Device> devices;
        public List<AddressRange> address_ranges;
        public Address forward_allocation_address = Address.zero;
        public Address backward_allocation_address = Address.max_address;
        public AddressRange? ram_address_range;
        public Motherboard? motherboard;
        public PowerStatus power_status = PowerStatus.OFF;

        public IoController(List<RAM> ram_slots, List<Device> devices)
        {
            this.ram_slots = ram_slots;
            Debug.Assert(ram_slots.Count != 0, "No RAM devices found");
            Debug.Assert(
                ram_slots.Aggregate(
                    true,
                    (acc, left) => acc && (left is RAM || left.GetType().IsSubclassOf(typeof(RAM)))
                ),
                "Not all devices are RAM devices"
            );

            this.devices = devices;

            address_ranges = new List<AddressRange>();
        }

        public void pushIoMessage(IoMessage message)
        {
            Debug.Assert(power_status != PowerStatus.OFF, "MemoryController is powered off");

            foreach (var ram in ram_slots)
            {
                if (ram.isInMyAddressRange(message.address))
                {
                    ram.pushIoMessage(message);
                    return;
                }
            }
            foreach (var device in devices)
            {
                if (device.isInMyAddressRange(message.address))
                {
                    device.pushIoMessage(message);
                    return;
                }
            }
        }

        public void powerOn(Motherboard? motherboard)
        {
            Debug.Assert(power_status != PowerStatus.ON, "MemoryController is powered on");
            power_status = PowerStatus.STARTING;

            this.motherboard = motherboard;

            foreach (var ram in ram_slots)
            {
                ram.powerOn(this);
                address_ranges.Add(new AddressRange(forward_allocation_address, ram.getCapacityBytes()));
                forward_allocation_address += ram.getCapacityBytes();
            }
            Debug.Assert(address_ranges.Count != 0, "No address ranges allocated to RAM devices.");
            ram_address_range = AddressRange.FromSpan(Address.zero, address_ranges.Last().end_address);

            foreach (var device in devices)
            {
                device.powerOn(this);
            }
            power_status = PowerStatus.ON;
        }

        public AddressRange getRamAddressRange()
        {
            Debug.Assert(power_status != PowerStatus.OFF, "MemoryController is powered off");
            return ram_address_range ?? throw new Exception("RAM address range not allocated");
        }

        public void powerOff()
        {
            Debug.Assert(power_status != PowerStatus.OFF, "MemoryController is powered off");
            power_status = PowerStatus.STARTING;

            foreach (var device in devices)
            {
                device.powerOff();
                Debug.Assert(device.getPowerStatus() == PowerStatus.OFF, "Device is still powered on");
            }
            foreach (var ram in ram_slots)
            {
                ram.powerOff();
                Debug.Assert(ram.getPowerStatus() == PowerStatus.OFF, "RAM is still powered on");
            }
            address_ranges.Clear();
            motherboard = null;
            power_status = PowerStatus.OFF;

        }

        public PowerStatus getPowerStatus()
        {
            return power_status;
        }

        public AddressRange allocateAddressRange(long size)
        {
            Debug.Assert(power_status != PowerStatus.OFF, "MemoryController is powered off");

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
                    throw new Exception("Out of address space");
                }
                address_ranges.Add(new_address_range);
                return new_address_range;
            }
        }

        public void deallocateAddressRange(Address base_address)
        {
            Debug.Assert(power_status != PowerStatus.OFF, "MemoryController is powered off");

            List<AddressRange> address_ranges = this.address_ranges;

            for (int i = 0; i < address_ranges.Count; i++)
            {
                if (address_ranges[i].base_address == base_address)
                {
                    address_ranges.RemoveAt(i);
                    return;
                }
            }
            throw new Exception($"Base address 0x{base_address.ToLong():X8} not found");
        }
    }
}