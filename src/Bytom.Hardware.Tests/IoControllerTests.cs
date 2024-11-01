namespace Bytom.Hardware.Tests
{
    public class IoControllerTests
    {
        [Test]
        public void TestRamAddressRanges()
        {
            var ram = new RAM(1024, 1000, 1, 1, 1);
            var controller = new IoController([ram], []);

            controller.powerOn(null);

            Assert.That(ram.address_range!.base_address, Is.EqualTo(Address.zero));
            Assert.That(ram.address_range!.end_address, Is.EqualTo(new Address(1024)));

            Assert.That(controller.ram_address_range, Is.EqualTo(new AddressRange(Address.zero, 1024)));
        }
        [Test]
        public void TestRamAddressRanges2()
        {
            var ram = new RAM(1024, 1000, 1, 1, 1);
            var ram2 = new RAM(1024, 1000, 1, 1, 1);
            var controller = new IoController([ram, ram2], []);

            controller.powerOn(null);

            Assert.That(ram.address_range!.base_address, Is.EqualTo(Address.zero));
            Assert.That(ram.address_range!.end_address, Is.EqualTo(new Address(1024)));

            Assert.That(ram2.address_range!.base_address, Is.EqualTo(new Address(1024)));
            Assert.That(ram2.address_range!.end_address, Is.EqualTo(new Address(2048)));

            Assert.That(controller.ram_address_range, Is.EqualTo(new AddressRange(Address.zero, 2048)));
        }
    }
}