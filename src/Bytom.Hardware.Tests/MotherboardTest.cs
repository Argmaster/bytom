using Bytom.Hardware.CPU;
using Bytom.Hardware.RAM;

namespace Bytom.Hardware.Tests
{
    public class MachineTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestMotherboardConstruction()
        {
            byte[] firmware = Assembler.Assembler.assemble(@"
                mov RD0, 0x1
                halt
                "
            ).ToArray();

            Motherboard motherboard = createBytomIncB1(firmware);
            motherboard.powerOn().Wait();

            Assert.That(motherboard.getPowerStatus(), Is.EqualTo(PowerStatus.OFF));
        }

        private static Motherboard createBytomIncB1(byte[] firmware)
        {
            Controller ram = new Controller(new List<MemoryChip>{
                new BytomIncRam16KGen1(),
                new BytomIncRam16KGen1(),
            });
            Package cpu = new BytomIncGen1(ram);
            FirmwareRom rom = new FirmwareRom(4096);

            rom.writeAllNoDelay(0x0, firmware).Wait();

            DeviceManager manager = new DeviceManager([new Disk(16384)]);

            Motherboard machine = new Motherboard(cpu, ram, rom, manager);
            return machine;
        }
    }
}
