using System;

namespace Bytom.Assembler.Operands
{
    public class Operand
    {
        public Operand()
        {
        }
    }

    public enum RegisterName
    {
        RD0 = 0b00_0001,
        RD1 = 0b00_0010,
        RD2 = 0b00_0011,
        RD3 = 0b00_0100,
        RD4 = 0b00_0101,
        RD5 = 0b00_0110,
        RD6 = 0b00_0111,
        RD7 = 0b00_1000,
        CR0 = 0b10_0000,
        CSTP = 0b10_0100,
        CSBP = 0b10_0101,
        VATTA = 0b10_0110,
        SIGV = 0b10_0111,
        SIGHTA = 0b10_1000,
        IP = 0b10_1001,
        HWQ = 0b10_1010,
        HWQRA = 0b10_1011,
        PAGEF = 0b10_1100,
    }

    public class Register : Operand
    {
        public RegisterName name { get; set; }
        public Register(RegisterName name)
        {
            this.name = name;
        }
    }

    public class MemoryAddress : Operand
    {
        public RegisterName register { get; set; }
        public MemoryAddress(RegisterName register)
        {
            this.register = register;
        }
    }

    public class Constant : Operand
    {
        public Constant()
        {
        }
        public virtual byte[] GetBytes()
        {
            throw new NotImplementedException();
        }
    }
    public class ConstantInt : Constant
    {
        public static readonly uint operand_type_code = 0b0010;
        public uint value { get; set; }
        public ConstantInt(uint value)
        {
            this.value = value;
        }
        public override byte[] GetBytes()
        {
            return Serialization.Int32ToBytesBigEndian(value);
        }
    }

    public class ConstantFloat : Constant
    {
        public static readonly uint operand_type_code = 0b0011;
        public float value { get; set; }
        public ConstantFloat(float value)
        {
            this.value = value;
        }
        public override byte[] GetBytes()
        {
            return Serialization.Float32ToBytesBigEndian(value);
        }
    }

    public class Label : Operand
    {
        public string name { get; set; }
        public Label(string name)
        {
            this.name = name;
        }
    }
}