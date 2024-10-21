using Bytom.Hardware.CPU;
using Bytom.Hardware.RAM;
using Bytom.Tools;

namespace Bytom.Hardware.Tests
{
    public class DebugCore : Core
    {
        public DebugCore(Controller ram) : base(
            0, 200, new List<Cache> { new Cache(256, 10) }, ram
        )
        { }
    }

    public class DebugMemoryChip : MemoryChip
    {
        public DebugMemoryChip() : base(16 * 1024, 100, 0) { }
    }

    public class CoreTest
    {
        [SetUp]
        public void Setup()
        {
        }

        private static Core createBytomIncB1(string firmware_source)
        {
            Controller ram = new Controller([new DebugMemoryChip()]);
            var core = new DebugCore(ram);

            byte[] firmware = Assembler.Assembler.assemble(firmware_source).ToArray();
            ram.writeAll(0x0, firmware).Wait();

            return core;
        }

        [Test]
        public void TestKernelMode()
        {
            var core = createBytomIncB1("");
            Assert.That(core.isKernelMode(), Is.EqualTo(true));
        }

        [TestCase(1, 1u, 1)]
        [TestCase(int.MaxValue, uint.MaxValue / 2, int.MaxValue)]
        [TestCase(int.MinValue, uint.MaxValue / 2 + 1, int.MinValue)]
        public void TestMovRegReg(int value, uint expected_unsigned_value, int expected_signed_value)
        {
            var core = createBytomIncB1($"mov RD1, RD0");

            core.RD0.writeInt32(value);
            core.executeNext().Wait();

            Assert.That(core.RD1.readUInt32(), Is.EqualTo(expected_unsigned_value));
            Assert.That(core.RD1.readInt32(), Is.EqualTo(expected_signed_value));
        }

        [TestCase(1, 1u, 1)]
        [TestCase(int.MaxValue, uint.MaxValue / 2, int.MaxValue)]
        [TestCase(int.MinValue, uint.MaxValue / 2 + 1, int.MinValue)]
        public void TestMovRegMem(int value, uint expected_unsigned_value, int expected_signed_value)
        {
            var core = createBytomIncB1($"mov RD1, [RD0]");
            // Write some random memory address to RD0
            uint address = 0x100;
            core.writeBytesToMemory(address, Serialization.Int32ToBytesBigEndian(value)).Wait();
            core.RD0.writeUInt32(address);

            core.executeNext().Wait();

            Assert.That(core.RD1.readUInt32(), Is.EqualTo(expected_unsigned_value));
            Assert.That(core.RD1.readInt32(), Is.EqualTo(expected_signed_value));
        }

        [TestCase(1, 1u, 1)]
        [TestCase(int.MaxValue, uint.MaxValue / 2, int.MaxValue)]
        [TestCase(int.MinValue, uint.MaxValue / 2 + 1, int.MinValue)]
        public async Task TestMovMemReg(int value, uint expected_unsigned_value, int expected_signed_value)
        {
            var core = createBytomIncB1($"mov [RD0], RD1");
            // Write some random memory address to RD0
            uint address = 0x100;
            core.RD0.writeUInt32(address);
            core.RD1.writeInt32(value);

            await core.executeNext();

            byte[] memory = await core.readBytesFromMemory(address, 4);

            Assert.That(
                Serialization.UInt32FromBytesBigEndian(memory),
                Is.EqualTo(expected_unsigned_value)
            );
            Assert.That(
                Serialization.Int32FromBytesBigEndian(memory),
                Is.EqualTo(expected_signed_value)
            );
        }

        [TestCase(1, 1u, 1)]
        [TestCase(63, 63u, 63)]
        [TestCase(255, 255u, 255)]
        [TestCase(734534, 734534u, 734534)]
        [TestCase(-734534, 4294232762, -734534)]
        [TestCase(int.MaxValue, uint.MaxValue / 2, int.MaxValue)]
        [TestCase(int.MinValue, uint.MaxValue / 2 + 1, int.MinValue)]
        public void TestMovRegCon(int value, uint expected_unsigned_value, int expected_signed_value)
        {
            var core = createBytomIncB1($"mov RD0, {value}");

            core.executeNext().Wait();

            Assert.That(core.RD0.readUInt32(), Is.EqualTo(expected_unsigned_value));
            Assert.That(core.RD0.readInt32(), Is.EqualTo(expected_signed_value));
        }

        [TestCase(1, 1u, 1)]
        [TestCase(int.MaxValue, uint.MaxValue / 2, int.MaxValue)]
        [TestCase(int.MinValue, uint.MaxValue / 2 + 1, int.MinValue)]
        public async Task TestMovMemCon(int value, uint expected_unsigned_value, int expected_signed_value)
        {
            var core = createBytomIncB1($"mov [RD0], {value}");
            // Write some random memory address to RD0
            uint address = 0x100;
            core.RD0.writeUInt32(address);

            await core.executeNext();

            byte[] memory = await core.readBytesFromMemory(address, 4);

            Assert.That(
                Serialization.UInt32FromBytesBigEndian(memory),
                Is.EqualTo(expected_unsigned_value)
            );
            Assert.That(
                Serialization.Int32FromBytesBigEndian(memory),
                Is.EqualTo(expected_signed_value)
            );
        }

        [TestCase(1, 1u, 1)]
        [TestCase(int.MaxValue, uint.MaxValue / 2, int.MaxValue)]
        [TestCase(int.MinValue, uint.MaxValue / 2 + 1, int.MinValue)]
        public async Task TestPushReg(int value, uint expected_unsigned_value, int expected_signed_value)
        {
            var core = createBytomIncB1($"push RD0");

            core.RD0.writeInt32(value);
            core.executeNext().Wait();

            byte[] constant_bytes = await core.readBytesFromMemory(core.STP.readUInt32(), 4);

            Assert.That(Serialization.UInt32FromBytesBigEndian(constant_bytes), Is.EqualTo(expected_unsigned_value));
            Assert.That(Serialization.Int32FromBytesBigEndian(constant_bytes), Is.EqualTo(expected_signed_value));
        }

        [TestCase(1, 1u, 1)]
        [TestCase(int.MaxValue, uint.MaxValue / 2, int.MaxValue)]
        [TestCase(int.MinValue, uint.MaxValue / 2 + 1, int.MinValue)]
        public async Task TestPushCon(int value, uint expected_unsigned_value, int expected_signed_value)
        {
            var core = createBytomIncB1($"push {value}");

            core.executeNext().Wait();

            byte[] constant_bytes = await core.readBytesFromMemory(core.STP.readUInt32(), 4);

            Assert.That(Serialization.UInt32FromBytesBigEndian(constant_bytes), Is.EqualTo(expected_unsigned_value));
            Assert.That(Serialization.Int32FromBytesBigEndian(constant_bytes), Is.EqualTo(expected_signed_value));
        }

        [TestCase(1, 1u, 1)]
        [TestCase(int.MaxValue, uint.MaxValue / 2, int.MaxValue)]
        [TestCase(int.MinValue, uint.MaxValue / 2 + 1, int.MinValue)]
        public async Task TestPushMem(int value, uint expected_unsigned_value, int expected_signed_value)
        {
            var core = createBytomIncB1($"push [RD0]");
            // Write some random memory address to RD0
            uint address = 0x100;
            await core.writeBytesToMemory(address, Serialization.Int32ToBytesBigEndian(value));
            core.RD0.writeUInt32(address);

            await core.executeNext();

            byte[] constant_bytes = await core.readBytesFromMemory(core.STP.readUInt32(), 4);

            Assert.That(Serialization.UInt32FromBytesBigEndian(constant_bytes), Is.EqualTo(expected_unsigned_value));
            Assert.That(Serialization.Int32FromBytesBigEndian(constant_bytes), Is.EqualTo(expected_signed_value));
        }

        [TestCase(1, 1u, 1)]
        [TestCase(int.MaxValue, uint.MaxValue / 2, int.MaxValue)]
        [TestCase(int.MinValue, uint.MaxValue / 2 + 1, int.MinValue)]
        public void TestPopReg(int value, uint expected_unsigned_value, int expected_signed_value)
        {
            var core = createBytomIncB1($@"
                push {value}
                pop RD0
            ");

            core.executeNext().Wait();
            core.executeNext().Wait();

            Assert.That(core.RD0.readUInt32(), Is.EqualTo(expected_unsigned_value));
            Assert.That(core.RD0.readInt32(), Is.EqualTo(expected_signed_value));
        }

        [TestCase(1, 1u, 1)]
        [TestCase(int.MaxValue, uint.MaxValue / 2, int.MaxValue)]
        [TestCase(int.MinValue, uint.MaxValue / 2 + 1, int.MinValue)]
        public async Task TestPopMem(int value, uint expected_unsigned_value, int expected_signed_value)
        {
            var core = createBytomIncB1($@"
                push {value}
                pop [RD0]
            ");
            uint address = 0x100;
            core.RD0.writeUInt32(address);

            core.executeNext().Wait();
            core.executeNext().Wait();

            byte[] memory = await core.readBytesFromMemory(address, 4);

            Assert.That(Serialization.UInt32FromBytesBigEndian(memory), Is.EqualTo(expected_unsigned_value));
            Assert.That(Serialization.Int32FromBytesBigEndian(memory), Is.EqualTo(expected_signed_value));
        }

        [TestCase("add", int.MaxValue, int.MaxValue, -2, false, true, true)]
        [TestCase("add", 0, 0, 0, true, false, false)]
        [TestCase("add", 534216, 34245, 568461, false, false, false)]
        [TestCase("add", -1, 1, 0, true, false, false)]
        [TestCase("add", -1, -1, -2, false, true, false)]
        [TestCase("sub", int.MinValue, 1, int.MaxValue, false, false, true)]
        [TestCase("sub", 1, 1, 0, true, false, false)]
        [TestCase("sub", -10, 10, -20, false, true, false)]
        [TestCase("sub", int.MaxValue, int.MinValue, -1, false, true, true)]
        public void TestArithmeticSigned(
            string instruction,
            int left,
            int right,
            int expected,
            bool zero,
            bool negative,
            bool overflow
        )
        {
            var core = createBytomIncB1($@"
                mov RD0, {left}
                mov RD1, {right}
                {instruction} RD0, RD1
            ");

            core.executeNext().Wait();
            core.executeNext().Wait();
            core.executeNext().Wait();

            Assert.That(core.RD0.readInt32(), Is.EqualTo(expected));

            core.executeNext().Wait();

            Assert.That(core.CCR.readBit(0), Is.EqualTo(zero));
            Assert.That(core.CCR.readBit(2), Is.EqualTo(negative));
            Assert.That(core.CCR.readBit(3), Is.EqualTo(overflow));
        }

        [TestCase("add", uint.MaxValue, 1u, 0u, true, true)]
        [TestCase("add", 1u, 1u, 2u, false, false)]
        [TestCase("add", uint.MaxValue, 64324u, 64323u, false, true)]
        [TestCase("add", 45u, 78u, 123u, false, false)]
        [TestCase("add", 78u, 45u, 123u, false, false)]
        [TestCase("sub", uint.MaxValue, 1u, uint.MaxValue - 1u, false, false)]
        [TestCase("sub", 45u, 78u, 4294967263u, false, true)]
        [TestCase("sub", 78u, 45u, 33u, false, false)]
        public void TestArithmeticUnsigned(
            string instruction,
            uint left,
            uint right,
            uint expected,
            bool zero,
            bool carry
        )
        {
            var core = createBytomIncB1($@"
                mov RD0, {left}
                mov RD1, {right}
                {instruction} RD0, RD1
            ");

            core.executeNext().Wait();
            core.executeNext().Wait();
            core.executeNext().Wait();

            Assert.That(core.RD0.readUInt32(), Is.EqualTo(expected));

            core.executeNext().Wait();

            Assert.That(core.CCR.readBit(0), Is.EqualTo(zero));
            Assert.That(core.CCR.readBit(1), Is.EqualTo(carry));
        }
    }
}
