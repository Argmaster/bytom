using System;
using Bytom.Tools;

namespace Bytom.Hardware
{
    public class Address
    {
        private long address { get; }
        public static Address zero { get { return new Address(0); } }
        public static Address max_address { get { return new Address(uint.MaxValue); } }
        public Address(long address)
        {
            this.address = address;
        }
        public static bool operator ==(Address a, Address b)
        {
            return a.address == b.address;
        }
        public static bool operator !=(Address a, Address b)
        {
            return a.address != b.address;
        }
        public override bool Equals(object obj)
        {
            return obj is Address address && this == address;
        }
        public override int GetHashCode()
        {
            return address.GetHashCode();
        }
        public static bool operator >=(Address a, Address b)
        {
            return a.address >= b.address;
        }
        public static bool operator <=(Address a, Address b)
        {
            return a.address <= b.address;
        }
        public static bool operator >(Address a, Address b)
        {
            return a.address > b.address;
        }
        public static bool operator <(Address a, Address b)
        {
            return a.address < b.address;
        }
        public static Address operator +(Address a, long b)
        {
            return new Address(a.address + b);
        }
        public static Address operator -(Address a, Address b)
        {
            return new Address(a.address - b.address);
        }
        public static Address operator -(Address a, long b)
        {
            return new Address(a.address - b);
        }
        public long ToLong()
        {
            return address;
        }
        public uint ToUInt32()
        {
            return (uint)address;
        }
        public int ToInt32()
        {
            return (int)address;
        }
        public byte[] ToBytes()
        {
            return Serialization.UInt32ToBytesBigEndian((uint)address);
        }
        public override string ToString()
        {
            return $"Address(0x{address:X8})";
        }
    }

    public class AddressRange
    {
        // Address of the first byte in the range.
        public Address base_address { get; }
        // Size of the range in bytes.
        public long size { get; }
        // Address of the first byte after the range.
        public Address end_address { get { return base_address + size; } }

        public AddressRange(
            Address base_address,
            long size
        )
        {
            this.base_address = base_address;
            this.size = size;
        }

        public static AddressRange FromSpan(Address base_address, Address end_address)
        {
            return new AddressRange(base_address, (end_address - base_address).ToLong());
        }

        public bool contains(Address address)
        {
            return address >= base_address && address < end_address;
        }

        public override string ToString()
        {
            return $"AddressRange({base_address}, {end_address})";
        }

        public static bool operator ==(AddressRange a, AddressRange b)
        {
            return a.base_address == b.base_address && a.size == b.size;
        }

        public static bool operator !=(AddressRange a, AddressRange b)
        {
            return a.base_address != b.base_address || a.size != b.size;
        }

        public override bool Equals(object obj)
        {
            return obj is AddressRange range &&
                    base_address == range.base_address &&
                    size == range.size;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base_address, size);
        }
    }
}