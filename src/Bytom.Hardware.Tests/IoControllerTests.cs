namespace Bytom.Hardware.Tests
{
    public class IoControllerTests
    {
        [Test]
        public void TestRamAddressRanges()
        {
            var ram = new RAM(1024, 1000, 1, 1, 1);
            var rom = new FirmwareRom(1024, 1000, 1, 1, 1);
            var controller = new IoController([ram], rom, []);

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
            var rom = new FirmwareRom(1024, 1000, 1, 1, 1);
            var controller = new IoController([ram, ram2], rom, []);

            controller.powerOn(null);

            Assert.That(ram.address_range!.base_address, Is.EqualTo(Address.zero));
            Assert.That(ram.address_range!.end_address, Is.EqualTo(new Address(1024)));

            Assert.That(ram2.address_range!.base_address, Is.EqualTo(new Address(1024)));
            Assert.That(ram2.address_range!.end_address, Is.EqualTo(new Address(2048)));

            Assert.That(controller.ram_address_range, Is.EqualTo(new AddressRange(Address.zero, 2048)));
        }

        [Test]
        public void TestFirmwareRomAddressRanges()
        {
            var ram = new RAM(1024, 1000, 1, 1, 1);
            var rom = new FirmwareRom(1024, 1000, 1, 1, 1);
            var controller = new IoController([ram], rom, []);

            controller.powerOn(null);

            Assert.That(rom.address_range!.base_address, Is.EqualTo(new Address(0xFFFFFBFF)));
            Assert.That(rom.address_range!.end_address, Is.EqualTo(new Address(0xFFFFFFFF)));

            Assert.That(controller.firmware_address_range, Is.EqualTo(new AddressRange(new Address(0xFFFFFBFF), 1024)));
        }
    }
}