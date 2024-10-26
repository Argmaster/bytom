using System;
using System.Collections.Generic;

namespace Bytom.Hardware
{
    public class DeviceManager
    {
        public Dictionary<uint, Device> devices { get; }

        public DeviceManager(Dictionary<uint, Device> devices)
        {
            this.devices = devices;
        }
    }
}