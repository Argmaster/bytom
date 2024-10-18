using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Bytom.Assembler.Instructions;
using Bytom.Assembler.Operands;

namespace Bytom.Assembler
{
    public class Frontend
    {
        public Frontend()
        {

        }

        public List<Node> parse(string source_code)
        {
            List<Node> nodes = new List<Node>();
            // Requires that each instruction is on a new line, kind of standard right?
            foreach (string line in source_code.Split('\n'))
            {
                string trimmed_line = line.Trim();
                if (trimmed_line.Length == 0)
                {
                    continue;
                }

                if (trimmed_line.EndsWith(":"))
                {
                    var match = Regex.Match(trimmed_line, @"^([A-Fa-f_]+):$");
                    if (match.Success)
                    {
                        nodes.Add(new LabelNode(match.Groups[1].Value));
                        continue;
                    }
                    throw new Exception("Invalid label");
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

            return nodes;
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
                default:
                    throw new Exception($"Invalid instruction {instruction_name}");
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
                    throw new Exception("Invalid destination operand");

            }
            throw new NotImplementedException();
        }

        private static void expectOperandsCount(string[] instruction_elements, uint n)
        {
            if (instruction_elements.Length != n)
            {
                throw new System.Exception("Invalid number of operands");
            }
        }

        private Operand parseOperand(string operand)
        {
            var sanitized_operand = operand.Trim();
            if (sanitized_operand.StartsWith("[") && sanitized_operand.EndsWith("]"))
            {
                var register_name = sanitized_operand.Substring(1, sanitized_operand.Length - 2);
                if (Enum.TryParse(register_name, out RegisterName register))
                {
                    return new MemoryAddress(register);
                }
                throw new Exception($"Invalid register name {register_name}");
            }

            if (Enum.GetNames(typeof(RegisterName)).Contains(sanitized_operand))
            {
                var register_name = sanitized_operand;
                if (Enum.TryParse(sanitized_operand, out RegisterName register))
                {
                    return new Register(register);
                }
                throw new Exception($"Invalid register name {register_name}");
            }

            var match = Regex.Match(sanitized_operand, @"^0x([0-9A-Fa-f]+)$");
            if (match.Success)
            {
                return new ConstantInt(
                    uint.Parse(match.Groups[1].Value, NumberStyles.HexNumber)
                );
            }

            match = Regex.Match(sanitized_operand, @"^([0-9A-Fa-f]+)$");
            if (match.Success)
            {
                return new ConstantInt(uint.Parse(match.Groups[0].Value, NumberStyles.HexNumber));
            }

            match = Regex.Match(sanitized_operand, @"^([0-9]+\.[0-9]+)$");
            if (match.Success)
            {
                return new ConstantFloat(float.Parse(match.Groups[0].Value));
            }

            throw new System.Exception("Invalid number of operands");
        }
    }
}
