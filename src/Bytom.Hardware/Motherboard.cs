using System.Threading.Tasks;

namespace Bytom.Hardware
{
    public enum PowerStatus { ON, OFF, STARTING, STOPPING }

    public class Motherboard
    {
        public CPU.Package cpu { get; }
        public MemoryController controller { get; }
        protected PowerStatus power_status;
        protected Clock clock;

        public Motherboard(
            CPU.Package cpu,
            MemoryController controller
        )
        {
            this.cpu = cpu;
            this.controller = controller;
            clock = new Clock(1);

            power_status = PowerStatus.OFF;
        }
        public void powerOn()
        {
            if (power_status == PowerStatus.ON)
            {
                throw new System.Exception("Motherboard is already powered on");
            }
            power_status = PowerStatus.STARTING;
            controller.powerOn(this);
            cpu.powerOn(this);
            power_status = PowerStatus.ON;
        }

        public void powerOff()
        {
            if (power_status == PowerStatus.OFF)
            {
                throw new System.Exception("Motherboard is already powered off");
            }
            cpu.powerOff();
            controller.powerOff();
            power_status = PowerStatus.OFF;
        }

        public void softwarePowerOff()
        {
            if (power_status == PowerStatus.OFF)
            {
                throw new System.Exception("Motherboard is already powered off");
            }
            power_status = PowerStatus.OFF;
        }

        public PowerStatus getPowerStatus()
        {
            if (cpu.getPowerStatus() != power_status)
            {
                throw new System.Exception($"CPU incorrect power status {power_status}");
            }
            return power_status;
        }

        public void waitUntilRunning()
        {
            while (power_status != PowerStatus.OFF)
            {
                clock.waitForCycles(1);
            }
            getPowerStatus();
        }

    }
}

