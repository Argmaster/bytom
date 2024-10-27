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

    public class OpRegister : Operand
    {
        public RegisterID name { get; set; }
        public OpRegister(RegisterID name)
        {
            this.name = name;
        }
        public override string ToAssembly()
        {
            return name.ToString();
        }
    }

    public class OpMemoryAddress : Operand
    {
        public RegisterID register { get; set; }
        public OpMemoryAddress(RegisterID register)
        {
            this.register = register;
        }
        public override string ToAssembly()
        {
            return $"[{register}]";
        }
    }

    public class OpConstant : Operand
    {
        public OpConstant()
        {
        }
        public virtual byte[] GetBytes()
        {
            throw new NotImplementedException();
        }
    }
    public class OpConstantInt : OpConstant
    {
        public static readonly int operand_type_code = 0b0010;
        public long value { get; set; }
        public OpConstantInt(long value)
        {
            this.value = value;
        }
        public override byte[] GetBytes()
        {
            return Serialization.Int32ToBytesBigEndian((int)value);
        }
        public override string ToAssembly()
        {
            return value.ToString();
        }
    }

    public class OpConstantFloat : OpConstant
    {
        public static readonly uint operand_type_code = 0b0011;
        public float value { get; set; }
        public OpConstantFloat(float value)
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

    public class OpLabel : Operand
    {
        public string name { get; set; }
        public OpLabel(string name)
        {
            this.name = name;
        }
        public override string ToAssembly()
        {
            return this.name;
        }
    }
}