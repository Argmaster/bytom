using System;

namespace Bytom.Assembler.Operands
{
    public class Operand
    {
        public Operand()
        {
        }
        public virtual string ToAssembly()
        {
            throw new NotImplementedException();
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
        RD8 = 0b00_1001,
        RD9 = 0b00_1010,
        RDA = 0b00_1011,
        RDB = 0b00_1100,
        RDC = 0b00_1101,
        RDD = 0b00_1110,
        RDE = 0b00_1111,
        RDF = 0b01_0000,
        CR0 = 0b10_0000,
        CSTP = 0b10_0100,
        CSBP = 0b10_0101,
        VATTA = 0b10_0110,
        IDT = 0b10_0111,
        IP = 0b10_1001,
    }

    public class Register : Operand
    {
        public RegisterName name { get; set; }
        public Register(RegisterName name)
        {
            this.name = name;
        }
        public override string ToAssembly()
        {
            return name.ToString();
        }
    }

    public class MemoryAddress : Operand
    {
        public RegisterName register { get; set; }
        public MemoryAddress(RegisterName register)
        {
            this.register = register;
        }
        public override string ToAssembly()
        {
            return $"[{register}]";
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
        public static readonly int operand_type_code = 0b0010;
        public int value { get; set; }
        public ConstantInt(int value)
        {
            this.value = value;
        }
        public override byte[] GetBytes()
        {
            return Serialization.ToBytesBigEndian(value);
        }
        public override string ToAssembly()
        {
            return value.ToString();
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
            return Serialization.ToBytesBigEndian(value);
        }
        public override string ToAssembly()
        {
            return value.ToString();
        }
    }

    public class Label : Operand
    {
        public string name { get; set; }
        public Label(string name)
        {
            this.name = name;
        }
        public override string ToAssembly()
        {
            return this.name;
        }
    }
}