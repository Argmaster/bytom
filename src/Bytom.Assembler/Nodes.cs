using System;
using Bytom.Assembler.Operands;

namespace Bytom.Assembler.Instructions
{
    public class Node
    {
        public virtual uint GetSizeBits()
        {
            return 32;
        }
        public uint GetSizeBytes()
        {
            return GetSizeBits() / 8;
        }
        public virtual string ToAssembly()
        {
            throw new NotImplementedException();
        }
    }

    public class LabelNode : Node
    {
        public string name { get; set; }
        public LabelNode(string name)
        {
            this.name = name;
        }
        public override uint GetSizeBits()
        {
            return 0;
        }
        public override string ToAssembly()
        {
            return $"{name}:";
        }
    }

    public class Instruction : Node
    {
        public static readonly uint code = 0b0000_0000_0000_0000;

        public virtual uint GetOpCode()
        {
            return code;
        }
        public virtual byte[] ToMachineCode()
        {
            throw new NotImplementedException();
        }
    }

    public class Nop : Instruction
    {
        public static new readonly uint code = 0b0000_0000_0000_0000;
        public Nop()
        {
        }
        public override string ToAssembly()
        {
            return "nop";
        }
        public override byte[] ToMachineCode()
        {
            return new MachineInstructionBuilder(code)
                .GetInstruction();
        }
    }

    public class Halt : Instruction
    {
        public static new readonly uint code = 0b0000_0000_0000_0001;
        public Halt()
        {
        }
        public override string ToAssembly()
        {
            return "halt";
        }
        public override byte[] ToMachineCode()
        {
            return new MachineInstructionBuilder(code)
                .GetInstruction();
        }
    }

    public class MovRegReg : Instruction
    {
        public static new readonly uint code = 0b0000_0000_0000_0001;
        public Register destination { get; set; }
        public Register source { get; set; }
        public MovRegReg(Register destination, Register source)
        {
            this.destination = destination;
            this.source = source;
        }
        public override string ToAssembly()
        {
            return $"mov {destination.ToAssembly()}, {source.ToAssembly()}";
        }
        public override byte[] ToMachineCode()
        {
            return new MachineInstructionBuilder(code)
                .SetFirstOperandType(OperandType.REGISTER)
                .SetFirstRegisterID(destination.name)
                .SetSecondOperandType(OperandType.REGISTER)
                .SetSecondRegisterID(source.name)
                .GetInstruction();
        }
    }

    public class MovRegMem : Instruction
    {
        public static new readonly uint code = 0b0001_0000_0000_0001;
        public Register destination { get; set; }
        public MemoryAddress source { get; set; }
        public MovRegMem(Register destination, MemoryAddress source)
        {
            this.destination = destination;
            this.source = source;
        }
        public override string ToAssembly()
        {
            return $"mov {destination.ToAssembly()}, {source.ToAssembly()}";
        }
        public override byte[] ToMachineCode()
        {
            return new MachineInstructionBuilder(code)
                .SetFirstOperandType(OperandType.REGISTER)
                .SetFirstRegisterID(destination.name)
                .SetSecondOperandType(OperandType.MEMORY_ADDRESS)
                .SetSecondRegisterID(source.register)
                .GetInstruction();
        }
    }

    public class MovMemReg : Instruction
    {
        public static new readonly uint code = 0b0100_0000_0000_0001;
        public MemoryAddress destination { get; set; }
        public Register source { get; set; }
        public MovMemReg(MemoryAddress destination, Register source)
        {
            this.source = source;
            this.destination = destination;
        }
        public override string ToAssembly()
        {
            return $"mov {destination.ToAssembly()}, {source.ToAssembly()}";
        }
        public override byte[] ToMachineCode()
        {
            return new MachineInstructionBuilder(code)
                .SetFirstOperandType(OperandType.MEMORY_ADDRESS)
                .SetFirstRegisterID(destination.register)
                .SetSecondOperandType(OperandType.REGISTER)
                .SetSecondRegisterID(source.name)
                .GetInstruction();
        }
    }

    public class MovRegCon : Instruction
    {
        public static new readonly uint code = 0b0010_0000_0000_0001;
        public Register destination { get; set; }
        public Constant source { get; set; }
        public MovRegCon(Register destination, Constant source)
        {
            this.destination = destination;
            this.source = source;
        }
        public override uint GetSizeBits()
        {
            return 64;
        }
        public override string ToAssembly()
        {
            return $"mov {destination.ToAssembly()}, {source.ToAssembly()}";
        }
        public override byte[] ToMachineCode()
        {
            return new MachineInstructionBuilder(code)
                .SetFirstOperandType(OperandType.REGISTER)
                .SetFirstRegisterID(destination.name)
                .SetSecondOperandType(OperandType.CONSTANT)
                .SetConstant(source.GetBytes())
                .GetInstruction();
        }
    }

    public class MovMemCon : Instruction
    {
        public static new readonly uint code = 0b0110_0000_0000_0001;
        public MemoryAddress destination { get; set; }
        public Constant source { get; set; }
        public MovMemCon(MemoryAddress destination, Constant source)
        {
            this.destination = destination;
            this.source = source;
        }
        public override uint GetSizeBits()
        {
            return 64;
        }
        public override string ToAssembly()
        {
            return $"mov {destination.ToAssembly()}, {source.ToAssembly()}";
        }
        public override byte[] ToMachineCode()
        {
            return new MachineInstructionBuilder(code)
                .SetFirstOperandType(OperandType.MEMORY_ADDRESS)
                .SetFirstRegisterID(destination.register)
                .SetSecondOperandType(OperandType.CONSTANT)
                .SetConstant(source.GetBytes())
                .GetInstruction();
        }
    }

    public class PushReg : Instruction
    {
        public static new readonly uint code = 0b0000_0000_0000_0010;
        public Register source { get; set; }
        public PushReg(Register source)
        {
            this.source = source;
        }
        public override string ToAssembly()
        {
            return $"push {source.ToAssembly()}";
        }
        public override byte[] ToMachineCode()
        {
            return new MachineInstructionBuilder(code)
                .SetFirstOperandType(OperandType.REGISTER)
                .SetFirstRegisterID(source.name)
                .GetInstruction();
        }
    }

    public class PushMem : Instruction
    {
        public static new readonly uint code = 0b0100_0000_0000_0010;
        public MemoryAddress source { get; set; }
        public PushMem(MemoryAddress source)
        {
            this.source = source;
        }
        public override string ToAssembly()
        {
            return $"push {source.ToAssembly()}";
        }
        public override byte[] ToMachineCode()
        {
            return new MachineInstructionBuilder(code)
                .SetFirstOperandType(OperandType.MEMORY_ADDRESS)
                .SetFirstRegisterID(source.register)
                .GetInstruction();
        }
    }

    public class PushCon : Instruction
    {
        public static new readonly uint code = 0b1000_0000_0000_0010;
        public Constant source { get; set; }
        public PushCon(Constant source)
        {
            this.source = source;
        }
        public override uint GetSizeBits()
        {
            return 64;
        }
        public override string ToAssembly()
        {
            return $"push {source.ToAssembly()}";
        }
        public override byte[] ToMachineCode()
        {
            return new MachineInstructionBuilder(code)
                .SetFirstOperandType(OperandType.CONSTANT)
                .SetConstant(source.GetBytes())
                .GetInstruction();
        }
    }

    public class PopReg : Instruction
    {
        public static new readonly uint code = 0b0000_0000_0000_0011;
        public Register destination { get; set; }
        public PopReg(Register destination)
        {
            this.destination = destination;
        }
        public override string ToAssembly()
        {
            return $"pop {destination.ToAssembly()}";
        }
        public override byte[] ToMachineCode()
        {
            return new MachineInstructionBuilder(code)
                .SetFirstOperandType(OperandType.REGISTER)
                .SetFirstRegisterID(destination.name)
                .GetInstruction();
        }
    }

    public class PopMem : Instruction
    {
        public static new readonly uint code = 0b0100_0000_0000_0011;
        public MemoryAddress destination { get; set; }
        public PopMem(MemoryAddress destination)
        {
            this.destination = destination;
        }
        public override string ToAssembly()
        {
            return $"pop {destination.ToAssembly()}";
        }
        public override byte[] ToMachineCode()
        {
            return new MachineInstructionBuilder(code)
                .SetFirstOperandType(OperandType.MEMORY_ADDRESS)
                .SetFirstRegisterID(destination.register)
                .GetInstruction();
        }
    }



    public class Swap : Instruction
    {
        public static new readonly uint code = 0b0000_0000_0000_0100;
        public Register destination { get; set; }
        public Register source { get; set; }
        public Swap(Register destination, Register source)
        {
            this.destination = destination;
            this.source = source;
        }
        public override string ToAssembly()
        {
            return $"swap {destination.ToAssembly()}, {source.ToAssembly()}";
        }
        public override byte[] ToMachineCode()
        {
            return new MachineInstructionBuilder(code)
                .SetFirstOperandType(OperandType.REGISTER)
                .SetFirstRegisterID(destination.name)
                .SetSecondOperandType(OperandType.REGISTER)
                .SetSecondRegisterID(source.name)
                .GetInstruction();
        }
    }

    public class Add : Instruction
    {
        public static new readonly uint code = 0b0000_0000_0001_0000;
        public Register destination { get; set; }
        public Register source { get; set; }
        public Add(Register destination, Register source)
        {
            this.destination = destination;
            this.source = source;
        }
        public override string ToAssembly()
        {
            return $"add {destination.ToAssembly()}, {source.ToAssembly()}";
        }
    }

    public class Sub : Instruction
    {
        public static new readonly uint code = 0b0000_0000_0001_0001;
        public Register destination { get; set; }
        public Register source { get; set; }
        public Sub(Register destination, Register source)
        {
            this.destination = destination;
            this.source = source;
        }
        public override string ToAssembly()
        {
            return $"sub {destination.ToAssembly()}, {source.ToAssembly()}";
        }
    }

    public class Inc : Instruction
    {
        public static new readonly uint code = 0b0000_0000_0001_0010;
        public Register destination { get; set; }
        public Inc(Register destination)
        {
            this.destination = destination;
        }
        public override string ToAssembly()
        {
            return $"inc {destination.ToAssembly()}";
        }
    }

    public class Dec : Instruction
    {
        public static new readonly uint code = 0b0000_0000_0001_0011;
        public Register destination { get; set; }
        public Dec(Register destination)
        {
            this.destination = destination;
        }
        public override string ToAssembly()
        {
            return $"dec {destination.ToAssembly()}";
        }
    }

    public class Mul : Instruction
    {
        public static new readonly uint code = 0b0000_0000_0001_0100;
        public Register destination { get; set; }
        public Register source { get; set; }
        public Mul(Register destination, Register source)
        {
            this.destination = destination;
            this.source = source;
        }
        public override string ToAssembly()
        {
            return $"mul {destination.ToAssembly()}, {source.ToAssembly()}";
        }
    }

    public class IMul : Instruction
    {
        public static new readonly uint code = 0b0000_0000_0001_0101;
        public Register destination { get; set; }
        public Register source { get; set; }
        public IMul(Register destination, Register source)
        {
            this.destination = destination;
            this.source = source;
        }
        public override string ToAssembly()
        {
            return $"imul {destination.ToAssembly()}, {source.ToAssembly()}";
        }
    }

    public class Div : Instruction
    {
        public static new readonly uint code = 0b0000_0000_0001_0110;
        public Register destination { get; set; }
        public Register source { get; set; }
        public Div(Register destination, Register source)
        {
            this.destination = destination;
            this.source = source;
        }
        public override string ToAssembly()
        {
            return $"div {destination.ToAssembly()}, {source.ToAssembly()}";
        }
    }

    public class IDiv : Instruction
    {
        public static new readonly uint code = 0b0000_0000_0001_0111;
        public Register destination { get; set; }
        public Register source { get; set; }
        public IDiv(Register destination, Register source)
        {
            this.destination = destination;
            this.source = source;
        }
        public override string ToAssembly()
        {
            return $"idiv {destination.ToAssembly()}, {source.ToAssembly()}";
        }
    }

    public class And : Instruction
    {
        public static new readonly uint code = 0b0000_0000_0001_1000;
        public Register destination { get; set; }
        public Register source { get; set; }
        public And(Register destination, Register source)
        {
            this.destination = destination;
            this.source = source;
        }
        public override string ToAssembly()
        {
            return $"and {destination.ToAssembly()}, {source.ToAssembly()}";
        }
    }

    public class Or : Instruction
    {
        public static new readonly uint code = 0b0000_0000_0001_1001;
        public Register destination { get; set; }
        public Register source { get; set; }
        public Or(Register destination, Register source)
        {
            this.destination = destination;
            this.source = source;
        }
        public override string ToAssembly()
        {
            return $"or {destination.ToAssembly()}, {source.ToAssembly()}";
        }
    }

    public class Xor : Instruction
    {
        public static new readonly uint code = 0b0000_0000_0001_1010;
        public Register destination { get; set; }
        public Register source { get; set; }
        public Xor(Register destination, Register source)
        {
            this.destination = destination;
            this.source = source;
        }
        public override string ToAssembly()
        {
            return $"xor {destination.ToAssembly()}, {source.ToAssembly()}";
        }
    }

    public class Not : Instruction
    {
        public static new readonly uint code = 0b0000_0000_0001_1011;
        public Register destination { get; set; }
        public Not(Register destination)
        {
            this.destination = destination;
        }
        public override string ToAssembly()
        {
            return $"not {destination.ToAssembly()}";
        }
    }

    public class Shl : Instruction
    {
        public static new readonly uint code = 0b0000_0000_0001_1100;
        public Register destination { get; set; }
        public Register source { get; set; }
        public Shl(Register destination, Register source)
        {
            this.destination = destination;
            this.source = source;
        }
        public override string ToAssembly()
        {
            return $"shl {destination.ToAssembly()}, {source.ToAssembly()}";
        }
    }

    public class Shr : Instruction
    {
        public static new readonly uint code = 0b0000_0000_0001_1101;
        public Register destination { get; set; }
        public Register source { get; set; }
        public Shr(Register destination, Register source)
        {
            this.destination = destination;
            this.source = source;
        }
        public override string ToAssembly()
        {
            return $"shr {destination.ToAssembly()}, {source.ToAssembly()}";
        }
    }

    public class Fadd : Instruction
    {
        public static new readonly uint code = 0b0000_0000_0001_0000;
        public Register destination { get; set; }
        public Register source { get; set; }
        public Fadd(Register destination, Register source)
        {
            this.destination = destination;
            this.source = source;
        }
        public override string ToAssembly()
        {
            return $"fadd {destination.ToAssembly()}, {source.ToAssembly()}";
        }
    }

    public class Fsub : Instruction
    {
        public static new readonly uint code = 0b0000_0000_0001_0001;
        public Register destination { get; set; }
        public Register source { get; set; }
        public Fsub(Register destination, Register source)
        {
            this.destination = destination;
            this.source = source;
        }
        public override string ToAssembly()
        {
            return $"fsub {destination.ToAssembly()}, {source.ToAssembly()}";
        }
    }

    public class Fmul : Instruction
    {
        public static new readonly uint code = 0b0000_0000_0001_0010;
        public Register destination { get; set; }
        public Register source { get; set; }
        public Fmul(Register destination, Register source)
        {
            this.destination = destination;
            this.source = source;
        }
        public override string ToAssembly()
        {
            return $"fmul {destination.ToAssembly()}, {source.ToAssembly()}";
        }
    }

    public class Fdiv : Instruction
    {
        public static new readonly uint code = 0b0000_0000_0001_0011;
        public Register destination { get; set; }
        public Register source { get; set; }
        public Fdiv(Register destination, Register source)
        {
            this.destination = destination;
            this.source = source;
        }
        public override string ToAssembly()
        {
            return $"fdiv {destination.ToAssembly()}, {source.ToAssembly()}";
        }
    }

    public class Fcmp : Instruction
    {
        public static new readonly uint code = 0b0000_0000_0001_0100;
        public Register left { get; set; }
        public Register right { get; set; }
        public Fcmp(Register left, Register right)
        {
            this.left = left;
            this.right = right;
        }
        public override string ToAssembly()
        {
            return $"fcmp {left.ToAssembly()}, {right.ToAssembly()}";
        }
    }

    public class JumpMemoryAddressInstruction : Instruction
    {

        public MemoryAddress destination { get; set; }
        public JumpMemoryAddressInstruction(MemoryAddress destination)
        {
            this.destination = destination;
        }
    }

    public class LabelJumpInstruction : Instruction
    {

        public Label label { get; set; }

        public LabelJumpInstruction(Label label)
        {
            this.label = label;
        }
        public override uint GetSizeBits()
        {
            // push RDE  // 32 bits
            // push RDF  // 32 bits
            // mov RDE, IP  // 32 bits
            // mov RDF, <offset>  // 64 bits
            // add RDE, RDF  // 32 bits
            // pop RDE  // 32 bits
            // <j> [RDE]  // 32 bits
            return 32 + 32 + 32 + 64 + 32 + 32 + 32;
        }
        public virtual JumpMemoryAddressInstruction GetJumpInstruction(
            RegisterName name = RegisterName.RDF
        )
        {
            throw new NotImplementedException();
        }
    }

    public class JmpMem : JumpMemoryAddressInstruction
    {
        public static new readonly uint code = 0b0000_0000_0000_0010;
        public JmpMem(MemoryAddress destination) : base(destination)
        {
        }
        public override string ToAssembly()
        {
            return $"jmp {destination.ToAssembly()}";
        }
    }

    public class JmpLabel : LabelJumpInstruction
    {
        public static new readonly uint code = 0b0000_0000_0010_0000;

        public JmpLabel(Label label) : base(label)
        {
        }
        public override JumpMemoryAddressInstruction GetJumpInstruction(
            RegisterName name = RegisterName.RDF
        )
        {
            return new JmpMem(new MemoryAddress(name));
        }
        public override string ToAssembly()
        {
            return $"jmp {label.ToAssembly()}";
        }
    }

    public class JeqMem : JumpMemoryAddressInstruction
    {
        public static new readonly uint code = 0b0000_0000_0010_0001;
        public JeqMem(MemoryAddress destination) : base(destination)
        {
        }
        public override string ToAssembly()
        {
            return $"jeq {destination.ToAssembly()}";
        }
    }

    public class JeqLabel : LabelJumpInstruction
    {
        public static new readonly uint code = 0b0000_0000_0010_0001;

        public JeqLabel(Label label) : base(label)
        {
        }
        public override JumpMemoryAddressInstruction GetJumpInstruction(
            RegisterName name = RegisterName.RDF
        )
        {
            return new JeqMem(new MemoryAddress(name));
        }
        public override string ToAssembly()
        {
            return $"jeq {label.ToAssembly()}";
        }
    }

    public class JneMem : JumpMemoryAddressInstruction
    {
        public static new readonly uint code = 0b0000_0000_0010_0010;
        public JneMem(MemoryAddress destination) : base(destination)
        {
        }
        public override string ToAssembly()
        {
            return $"jne {destination.ToAssembly()}";
        }
    }

    public class JneLabel : LabelJumpInstruction
    {
        public static new readonly uint code = 0b0000_0000_0010_0010;

        public JneLabel(Label label) : base(label)
        {
        }
        public override JumpMemoryAddressInstruction GetJumpInstruction(
            RegisterName name = RegisterName.RDF
        )
        {
            return new JneMem(new MemoryAddress(name));
        }
        public override string ToAssembly()
        {
            return $"jne {label.ToAssembly()}";
        }
    }

    public class JltMem : JumpMemoryAddressInstruction
    {
        public static new readonly uint code = 0b0000_0000_0010_0011;
        public JltMem(MemoryAddress destination) : base(destination)
        {
        }
        public override string ToAssembly()
        {
            return $"jlt {destination.ToAssembly()}";
        }
    }

    public class JltLabel : LabelJumpInstruction
    {
        public static new readonly uint code = 0b0000_0000_0010_0011;

        public JltLabel(Label label) : base(label)
        {
        }
        public override JumpMemoryAddressInstruction GetJumpInstruction(
            RegisterName name = RegisterName.RDF
        )
        {
            return new JltMem(new MemoryAddress(name));
        }
        public override string ToAssembly()
        {
            return $"jlt {label.ToAssembly()}";
        }
    }

    public class JleMem : JumpMemoryAddressInstruction
    {
        public static new readonly uint code = 0b0000_0000_0010_0100;
        public JleMem(MemoryAddress destination) : base(destination)
        {
        }
        public override string ToAssembly()
        {
            return $"jle {destination.ToAssembly()}";
        }
    }

    public class JleLabel : LabelJumpInstruction
    {
        public static new readonly uint code = 0b0000_0000_0010_0100;

        public JleLabel(Label label) : base(label)
        {
        }
        public override JumpMemoryAddressInstruction GetJumpInstruction(
            RegisterName name = RegisterName.RDF
        )
        {
            return new JleMem(new MemoryAddress(name));
        }
        public override string ToAssembly()
        {
            return $"jle {label.ToAssembly()}";
        }
    }

    public class JgtMem : JumpMemoryAddressInstruction
    {
        public static new readonly uint code = 0b0000_0000_0010_0101;
        public JgtMem(MemoryAddress destination) : base(destination)
        {
        }
        public override string ToAssembly()
        {
            return $"jgt {destination.ToAssembly()}";
        }
    }

    public class JgtLabel : LabelJumpInstruction
    {
        public static new readonly uint code = 0b0000_0000_0010_0101;

        public JgtLabel(Label label) : base(label)
        {
        }
        public override JumpMemoryAddressInstruction GetJumpInstruction(
            RegisterName name = RegisterName.RDF
        )
        {
            return new JgtMem(new MemoryAddress(name));
        }
        public override string ToAssembly()
        {
            return $"jgt {label.ToAssembly()}";
        }
    }

    public class JgeMem : JumpMemoryAddressInstruction
    {
        public static new readonly uint code = 0b0000_0000_0010_0110;
        public JgeMem(MemoryAddress destination) : base(destination)
        {
        }
        public override string ToAssembly()
        {
            return $"jge {destination.ToAssembly()}";
        }
    }

    public class JgeLabel : LabelJumpInstruction
    {
        public static new readonly uint code = 0b0000_0000_0010_0110;

        public JgeLabel(Label label) : base(label)
        {
        }
        public override JumpMemoryAddressInstruction GetJumpInstruction(
            RegisterName name = RegisterName.RDF
        )
        {
            return new JgeMem(new MemoryAddress(name));
        }
        public override string ToAssembly()
        {
            return $"jge {label.ToAssembly()}";
        }
    }

    public class CallMem : JumpMemoryAddressInstruction
    {
        public static new readonly uint code = 0b0000_0000_0010_1000;
        public CallMem(MemoryAddress destination) : base(destination)
        {
        }
        public override string ToAssembly()
        {
            return $"call {destination.ToAssembly()}";
        }
    }

    public class CallLabel : LabelJumpInstruction
    {
        public static new readonly uint code = 0b0000_0000_0010_1000;

        public CallLabel(Label label) : base(label)
        {
        }
        public override JumpMemoryAddressInstruction GetJumpInstruction(
            RegisterName name = RegisterName.RDF
        )
        {
            return new CallMem(new MemoryAddress(name));
        }
        public override string ToAssembly()
        {
            return $"call {label.ToAssembly()}";
        }
    }

    public class Ret : Instruction
    {
        public static new readonly uint code = 0b0000_0000_0010_1001;
        public Ret()
        {
        }
        public override string ToAssembly()
        {
            return "ret";
        }
    }

    public class Cmp : Instruction
    {
        public static new readonly uint code = 0b0000_0000_0001_1111;
        public Register left { get; set; }
        public Register right { get; set; }
        public Cmp(Register left, Register right)
        {
            this.left = left;
            this.right = right;
        }
        public override string ToAssembly()
        {
            return $"cmp {left.ToAssembly()}, {right.ToAssembly()}";
        }
    }
}
