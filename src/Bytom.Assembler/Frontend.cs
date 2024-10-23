using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Bytom.Assembler.Nodes;
using Bytom.Assembler.Operands;
using Bytom.Hardware.CPU;


namespace Bytom.Assembler
{
    public class AbstractSyntaxTree
    {
        public List<Node> nodes;
        public AbstractSyntaxTree(List<Node> nodes)
        {
            this.nodes = nodes;
        }
    }

    public class Frontend
    {
        private string? currentLine;
        private uint lineIndex = 0;

        public Frontend()
        {
        }

        public AbstractSyntaxTree parse(string source_code)
        {
            List<Node> nodes = new List<Node>();
            lineIndex = 0;

            // Requires that each instruction is on a new line, kind of standard right?
            foreach (string line in source_code.Split('\n'))
            {
                currentLine = line;
                lineIndex = lineIndex + 1;

                string trimmed_line = line.Trim();
                if (trimmed_line.Length == 0)
                {
                    continue;
                }

                if (trimmed_line.EndsWith(":"))
                {
                    nodes.Add(new LabelNode(trimmed_line.Substring(0, trimmed_line.Length - 1)));
                    continue;
                }

                var instruction_elements = trimmed_line.Split(" ", 2);
                if (instruction_elements.Length == 0)
                {
                    throw new Exception("Invalid instruction");
                }

                string instruction_name = instruction_elements[0].ToLower();
                string[] instruction_parameters;
                if (instruction_elements.Length > 1)
                {
                    instruction_parameters = instruction_elements[1].Split(",");
                }
                else
                {
                    instruction_parameters = new string[0];
                }

                nodes.Add(dispatchInstruction(instruction_name, instruction_parameters));
            }

            return new AbstractSyntaxTree(nodes);
        }

        private Instruction dispatchInstruction(string instruction_name, string[] parameters)
        {
            switch (instruction_name)
            {
                case "nop":
                    expectOperandsCount(parameters, 0);
                    return new Nop();
                case "halt":
                    expectOperandsCount(parameters, 0);
                    return new Halt();
                case "mov":
                    return dispatchMov(parameters);
                case "push":
                    expectOperandsCount(parameters, 1);
                    return dispatchPush(parameters);
                case "pop":
                    expectOperandsCount(parameters, 1);
                    return dispatchPop(parameters);
                case "add":
                    {
                        expectOperandsCount(parameters, 2);
                        Register destination = parseRegister(parameters[0]);
                        Register source = parseRegister(parameters[1]);
                        return new Add(destination, source);
                    }
                case "sub":
                    {
                        expectOperandsCount(parameters, 2);
                        Register destination = parseRegister(parameters[0]);
                        Register source = parseRegister(parameters[1]);
                        return new Sub(destination, source);
                    }
                case "inc":
                    {
                        expectOperandsCount(parameters, 1);
                        Register destination = parseRegister(parameters[0]);
                        return new Inc(destination);
                    }
                case "dec":
                    {
                        expectOperandsCount(parameters, 1);
                        Register destination = parseRegister(parameters[0]);
                        return new Dec(destination);
                    }
                case "mul":
                    {
                        expectOperandsCount(parameters, 2);
                        Register destination = parseRegister(parameters[0]);
                        Register source = parseRegister(parameters[1]);
                        return new Mul(destination, source);
                    }
                case "imul":
                    {
                        expectOperandsCount(parameters, 2);
                        Register destination = parseRegister(parameters[0]);
                        Register source = parseRegister(parameters[1]);
                        return new IMul(destination, source);
                    }
                case "div":
                    {
                        expectOperandsCount(parameters, 2);
                        Register destination = parseRegister(parameters[0]);
                        Register source = parseRegister(parameters[1]);
                        return new Div(destination, source);
                    }
                case "idiv":
                    {
                        expectOperandsCount(parameters, 2);
                        Register destination = parseRegister(parameters[0]);
                        Register source = parseRegister(parameters[1]);
                        return new IDiv(destination, source);
                    }
                case "and":
                    {
                        expectOperandsCount(parameters, 2);
                        Register destination = parseRegister(parameters[0]);
                        Register source = parseRegister(parameters[1]);
                        return new And(destination, source);
                    }
                case "or":
                    {
                        expectOperandsCount(parameters, 2);
                        Register destination = parseRegister(parameters[0]);
                        Register source = parseRegister(parameters[1]);
                        return new Or(destination, source);
                    }
                case "xor":
                    {
                        expectOperandsCount(parameters, 2);
                        Register destination = parseRegister(parameters[0]);
                        Register source = parseRegister(parameters[1]);
                        return new Xor(destination, source);
                    }
                case "not":
                    {
                        expectOperandsCount(parameters, 1);
                        Register destination = parseRegister(parameters[0]);
                        return new Not(destination);
                    }
                case "shl":
                    {
                        expectOperandsCount(parameters, 2);
                        Register destination = parseRegister(parameters[0]);
                        Register source = parseRegister(parameters[1]);
                        return new Shl(destination, source);
                    }
                case "shr":
                    {
                        expectOperandsCount(parameters, 2);
                        Register destination = parseRegister(parameters[0]);
                        Register source = parseRegister(parameters[1]);
                        return new Shr(destination, source);
                    }
                case "fadd":
                    {
                        expectOperandsCount(parameters, 2);
                        Register destination = parseRegister(parameters[0]);
                        Register source = parseRegister(parameters[1]);
                        return new Fadd(destination, source);
                    }
                case "fsub":
                    {
                        expectOperandsCount(parameters, 2);
                        Register destination = parseRegister(parameters[0]);
                        Register source = parseRegister(parameters[1]);
                        return new Fsub(destination, source);
                    }
                case "fmul":
                    {
                        expectOperandsCount(parameters, 2);
                        Register destination = parseRegister(parameters[0]);
                        Register source = parseRegister(parameters[1]);
                        return new Fmul(destination, source);
                    }
                case "fdiv":
                    {
                        expectOperandsCount(parameters, 2);
                        Register destination = parseRegister(parameters[0]);
                        Register source = parseRegister(parameters[1]);
                        return new Fdiv(destination, source);
                    }
                case "fcmp":
                    {
                        expectOperandsCount(parameters, 2);
                        Register left = parseRegister(parameters[0]);
                        Register right = parseRegister(parameters[1]);
                        return new Fcmp(left, right);
                    }
                case "jmp":
                    return new ParseJumpInstruction<JmpMem, JmpCon, JmpLabel>(this).make(parameters);

                case "jeq":
                    return new ParseJumpInstruction<JeqMem, JeqCon, JeqLabel>(this).make(parameters);

                case "jne":
                    return new ParseJumpInstruction<JneMem, JneCon, JneLabel>(this).make(parameters);

                case "jb":
                    return new ParseJumpInstruction<Nodes.JB.Mem, Nodes.JB.Con, Nodes.JB.Label>(this).make(parameters);

                case "jbe":
                    return new ParseJumpInstruction<Nodes.JBE.Mem, Nodes.JBE.Con, Nodes.JBE.Label>(this).make(parameters);

                case "ja":
                    return new ParseJumpInstruction<Nodes.JA.Mem, Nodes.JA.Con, Nodes.JA.Label>(this).make(parameters);

                case "jae":
                    return new ParseJumpInstruction<Nodes.JAE.Mem, Nodes.JAE.Con, Nodes.JAE.Label>(this).make(parameters);

                case "jlt":
                    return new ParseJumpInstruction<Nodes.JLT.Mem, Nodes.JLT.Con, Nodes.JLT.Label>(this).make(parameters);

                case "jle":
                    return new ParseJumpInstruction<Nodes.JLE.Mem, Nodes.JLE.Con, Nodes.JLE.Label>(this).make(parameters);

                case "jgt":
                    return new ParseJumpInstruction<Nodes.JGT.Mem, Nodes.JGT.Con, Nodes.JGT.Label>(this).make(parameters);

                case "jge":
                    return new ParseJumpInstruction<Nodes.JGE.Mem, Nodes.JGE.Con, Nodes.JGE.Label>(this).make(parameters);

                case "call":
                    return new ParseJumpInstruction<Nodes.CALL.Mem, Nodes.CALL.Con, Nodes.CALL.Label>(this).make(parameters);

                case "ret":
                    expectOperandsCount(parameters, 0);
                    return new Ret();
                case "cmp":
                    {
                        expectOperandsCount(parameters, 2);
                        Register left = parseRegister(parameters[0]);
                        Register right = parseRegister(parameters[1]);
                        return new Cmp(left, right);
                    }
                default:
                    throw new Exception($"Invalid instruction {instruction_name} in line {lineIndex}: '{currentLine}'");
            }
        }

        internal class ParseJumpInstruction<JMem, JCon, JLabel>
            where JMem : JumpMemoryAddressInstruction
            where JCon : JumpConstantIntInstruction
            where JLabel : JumpLabelInstruction
        {
            private Frontend frontend;

            public ParseJumpInstruction(Frontend frontend)
            {
                this.frontend = frontend;
            }
            public Instruction make(string[] parameters)
            {
                frontend.expectOperandsCount(parameters, 1);
                {
                    MemoryAddress? destination = frontend.tryParseMemoryAddress(parameters[0]);
                    if (destination != null)
                    {
                        return (JMem)Activator.CreateInstance(typeof(JMem), destination);
                    }
                }
                {
                    ConstantInt? destination = frontend.tryParseConstantInt(parameters[0]);
                    if (destination != null)
                    {
                        return (JCon)Activator.CreateInstance(typeof(JCon), destination);
                    }
                }
                return (JLabel)Activator.CreateInstance(typeof(JLabel), new Label(parameters[0].Trim()));
            }
        }

        private Instruction dispatchMov(string[] parameters)
        {
            expectOperandsCount(parameters, 2);
            Operand destination = parseOperand(parameters[0]);
            switch (destination.GetType().Name)
            {
                case "Register":
                    {
                        Operand source = parseOperand(parameters[1]);
                        switch (source.GetType().Name)
                        {
                            case "Register":
                                return new MovRegReg((Register)destination, (Register)source);
                            case "MemoryAddress":
                                return new MovRegMem((Register)destination, (MemoryAddress)source);
                            case "ConstantInt":
                                return new MovRegCon((Register)destination, (ConstantInt)source);
                            case "ConstantFloat":
                                return new MovRegCon((Register)destination, (ConstantFloat)source);
                        }
                        break;
                    }
                case "MemoryAddress":
                    {
                        Operand source = parseOperand(parameters[1]);
                        switch (source.GetType().Name)
                        {
                            case "Register":
                                return new MovMemReg((MemoryAddress)destination, (Register)source);
                            case "MemoryAddress":
                                throw new Exception("Invalid source operand");
                            case "ConstantInt":
                                return new MovMemCon((MemoryAddress)destination, (ConstantInt)source);
                            case "ConstantFloat":
                                return new MovMemCon((MemoryAddress)destination, (ConstantFloat)source);
                        }
                        break;
                    }
                default:
                    throw new Exception($"Invalid destination operand in line {lineIndex}: '{currentLine}'");

            }
            throw new NotImplementedException();
        }

        private Instruction dispatchPush(string[] parameters)
        {
            Operand source = parseOperand(parameters[0]);
            switch (source.GetType().Name)
            {
                case "Register":
                    return new PushReg((Register)source);
                case "MemoryAddress":
                    return new PushMem((MemoryAddress)source);
                case "ConstantInt":
                    return new PushCon((ConstantInt)source);
                case "ConstantFloat":
                    return new PushCon((ConstantFloat)source);
                default:
                    throw new Exception("Invalid source operand");
            }
        }

        private Instruction dispatchPop(string[] parameters)
        {
            Operand destination = parseOperand(parameters[0]);
            switch (destination.GetType().Name)
            {
                case "Register":
                    return new PopReg((Register)destination);
                case "MemoryAddress":
                    return new PopMem((MemoryAddress)destination);
                default:
                    throw new Exception("Invalid destination operand");
            }
        }

        private void expectOperandsCount(string[] instruction_elements, uint n)
        {
            if (instruction_elements.Length != n)
            {
                throw new Exception($"Invalid number of operands in line {lineIndex}: '{currentLine}'");
            }
        }

        private Operand parseOperand(string source_code)
        {
            string trimmed_source_code = source_code.Trim();
            Operand? operand = tryParseMemoryAddress(trimmed_source_code);
            if (operand != null)
            {
                return operand;
            }

            operand = tryParseRegister(trimmed_source_code);
            if (operand != null)
            {
                return operand;
            }

            operand = tryParseConstantInt(trimmed_source_code);
            if (operand != null)
            {
                return operand;
            }

            operand = tryParseConstantFloat(trimmed_source_code);
            if (operand != null)
            {
                return operand;
            }

            throw new Exception($"Invalid operand in line {lineIndex}: '{currentLine}'");
        }

        private ConstantFloat? tryParseConstantFloat(string trimmed_source_code)
        {
            Match match = Regex.Match(trimmed_source_code, @"^([-+]?[0-9]+\.[0-9]+)$");
            if (match.Success)
            {
                return new ConstantFloat(float.Parse(match.Groups[0].Value));
            }
            return null;
        }

        private ConstantInt? tryParseConstantInt(string trimmed_source_code)
        {
            Match match = Regex.Match(trimmed_source_code, @"^0x([0-9A-Fa-f]+)$");
            if (match.Success)
            {
                return new ConstantInt(
                    long.Parse(match.Groups[1].Value, NumberStyles.HexNumber)
                );
            }

            match = Regex.Match(trimmed_source_code, @"^([-+]?[0-9A-Fa-f]+)$");
            if (match.Success)
            {
                return new ConstantInt(long.Parse(match.Groups[0].Value));
            }
            return null;
        }

        private Register parseRegister(string source_code)
        {
            string trimmed_source_code = source_code.Trim();
            var operand = tryParseRegister(trimmed_source_code);
            if (operand != null)
            {
                return operand;
            }
            throw new Exception($"Invalid register in line {lineIndex}: '{currentLine}'");
        }

        private static Register? tryParseRegister(string trimmed_source_code)
        {
            var register_name = trimmed_source_code.ToUpper();
            if (Enum.GetNames(typeof(RegisterID)).Contains(register_name))
            {
                Enum.TryParse(register_name, out RegisterID register);
                return new Register(register);
            }
            return null;
        }

        private MemoryAddress? tryParseMemoryAddress(string source)
        {
            if (source.StartsWith("[") && source.EndsWith("]"))
            {
                var register_name = source.Substring(1, source.Length - 2).ToUpper();
                if (Enum.TryParse(register_name, out RegisterID register))
                {
                    return new MemoryAddress(register);
                }
                throw new Exception($"Invalid register name {register_name} in line {lineIndex}: '{currentLine}'");
            }
            return null;
        }
    }
}
