using System;
using Bytom.Tools;

namespace Bytom.Hardware.CPU
{
    public class Register32
    {
        public byte[] value { get; set; }
        public int initial_value { get; }
        public bool write_kernel_only { get; }
        public bool read_kernel_only { get; }
        public bool no_move_write { get; }


        public Register32(
            int initial_value,
            bool write_kernel_only = false,
            bool read_kernel_only = false,
            bool no_move_write = false
        )
        {
            value = new byte[4] { 0, 0, 0, 0 };

            this.initial_value = initial_value;
            this.write_kernel_only = write_kernel_only;
            this.read_kernel_only = read_kernel_only;
            this.no_move_write = no_move_write;

            reset();
        }

        public void reset()
        {
            WriteInt32(initial_value);
        }

        public void WriteUInt32(uint value)
        {
            Serialization.UInt32ToBytesBigEndian(value).CopyTo(this.value, 0);
        }

        public uint ReadUInt32()
        {
            return Serialization.Uint32FromBytesBigEndian(value);
        }

        public void WriteInt32(int value)
        {
            Serialization.Int32ToBytesBigEndian(value).CopyTo(this.value, 0);
        }

        public int ReadInt32()
        {
            return Serialization.Int32FromBytesBigEndian(value);
        }

        public void WriteFloat32(float value)
        {
            Serialization.Float32ToBytesBigEndian(value).CopyTo(this.value, 0);
        }

        public float ReadFloat32()
        {
            return Serialization.Float32FromBytesBigEndian(value);
        }
        public void WriteBytes(byte[] value)
        {
            if (value.Length != 4)
            {
                throw new Exception("Invalid number of bytes");
            }
            Array.Copy(value, this.value, 4);
        }
        public byte[] ReadBytes()
        {
            return value;
        }
    }
}