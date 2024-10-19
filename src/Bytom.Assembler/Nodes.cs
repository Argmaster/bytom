using System;
using Bytom.Assembler.Operands;
using Bytom.Hardware.CPU;

namespace Bytom.Assembler.Nodes
{
    public class Node
    {
        public virtual uint GetSizeBits()
        {
            throw new NotImplementedException();
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
        public override uint GetSizeBits()
        {
            return 32;
        }

        public virtual OpCode GetOpCode()
        {
            throw new NotImplementedException();
        }

        public virtual byte[] ToMachineCode()
        {
            throw new NotImplementedException();
        }
    }

    public class Nop : Instruction
    {
        public Nop()
        {
        }

        public override OpCode GetOpCode()
        {
            return OpCode.Nop;
        }

        public override string ToAssembly()
        {
            return "nop";
        }

        public override byte[] ToMachineCode()
        {
            return new MachineInstructionBuilder(GetOpCode())
                .GetInstruction();
        }
    }

    public class Halt : Instruction
    {
        public Halt()
        {
        }

        public override OpCode GetOpCode()
        {
            return OpCode.Halt;
        }

        public override string ToAssembly()
        {
            return "halt";
        }

        public override byte[] ToMachineCode()
        {
            return new MachineInstructionBuilder(GetOpCode())
                .GetInstruction();
        }
    }

    public class MovRegReg : Instruction
    {
        public Register destination { get; set; }
        public Register source { get; set; }

        public MovRegReg(Register destination, Register source)
        {
            this.destination = destination;
            this.source = source;
        }

        public override OpCode GetOpCode()
        {
            return OpCode.MovRegReg;
        }

        public override string ToAssembly()
        {
            return $"mov {destination.ToAssembly()}, {source.ToAssembly()}";
        }

        public override byte[] ToMachineCode()
        {
            return new MachineInstructionBuilder(GetOpCode())
                .SetFirstRegisterID(destination.name)
                .SetSecondRegisterID(source.name)
                .GetInstruction();
        }
    }

    public class MovRegMem : Instruction
    {
        public Register destination { get; set; }
        public MemoryAddress source { get; set; }

        public MovRegMem(Register destination, MemoryAddress source)
        {
            this.destination = destination;
            this.source = source;
        }

        public override OpCode GetOpCode()
        {
            return OpCode.MovRegMem;
        }

        public override string ToAssembly()
        {
            return $"mov {destination.ToAssembly()}, {source.ToAssembly()}";
        }

        public override byte[] ToMachineCode()
        {
            return new MachineInstructionBuilder(GetOpCode())
                .SetFirstRegisterID(destination.name)
                .SetSecondRegisterID(source.register)
                .GetInstruction();
        }
    }

    public class MovMemReg : Instruction
    {
        public MemoryAddress destination { get; set; }
        public Register source { get; set; }

        public MovMemReg(MemoryAddress destination, Register source)
        {
            this.source = source;
            this.destination = destination;
        }

        public override OpCode GetOpCode()
        {
            return OpCode.MovMemReg;
        }

        public override string ToAssembly()
        {
            return $"mov {destination.ToAssembly()}, {source.ToAssembly()}";
        }

        public override byte[] ToMachineCode()
        {
            return new MachineInstructionBuilder(GetOpCode())
                .SetFirstRegisterID(destination.register)
                .SetSecondRegisterID(source.name)
                .GetInstruction();
        }
    }

    public class MovRegCon : Instruction
    {
        public Register destination { get; set; }
        public Constant source { get; set; }

        public MovRegCon(Register destination, Constant source)
        {
            this.destination = destination;
            this.source = source;
        }

        public override OpCode GetOpCode()
        {
            return OpCode.MovRegCon;
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
            return new MachineInstructionBuilder(GetOpCode())
                .SetFirstRegisterID(destination.name)
                .SetConstant(source.GetBytes())
                .GetInstruction();
        }
    }

    public class MovMemCon : Instruction
    {
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

        public override OpCode GetOpCode()
        {
            return OpCode.MovMemCon;
        }

        public override string ToAssembly()
        {
            return $"mov {destination.ToAssembly()}, {source.ToAssembly()}";
        }

        public override byte[] ToMachineCode()
        {
            return new MachineInstructionBuilder(GetOpCode())
                .SetFirstRegisterID(destination.register)
                .SetConstant(source.GetBytes())
                .GetInstruction();
        }
    }

    public class PushReg : Instruction
    {
        public Register source { get; set; }

        public PushReg(Register source)
        {
            this.source = source;
        }

        public override OpCode GetOpCode()
        {
            return OpCode.PushReg;
        }

        public override string ToAssembly()
        {
            return $"push {source.ToAssembly()}";
        }

        public override byte[] ToMachineCode()
        {
            return new MachineInstructionBuilder(GetOpCode())
                .SetFirstRegisterID(source.name)
                .GetInstruction();
        }
    }

    public class PushMem : Instruction
    {
        public MemoryAddress source { get; set; }

        public PushMem(MemoryAddress source)
        {
            this.source = source;
        }

        public override OpCode GetOpCode()
        {
            return OpCode.PushMem;
        }

        public override string ToAssembly()
        {
            return $"push {source.ToAssembly()}";
        }

        public override byte[] ToMachineCode()
        {
            return new MachineInstructionBuilder(GetOpCode())
                .SetFirstRegisterID(source.register)
                .GetInstruction();
        }
    }

    public class PushCon : Instruction
    {
        public Constant source { get; set; }

        public PushCon(Constant source)
        {
            this.source = source;
        }

        public override OpCode GetOpCode()
        {
            return OpCode.PushCon;
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
            return new MachineInstructionBuilder(GetOpCode())
                .SetConstant(source.GetBytes())
                .GetInstruction();
        }
    }

    public class PopReg : Instruction
    {
        public Register destination { get; set; }

        public PopReg(Register destination)
        {
            this.destination = destination;
        }

        public override OpCode GetOpCode()
        {
            return OpCode.PopReg;
        }

        public override string ToAssembly()
        {
            return $"pop {destination.ToAssembly()}";
        }

        public override byte[] ToMachineCode()
        {
            return new MachineInstructionBuilder(GetOpCode())
                .SetFirstRegisterID(destination.name)
                .GetInstruction();
        }
    }

    public class PopMem : Instruction
    {
        public MemoryAddress destination { get; set; }

        public PopMem(MemoryAddress destination)
        {
            this.destination = destination;
        }

        public override OpCode GetOpCode()
        {
            return OpCode.PopMem;
        }

        public override string ToAssembly()
        {
            return $"pop {destination.ToAssembly()}";
        }

        public override byte[] ToMachineCode()
        {
            return new MachineInstructionBuilder(GetOpCode())
                .SetFirstRegisterID(destination.register)
                .GetInstruction();
        }
    }



    public class Swap : Instruction
    {
        public Register destination { get; set; }
        public Register source { get; set; }

        public Swap(Register destination, Register source)
        {
            this.destination = destination;
            this.source = source;
        }

        public override OpCode GetOpCode()
        {
            return OpCode.Swap;
        }

        public override string ToAssembly()
        {
            return $"swap {destination.ToAssembly()}, {source.ToAssembly()}";
        }

        public override byte[] ToMachineCode()
        {
            return new MachineInstructionBuilder(GetOpCode())
                .SetFirstRegisterID(destination.name)
                .SetSecondRegisterID(source.name)
                .GetInstruction();
        }
    }

    public class Add : Instruction
    {
        public Register destination { get; set; }
        public Register source { get; set; }

        public Add(Register destination, Register source)
        {
            this.destination = destination;
            this.source = source;
        }

        public override OpCode GetOpCode()
        {
            return OpCode.Add;
        }

        public override string ToAssembly()
        {
            return $"add {destination.ToAssembly()}, {source.ToAssembly()}";
        }

        public override byte[] ToMachineCode()
        {
            return new MachineInstructionBuilder(GetOpCode())
                .SetFirstRegisterID(destination.name)
                .SetSecondRegisterID(source.name)
                .GetInstruction();
        }
    }

    public class Sub : Instruction
    {
        public Register destination { get; set; }
        public Register source { get; set; }

        public Sub(Register destination, Register source)
        {
            this.destination = destination;
            this.source = source;
        }

        public override OpCode GetOpCode()
        {
            return OpCode.Sub;
        }

        public override string ToAssembly()
        {
            return $"sub {destination.ToAssembly()}, {source.ToAssembly()}";
        }

        public override byte[] ToMachineCode()
        {
            return new MachineInstructionBuilder(GetOpCode())
                .SetFirstRegisterID(destination.name)
                .SetSecondRegisterID(source.name)
                .GetInstruction();
        }
    }

    public class Inc : Instruction
    {
        public Register destination { get; set; }

        public Inc(Register destination)
        {
            this.destination = destination;
        }

        public override OpCode GetOpCode()
        {
            return OpCode.Inc;
        }

        public override string ToAssembly()
        {
            return $"inc {destination.ToAssembly()}";
        }

        public override byte[] ToMachineCode()
        {
            return new MachineInstructionBuilder(GetOpCode())
                .SetFirstRegisterID(destination.name)
                .GetInstruction();
        }
    }

    public class Dec : Instruction
    {
        public Register destination { get; set; }

        public Dec(Register destination)
        {
            this.destination = destination;
        }

        public override OpCode GetOpCode()
        {
            return OpCode.Dec;
        }

        public override string ToAssembly()
        {
            return $"dec {destination.ToAssembly()}";
        }

        public override byte[] ToMachineCode()
        {
            return new MachineInstructionBuilder(GetOpCode())
                .SetFirstRegisterID(destination.name)
                .GetInstruction();
        }
    }

    public class Mul : Instruction
    {
        public Register destination { get; set; }
        public Register source { get; set; }

        public Mul(Register destination, Register source)
        {
            this.destination = destination;
            this.source = source;
        }

        public override OpCode GetOpCode()
        {
            return OpCode.Mul;
        }

        public override string ToAssembly()
        {
            return $"mul {destination.ToAssembly()}, {source.ToAssembly()}";
        }

        public override byte[] ToMachineCode()
        {
            return new MachineInstructionBuilder(GetOpCode())
                .SetFirstRegisterID(destination.name)
                .SetSecondRegisterID(source.name)
                .GetInstruction();
        }
    }

    public class IMul : Instruction
    {
        public Register destination { get; set; }
        public Register source { get; set; }

        public IMul(Register destination, Register source)
        {
            this.destination = destination;
            this.source = source;
        }

        public override OpCode GetOpCode()
        {
            return OpCode.IMul;
        }

        public override string ToAssembly()
        {
            return $"imul {destination.ToAssembly()}, {source.ToAssembly()}";
        }

        public override byte[] ToMachineCode()
        {
            return new MachineInstructionBuilder(GetOpCode())
                .SetFirstRegisterID(destination.name)
                .SetSecondRegisterID(source.name)
                .GetInstruction();
        }
    }

    public class Div : Instruction
    {
        public Register destination { get; set; }
        public Register source { get; set; }

        public Div(Register destination, Register source)
        {
            this.destination = destination;
            this.source = source;
        }

        public override OpCode GetOpCode()
        {
            return OpCode.Div;
        }

        public override string ToAssembly()
        {
            return $"div {destination.ToAssembly()}, {source.ToAssembly()}";
        }

        public override byte[] ToMachineCode()
        {
            return new MachineInstructionBuilder(GetOpCode())
                .SetFirstRegisterID(destination.name)
                .SetSecondRegisterID(source.name)
                .GetInstruction();
        }
    }

    public class IDiv : Instruction
    {
        public Register destination { get; set; }
        public Register source { get; set; }

        public IDiv(Register destination, Register source)
        {
            this.destination = destination;
            this.source = source;
        }

        public override OpCode GetOpCode()
        {
            return OpCode.IDiv;
        }

        public override string ToAssembly()
        {
            return $"idiv {destination.ToAssembly()}, {source.ToAssembly()}";
        }

        public override byte[] ToMachineCode()
        {
            return new MachineInstructionBuilder(GetOpCode())
                .SetFirstRegisterID(destination.name)
                .SetSecondRegisterID(source.name)
                .GetInstruction();
        }
    }

    public class And : Instruction
    {
        public Register destination { get; set; }
        public Register source { get; set; }

        public And(Register destination, Register source)
        {
            this.destination = destination;
            this.source = source;
        }

        public override OpCode GetOpCode()
        {
            return OpCode.And;
        }

        public override string ToAssembly()
        {
            return $"and {destination.ToAssembly()}, {source.ToAssembly()}";
        }

        public override byte[] ToMachineCode()
        {
            return new MachineInstructionBuilder(GetOpCode())
                .SetFirstRegisterID(destination.name)
                .SetSecondRegisterID(source.name)
                .GetInstruction();
        }
    }

    public class Or : Instruction
    {
        public Register destination { get; set; }
        public Register source { get; set; }

        public Or(Register destination, Register source)
        {
            this.destination = destination;
            this.source = source;
        }

        public override OpCode GetOpCode()
        {
            return OpCode.Or;
        }

        public override string ToAssembly()
        {
            return $"or {destination.ToAssembly()}, {source.ToAssembly()}";
        }

        public override byte[] ToMachineCode()
        {
            return new MachineInstructionBuilder(GetOpCode())
                .SetFirstRegisterID(destination.name)
                .SetSecondRegisterID(source.name)
                .GetInstruction();
        }
    }

    public class Xor : Instruction
    {
        public Register destination { get; set; }
        public Register source { get; set; }

        public Xor(Register destination, Register source)
        {
            this.destination = destination;
            this.source = source;
        }

        public override OpCode GetOpCode()
        {
            return OpCode.Xor;
        }

        public override string ToAssembly()
        {
            return $"xor {destination.ToAssembly()}, {source.ToAssembly()}";
        }

        public override byte[] ToMachineCode()
        {
            return new MachineInstructionBuilder(GetOpCode())
                .SetFirstRegisterID(destination.name)
                .SetSecondRegisterID(source.name)
                .GetInstruction();
        }
    }

    public class Not : Instruction
    {
        public Register destination { get; set; }

        public Not(Register destination)
        {
            this.destination = destination;
        }

        public override OpCode GetOpCode()
        {
            return OpCode.Not;
        }

        public override string ToAssembly()
        {
            return $"not {destination.ToAssembly()}";
        }

        public override byte[] ToMachineCode()
        {
            return new MachineInstructionBuilder(GetOpCode())
                .SetFirstRegisterID(destination.name)
                .GetInstruction();
        }
    }

    public class Shl : Instruction
    {
        public Register destination { get; set; }
        public Register source { get; set; }

        public Shl(Register destination, Register source)
        {
            this.destination = destination;
            this.source = source;
        }

        public override OpCode GetOpCode()
        {
            return OpCode.Shl;
        }

        public override string ToAssembly()
        {
            return $"shl {destination.ToAssembly()}, {source.ToAssembly()}";
        }

        public override byte[] ToMachineCode()
        {
            return new MachineInstructionBuilder(GetOpCode())
                .SetFirstRegisterID(destination.name)
                .SetSecondRegisterID(source.name)
                .GetInstruction();
        }
    }

    public class Shr : Instruction
    {
        public Register destination { get; set; }
        public Register source { get; set; }

        public Shr(Register destination, Register source)
        {
            this.destination = destination;
            this.source = source;
        }

        public override OpCode GetOpCode()
        {
            return OpCode.Shr;
        }

        public override string ToAssembly()
        {
            return $"shr {destination.ToAssembly()}, {source.ToAssembly()}";
        }

        public override byte[] ToMachineCode()
        {
            return new MachineInstructionBuilder(GetOpCode())
                .SetFirstRegisterID(destination.name)
                .SetSecondRegisterID(source.name)
                .GetInstruction();
        }
    }

    public class Fadd : Instruction
    {
        public Register destination { get; set; }
        public Register source { get; set; }

        public Fadd(Register destination, Register source)
        {
            this.destination = destination;
            this.source = source;
        }

        public override OpCode GetOpCode()
        {
            return OpCode.Fadd;
        }

        public override string ToAssembly()
        {
            return $"fadd {destination.ToAssembly()}, {source.ToAssembly()}";
        }

        public override byte[] ToMachineCode()
        {
            return new MachineInstructionBuilder(GetOpCode())
                .SetFirstRegisterID(destination.name)
                .SetSecondRegisterID(source.name)
                .GetInstruction();
        }
    }

    public class Fsub : Instruction
    {
        public Register destination { get; set; }
        public Register source { get; set; }

        public Fsub(Register destination, Register source)
        {
            this.destination = destination;
            this.source = source;
        }

        public override OpCode GetOpCode()
        {
            return OpCode.Fsub;
        }

        public override string ToAssembly()
        {
            return $"fsub {destination.ToAssembly()}, {source.ToAssembly()}";
        }

        public override byte[] ToMachineCode()
        {
            return new MachineInstructionBuilder(GetOpCode())
                .SetFirstRegisterID(destination.name)
                .SetSecondRegisterID(source.name)
                .GetInstruction();
        }
    }

    public class Fmul : Instruction
    {
        public Register destination { get; set; }
        public Register source { get; set; }

        public Fmul(Register destination, Register source)
        {
            this.destination = destination;
            this.source = source;
        }

        public override OpCode GetOpCode()
        {
            return OpCode.Fmul;
        }

        public override string ToAssembly()
        {
            return $"fmul {destination.ToAssembly()}, {source.ToAssembly()}";
        }

        public override byte[] ToMachineCode()
        {
            return new MachineInstructionBuilder(GetOpCode())
                .SetFirstRegisterID(destination.name)
                .SetSecondRegisterID(source.name)
                .GetInstruction();
        }
    }

    public class Fdiv : Instruction
    {
        public Register destination { get; set; }
        public Register source { get; set; }

        public Fdiv(Register destination, Register source)
        {
            this.destination = destination;
            this.source = source;
        }

        public override OpCode GetOpCode()
        {
            return OpCode.Fdiv;
        }

        public override string ToAssembly()
        {
            return $"fdiv {destination.ToAssembly()}, {source.ToAssembly()}";
        }

        public override byte[] ToMachineCode()
        {
            return new MachineInstructionBuilder(GetOpCode())
                .SetFirstRegisterID(destination.name)
                .SetSecondRegisterID(source.name)
                .GetInstruction();
        }
    }

    public class Fcmp : Instruction
    {
        public Register left { get; set; }
        public Register right { get; set; }

        public Fcmp(Register left, Register right)
        {
            this.left = left;
            this.right = right;
        }

        public override OpCode GetOpCode()
        {
            return OpCode.Fcmp;
        }

        public override string ToAssembly()
        {
            return $"fcmp {left.ToAssembly()}, {right.ToAssembly()}";
        }

        public override byte[] ToMachineCode()
        {
            return new MachineInstructionBuilder(GetOpCode())
                .SetFirstRegisterID(left.name)
                .SetSecondRegisterID(right.name)
                .GetInstruction();
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
            RegisterID name = RegisterID.RDF
        )
        {
            throw new NotImplementedException();
        }
    }

    public class JmpMem : JumpMemoryAddressInstruction
    {
        public JmpMem(MemoryAddress destination) : base(destination)
        {
        }

        public override OpCode GetOpCode()
        {
            return OpCode.Jmp;
        }

        public override string ToAssembly()
        {
            return $"jmp {destination.ToAssembly()}";
        }
    }

    public class JmpLabel : LabelJumpInstruction
    {

        public JmpLabel(Label label) : base(label)
        {
        }

        public override JumpMemoryAddressInstruction GetJumpInstruction(
            RegisterID name = RegisterID.RDF
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
        public JeqMem(MemoryAddress destination) : base(destination)
        {
        }

        public override OpCode GetOpCode()
        {
            return OpCode.Jeq;
        }

        public override string ToAssembly()
        {
            return $"jeq {destination.ToAssembly()}";
        }
    }

    public class JeqLabel : LabelJumpInstruction
    {

        public JeqLabel(Label label) : base(label)
        {
        }

        public override JumpMemoryAddressInstruction GetJumpInstruction(
            RegisterID name = RegisterID.RDF
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
        public JneMem(MemoryAddress destination) : base(destination)
        {
        }

        public override OpCode GetOpCode()
        {
            return OpCode.Jne;
        }

        public override string ToAssembly()
        {
            return $"jne {destination.ToAssembly()}";
        }
    }

    public class JneLabel : LabelJumpInstruction
    {

        public JneLabel(Label label) : base(label)
        {
        }

        public override JumpMemoryAddressInstruction GetJumpInstruction(
            RegisterID name = RegisterID.RDF
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
        public JltMem(MemoryAddress destination) : base(destination)
        {
        }

        public override OpCode GetOpCode()
        {
            return OpCode.Jlt;
        }

        public override string ToAssembly()
        {
            return $"jlt {destination.ToAssembly()}";
        }
    }

    public class JltLabel : LabelJumpInstruction
    {

        public JltLabel(Label label) : base(label)
        {
        }
        public override JumpMemoryAddressInstruction GetJumpInstruction(
            RegisterID name = RegisterID.RDF
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
        public JleMem(MemoryAddress destination) : base(destination)
        {
        }

        public override OpCode GetOpCode()
        {
            return OpCode.Jle;
        }

        public override string ToAssembly()
        {
            return $"jle {destination.ToAssembly()}";
        }
    }

    public class JleLabel : LabelJumpInstruction
    {

        public JleLabel(Label label) : base(label)
        {
        }

        public override JumpMemoryAddressInstruction GetJumpInstruction(
            RegisterID name = RegisterID.RDF
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
        public JgtMem(MemoryAddress destination) : base(destination)
        {
        }

        public override OpCode GetOpCode()
        {
            return OpCode.Jgt;
        }

        public override string ToAssembly()
        {
            return $"jgt {destination.ToAssembly()}";
        }
    }

    public class JgtLabel : LabelJumpInstruction
    {

        public JgtLabel(Label label) : base(label)
        {
        }
        public override JumpMemoryAddressInstruction GetJumpInstruction(
            RegisterID name = RegisterID.RDF
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
        public JgeMem(MemoryAddress destination) : base(destination)
        {
        }

        public override OpCode GetOpCode()
        {
            return OpCode.Jge;
        }

        public override string ToAssembly()
        {
            return $"jge {destination.ToAssembly()}";
        }
    }

    public class JgeLabel : LabelJumpInstruction
    {

        public JgeLabel(Label label) : base(label)
        {
        }

        public override JumpMemoryAddressInstruction GetJumpInstruction(
            RegisterID name = RegisterID.RDF
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
        public CallMem(MemoryAddress destination) : base(destination)
        {
        }

        public override OpCode GetOpCode()
        {
            return OpCode.Call;
        }

        public override string ToAssembly()
        {
            return $"call {destination.ToAssembly()}";
        }
    }

    public class CallLabel : LabelJumpInstruction
    {

        public CallLabel(Label label) : base(label)
        {
        }

        public override JumpMemoryAddressInstruction GetJumpInstruction(
            RegisterID name = RegisterID.RDF
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
        public Ret()
        {
        }

        public override OpCode GetOpCode()
        {
            return OpCode.Ret;
        }

        public override string ToAssembly()
        {
            return "ret";
        }
    }

    public class Cmp : Instruction
    {
        public Register left { get; set; }
        public Register right { get; set; }

        public Cmp(Register left, Register right)
        {
            this.left = left;
            this.right = right;
        }

        public override OpCode GetOpCode()
        {
            return OpCode.Cmp;
        }

        public override string ToAssembly()
        {
            return $"cmp {left.ToAssembly()}, {right.ToAssembly()}";
        }
    }
}
