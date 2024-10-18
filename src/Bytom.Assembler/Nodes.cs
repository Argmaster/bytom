using System;
using Bytom.Assembler.Operands;

namespace Bytom.Assembler.Instructions
{
    public class Node { }

    public class LabelNode : Node
    {
        public string name { get; set; }
        public LabelNode(string name)
        {
            this.name = name;
        }
    }

    public class Instruction : Node
    {
        public static readonly uint code = 0b0000_0000_0000_0000;
        public static readonly uint size = 32;

        public virtual uint GetOpCode()
        {
            return code;
        }
        public virtual uint GetSize()
        {
            return size;
        }
        public virtual byte[] GetMachineCode()
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
    }

    public class Halt : Instruction
    {
        public static new readonly uint code = 0b0000_0000_0000_0001;
        public Halt()
        {
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
    }

    public class MovRegCon : Instruction
    {
        public static new readonly uint code = 0b1000_0000_0000_0001;
        public static new readonly uint size = 64;
        public Register destination { get; set; }
        public Constant source { get; set; }
        public MovRegCon(Register destination, Constant source)
        {
            this.destination = destination;
            this.source = source;
        }
    }

    public class MovMemCon : Instruction
    {
        public static new readonly uint code = 0b1000_0000_0000_0001;
        public static new readonly uint size = 64;
        public MemoryAddress destination { get; set; }
        public Constant source { get; set; }
        public MovMemCon(MemoryAddress destination, Constant source)
        {
            this.destination = destination;
            this.source = source;
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
    }

    public class PushMem : Instruction
    {
        public static new readonly uint code = 0b0001_0000_0000_0010;
        public MemoryAddress source { get; set; }
        public PushMem(MemoryAddress source)
        {
            this.source = source;
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
    }

    public class PopReg : Instruction
    {
        public static new readonly uint code = 0b0000_0000_0000_0011;
        public Register destination { get; set; }
        public PopReg(Register destination)
        {
            this.destination = destination;
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
    }

    public class Inc : Instruction
    {
        public static new readonly uint code = 0b0000_0000_0001_0010;
        public Register destination { get; set; }
        public Inc(Register destination)
        {
            this.destination = destination;
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
    }

    public class Not : Instruction
    {
        public static new readonly uint code = 0b0000_0000_0001_1011;
        public Register destination { get; set; }
        public Not(Register destination)
        {
            this.destination = destination;
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
    }

    public class JmpMem : JumpMemoryAddressInstruction
    {
        public static new readonly uint code = 0b0000_0000_0000_0010;
        public JmpMem(MemoryAddress destination) : base(destination)
        {
        }
    }

    public class JmpLabel : LabelJumpInstruction
    {
        public static new readonly uint code = 0b0000_0000_0010_0000;

        public JmpLabel(Label label) : base(label)
        {
        }
    }

    public class JeqMem : JumpMemoryAddressInstruction
    {
        public static new readonly uint code = 0b0000_0000_0010_0001;
        public JeqMem(MemoryAddress destination) : base(destination)
        {
        }
    }

    public class JeqLabel : LabelJumpInstruction
    {
        public static new readonly uint code = 0b0000_0000_0010_0001;

        public JeqLabel(Label label) : base(label)
        {
        }
    }

    public class JneMem : JumpMemoryAddressInstruction
    {
        public static new readonly uint code = 0b0000_0000_0010_0010;
        public JneMem(MemoryAddress destination) : base(destination)
        {
        }
    }

    public class JneLabel : LabelJumpInstruction
    {
        public static new readonly uint code = 0b0000_0000_0010_0010;

        public JneLabel(Label label) : base(label)
        {
        }
    }

    public class JltMem : JumpMemoryAddressInstruction
    {
        public static new readonly uint code = 0b0000_0000_0010_0011;
        public JltMem(MemoryAddress destination) : base(destination)
        {
        }
    }

    public class JltLabel : LabelJumpInstruction
    {
        public static new readonly uint code = 0b0000_0000_0010_0011;

        public JltLabel(Label label) : base(label)
        {
        }
    }

    public class JleMem : JumpMemoryAddressInstruction
    {
        public static new readonly uint code = 0b0000_0000_0010_0100;
        public JleMem(MemoryAddress destination) : base(destination)
        {
        }
    }

    public class JleLabel : LabelJumpInstruction
    {
        public static new readonly uint code = 0b0000_0000_0010_0100;

        public JleLabel(Label label) : base(label)
        {
        }
    }

    public class JgtMem : JumpMemoryAddressInstruction
    {
        public static new readonly uint code = 0b0000_0000_0010_0101;
        public JgtMem(MemoryAddress destination) : base(destination)
        {
        }
    }

    public class JgtLabel : LabelJumpInstruction
    {
        public static new readonly uint code = 0b0000_0000_0010_0101;

        public JgtLabel(Label label) : base(label)
        {
        }
    }

    public class JgeMem : JumpMemoryAddressInstruction
    {
        public static new readonly uint code = 0b0000_0000_0010_0110;
        public JgeMem(MemoryAddress destination) : base(destination)
        {
        }
    }

    public class JgeLabel : LabelJumpInstruction
    {
        public static new readonly uint code = 0b0000_0000_0010_0110;

        public JgeLabel(Label label) : base(label)
        {
        }
    }

    public class CallMem : JumpMemoryAddressInstruction
    {
        public static new readonly uint code = 0b0000_0000_0010_1000;
        public CallMem(MemoryAddress destination) : base(destination)
        {
        }
    }

    public class CallLabel : LabelJumpInstruction
    {
        public static new readonly uint code = 0b0000_0000_0010_1000;

        public CallLabel(Label label) : base(label)
        {
        }
    }

    public class Ret : Instruction
    {
        public static new readonly uint code = 0b0000_0000_0010_1001;
        public Ret()
        {
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
    }
}
