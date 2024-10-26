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
            motherboard.powerOn();
            motherboard.waitUntilRunning();

            Assert.That(motherboard.getPowerStatus(), Is.EqualTo(PowerStatus.OFF));
        }

        private static Motherboard createBytomIncB1(byte[] firmware)
        {
            Controller ram = new Controller(new List<MemoryChip>{
                new BytomIncRam16KGen1(),
                new BytomIncRam16KGen1(),
            });
            Package cpu = new BytomIncGen1();
            FirmwareRom rom = new FirmwareRom(4096);

            rom.writeAllNoDelay(0x0, firmware);

            DeviceManager manager = new DeviceManager(
                new Dictionary<uint, Device>{
                    {0x0, new Disk(16 *1024)},
                }
            );

            Motherboard machine = new Motherboard(cpu, ram, rom, manager);
            return machine;
        }
    }
}
