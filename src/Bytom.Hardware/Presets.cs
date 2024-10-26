using System.Collections.Generic;
using Bytom.Hardware.CPU;
using Bytom.Hardware.RAM;

namespace Bytom.Hardware
{
    public class BytomIncGen1Core : Core
    {
        public BytomIncGen1Core() : base(0, 200)
        { }
    }

    public class BytomIncGen1 : Package
    {
        public BytomIncGen1() : base(
            new List<Core>{new BytomIncGen1Core()},
            4096
        )
        { }
    }

    public class BytomIncRam16KGen1 : MemoryChip
    {
        public BytomIncRam16KGen1() : base(16 * 1024, 100, 10) { }
    }
}