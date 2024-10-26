using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bytom.Hardware.CPU
{
    public class Package
    {
        public List<Core> cores { get; }
        public uint firmware_size { get; }
        public PowerStatus power_status;
        public Motherboard? motherboard;

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

        public void powerOn(Motherboard motherboard)
        {
            if (power_status == PowerStatus.ON)
            {
                throw new System.Exception("Package is already powered on");
            }
            power_status = PowerStatus.ON;
            this.motherboard = motherboard;

            foreach (var core in cores)
            {
                core.powerOn(this);
            }
            getPowerStatus();
        }

        public void powerOff()
        {
            if (power_status == PowerStatus.OFF)
            {
                throw new System.Exception("Package is already powered off");
            }
            power_status = PowerStatus.OFF;

            foreach (var core in cores)
            {
                if (core.getPowerStatus() == PowerStatus.ON)
                {
                    core.powerOff();
                }
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
