
using System;
using System.Collections.Generic;
using Bytom.Assembler.Nodes;
using Bytom.Assembler.Operands;
using Bytom.Hardware.CPU;
using Bytom.Tools;

namespace Bytom.Assembler
{
    public class BinaryFrontend
    {
        public BinaryFrontend()
        {
        }
        public AbstractSyntaxTree parse(byte[] machineCode)
        {
            List<Node> nodes = new List<Node>();

            Index currentOffset = 0;
            Index currentEndOffset = 4;

            while (currentEndOffset.Value < machineCode.Length)
            {
                var instruction = machineCode[currentOffset..currentEndOffset];
                var decoder = new InstructionDecoder(instruction);
                int nextInt;

                switch (decoder.GetOpCode())
                {
                    case OpCode.Nop:
                        nodes.Add(new Nop());
                        break;
                    case OpCode.Halt:
                        nodes.Add(new Halt());
                        break;
                    case OpCode.MovRegReg:
                        nodes.Add(new MovRegReg(
                            new OpRegister(decoder.GetFirstRegisterID()),
                            new OpRegister(decoder.GetSecondRegisterID())
                        ));
                        break;
                    case OpCode.MovRegMem:
                        nodes.Add(new MovRegMem(
                            new OpRegister(decoder.GetFirstRegisterID()),
                            new OpMemoryAddress(decoder.GetSecondRegisterID())
                        ));
                        break;
                    case OpCode.MovMemReg:
                        nodes.Add(new MovMemReg(
                            new OpMemoryAddress(decoder.GetFirstRegisterID()),
                            new OpRegister(decoder.GetSecondRegisterID())
                        ));
                        break;
                    case OpCode.MovRegCon:
                        nextInt = decodeNextInt(machineCode, ref currentOffset, ref currentEndOffset);
                        nodes.Add(new MovRegCon(
                            new OpRegister(decoder.GetFirstRegisterID()),
                            new OpConstantInt(nextInt)
                        ));
                        break;
                    case OpCode.MovMemCon:
                        nextInt = decodeNextInt(machineCode, ref currentOffset, ref currentEndOffset);
                        nodes.Add(new MovMemCon(
                            new OpMemoryAddress(decoder.GetFirstRegisterID()),
                            new OpConstantInt(nextInt)
                        ));
                        break;
                    case OpCode.PushReg:
                        nodes.Add(new PushReg(
                            new OpRegister(decoder.GetFirstRegisterID())
                        ));
                        break;
                    case OpCode.PushMem:
                        nodes.Add(new PushMem(
                            new OpMemoryAddress(decoder.GetFirstRegisterID())
                        ));
                        break;
                    case OpCode.PushCon:
                        nextInt = decodeNextInt(machineCode, ref currentOffset, ref currentEndOffset);
                        nodes.Add(new PushCon(
                            new OpConstantInt(nextInt)
                        ));
                        break;
                    case OpCode.PopReg:
                        nodes.Add(new PopReg(
                            new OpRegister(decoder.GetFirstRegisterID())
                        ));
                        break;
                    case OpCode.PopMem:
                        nodes.Add(new PopMem(
                            new OpMemoryAddress(decoder.GetFirstRegisterID())
                        ));
                        break;
                    case OpCode.Add:
                        nodes.Add(new Add(
                            new OpRegister(decoder.GetFirstRegisterID()),
                            new OpRegister(decoder.GetSecondRegisterID())
                        ));
                        break;
                    case OpCode.Sub:
                        nodes.Add(new Sub(
                            new OpRegister(decoder.GetFirstRegisterID()),
                            new OpRegister(decoder.GetSecondRegisterID())
                        ));
                        break;
                    case OpCode.Inc:
                        nodes.Add(new Inc(
                            new OpRegister(decoder.GetFirstRegisterID())
                        ));
                        break;
                    case OpCode.Dec:
                        nodes.Add(new Dec(
                            new OpRegister(decoder.GetFirstRegisterID())
                        ));
                        break;
                    case OpCode.Mul:
                        nodes.Add(new Mul(
                            new OpRegister(decoder.GetFirstRegisterID()),
                            new OpRegister(decoder.GetSecondRegisterID())
                        ));
                        break;
                    case OpCode.IMul:
                        nodes.Add(new IMul(
                            new OpRegister(decoder.GetFirstRegisterID()),
                            new OpRegister(decoder.GetSecondRegisterID())
                        ));
                        break;
                    case OpCode.Div:
                        nodes.Add(new Div(
                            new OpRegister(decoder.GetFirstRegisterID()),
                            new OpRegister(decoder.GetSecondRegisterID())
                        ));
                        break;
                    case OpCode.IDiv:
                        nodes.Add(new IDiv(
                            new OpRegister(decoder.GetFirstRegisterID()),
                            new OpRegister(decoder.GetSecondRegisterID())
                        ));
                        break;
                    case OpCode.And:
                        nodes.Add(new And(
                            new OpRegister(decoder.GetFirstRegisterID()),
                            new OpRegister(decoder.GetSecondRegisterID())
                        ));
                        break;
                    case OpCode.Or:
                        nodes.Add(new Or(
                            new OpRegister(decoder.GetFirstRegisterID()),
                            new OpRegister(decoder.GetSecondRegisterID())
                        ));
                        break;
                    case OpCode.Xor:
                        nodes.Add(new Xor(
                            new OpRegister(decoder.GetFirstRegisterID()),
                            new OpRegister(decoder.GetSecondRegisterID())
                        ));
                        break;
                    case OpCode.Not:
                        nodes.Add(new Not(
                            new OpRegister(decoder.GetFirstRegisterID())
                        ));
                        break;
                    case OpCode.Shl:
                        nodes.Add(new Shl(
                            new OpRegister(decoder.GetFirstRegisterID()),
                            new OpRegister(decoder.GetSecondRegisterID())
                        ));
                        break;
                    case OpCode.Shr:
                        nodes.Add(new Shr(
                            new OpRegister(decoder.GetFirstRegisterID()),
                            new OpRegister(decoder.GetSecondRegisterID())
                        ));
                        break;
                    case OpCode.Fadd:
                        nodes.Add(new Fadd(
                            new OpRegister(decoder.GetFirstRegisterID()),
                            new OpRegister(decoder.GetSecondRegisterID())
                        ));
                        break;
                    case OpCode.Fsub:
                        nodes.Add(new Fsub(
                            new OpRegister(decoder.GetFirstRegisterID()),
                            new OpRegister(decoder.GetSecondRegisterID())
                        ));
                        break;
                    case OpCode.Fmul:
                        nodes.Add(new Fmul(
                            new OpRegister(decoder.GetFirstRegisterID()),
                            new OpRegister(decoder.GetSecondRegisterID())
                        ));
                        break;
                    case OpCode.Fdiv:
                        nodes.Add(new Fdiv(
                            new OpRegister(decoder.GetFirstRegisterID()),
                            new OpRegister(decoder.GetSecondRegisterID())
                        ));
                        break;
                    case OpCode.Fcmp:
                        nodes.Add(new Fcmp(
                            new OpRegister(decoder.GetFirstRegisterID()),
                            new OpRegister(decoder.GetSecondRegisterID())
                        ));
                        break;
                    case OpCode.JmpMem:
                        nodes.Add(new JmpMem(
                            new OpMemoryAddress(decoder.GetFirstRegisterID())
                        ));
                        break;
                    case OpCode.JmpCon:
                        nextInt = decodeNextInt(machineCode, ref currentOffset, ref currentEndOffset);
                        nodes.Add(new JmpCon(
                            new OpConstantInt(nextInt)
                        ));
                        break;
                    case OpCode.JeqMem:
                        nodes.Add(new JeqMem(
                            new OpMemoryAddress(decoder.GetFirstRegisterID())
                        ));
                        break;
                    case OpCode.JeqCon:
                        nextInt = decodeNextInt(machineCode, ref currentOffset, ref currentEndOffset);
                        nodes.Add(new JeqCon(
                            new OpConstantInt(nextInt)
                        ));
                        break;
                    case OpCode.JneMem:
                        nodes.Add(new JneMem(
                            new OpMemoryAddress(decoder.GetFirstRegisterID())
                        ));
                        break;
                    case OpCode.JneCon:
                        nextInt = decodeNextInt(machineCode, ref currentOffset, ref currentEndOffset);
                        nodes.Add(new JneCon(
                            new OpConstantInt(nextInt)
                        ));
                        break;
                    case OpCode.JltMem:
                        nodes.Add(new Nodes.JLT.Mem(
                            new OpMemoryAddress(decoder.GetFirstRegisterID())
                        ));
                        break;
                    case OpCode.JltCon:
                        nextInt = decodeNextInt(machineCode, ref currentOffset, ref currentEndOffset);
                        nodes.Add(new Nodes.JLT.Con(
                            new OpConstantInt(nextInt)
                        ));
                        break;
                    case OpCode.JleMem:
                        nodes.Add(new Nodes.JLE.Mem(
                            new OpMemoryAddress(decoder.GetFirstRegisterID())
                        ));
                        break;
                    case OpCode.JleCon:
                        nextInt = decodeNextInt(machineCode, ref currentOffset, ref currentEndOffset);
                        nodes.Add(new Nodes.JLE.Con(
                            new OpConstantInt(nextInt)
                        ));
                        break;
                    case OpCode.JgtMem:
                        nodes.Add(new Nodes.JGT.Mem(
                            new OpMemoryAddress(decoder.GetFirstRegisterID())
                        ));
                        break;
                    case OpCode.JgtCon:
                        nextInt = decodeNextInt(machineCode, ref currentOffset, ref currentEndOffset);
                        nodes.Add(new Nodes.JGT.Con(
                            new OpConstantInt(nextInt)
                        ));
                        break;
                    case OpCode.JgeMem:
                        nodes.Add(new Nodes.JGE.Mem(
                            new OpMemoryAddress(decoder.GetFirstRegisterID())
                        ));
                        break;
                    case OpCode.JgeCon:
                        nextInt = decodeNextInt(machineCode, ref currentOffset, ref currentEndOffset);
                        nodes.Add(new Nodes.JGE.Con(
                            new OpConstantInt(nextInt)
                        ));
                        break;
                    case OpCode.CallMem:
                        nodes.Add(new Nodes.CALL.Mem(
                            new OpMemoryAddress(decoder.GetFirstRegisterID())
                        ));
                        break;
                    case OpCode.CallCon:
                        nextInt = decodeNextInt(machineCode, ref currentOffset, ref currentEndOffset);
                        nodes.Add(new Nodes.CALL.Con(
                            new OpConstantInt(nextInt)
                        ));
                        break;
                    case OpCode.Ret:
                        nodes.Add(new Ret());
                        break;
                    case OpCode.Cmp:
                        nodes.Add(new Cmp(
                            new OpRegister(decoder.GetFirstRegisterID()),
                            new OpRegister(decoder.GetSecondRegisterID())
                        ));
                        break;
                    default:
                        throw new Exception($"Invalid instruction at offset {currentOffset.Value}");
                }

                currentOffset = currentEndOffset;
                currentEndOffset = new Index(currentEndOffset.Value + 4);
            }
            return new AbstractSyntaxTree(nodes);
        }

        private int decodeNextInt(byte[] machineCode, ref Index currentOffset, ref Index currentEndOffset)
        {
            currentOffset = currentEndOffset;
            currentEndOffset = new Index(currentEndOffset.Value + 4);

            return Serialization.Int32FromBytesBigEndian(machineCode[currentOffset..currentEndOffset]);
        }
    }
}