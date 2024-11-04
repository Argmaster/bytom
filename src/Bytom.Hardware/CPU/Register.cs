using System;
using Bytom.Tools;

namespace Bytom.Hardware.CPU
{
    public abstract class Register
    {
        // Current value of the register as array of bytes big-endian.
        public byte[] value { get; set; }
        // Human readable name of the register used for debugging purposes.
        public string name { get; }

        public Register(byte[] value, string name = "Register")
        {
            if (value.Length != getSizeBytes())
            {
                throw new Exception("Invalid number of bytes");
            }
            this.value = value;
            this.name = name;
        }
        public abstract void reset();
        public abstract uint getSizeBytes();
        public void writeBytes(byte[] value)
        {
            if (value.Length != getSizeBytes())
            {
                throw new Exception("Invalid number of bytes");
            }
            Array.Copy(value, this.value, getSizeBytes());
        }

        public byte[] readBytes()
        {
            return value;
        }

        public override string ToString()
        {
            return $"Register::{name}({BitConverter.ToString(value)})";
        }
    }

    public class Register32 : Register
    {
        public uint initial_value { get; }
        public bool write_kernel_only { get; }
        public bool read_kernel_only { get; }
        public bool no_move_write { get; }


        public Register32(
            uint initial_value,
            bool write_kernel_only = false,
            bool read_kernel_only = false,
            bool no_move_write = false,
            string name = "Register32"
        ) : base(new byte[4] { 0, 0, 0, 0 }, name)
        {
            this.initial_value = initial_value;
            this.write_kernel_only = write_kernel_only;
            this.read_kernel_only = read_kernel_only;
            this.no_move_write = no_move_write;

            reset();
        }
        // Reset the register to its initial value.
        public override void reset()
        {
            writeUInt32(initial_value);
        }
        // Get size of register in bytes.
        public override uint getSizeBytes()
        {
            return 4;
        }
        // Overwrite value of register with unsigned 32-bit integer value.
        public void writeUInt32(uint value)
        {
            Serialization.UInt32ToBytesBigEndian(value).CopyTo(this.value, 0);
        }

        public uint readUInt32()
        {
            return Serialization.UInt32FromBytesBigEndian(value);
        }

        public void writeInt32(int value)
        {
            Serialization.Int32ToBytesBigEndian(value).CopyTo(this.value, 0);
        }

        public int readInt32()
        {
            return Serialization.Int32FromBytesBigEndian(value);
        }

        public void writeFloat32(float value)
        {
            Serialization.Float32ToBytesBigEndian(value).CopyTo(this.value, 0);
        }

        public float readFloat32()
        {
            return Serialization.Float32FromBytesBigEndian(value);
        }

        public bool readBit(int offset)
        {
            return (readUInt32() & (1u << offset)) != 0;
        }

        public void writeBit(int offset, bool set)
        {
            if (set)
            {
                writeUInt32(readUInt32() | (1u << offset));
            }
            else
            {
                writeUInt32(readUInt32() & ~(1u << offset));
            }
        }

        public Address readAddress()
        {
            return new Address(readUInt32());
        }

        public void writeAddress(Address address)
        {
            writeUInt32(address.ToUInt32());
        }

        public override string ToString()
        {
            return $"Register32::{name}({BitConverter.ToString(value)})";
        }
    }

    public class ConditionCodeRegister : Register32
    {
        public ConditionCodeRegister(string name = "ConditionCodeRegister")
            : base(0, name: name, no_move_write: true)
        {
        }

        public bool getZeroFlag()
        {
            return readBit(0);
        }
        public void setZeroFlag(bool value)
        {
            writeBit(0, value);
        }
        public bool getCarryFlag()
        {
            return readBit(1);
        }
        public void setCarryFlag(bool value)
        {
            writeBit(1, value);
        }
        public bool getSignFlag()
        {
            return readBit(2);
        }
        public void setSignFlag(bool value)
        {
            writeBit(2, value);
        }
        public bool getOverflowFlag()
        {
            return readBit(3);
        }
        public void setOverflowFlag(bool value)
        {
            writeBit(3, value);
        }
        public bool getZeroDivisionFlag()
        {
            return readBit(4);
        }
        public void setZeroDivisionFlag(bool value)
        {
            writeBit(4, value);
        }

        public bool isEqual()
        {
            return getZeroFlag();
        }
        public bool isNotEqual()
        {
            return !isEqual();
        }
        public bool isBelow()
        {
            return getCarryFlag();
        }
        public bool isBelowOrEqual()
        {
            return isBelow() || isEqual();
        }
        public bool isAbove()
        {
            return !getCarryFlag() && !getZeroFlag();
        }
        public bool isAboveOrEqual()
        {
            return isEqual() || isAbove();
        }
        public bool isLessThan()
        {
            return getSignFlag() != getOverflowFlag();
        }
        public bool isLessThanOrEqual()
        {
            return isEqual() || isLessThan();
        }
        public bool isGreaterThan()
        {
            return !getZeroFlag() && getSignFlag() == getOverflowFlag();
        }
        public bool isGreaterThanOrEqual()
        {
            return getSignFlag() == getOverflowFlag();
        }
    }
}