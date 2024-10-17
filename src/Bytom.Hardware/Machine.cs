using System;

namespace Bytom.Hardware {
    public class Machine
    {
        public Disk disk { get; set; }
        public CPU cpu { get; set; }
        public Ram ram { get; set; }

        public Machine(Disk disk_, CPU cpu_, Ram ram_)
        {
            this.disk = disk_;
            this.cpu = cpu_;
            this.ram = ram_;
        }
    }
}

