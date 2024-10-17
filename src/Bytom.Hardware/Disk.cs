using System;

namespace Bytom.Hardware
{
    public class Disk
    {
        public uint capacity_bytes { get; set; }

        public Disk(uint capacity_bytes_)
        {
            this.capacity_bytes = capacity_bytes_;
        }
    }
}

