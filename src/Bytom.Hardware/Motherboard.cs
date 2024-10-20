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
            this.cpu.motherboard = this;
            power_status = PowerStatus.OFF;
        }
        public async Task powerOn()
        {
            if (power_status == PowerStatus.ON)
            {
                throw new System.Exception("Motherboard is already powered on");
            }
            power_status = PowerStatus.ON;
            byte[] firmware_image = await rom.readAll(0, cpu.firmware_size);
            await ram.writeAll(0, firmware_image);
            await cpu.powerOn();
        }

        public async Task powerOff()
        {
            if (power_status == PowerStatus.OFF)
            {
                throw new System.Exception("Motherboard is already powered off");
            }
            await cpu.powerOff();
            power_status = PowerStatus.OFF;
        }

        public async Task softwarePowerOff()
        {
            if (power_status == PowerStatus.OFF)
            {
                throw new System.Exception("Motherboard is already powered off");
            }
            await Task.Delay(0);
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

        public async Task waitUntilRunning()
        {
            while (power_status != PowerStatus.OFF)
            {
                await Task.Delay(10);
            }
            getPowerStatus();
        }

    }
}

