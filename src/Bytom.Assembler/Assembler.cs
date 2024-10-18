using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Bytom.Assembler.Instructions;
using Bytom.Assembler.Operands;
using Serilog;

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
                case "push":
                    expectOperandsCount(parameters, 1);
                    return dispatchPush(parameters);
                case "pop":
                    expectOperandsCount(parameters, 1);
                    return dispatchPop(parameters);
                case "swap":
                    {
                        expectOperandsCount(parameters, 2);
                        Register destination = parseRegister(parameters[0]);
                        Register source = parseRegister(parameters[1]);
                        return new Swap(destination, source);
                    }
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
                    {
                        expectOperandsCount(parameters, 1);
                        MemoryAddress? destination = tryParseMemoryAddress(parameters[0]);
                        if (destination != null)
                        {
                            return new JmpMem(destination);
                        }
                        return new JmpLabel(new Label(parameters[0].Trim()));
                    }
                case "jeq":
                    {
                        expectOperandsCount(parameters, 1);
                        MemoryAddress? destination = tryParseMemoryAddress(parameters[0]);
                        if (destination != null)
                        {
                            return new JeqMem(destination);
                        }
                        return new JeqLabel(new Label(parameters[0].Trim()));
                    }
                case "jne":
                    {
                        expectOperandsCount(parameters, 1);
                        MemoryAddress? destination = tryParseMemoryAddress(parameters[0]);
                        if (destination != null)
                        {
                            return new JneMem(destination);
                        }
                        return new JneLabel(new Label(parameters[0].Trim()));
                    }
                case "jlt":
                    {
                        expectOperandsCount(parameters, 1);
                        MemoryAddress? destination = tryParseMemoryAddress(parameters[0]);
                        if (destination != null)
                        {
                            return new JltMem(destination);
                        }
                        return new JltLabel(new Label(parameters[0].Trim()));
                    }
                case "jle":
                    {
                        expectOperandsCount(parameters, 1);
                        MemoryAddress? destination = tryParseMemoryAddress(parameters[0]);
                        if (destination != null)
                        {
                            return new JleMem(destination);
                        }
                        return new JleLabel(new Label(parameters[0].Trim()));
                    }
                case "jgt":
                    {
                        expectOperandsCount(parameters, 1);
                        MemoryAddress? destination = tryParseMemoryAddress(parameters[0]);
                        if (destination != null)
                        {
                            return new JgtMem(destination);
                        }
                        return new JgtLabel(new Label(parameters[0].Trim()));
                    }
                case "jge":
                    {
                        expectOperandsCount(parameters, 1);
                        MemoryAddress? destination = tryParseMemoryAddress(parameters[0]);
                        if (destination != null)
                        {
                            return new JgeMem(destination);
                        }
                        return new JgeLabel(new Label(parameters[0].Trim()));
                    }
                case "call":
                    {
                        expectOperandsCount(parameters, 1);
                        MemoryAddress? destination = tryParseMemoryAddress(parameters[0]);
                        if (destination != null)
                        {
                            return new CallMem(destination);
                        }
                        return new CallLabel(new Label(parameters[0].Trim()));
                    }
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

        private static void expectOperandsCount(string[] instruction_elements, uint n)
        {
            if (instruction_elements.Length != n)
            {
                throw new Exception("Invalid number of operands");
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

            throw new Exception("Invalid number of operands");
        }

        private static ConstantFloat? tryParseConstantFloat(string trimmed_source_code)
        {
            Match match = Regex.Match(trimmed_source_code, @"^([-+]?[0-9]+\.[0-9]+)$");
            if (match.Success)
            {
                return new ConstantFloat(float.Parse(match.Groups[0].Value));
            }
            return null;
        }

        private static ConstantInt? tryParseConstantInt(string trimmed_source_code)
        {
            Match match = Regex.Match(trimmed_source_code, @"^0x([0-9A-Fa-f]+)$");
            if (match.Success)
            {
                return new ConstantInt(
                    int.Parse(match.Groups[1].Value, NumberStyles.HexNumber)
                );
            }

            match = Regex.Match(trimmed_source_code, @"^([-+]?[0-9A-Fa-f]+)$");
            if (match.Success)
            {
                return new ConstantInt(int.Parse(match.Groups[0].Value, NumberStyles.HexNumber));
            }
            return null;
        }

        private static Register parseRegister(string source_code)
        {
            string trimmed_source_code = source_code.Trim();
            var operand = tryParseRegister(trimmed_source_code);
            if (operand != null)
            {
                return operand;
            }
            throw new Exception("Invalid register");
        }

        private static Register? tryParseRegister(string trimmed_source_code)
        {
            var register_name = trimmed_source_code.ToUpper();
            if (Enum.TryParse(register_name, out RegisterName register))
            {
                return new Register(register);
            }
            return null;
        }

        private static MemoryAddress? tryParseMemoryAddress(string source)
        {
            if (source.StartsWith("[") && source.EndsWith("]"))
            {
                var register_name = source.Substring(1, source.Length - 2).ToUpper();
                if (Enum.TryParse(register_name, out RegisterName register))
                {
                    return new MemoryAddress(register);
                }
                throw new Exception($"Invalid register name {register_name}");
            }
            return null;
        }
    }

    public class Backend
    {
        public Backend()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File("/home/argmaster/dev/bytom/assembler_backend.log")
                .CreateLogger();
        }

        public Code compile(List<Node> nodes)
        {
            Log.Information($"Starting compilation of {nodes.Count} nodes.");
            Dictionary<string, long> label_offsets = getLabelOffsets(nodes);
            var instructions = expandLabels(nodes, label_offsets);


            return new Code(instructions);
        }

        private static Dictionary<string, long> getLabelOffsets(List<Node> nodes)
        {
            Dictionary<string, long> label_offsets = new Dictionary<string, long>();
            uint current_offset = 0;

            foreach (Node node in nodes)
            {
                if (node is LabelNode)
                {
                    string node_name = ((LabelNode)node).name;
                    Log.Information($"Found label '{node_name}' at offset '{current_offset}'.");
                    label_offsets[node_name] = current_offset;
                }
                current_offset += node.GetSizeBytes();
            }

            return label_offsets;
        }

        private static List<Instruction> expandLabels(List<Node> nodes, Dictionary<string, long> label_offsets)
        {
            List<Instruction> new_nodes = new List<Instruction>(nodes.Capacity);
            long current_offset = 0;

            foreach (Node node in nodes)
            {
                if (node is LabelJumpInstruction)
                {
                    // push RDE  // 32 bits
                    // push RDF  // 32 bits
                    // mov RDF, IP  // 32 bits
                    // mov RDE, <offset>  // 64 bits
                    // add RDF, RDE  // 32 bits
                    // pop RDE  // 32 bits
                    // <j> [RDF]  // 32 bits
                    var push_rde = new PushReg(new Register(RegisterName.RDE));
                    var push_rdf = new PushReg(new Register(RegisterName.RDF));
                    var mov_rdf_ip = new MovRegReg(
                        new Register(RegisterName.RDF),
                        new Register(RegisterName.IP)
                    );
                    var mov_rde_offset = new MovRegCon(
                        new Register(RegisterName.RDE),
                        new ConstantInt(0)
                    );
                    var add_rdf_rde = new Add(
                        new Register(RegisterName.RDF),
                        new Register(RegisterName.RDE)
                    );
                    var pop_rde = new PopReg(new Register(RegisterName.RDE));
                    var jmp_rdf = ((LabelJumpInstruction)node).GetJumpInstruction(RegisterName.RDF);

                    string label_name = ((LabelJumpInstruction)node).label.name;
                    long label_offset = label_offsets[label_name];
                    long jmp_offset = current_offset;
                    jmp_offset += push_rde.GetSizeBytes();
                    jmp_offset += push_rdf.GetSizeBytes();

                    long relative_offset = label_offset - jmp_offset;
                    ((ConstantInt)mov_rde_offset.source).value = (int)relative_offset;

                    new_nodes.Add(push_rde);
                    new_nodes.Add(push_rdf);
                    new_nodes.Add(mov_rdf_ip);
                    new_nodes.Add(mov_rde_offset);
                    new_nodes.Add(add_rdf_rde);
                    new_nodes.Add(pop_rde);
                    new_nodes.Add(jmp_rdf);
                }
                else if (node is Instruction)
                {
                    new_nodes.Add((Instruction)node);
                }
                current_offset += node.GetSizeBytes();
            }
            return new_nodes;
        }
    }

    public class Code
    {
        public List<Instruction> instructions { get; set; }
        public Code(List<Instruction> instructions)
        {
            this.instructions = instructions;
        }
        public List<byte> ToMachineCode()
        {
            List<byte> machine_code = new List<byte>();

            foreach (Node node in instructions)
            {
                if (node is Instruction)
                {
                    machine_code.AddRange(((Instruction)node).ToMachineCode());
                }
                else if (node is LabelNode)
                {
                    // Do nothing
                }
                else
                {
                    throw new Exception("Invalid node type");
                }
            }
            return machine_code;
        }

        public string ToAssembly()
        {
            string assembly = "";
            foreach (Node node in instructions)
            {
                assembly += node.ToAssembly() + "\n";
            }
            return assembly;
        }
    }
}
