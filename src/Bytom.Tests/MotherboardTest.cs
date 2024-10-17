using Bytom.Hardware;
using Bytom.Hardware.CPU;
using Bytom.Hardware.RAM;

namespace Bytom.Tests
{
    public class MachineTest
    {
        [Fact]
        public void TestMotherboardConstruction()
        {
            Controller ram = new Controller(new List<Stick>{
                new BytomIncRam1K100(),
                new BytomIncRam1K100(),
            });
            Package cpu = new BytomIncB1(ram);
            List<Disk> disk = new List<Disk>{
                new Disk(4096),
            };
            Motherboard machine = new Motherboard(cpu, ram, disk);
        }
    }
}
