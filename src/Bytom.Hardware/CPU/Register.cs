using System;

namespace Bytom.Hardware.CPU
{
    public class Register32
    {
        public byte[] value { get; set; }

        public Register32()
        {
            this.value = new byte[4] { 0, 0, 0, 0 };
        }
        public void WriteUInt32(uint value)
        {
            this.value = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(this.value);
            }
        }

        public uint ReadUInt32()
        {
            byte[] reversed = value;
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(reversed);
            }
            return BitConverter.ToUInt32(reversed, 0);
        }

        public void WriteInt32(int value)
        {
            this.value = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(this.value);
            }
        }

        public int ReadInt32()
        {
            byte[] reversed = value;
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(reversed);
            }
            return BitConverter.ToInt32(reversed, 0);
        }

        public void WriteFloat32(float value)
        {
            this.value = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(this.value);
            }
        }

        public float ReadFloat32()
        {
            byte[] reversed = value;
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(reversed);
            }
            return BitConverter.ToSingle(reversed, 0);
        }
        public void WriteBytes(byte[] value)
        {
            if (value.Length != 4)
            {
                throw new Exception("Invalid number of bytes");
            }
            this.value = value;
        }
        public byte[] ReadBytes()
        {
            return value;
        }
    }
}