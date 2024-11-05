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
        public FirmwareRom firmware_rom;
        public List<Device> devices;
        public List<AddressRange> address_ranges;
        public Address forward_allocation_address = Address.zero;
        public Address backward_allocation_address = Address.max_address;
        public AddressRange? ram_address_range;
        public AddressRange? firmware_address_range;
        public Motherboard? motherboard;
        public PowerStatus power_status = PowerStatus.OFF;

        public IoController(List<RAM> ram_slots, FirmwareRom firmware_rom, List<Device> devices)
        {
            this.ram_slots = ram_slots;
            this.firmware_rom = firmware_rom;
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
            if (firmware_rom.isInMyAddressRange(message.address))
            {
                firmware_rom.pushIoMessage(message);
                return;
            }
            foreach (var device in devices)
            {
                if (device.isInMyAddressRange(message.address))
                {
                    device.pushIoMessage(message);
                    return;
                }
            }
            Debug.Assert(false, $"Address {message.address:X8} not allocated to any device.");
        }

        public void powerOn(Motherboard? motherboard)
        {
            Debug.Assert(power_status != PowerStatus.ON, "MemoryController is powered on");
            power_status = PowerStatus.STARTING;

            this.motherboard = motherboard;

            Debug.Assert(address_ranges.Count == 0, "Address ranges occupied before power on");
            // Power on RAM devices and allocate address ranges for them.
            foreach (var ram in ram_slots)
            {
                ram.powerOn(this);
                var ram_slot_address_range = new AddressRange(forward_allocation_address, ram.getCapacityBytes());
                address_ranges.Add(ram_slot_address_range);
                ram.address_range = ram_slot_address_range;
                forward_allocation_address += ram.getCapacityBytes();
            }
            Debug.Assert(address_ranges.Count != 0, "No address ranges allocated to RAM devices.");
            ram_address_range = AddressRange.FromSpan(Address.zero, address_ranges.Last().end_address);

            // Power on firmware and allocate address range for it.
            firmware_rom.powerOn(this);
            firmware_address_range = allocateAddressRange(firmware_rom.getCapacityBytes());
            firmware_rom.address_range = firmware_address_range;
            address_ranges.Add(firmware_address_range);

            // Power on rest of the devices.
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

        public AddressRange getFirmwareAddressRange()
        {
            Debug.Assert(power_status != PowerStatus.OFF, "MemoryController is powered off");
            return firmware_address_range ?? throw new Exception("Firmware address range not allocated");
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

            firmware_rom.powerOff();
            Debug.Assert(firmware_rom.getPowerStatus() == PowerStatus.OFF, "Device is still powered on");

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
            Debug.Assert(!(ram_address_range is null), "RAM address range not allocated");

            var new_start_address = backward_allocation_address - size;
            Debug.Assert(new_start_address >= ram_address_range!.end_address, "Out of address space");

            var new_address_range = new AddressRange(new_start_address, size);
            address_ranges.Add(new_address_range);
            return new_address_range;
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