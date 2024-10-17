using System;

namespace Bytom.Hardware
{
    public class CPU
    {
        public UInt32 clock_speed_hz { get; set; }


        public CPU(UInt32 clock_speed_hz_)
        {
            this.clock_speed_hz = clock_speed_hz_;
        }
    }
}
