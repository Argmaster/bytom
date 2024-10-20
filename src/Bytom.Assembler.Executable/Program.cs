using System;
using System.Collections;
using System.IO;
using System.Linq;
using Bytom.Assembler;

using CommandLine;

public class Options
{
    [Value(0, MetaName = "file", HelpText = "Path to file to assemble.")]
    public string? file { get; set; }

    [Option('o', "output", Required = true, HelpText = "Output file name.")]
    public string? output { get; set; }

    [Option('d', "disassemble", Required = false, Default = false, HelpText = "Instead of assembling text file, disassemble input binary into assembly code.")]
    public bool disassemble { get; set; }

    [Option('v', "verbose", Default = false, HelpText = "Enable verbose logging.")]
    public bool Verbose { get; set; }
}

namespace Bytom.Assembler.Executable
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(RunOptions)
                .WithNotParsed(HandleParseError);
        }
        static void RunOptions(Options opts)
        {
            if (opts.Verbose)
            {
                //enable verbose logging
            }
            if (opts.disassemble)
            {
                var machineCode = File.ReadAllBytes(opts.file);
                var source = Assembler.disassemble(machineCode.ToList());
                File.WriteAllText(opts.output, source);
                return;
            }
            else
            {
                var source = File.ReadAllText(opts.file);
                var machineCode = Assembler.assemble(source);
                File.WriteAllBytes(opts.output, machineCode.ToArray());
            }
        }
        static void HandleParseError(IEnumerable errs)
        {
            //handle errors
        }
    }
}