using System.Collections.Generic;

namespace Bytom.Assembler
{
    public class Assembler
    {
        public static List<byte> assemble(string source)
        {
            Frontend frontend = new Frontend();
            var code = frontend.parse(source);

            Backend backend = new Backend();
            var compiled = backend.compile(code);

            return compiled.ToMachineCode();
        }

        public static string disassemble(List<byte> machineCode)
        {
            BinaryFrontend frontend = new BinaryFrontend();
            var code = frontend.parse(machineCode.ToArray());

            Backend backend = new Backend();
            var compiled = backend.compile(code);

            return compiled.ToAssembly();
        }
    }
}
