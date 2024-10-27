using Bytom.Hardware.CPU;
using Bytom.Tools;

namespace Bytom.Hardware.Tests
{
    public class MotherboardTests
    {

        public RAM? ram;
        public MemoryController? controller;
        public Core? core;
        public Package? cpu;
        public Motherboard? motherboard;

        [SetUp]
        public void Setup()
        {
            ram = new RAM(256, 500, 0, 0);
            controller = new MemoryController([ram]);
            core = new Core(0, 500);
            cpu = new Package([core], 128);
            motherboard = new Motherboard(cpu, controller);
        }

        public void writeFirmwareAndPowerOn(string firmware_source)
        {
            byte[] firmware = Assembler.Assembler.assemble(firmware_source).ToArray();
            if (firmware.Length > 128)
            {
                throw new System.Exception("Firmware is too large");
            }

            ram!.writeDebug(firmware, 0);
            motherboard!.powerOn();
        }

        [Test]
        public void TestHardwarePowerCycle()
        {
            writeFirmwareAndPowerOn("jmp 0");
            Assert.That(motherboard!.getPowerStatus(), Is.EqualTo(PowerStatus.ON));
            Assert.That(motherboard!.cpu!.cores[0].getPowerStatus(), Is.EqualTo(PowerStatus.ON));
            Assert.That(motherboard!.cpu!.getPowerStatus(), Is.EqualTo(PowerStatus.ON));
            Assert.That(motherboard!.controller!.getPowerStatus(), Is.EqualTo(PowerStatus.ON));
            Assert.That(ram!.getPowerStatus(), Is.EqualTo(PowerStatus.ON));

            motherboard!.powerOff();
            Assert.That(motherboard!.getPowerStatus(), Is.EqualTo(PowerStatus.OFF));
            Assert.That(motherboard!.cpu!.cores[0].getPowerStatus(), Is.EqualTo(PowerStatus.OFF));
            Assert.That(motherboard!.cpu!.getPowerStatus(), Is.EqualTo(PowerStatus.OFF));
            Assert.That(motherboard!.controller!.getPowerStatus(), Is.EqualTo(PowerStatus.OFF));
            Assert.That(ram!.getPowerStatus(), Is.EqualTo(PowerStatus.OFF));
        }
    }
}