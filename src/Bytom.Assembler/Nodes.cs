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

        public override byte[] ToMachineCode()
        {
            return new MachineInstructionBuilder(GetOpCode())
                .SetFirstRegisterID(destination.register)
                .GetInstruction();
        }
    }

    public class JumpConstantIntInstruction : Instruction
    {

        public ConstantInt destination { get; set; }

        public JumpConstantIntInstruction(ConstantInt destination)
        {
            this.destination = destination;
        }

        public override uint GetSizeBits()
        {
            return 64;
        }

        public override byte[] ToMachineCode()
        {
            return new MachineInstructionBuilder(GetOpCode())
                .SetConstant(destination.GetBytes())
                .GetInstruction();
        }
    }

    public class JumpLabelInstruction : Instruction
    {

        public Label label { get; set; }

        public JumpLabelInstruction(Label label)
        {
            this.label = label;
        }
        public override uint GetSizeBits()
        {
            return 64;
        }

        public virtual JumpConstantIntInstruction GetJumpInstruction(
            int offset
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
            return OpCode.JmpMem;
        }

        public override string ToAssembly()
        {
            return $"jmp {destination.ToAssembly()}";
        }
    }

    public class JmpCon : JumpConstantIntInstruction
    {

        public JmpCon(ConstantInt destination) : base(destination)
        {
        }

        public override OpCode GetOpCode()
        {
            return OpCode.JmpCon;
        }

        public override string ToAssembly()
        {
            return $"jmp {destination.ToAssembly()}";
        }
    }

    public class JmpLabel : JumpLabelInstruction
    {

        public JmpLabel(Label label) : base(label)
        {
        }

        public override JumpConstantIntInstruction GetJumpInstruction(
            int offset
        )
        {
            return new JmpCon(new ConstantInt(offset));
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
            return OpCode.JeqMem;
        }

        public override string ToAssembly()
        {
            return $"jeq {destination.ToAssembly()}";
        }
    }

    public class JeqCon : JumpConstantIntInstruction
    {

        public JeqCon(ConstantInt destination) : base(destination)
        {
        }

        public override OpCode GetOpCode()
        {
            return OpCode.JeqCon;
        }

        public override string ToAssembly()
        {
            return $"jeq {destination.ToAssembly()}";
        }
    }

    public class JeqLabel : JumpLabelInstruction
    {

        public JeqLabel(Label label) : base(label)
        {
        }

        public override JumpConstantIntInstruction GetJumpInstruction(
            int offset
        )
        {
            return new JeqCon(new ConstantInt(offset));
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
            return OpCode.JneMem;
        }

        public override string ToAssembly()
        {
            return $"jne {destination.ToAssembly()}";
        }
    }

    public class JneCon : JumpConstantIntInstruction
    {

        public JneCon(ConstantInt destination) : base(destination)
        {
        }

        public override OpCode GetOpCode()
        {
            return OpCode.JneCon;
        }

        public override string ToAssembly()
        {
            return $"jne {destination.ToAssembly()}";
        }
    }

    public class JneLabel : JumpLabelInstruction
    {

        public JneLabel(Label label) : base(label)
        {
        }

        public override JumpConstantIntInstruction GetJumpInstruction(
            int offset
        )
        {
            return new JneCon(new ConstantInt(offset));
        }

        public override string ToAssembly()
        {
            return $"jne {label.ToAssembly()}";
        }
    }

    namespace JB
    {
        public class Mem : JumpMemoryAddressInstruction
        {
            public Mem(MemoryAddress destination) : base(destination)
            {
            }

            public override OpCode GetOpCode()
            {
                return OpCode.JbMem;
            }

            public override string ToAssembly()
            {
                return $"jb {destination.ToAssembly()}";
            }
        }

        public class Con : JumpConstantIntInstruction
        {
            public Con(ConstantInt destination) : base(destination)
            {
            }

            public override OpCode GetOpCode()
            {
                return OpCode.JbCon;
            }

            public override string ToAssembly()
            {
                return $"jb {destination.ToAssembly()}";
            }
        }

        public class Label : JumpLabelInstruction
        {
            public Label(Operands.Label label) : base(label)
            {
            }

            public override JumpConstantIntInstruction GetJumpInstruction(
                int offset
            )
            {
                return new Con(new ConstantInt(offset));
            }

            public override string ToAssembly()
            {
                return $"jb {label.ToAssembly()}";
            }
        }
    }

    namespace JBE
    {
        public class Mem : JumpMemoryAddressInstruction
        {
            public Mem(MemoryAddress destination) : base(destination)
            {
            }

            public override OpCode GetOpCode()
            {
                return OpCode.JbeMem;
            }

            public override string ToAssembly()
            {
                return $"jbe {destination.ToAssembly()}";
            }
        }

        public class Con : JumpConstantIntInstruction
        {
            public Con(ConstantInt destination) : base(destination)
            {
            }

            public override OpCode GetOpCode()
            {
                return OpCode.JbeCon;
            }

            public override string ToAssembly()
            {
                return $"jbe {destination.ToAssembly()}";
            }
        }

        public class Label : JumpLabelInstruction
        {
            public Label(Operands.Label label) : base(label)
            {
            }

            public override JumpConstantIntInstruction GetJumpInstruction(
                int offset
            )
            {
                return new Con(new ConstantInt(offset));
            }

            public override string ToAssembly()
            {
                return $"jbe {label.ToAssembly()}";
            }
        }
    }

    namespace JA
    {
        public class Mem : JumpMemoryAddressInstruction
        {
            public Mem(MemoryAddress destination) : base(destination)
            {
            }

            public override OpCode GetOpCode()
            {
                return OpCode.JaMem;
            }

            public override string ToAssembly()
            {
                return $"ja {destination.ToAssembly()}";
            }
        }

        public class Con : JumpConstantIntInstruction
        {
            public Con(ConstantInt destination) : base(destination)
            {
            }

            public override OpCode GetOpCode()
            {
                return OpCode.JaCon;
            }

            public override string ToAssembly()
            {
                return $"ja {destination.ToAssembly()}";
            }
        }

        public class Label : JumpLabelInstruction
        {
            public Label(Operands.Label label) : base(label)
            {
            }

            public override JumpConstantIntInstruction GetJumpInstruction(
                int offset
            )
            {
                return new Con(new ConstantInt(offset));
            }

            public override string ToAssembly()
            {
                return $"ja {label.ToAssembly()}";
            }
        }
    }

    namespace JAE
    {
        public class Mem : JumpMemoryAddressInstruction
        {
            public Mem(MemoryAddress destination) : base(destination)
            {
            }

            public override OpCode GetOpCode()
            {
                return OpCode.JaeMem;
            }

            public override string ToAssembly()
            {
                return $"jae {destination.ToAssembly()}";
            }
        }

        public class Con : JumpConstantIntInstruction
        {
            public Con(ConstantInt destination) : base(destination)
            {
            }

            public override OpCode GetOpCode()
            {
                return OpCode.JaeCon;
            }

            public override string ToAssembly()
            {
                return $"jae {destination.ToAssembly()}";
            }
        }

        public class Label : JumpLabelInstruction
        {
            public Label(Operands.Label label) : base(label)
            {
            }

            public override JumpConstantIntInstruction GetJumpInstruction(
                int offset
            )
            {
                return new Con(new ConstantInt(offset));
            }

            public override string ToAssembly()
            {
                return $"jae {label.ToAssembly()}";
            }
        }
    }

    namespace JLT
    {
        public class Mem : JumpMemoryAddressInstruction
        {
            public Mem(MemoryAddress destination) : base(destination)
            {
            }

            public override OpCode GetOpCode()
            {
                return OpCode.JltMem;
            }

            public override string ToAssembly()
            {
                return $"jlt {destination.ToAssembly()}";
            }
        }

        public class Con : JumpConstantIntInstruction
        {
            public Con(ConstantInt destination) : base(destination)
            {
            }

            public override OpCode GetOpCode()
            {
                return OpCode.JltCon;
            }

            public override string ToAssembly()
            {
                return $"jlt {destination.ToAssembly()}";
            }
        }

        public class Label : JumpLabelInstruction
        {
            public Label(Operands.Label label) : base(label)
            {
            }

            public override JumpConstantIntInstruction GetJumpInstruction(
                int offset
            )
            {
                return new Con(new ConstantInt(offset));
            }

            public override string ToAssembly()
            {
                return $"jlt {label.ToAssembly()}";
            }
        }
    }

    namespace JLE
    {
        public class Mem : JumpMemoryAddressInstruction
        {
            public Mem(MemoryAddress destination) : base(destination)
            {
            }

            public override OpCode GetOpCode()
            {
                return OpCode.JleMem;
            }

            public override string ToAssembly()
            {
                return $"jle {destination.ToAssembly()}";
            }
        }

        public class Con : JumpConstantIntInstruction
        {
            public Con(ConstantInt destination) : base(destination)
            {
            }

            public override OpCode GetOpCode()
            {
                return OpCode.JleCon;
            }

            public override string ToAssembly()
            {
                return $"jle {destination.ToAssembly()}";
            }
        }

        public class Label : JumpLabelInstruction
        {
            public Label(Operands.Label label) : base(label)
            {
            }


            public override JumpConstantIntInstruction GetJumpInstruction(
                int offset
            )
            {
                return new Con(new ConstantInt(offset));
            }

            public override string ToAssembly()
            {
                return $"jle {label.ToAssembly()}";
            }
        }
    }

    namespace JGT
    {
        public class Mem : JumpMemoryAddressInstruction
        {
            public Mem(MemoryAddress destination) : base(destination)
            {
            }

            public override OpCode GetOpCode()
            {
                return OpCode.JgtMem;
            }

            public override string ToAssembly()
            {
                return $"jgt {destination.ToAssembly()}";
            }
        }

        public class Con : JumpConstantIntInstruction
        {
            public Con(ConstantInt destination) : base(destination)
            {
            }

            public override OpCode GetOpCode()
            {
                return OpCode.JgtCon;
            }

            public override string ToAssembly()
            {
                return $"jgt {destination.ToAssembly()}";
            }
        }

        public class Label : JumpLabelInstruction
        {
            public Label(Operands.Label label) : base(label)
            {
            }

            public override JumpConstantIntInstruction GetJumpInstruction(
                int offset
            )
            {
                return new Con(new ConstantInt(offset));
            }

            public override string ToAssembly()
            {
                return $"jgt {label.ToAssembly()}";
            }
        }
    }

    namespace JGE
    {
        public class Mem : JumpMemoryAddressInstruction
        {
            public Mem(MemoryAddress destination) : base(destination)
            {
            }

            public override OpCode GetOpCode()
            {
                return OpCode.JgeMem;
            }

            public override string ToAssembly()
            {
                return $"jge {destination.ToAssembly()}";
            }
        }

        public class Con : JumpConstantIntInstruction
        {
            public Con(ConstantInt destination) : base(destination)
            {
            }

            public override OpCode GetOpCode()
            {
                return OpCode.JgeCon;
            }

            public override string ToAssembly()
            {
                return $"jge {destination.ToAssembly()}";
            }
        }

        public class Label : JumpLabelInstruction
        {

            public Label(Operands.Label label) : base(label)
            {
            }

            public override JumpConstantIntInstruction GetJumpInstruction(
                int offset
            )
            {
                return new Con(new ConstantInt(offset));
            }

            public override string ToAssembly()
            {
                return $"jge {label.ToAssembly()}";
            }
        }
    }

    namespace CALL
    {
        public class Mem : JumpMemoryAddressInstruction
        {
            public Mem(MemoryAddress destination) : base(destination)
            {
            }

            public override OpCode GetOpCode()
            {
                return OpCode.CallMem;
            }

            public override string ToAssembly()
            {
                return $"call {destination.ToAssembly()}";
            }
        }

        public class Con : JumpConstantIntInstruction
        {

            public Con(ConstantInt destination) : base(destination)
            {
            }

            public override OpCode GetOpCode()
            {
                return OpCode.CallCon;
            }

            public override string ToAssembly()
            {
                return $"call {destination.ToAssembly()}";
            }
        }

        public class Label : JumpLabelInstruction
        {

            public Label(Operands.Label label) : base(label)
            {
            }


            public override JumpConstantIntInstruction GetJumpInstruction(
                int offset
            )
            {
                return new Con(new ConstantInt(offset));
            }

            public override string ToAssembly()
            {
                return $"call {label.ToAssembly()}";
            }
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

        public override byte[] ToMachineCode()
        {
            return new MachineInstructionBuilder(GetOpCode())
                .GetInstruction();
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

        public override byte[] ToMachineCode()
        {
            return new MachineInstructionBuilder(GetOpCode())
                .SetFirstRegisterID(left.name)
                .SetSecondRegisterID(right.name)
                .GetInstruction();
        }

        public override string ToAssembly()
        {
            return $"cmp {left.ToAssembly()}, {right.ToAssembly()}";
        }
    }
}
