
using System.Collections;

namespace Bytom.Hardware.CPU
{

    public enum ALUOperationType
    {
        ADD,
        SUB,
        UNSIGNED_MUL,
        SIGNED_MUL,
        UNSIGNED_DIV,
        SIGNED_DIV,
        AND,
        OR,
        XOR,
        SHL,
        SHR
    }

    public class ALUOperation32 : MicroOp
    {
        public ALUOperationType operation { get; }
        public Register32 left { get; }
        public Register32 right { get; }
        public ConditionCodeRegister? ccr { get; }
        public ALUOperation32(
            ALUOperationType operation,
            Register32 left,
            Register32 right,
            ConditionCodeRegister? ccr = null
        )
        {
            this.operation = operation;
            this.left = left;
            this.right = right;
            this.ccr = ccr;
        }
        public override IEnumerable execute(Core core)
        {
            yield return null;
        }
    }

    public struct AluLatencies
    {
        public uint add;
        public uint sub;
        public uint unsigned_mul;
        public uint signed_mul;
        public uint unsigned_div;
        public uint signed_div;
        public uint and;
        public uint or;
        public uint xor;
        public uint shl;
        public uint shr;
    }

    public class ALU
    {
        AluLatencies latencies;

        public ALU(AluLatencies latencies)
        {
            this.latencies = latencies;
        }

        public IEnumerable execute(ALUOperation32 operation)
        {
            switch (operation.operation)
            {
                case ALUOperationType.ADD:
                    {
                        for (int i = 0; i < latencies.add; i++)
                        {
                            yield return null;
                        }
                        long left = operation.left.readInt32();
                        long right = operation.right.readInt32();
                        var ccr = operation.ccr;

                        long result = left + right;

                        ccr?.writeUInt32(0u);
                        ccr?.setZeroFlag(result == 0);
                        ccr?.setCarryFlag((ulong)(uint)left + (ulong)(uint)right > uint.MaxValue);
                        ccr?.setSignFlag((int)result < 0);
                        ccr?.setOverflowFlag((int)left + (int)right != result);

                        operation.left.writeUInt32((uint)result);
                        break;
                    }
                case ALUOperationType.SUB:
                    {
                        for (int i = 0; i < latencies.sub; i++)
                        {
                            yield return null;
                        }
                        long left = operation.left.readInt32();
                        long right = operation.right.readInt32();
                        var ccr = operation.ccr;

                        long result = left - right;

                        ccr?.writeUInt32(0u);
                        ccr?.setZeroFlag(result == 0);
                        ccr?.setCarryFlag((ulong)(uint)left - (ulong)(uint)right > uint.MaxValue);
                        ccr?.setSignFlag((int)result < 0);
                        ccr?.setOverflowFlag((int)left - (int)right != result);

                        operation.left.writeUInt32((uint)result);
                        break;
                    }
                case ALUOperationType.UNSIGNED_MUL:
                    {
                        for (int i = 0; i < latencies.unsigned_mul; i++)
                        {
                            yield return null;
                        }
                        long left = operation.left.readUInt32();
                        long right = operation.right.readUInt32();
                        var ccr = operation.ccr;

                        long result = left * right;

                        ccr?.writeUInt32(0u);
                        ccr?.setZeroFlag(result == 0);
                        ccr?.setCarryFlag((ulong)(uint)left * (ulong)(uint)right > uint.MaxValue);
                        ccr?.setSignFlag((int)result < 0);
                        ccr?.setOverflowFlag((int)left * (int)right != result);

                        operation.left.writeUInt32((uint)result);
                        break;
                    }
                case ALUOperationType.SIGNED_MUL:
                    {
                        for (int i = 0; i < latencies.signed_mul; i++)
                        {
                            yield return null;
                        }
                        long left = operation.left.readInt32();
                        long right = operation.right.readInt32();
                        var ccr = operation.ccr;

                        long result = left * right;

                        ccr?.writeUInt32(0u);
                        ccr?.setZeroFlag(result == 0);
                        ccr?.setCarryFlag((ulong)(uint)left * (ulong)(uint)right > uint.MaxValue);
                        ccr?.setSignFlag((int)result < 0);
                        ccr?.setOverflowFlag((int)left * (int)right != result);

                        operation.left.writeUInt32((uint)result);
                        break;
                    }
                case ALUOperationType.UNSIGNED_DIV:
                    {
                        for (int i = 0; i < latencies.unsigned_div; i++)
                        {
                            yield return null;
                        }
                        long left = operation.left.readUInt32();
                        long right = operation.right.readUInt32();
                        var ccr = operation.ccr;

                        if (right != 0)
                        {
                            long result = left / right;
                            long mod = left % right;

                            ccr?.setZeroFlag(result == 0);
                            ccr?.setCarryFlag(false);
                            ccr?.setSignFlag((int)result < 0);
                            ccr?.setOverflowFlag(false);

                            operation.left.writeUInt32((uint)result);
                            operation.right.writeUInt32((uint)mod);
                        }
                        else
                        {
                            ccr?.setZeroDivisionFlag(true);
                            operation.left.writeUInt32(0);
                            operation.right.writeUInt32(0);
                        }
                        break;
                    }
                case ALUOperationType.SIGNED_DIV:
                    {
                        for (int i = 0; i < latencies.signed_div; i++)
                        {
                            yield return null;
                        }
                        long left = operation.left.readInt32();
                        long right = operation.right.readInt32();
                        var ccr = operation.ccr;

                        if (right != 0)
                        {
                            long result = left / right;
                            long mod = left % right;

                            ccr?.setZeroFlag(result == 0);
                            ccr?.setCarryFlag(false);
                            ccr?.setSignFlag((int)result < 0);
                            ccr?.setOverflowFlag(false);

                            operation.left.writeInt32((int)result);
                            operation.right.writeInt32((int)mod);
                        }
                        else
                        {
                            ccr?.setZeroDivisionFlag(true);
                            operation.left.writeUInt32(0);
                            operation.right.writeUInt32(0);
                        }
                        break;
                    }
                case ALUOperationType.AND:
                    {
                        for (int i = 0; i < latencies.and; i++)
                        {
                            yield return null;
                        }
                        long left = operation.left.readInt32();
                        long right = operation.right.readInt32();
                        var ccr = operation.ccr;

                        long result = left & right;

                        ccr?.writeUInt32(0u);
                        ccr?.setZeroFlag(result == 0);
                        ccr?.setCarryFlag(false);
                        ccr?.setSignFlag((int)result < 0);
                        ccr?.setOverflowFlag(false);

                        operation.left.writeUInt32((uint)result);
                        break;
                    }
                case ALUOperationType.OR:
                    {
                        for (int i = 0; i < latencies.or; i++)
                        {
                            yield return null;
                        }
                        long left = operation.left.readInt32();
                        long right = operation.right.readInt32();
                        var ccr = operation.ccr;

                        long result = left | right;

                        ccr?.writeUInt32(0u);
                        ccr?.setZeroFlag(result == 0);
                        ccr?.setCarryFlag(false);
                        ccr?.setSignFlag((int)result < 0);
                        ccr?.setOverflowFlag(false);

                        operation.left.writeUInt32((uint)result);
                        break;
                    }
                case ALUOperationType.XOR:
                    {
                        for (int i = 0; i < latencies.xor; i++)
                        {
                            yield return null;
                        }
                        long left = operation.left.readInt32();
                        long right = operation.right.readInt32();
                        var ccr = operation.ccr;

                        long result = left ^ right;

                        ccr?.writeUInt32(0u);
                        ccr?.setZeroFlag(result == 0);
                        ccr?.setCarryFlag(false);
                        ccr?.setSignFlag((int)result < 0);
                        ccr?.setOverflowFlag(false);

                        operation.left.writeUInt32((uint)result);
                        break;
                    }
                case ALUOperationType.SHL:
                    {
                        for (int i = 0; i < latencies.shl; i++)
                        {
                            yield return null;
                        }
                        ulong left = operation.left.readUInt32();
                        ulong right = operation.right.readUInt32();
                        var ccr = operation.ccr;

                        ulong result;

                        if (right > 31)
                        {
                            result = 0;
                        }
                        else
                        {
                            result = (left << (int)right) & 0xFF_FF_FF_FF;
                        }

                        ccr?.writeUInt32(0u);
                        ccr?.setZeroFlag(result == 0);
                        ccr?.setCarryFlag(false);
                        ccr?.setSignFlag((int)result < 0);
                        ccr?.setOverflowFlag(right > 31);

                        operation.left.writeUInt32((uint)result);
                        break;
                    }
                case ALUOperationType.SHR:
                    {
                        for (int i = 0; i < latencies.shr; i++)
                        {
                            yield return null;
                        }
                        ulong left = operation.left.readUInt32();
                        ulong right = operation.right.readUInt32();
                        var ccr = operation.ccr;

                        ulong result;

                        if (right > 31)
                        {
                            result = 0;
                        }
                        else
                        {
                            result = (left >> (int)right) & 0xFF_FF_FF_FF;
                        }

                        ccr?.writeUInt32(0u);
                        ccr?.setZeroFlag(result == 0);
                        ccr?.setCarryFlag(false);
                        ccr?.setSignFlag((int)result < 0);
                        ccr?.setOverflowFlag(right > 31);

                        operation.left.writeUInt32((uint)result);
                        break;
                    }
            }
            yield break;
        }
    }
}