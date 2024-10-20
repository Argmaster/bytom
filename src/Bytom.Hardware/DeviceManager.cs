using System.Collections.Generic;

namespace Bytom.Hardware
{
    public class DeviceManager
    {
        public List<Device> devices { get; set; }

        public DeviceManager(List<Device> devices)
        {
            this.devices = devices;
        }
    }
}