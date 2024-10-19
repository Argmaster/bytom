using Bytom.Tools;


namespace Bytom.Hardware.CPU
{

    public enum OpCode
    {
        Nop = 0b0000_0000_0000_0000,
        Halt = 0b0000_0000_0001_0000,
        MovRegReg = 0b0000_0000_0010_0000,
        MovRegMem = 0b0000_0000_0010_0001,
        MovMemReg = 0b0000_0000_0010_0100,
        MovRegCon = 0b0000_0000_0010_0010,
        MovMemCon = 0b0000_0000_0010_0110,
        PushReg = 0b0000_0000_0011_0000,
        PushMem = 0b0000_0000_0011_0100,
        PushCon = 0b0000_0000_0011_1000,
        PopReg = 0b0000_0000_0100_0000,
        PopMem = 0b0000_0000_0100_0100,
        Swap = 0b0000_0000_0101_0000,
        Add = 0b0000_0001_0000_0000,
        Sub = 0b0000_0001_0001_0000,
        Inc = 0b0000_0001_0010_0000,
        Dec = 0b0000_0001_0011_0000,
        Mul = 0b0000_0001_0100_0000,
        IMul = 0b0000_0001_0101_0000,
        Div = 0b0000_0001_0110_0000,
        IDiv = 0b0000_0001_0111_0000,
        And = 0b0000_0001_1000_0000,
        Or = 0b0000_0001_1001_0000,
        Xor = 0b0000_0001_1010_0000,
        Not = 0b0000_0001_1011_0000,
        Shl = 0b0000_0001_1100_0000,
        Shr = 0b0000_0001_1101_0000,
        Fadd = 0b0000_0100_0000_0000,
        Fsub = 0b0000_0100_0001_0000,
        Fmul = 0b0000_0100_0100_0000,
        Fdiv = 0b0000_0100_0110_0000,
        Fcmp = 0b0000_0100_1111_0000,
        Jmp = 0b_0000_0010_0000_0000,
        Jeq = 0b0000_0010_0001_0000,
        Jne = 0b0000_0010_0010_0000,
        Jlt = 0b0000_0010_0011_0000,
        Jle = 0b0000_0010_0100_0000,
        Jgt = 0b0000_0010_0101_0000,
        Jge = 0b0000_0010_0110_0000,
        Call = 0b0000_0010_1000_0000,
        Ret = 0b0000_0010_1001_0000,
        Cmp = 0b0000_0001_1111_0000,
        In = 0b0000_1000_0001_0000,
        Out = 0b0000_1000_0010_0000,
    }

    internal class Util
    {
        public static uint Mask(int bits)
        {
            return (uint)((1 << bits) - 1);
        }

    }

    public class InstructionDecoder
    {
        private uint instruction;

        public InstructionDecoder(byte[] instruction)
        {
            if (instruction.Length != 4)
            {
                throw new System.ArgumentException("Instruction must be 4 bytes long");
            }
            this.instruction = Serialization.Uint32FromBytesBigEndian(instruction);
        }
        public OpCode GetOpCode()
        {
            return (OpCode)(instruction & ((1 << 16) - 1));
        }
        public RegisterID GetFirstRegisterID()
        {
            return (RegisterID)((instruction >> (16 + 6)) & Util.Mask(6));
        }
        public RegisterID GetSecondRegisterID()
        {
            return (RegisterID)((instruction >> 16) & Util.Mask(6));
        }
    }
}