using System.Linq;
using Bytom.Hardware.CPU;
using Bytom.Tools;

namespace Bytom.Assembler
{
    public enum InstructionSize
    {
        BITS_32 = 32,
        BITS_64 = 64
    }

    public class MachineInstructionBuilder
    {

        private uint instruction;
        private byte[] constant;

        public MachineInstructionBuilder(OpCode opcode)
        {
            this.instruction = (uint)opcode;
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

        public MachineInstructionBuilder SetFirstRegisterID(RegisterID id)
        {
            instruction |= ((uint)id & 0b1111_11) << (16 + 6);
            return this;
        }
        public MachineInstructionBuilder SetSecondRegisterID(RegisterID id)
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