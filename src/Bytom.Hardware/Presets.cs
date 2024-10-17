using System.Collections.Generic;
using Bytom.Hardware.CPU;
using Bytom.Hardware.RAM;

namespace Bytom.Hardware
{
    public class BytomIncB1 : Package
    {
        public BytomIncB1(Controller ram) : base(new List<Core>{
            new Core(0, 200, new List<Cache>{
                new Cache(256, 10),
            }, ram)
        })
        {

        }
    }

    public class BytomIncRam1K100 : Stick {
        public BytomIncRam1K100() : base(1024, 100, 100, 8) {}
    }
}