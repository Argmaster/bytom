using System;

namespace Bytom.Tools
{
    public class Serialization
    {
        public static byte[] UInt32ToBytesBigEndian(uint value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            return bytes;
        }
        public static byte[] Int32ToBytesBigEndian(int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            return bytes;
        }
        public static byte[] Float32ToBytesBigEndian(float value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            return bytes;
        }

        public static uint UInt32FromBytesBigEndian(byte[] bytes)
        {
            if (bytes.Length != 4)
            {
                throw new ArgumentException("bytes must be 4 bytes long");
            }

            byte[] bytesCopy = new byte[4];
            Array.Copy(bytes, bytesCopy, 4);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytesCopy);
            }
            return BitConverter.ToUInt32(bytesCopy);
        }

        public static int Int32FromBytesBigEndian(byte[] bytes)
        {
            if (bytes.Length != 4)
            {
                throw new ArgumentException("bytes must be 4 bytes long");
            }

            byte[] bytesCopy = new byte[4];
            Array.Copy(bytes, bytesCopy, 4);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytesCopy);
            }
            return BitConverter.ToInt32(bytesCopy);
        }

        public static float Float32FromBytesBigEndian(byte[] bytes)
        {
            if (bytes.Length != 4)
            {
                throw new ArgumentException("bytes must be 4 bytes long");
            }

            byte[] bytesCopy = new byte[4];
            Array.Copy(bytes, bytesCopy, 4);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytesCopy);
            }
            return BitConverter.ToSingle(bytesCopy);
        }

        public static uint Mask(uint length)
        {
            return (uint)((1UL << (int)length) - 1);
        }
    }
}