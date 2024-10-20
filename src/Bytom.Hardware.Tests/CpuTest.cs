using Bytom.Hardware.CPU;
using Bytom.Hardware.RAM;

namespace Bytom.Hardware.Tests
{
    public class CpuTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestMotherboardConstruction()
        {
            Controller ram = new Controller([
                new BytomIncRam16KGen1(),
            ]);
            Package cpu = new BytomIncGen1(ram);

            var core0 = cpu.cores[0];
            core0.registers[RegisterID.RD0].WriteInt32(0xFF);
            Assert.That(core0.RD0.ReadInt32(), Is.EqualTo(0xFF));
        }
    }
}
