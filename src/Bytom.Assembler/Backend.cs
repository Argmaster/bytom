using System;
using System.Collections.Generic;
using Bytom.Assembler.Instructions;
using Bytom.Assembler.Operands;
using Serilog;

namespace Bytom.Assembler
{
    public class Backend
    {
        public Backend()
        {
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