using System;
using System.Linq;
using Bytom.Assembler.Operands;

namespace Bytom.Assembler
{
    public class Serialization
    {
        public static byte[] ToBytesBigEndian(uint value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            return bytes;
        }
        public static byte[] ToBytesBigEndian(int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            return bytes;
        }
        public static byte[] ToBytesBigEndian(float value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            return bytes;
        }

        public static uint Uint32FromBytesBigEndian(byte[] bytes)
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
            return (uint)((1 << (int)length) - 1);
        }
    }

    public enum InstructionSize
    {
        BITS_32 = 32,
        BITS_64 = 64
    }

    public enum OperandType
    {
        REGISTER,
        MEMORY_ADDRESS,
        CONSTANT
    }

    public class MachineInstructionBuilder
    {

        public uint opcode { get; set; }
        private uint instruction;
        private byte[] constant;

        public MachineInstructionBuilder(uint opcode)
        {
            this.opcode = opcode;
            this.instruction = opcode;
            this.constant = new byte[0];
        }
        public void SetBit(int bit, bool value)
        {
            if (value)
            {
                instruction |= (uint)(1 << bit);
            }
            else
            {
                instruction &= (uint)~(1 << bit);
            }
        }

        public MachineInstructionBuilder SetInstructionSize(InstructionSize size)
        {
            if (size == InstructionSize.BITS_32)
            {
                SetBit(31, false);
            }
            else if (size == InstructionSize.BITS_64)
            {
                SetBit(31, true);
            }
            return this;
        }

        public MachineInstructionBuilder SetFirstOperandType(OperandType t)
        {
            if (t == OperandType.REGISTER)
            {
                SetBit(15, false);
                SetBit(14, false);
            }
            else if (t == OperandType.MEMORY_ADDRESS)
            {
                SetBit(15, false);
                SetBit(14, true);
            }
            else if (t == OperandType.CONSTANT)
            {
                SetBit(15, true);
                SetBit(14, false);
            }
            return this;
        }
        public MachineInstructionBuilder SetSecondOperandType(OperandType t)
        {
            if (t == OperandType.REGISTER)
            {
                SetBit(13, false);
                SetBit(12, false);
            }
            else if (t == OperandType.MEMORY_ADDRESS)
            {
                SetBit(13, false);
                SetBit(12, true);
            }
            else if (t == OperandType.CONSTANT)
            {
                SetBit(13, true);
                SetBit(12, false);
            }
            return this;
        }
        public MachineInstructionBuilder SetFirstRegisterID(RegisterName id)
        {
            instruction |= ((uint)id & 0b1111_11) << (16 + 6);
            return this;
        }
        public MachineInstructionBuilder SetSecondRegisterID(RegisterName id)
        {
            instruction |= ((uint)id & 0b11_1111) << 16;
            return this;
        }
        public MachineInstructionBuilder SetConstant(byte[] value)
        {
            constant = value;
            return this;
        }
        public byte[] GetInstruction()
        {
            return Serialization.ToBytesBigEndian(instruction).Concat(constant).ToArray();
        }
    }
}