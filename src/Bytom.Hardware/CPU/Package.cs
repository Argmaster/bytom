using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bytom.Hardware.CPU
{
    public class Package
    {
        public List<Core> cores { get; }
        public uint firmware_size { get; }
        public PowerStatus power_status { get; set; }
        public Motherboard? motherboard { get; set; }

        public Package(List<Core> cores, uint firmware_size)
        {
            this.cores = cores;
            this.firmware_size = firmware_size;

            foreach (var core in cores)
            {
                core.package = this;
            }

            power_status = PowerStatus.OFF;
        }

        public async Task powerOn()
        {
            if (power_status == PowerStatus.ON)
            {
                throw new System.Exception("Package is already powered on");
            }
            power_status = PowerStatus.ON;

            foreach (var core in cores)
            {
                await core.powerOn();
            }
            getPowerStatus();
        }

        public async Task powerOff()
        {
            if (power_status == PowerStatus.OFF)
            {
                throw new System.Exception("Package is already powered off");
            }
            power_status = PowerStatus.OFF;

            foreach (var core in cores)
            {
                await core.powerOff();
            }
            getPowerStatus();
        }

        public PowerStatus getPowerStatus()
        {
            for (int i = 0; i < cores.Count; i++)
            {
                if (cores[i].getPowerStatus() != power_status)
                {
                    throw new System.Exception($"Core {i} incorrect power status {power_status}");
                }
            }
            return power_status;
        }
    }
}
