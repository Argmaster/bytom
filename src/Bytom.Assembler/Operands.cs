using System;
using Bytom.Hardware.CPU;
using Bytom.Tools;

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

    public class Register : Operand
    {
        public RegisterID name { get; set; }
        public Register(RegisterID name)
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
        public RegisterID register { get; set; }
        public MemoryAddress(RegisterID register)
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
            return Serialization.Int32ToBytesBigEndian(value);
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
            return Serialization.Float32ToBytesBigEndian(value);
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