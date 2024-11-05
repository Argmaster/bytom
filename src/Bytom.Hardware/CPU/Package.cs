using System.Collections.Generic;

namespace Bytom.Hardware.CPU
{
    public class Package
    {
        public List<Core> cores { get; }
        protected PowerStatus power_status;
        public Motherboard? motherboard;

        public Package(List<Core> cores)
        {
            this.cores = cores;
            power_status = PowerStatus.OFF;
        }

        public virtual void powerOn(Motherboard motherboard)
        {
            if (power_status == PowerStatus.ON)
            {
                throw new System.Exception("Package is already powered on");
            }
            power_status = PowerStatus.STARTING;
            this.motherboard = motherboard;

            powerOnInit();
            powerOnCheck();

            power_status = PowerStatus.ON;
        }
        public virtual void powerOnInit()
        {
            foreach (var core in cores)
            {
                core.powerOn(this);
            }
        }

        public virtual void powerOnCheck()
        {
            foreach (var core in cores)
            {
                if (core.getPowerStatus() != PowerStatus.ON)
                {
                    power_status = PowerStatus.OFF;
                    throw new System.Exception("Core failed to power on");
                }
            }
        }

        public virtual void powerOff()
        {
            if (power_status == PowerStatus.OFF)
            {
                throw new System.Exception("Package is already powered off");
            }
            power_status = PowerStatus.STOPPING;
            powerOffTeardown();
            power_status = PowerStatus.OFF;
        }

        public virtual void powerOffTeardown()
        {
            foreach (var core in cores)
            {
                if (core.getPowerStatus() == PowerStatus.ON)
                {
                    core.powerOff();
                }
            }
        }

        public PowerStatus getPowerStatus()
        {
            return power_status;
        }
    }
}
