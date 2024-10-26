using System.Threading.Tasks;

namespace Bytom.Hardware
{
    public enum PowerStatus { ON, OFF }

    public class Motherboard
    {
        public CPU.Package cpu { get; set; }
        public RAM.Controller ram { get; set; }
        public FirmwareRom rom { get; set; }
        public DeviceManager device_manager { get; set; }
        public PowerStatus power_status { get; set; }
        private Clock clock { get; set; }

        public Motherboard(
            CPU.Package cpu,
            RAM.Controller ram,
            FirmwareRom rom,
            DeviceManager device_manager
        )
        {
            this.cpu = cpu;
            this.ram = ram;
            this.rom = rom;
            this.device_manager = device_manager;
            this.clock = new Clock(1000);

            power_status = PowerStatus.OFF;
        }
        public void powerOn()
        {
            if (power_status == PowerStatus.ON)
            {
                throw new System.Exception("Motherboard is already powered on");
            }
            power_status = PowerStatus.ON;
            byte[] firmware_image = rom.readAll(0, cpu.firmware_size);
            ram.writeAll(0, firmware_image);
            cpu.powerOn(this);
        }

        public void powerOff()
        {
            if (power_status == PowerStatus.OFF)
            {
                throw new System.Exception("Motherboard is already powered off");
            }
            cpu.powerOff();
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

