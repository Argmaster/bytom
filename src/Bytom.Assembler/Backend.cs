using System;
using System.Collections.Generic;
using Bytom.Assembler.Nodes;
using Bytom.Assembler.Operands;
using Serilog;
using Bytom.Hardware.CPU;

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
                if (node is JumpLabelInstruction)
                {
                    string label_name = ((JumpLabelInstruction)node).label.name;
                    long label_offset = label_offsets[label_name];

                    var jmp_con = ((JumpLabelInstruction)node).GetJumpInstruction((int)label_offset);

                    new_nodes.Add(jmp_con);
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
                    // Do nothing, discard label nodes, they are not part of machine code.
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