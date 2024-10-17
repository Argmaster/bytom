using System;
using System.Collections.Generic;


namespace Bytom.Hardware
{
    public class Motherboard
    {
        public CPU.Package cpu { get; set; }
        public List<Disk> disks { get; set; }
        public RAM.Controller ram { get; set; }

        public Motherboard(CPU.Package cpu_, RAM.Controller ram_, List<Disk> disks_)
        {
            this.cpu = cpu_;
            this.ram = ram_;
            this.disks = disks_;
        }
    }
}

