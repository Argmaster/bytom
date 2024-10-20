using System;

namespace Bytom.Hardware
{
    public class Disk : Device
    {
        public uint capacity_bytes { get; set; }

        public Disk(uint capacity_bytes_)
        {
            this.capacity_bytes = capacity_bytes_;
        }
    }
}

