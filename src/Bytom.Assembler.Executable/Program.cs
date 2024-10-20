﻿using System;
using Bytom.Assembler;

using CommandLine;

public class Options
{
    [Value(0, MetaName = "file", HelpText = "Path to file to assemble.")]
    public required string file { get; set; }

    [Option('o', "output", Required = true, HelpText = "Output file name.")]
    public required string output { get; set; }

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
            var source = File.ReadAllText(opts.file);
            var machineCode = Assembler.assemble(source);
            File.WriteAllBytes(opts.output, machineCode.ToArray());
        }
        static void HandleParseError(IEnumerable<Error> errs)
        {
            //handle errors
        }
    }
}