using Bytom.Hardware.CPU;
using Bytom.Tools;

namespace Bytom.Hardware.Tests
{
    public class HardwareTests
    {

        public RAM? ram;
        public FirmwareRom? rom;
        public IoController? controller;
        public Core? core;
        public Package? cpu;
        public Motherboard? motherboard;

        [SetUp]
        public void Setup()
        {
            ram = new RAM(
                capacity_bytes: 256,
                clock_speed_hz: 500,
                read_latency_cycles: 0,
                write_latency_cycles: 0,
                bandwidth_bytes: 1
            );
            rom = new FirmwareRom(1024, 500, 1, 1, 1);
            controller = new IoController([ram], rom, []);
            core = new Core(0, 500);
            cpu = new Package([core]);
            motherboard = new Motherboard(cpu, controller);
        }

        public void writeFirmwareAndPowerOn(string firmware_source)
        {
            byte[] firmware = Assembler.Assembler.assemble(firmware_source).ToArray();
            if (firmware.Length > 128)
            {
                throw new Exception("Firmware is too large");
            }

            rom!.writeDebug(firmware, 0);
            motherboard!.powerOn();
        }

        [Test]
        public void TestHardwarePowerCycle()
        {
            writeFirmwareAndPowerOn("jmp 0xFFFFFBFF");
            Assert.That(motherboard!.getPowerStatus(), Is.EqualTo(PowerStatus.ON));
            Assert.That(motherboard!.cpu!.cores[0].getPowerStatus(), Is.EqualTo(PowerStatus.ON));
            Assert.That(motherboard!.cpu!.getPowerStatus(), Is.EqualTo(PowerStatus.ON));
            Assert.That(motherboard!.controller!.getPowerStatus(), Is.EqualTo(PowerStatus.ON));
            Assert.That(ram!.getPowerStatus(), Is.EqualTo(PowerStatus.ON));
            Assert.That(rom!.getPowerStatus(), Is.EqualTo(PowerStatus.ON));

            motherboard!.powerOff();
            Assert.That(motherboard!.getPowerStatus(), Is.EqualTo(PowerStatus.OFF));
            Assert.That(motherboard!.cpu!.cores[0].getPowerStatus(), Is.EqualTo(PowerStatus.OFF));
            Assert.That(motherboard!.cpu!.getPowerStatus(), Is.EqualTo(PowerStatus.OFF));
            Assert.That(motherboard!.controller!.getPowerStatus(), Is.EqualTo(PowerStatus.OFF));
            Assert.That(ram!.getPowerStatus(), Is.EqualTo(PowerStatus.OFF));
            Assert.That(rom!.getPowerStatus(), Is.EqualTo(PowerStatus.OFF));
        }
    }
}