using Bytom.Assembler.Nodes;
using Bytom.Hardware.CPU;
using Bytom.Hardware.RAM;
using Bytom.Tools;

namespace Bytom.Hardware.Tests
{
    public class DebugCore : Core
    {
        public DebugCore() : base(0, 200)
        { }
    }

    public class DebugMemoryChip : MemoryChip
    {
        public DebugMemoryChip() : base(4 * 1024, 100, 0) { }
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
            var core = new DebugCore();

            var cpu = new Package([core], 128);

            byte[] firmware = Assembler.Assembler.assemble(firmware_source).ToArray();
            ram.writeAll(0x0, firmware);

            FirmwareRom rom = new FirmwareRom(128);

            DeviceManager manager = new DeviceManager(
                new Dictionary<uint, Device>()
            );

            Motherboard motherboard = new Motherboard(cpu, ram, rom, manager);

            cpu.motherboard = motherboard;
            core.package = cpu;
            core.reset();

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
            core.executeNext();

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
            core.writeBytesToMemory(address, Serialization.Int32ToBytesBigEndian(value));
            core.RD0.writeUInt32(address);

            core.executeNext();

            Assert.That(core.RD1.readUInt32(), Is.EqualTo(expected_unsigned_value));
            Assert.That(core.RD1.readInt32(), Is.EqualTo(expected_signed_value));
        }

        [TestCase(1, 1u, 1)]
        [TestCase(int.MaxValue, uint.MaxValue / 2, int.MaxValue)]
        [TestCase(int.MinValue, uint.MaxValue / 2 + 1, int.MinValue)]
        public void TestMovMemReg(int value, uint expected_unsigned_value, int expected_signed_value)
        {
            var core = createBytomIncB1($"mov [RD0], RD1");
            // Write some random memory address to RD0
            uint address = 0x100;
            core.RD0.writeUInt32(address);
            core.RD1.writeInt32(value);

            core.executeNext();

            byte[] memory = core.readBytesFromMemory(address, 4);

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

            core.executeNext();

            Assert.That(core.RD0.readUInt32(), Is.EqualTo(expected_unsigned_value));
            Assert.That(core.RD0.readInt32(), Is.EqualTo(expected_signed_value));
        }

        [TestCase(1, 1u, 1)]
        [TestCase(int.MaxValue, uint.MaxValue / 2, int.MaxValue)]
        [TestCase(int.MinValue, uint.MaxValue / 2 + 1, int.MinValue)]
        public void TestMovMemCon(int value, uint expected_unsigned_value, int expected_signed_value)
        {
            var core = createBytomIncB1($"mov [RD0], {value}");
            // Write some random memory address to RD0
            uint address = 0x100;
            core.RD0.writeUInt32(address);

            core.executeNext();

            byte[] memory = core.readBytesFromMemory(address, 4);

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
        public void TestPushReg(int value, uint expected_unsigned_value, int expected_signed_value)
        {
            var core = createBytomIncB1($"push RD0");

            core.RD0.writeInt32(value);
            core.executeNext();

            byte[] constant_bytes = core.readBytesFromMemory(core.STP.readUInt32(), 4);

            Assert.That(Serialization.UInt32FromBytesBigEndian(constant_bytes), Is.EqualTo(expected_unsigned_value));
            Assert.That(Serialization.Int32FromBytesBigEndian(constant_bytes), Is.EqualTo(expected_signed_value));
        }

        [TestCase(1, 1u, 1)]
        [TestCase(int.MaxValue, uint.MaxValue / 2, int.MaxValue)]
        [TestCase(int.MinValue, uint.MaxValue / 2 + 1, int.MinValue)]
        public void TestPushCon(int value, uint expected_unsigned_value, int expected_signed_value)
        {
            var core = createBytomIncB1($"push {value}");

            core.executeNext();

            byte[] constant_bytes = core.readBytesFromMemory(core.STP.readUInt32(), 4);

            Assert.That(Serialization.UInt32FromBytesBigEndian(constant_bytes), Is.EqualTo(expected_unsigned_value));
            Assert.That(Serialization.Int32FromBytesBigEndian(constant_bytes), Is.EqualTo(expected_signed_value));
        }

        [TestCase(1, 1u, 1)]
        [TestCase(int.MaxValue, uint.MaxValue / 2, int.MaxValue)]
        [TestCase(int.MinValue, uint.MaxValue / 2 + 1, int.MinValue)]
        public void TestPushMem(int value, uint expected_unsigned_value, int expected_signed_value)
        {
            var core = createBytomIncB1($"push [RD0]");
            // Write some random memory address to RD0
            uint address = 0x100;
            core.writeBytesToMemory(address, Serialization.Int32ToBytesBigEndian(value));
            core.RD0.writeUInt32(address);

            core.executeNext();

            byte[] constant_bytes = core.readBytesFromMemory(core.STP.readUInt32(), 4);

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

            core.executeNext();
            core.executeNext();

            Assert.That(core.RD0.readUInt32(), Is.EqualTo(expected_unsigned_value));
            Assert.That(core.RD0.readInt32(), Is.EqualTo(expected_signed_value));
        }

        [TestCase(1, 1u, 1)]
        [TestCase(int.MaxValue, uint.MaxValue / 2, int.MaxValue)]
        [TestCase(int.MinValue, uint.MaxValue / 2 + 1, int.MinValue)]
        public void TestPopMem(int value, uint expected_unsigned_value, int expected_signed_value)
        {
            var core = createBytomIncB1($@"
                push {value}
                pop [RD0]
            ");
            uint address = 0x100;
            core.RD0.writeUInt32(address);

            core.executeNext();
            core.executeNext();

            byte[] memory = core.readBytesFromMemory(address, 4);

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
        [TestCase("mul", 1, 0, 0, true, false, false)]
        [TestCase("mul", int.MaxValue, int.MaxValue, 1, false, false, true)]
        [TestCase("mul", int.MinValue, int.MaxValue, -2147483648, false, true, true)]
        [TestCase("mul", 56, -33, -1848, false, true, true)]
        [TestCase("div", 1, 0, 0, false, false, false)]
        [TestCase("div", 10, 2, 5, false, false, false)]
        [TestCase("div", -10, 2, 2147483643, false, false, false)]
        [TestCase("div", -10, -2, 0, true, false, false)]
        [TestCase("idiv", 1, 0, 0, false, false, false)]
        [TestCase("idiv", 10, 2, 5, false, false, false)]
        [TestCase("idiv", -10, 2, -5, false, true, false)]
        [TestCase("idiv", 10, -2, -5, false, true, false)]
        [TestCase("and", 0b0110_0110, 0b0000_1111, 0b0000_0110, false, false, false)]
        [TestCase("or", 0b0110_0110, 0b0000_1111, 0b0110_1111, false, false, false)]
        [TestCase("xor", 0b0110_0110, 0b0000_1111, 0b0110_1001, false, false, false)]
        [TestCase("shl", 1, 64, 0, true, false, true)]
        [TestCase("shl", 1, 32, 0, true, false, true)]
        [TestCase("shl", 1, 31, -2147483648, false, true, false)]
        [TestCase("shr", 1, 64, 0, true, false, true)]
        [TestCase("shr", 1, 32, 0, true, false, true)]
        [TestCase("shr", -2147483648, 31, 1, false, false, false)]
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

            core.executeNext();
            core.executeNext();
            core.executeNext();

            Assert.That(core.RD0.readInt32(), Is.EqualTo(expected));

            core.executeNext();

            Assert.That(core.CCR.readBit(0), Is.EqualTo(zero));
            Assert.That(core.CCR.readBit(2), Is.EqualTo(negative));
            Assert.That(core.CCR.readBit(3), Is.EqualTo(overflow));
        }

        [TestCase("add", uint.MaxValue, 1u, uint.MinValue, true, true)]
        [TestCase("add", 1u, 1u, 2u, false, false)]
        [TestCase("add", uint.MaxValue, 64324u, 64323u, false, true)]
        [TestCase("add", 45u, 78u, 123u, false, false)]
        [TestCase("add", 78u, 45u, 123u, false, false)]
        [TestCase("sub", uint.MaxValue, 1u, uint.MaxValue - 1u, false, false)]
        [TestCase("sub", 45u, 78u, 4294967263u, false, true)]
        [TestCase("sub", 78u, 45u, 33u, false, false)]
        [TestCase("mul", 1u, 0u, 0u, true, false)]
        [TestCase("mul", uint.MaxValue, uint.MaxValue, 1u, false, true)]
        [TestCase("mul", uint.MinValue, uint.MaxValue, 0u, true, false)]
        [TestCase("mul", 56u, 33u, 1848u, false, false)]
        [TestCase("imul", 1u, 0u, 0u, true, false)]
        [TestCase("imul", uint.MaxValue, uint.MaxValue, 1u, false, true)]
        [TestCase("imul", uint.MinValue, uint.MaxValue, 0u, true, false)]
        [TestCase("imul", 56u, 33u, 1848u, false, false)]
        [TestCase("div", 1u, 0u, 0u, false, false)]
        [TestCase("div", 10u, 2u, 5u, false, false)]
        [TestCase("div", 2u, 10u, 0u, true, false)]
        [TestCase("and", 0b0110_0110u, 0b0000_1111u, 0b0000_0110u, false, false)]
        [TestCase("or", 0b0110_0110u, 0b0000_1111u, 0b0110_1111u, false, false)]
        [TestCase("xor", 0b0110_0110u, 0b0000_1111u, 0b0110_1001u, false, false)]
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

            core.executeNext();
            core.executeNext();
            core.executeNext();

            Assert.That(core.RD0.readUInt32(), Is.EqualTo(expected));

            core.executeNext();

            Assert.That(core.CCR.readBit(0), Is.EqualTo(zero));
            Assert.That(core.CCR.readBit(1), Is.EqualTo(carry));
        }

        [Test]
        public void TestJmpMem()
        {
            var address = 0x100;
            var core = createBytomIncB1($@"
                jmp [RD0]
            ");
            core.RD0.writeInt32(address);

            core.executeNext();

            Assert.That(core.IP.readUInt32(), Is.EqualTo(address));
        }

        [Test]
        public void TestJmpCon()
        {
            var address = 0x100;
            var core = createBytomIncB1($@"
                jmp {address}
            ");

            core.executeNext();

            Assert.That(core.IP.readUInt32(), Is.EqualTo(address));
        }

        public class TestJEQ
        {
            public static string instruction = "jeq";

            [Test]
            public void TestMemTaken()
            {
                var address = 0x100;
                var core = createBytomIncB1($@"
                cmp RD1, RD2
                {instruction} [RD0]
            ");
                core.RD0.writeInt32(address);
                core.RD1.writeInt32(1);
                core.RD2.writeInt32(1);

                core.executeNext();
                core.executeNext();

                Assert.That(core.IP.readUInt32(), Is.EqualTo(address));
            }

            [Test]
            public void TestMemNotTaken()
            {
                var address = 0x100;
                var core = createBytomIncB1($@"
                cmp RD1, RD2
                {instruction} [RD0]
            ");
                core.RD0.writeInt32(address);
                core.RD1.writeInt32(1);
                core.RD2.writeInt32(2);

                core.executeNext();
                core.executeNext();

                Assert.That(core.IP.readUInt32(), Is.Not.EqualTo(address));
            }

            [Test]
            public void TestConTaken()
            {
                var address = 0x100;
                var core = createBytomIncB1($@"
                cmp RD1, RD2
                {instruction} {address}
            ");
                core.RD1.writeInt32(1);
                core.RD2.writeInt32(1);

                core.executeNext();
                core.executeNext();

                Assert.That(core.IP.readUInt32(), Is.EqualTo(address));
            }

            [Test]
            public void TestConNotTaken()
            {
                var address = 0x100;
                var core = createBytomIncB1($@"
                cmp RD1, RD2
                {instruction} {address}
            ");
                core.RD1.writeInt32(1);
                core.RD2.writeInt32(2);

                core.executeNext();
                core.executeNext();

                Assert.That(core.IP.readUInt32(), Is.Not.EqualTo(address));
            }
        }

        public class TestJNE
        {
            public static string instruction = "jne";

            [Test]
            public void TestMemTaken()
            {
                var address = 0x100;
                var core = createBytomIncB1($@"
                cmp RD1, RD2
                {instruction} [RD0]
            ");
                core.RD0.writeInt32(address);
                core.RD1.writeInt32(1);
                core.RD2.writeInt32(2);

                core.executeNext();
                core.executeNext();

                Assert.That(core.IP.readUInt32(), Is.EqualTo(address));
            }

            [Test]
            public void TestMemNotTaken()
            {
                var address = 0x100;
                var core = createBytomIncB1($@"
                cmp RD1, RD2
                {instruction} [RD0]
            ");
                core.RD0.writeInt32(address);
                core.RD1.writeInt32(1);
                core.RD2.writeInt32(1);

                core.executeNext();
                core.executeNext();

                Assert.That(core.IP.readUInt32(), Is.Not.EqualTo(address));
            }

            [Test]
            public void TestConTaken()
            {
                var address = 0x100;
                var core = createBytomIncB1($@"
                cmp RD1, RD2
                {instruction} {address}
            ");
                core.RD1.writeInt32(1);
                core.RD2.writeInt32(2);

                core.executeNext();
                core.executeNext();

                Assert.That(core.IP.readUInt32(), Is.EqualTo(address));
            }

            [Test]
            public void TestConNotTaken()
            {
                var address = 0x100;
                var core = createBytomIncB1($@"
                cmp RD1, RD2
                {instruction} {address}
            ");
                core.RD1.writeInt32(1);
                core.RD2.writeInt32(1);

                core.executeNext();
                core.executeNext();

                Assert.That(core.IP.readUInt32(), Is.Not.EqualTo(address));
            }
        }

        public class TestJA
        {
            public virtual string getInstruction() => "ja";

            [Test]
            public void TestMemTaken()
            {
                var address = 0x100;
                var core = createBytomIncB1($@"
                cmp RD1, RD2
                {getInstruction()} [RD0]
            ");
                core.RD0.writeInt32(address);
                core.RD1.writeInt32(2);
                core.RD2.writeInt32(1);

                core.executeNext();
                core.executeNext();

                Assert.That(core.IP.readUInt32(), Is.EqualTo(address));
            }

            [Test]
            public void TestMemNotTaken()
            {
                var address = 0x100;
                var core = createBytomIncB1($@"
                cmp RD1, RD2
                {getInstruction()} [RD0]
            ");
                core.RD0.writeInt32(address);
                core.RD1.writeInt32(1);
                core.RD2.writeInt32(1);

                core.executeNext();
                core.executeNext();

                Assert.That(core.IP.readUInt32(), Is.Not.EqualTo(address));
            }

            [Test]
            public void TestConTaken()
            {
                var address = 0x100;
                var core = createBytomIncB1($@"
                cmp RD1, RD2
                {getInstruction()} {address}
            ");
                core.RD1.writeInt32(2);
                core.RD2.writeInt32(1);

                core.executeNext();
                core.executeNext();

                Assert.That(core.IP.readUInt32(), Is.EqualTo(address));
            }

            [Test]
            public virtual void TestConNeg()
            {
                var address = 0x100;
                var core = createBytomIncB1($@"
                cmp RD1, RD2
                {getInstruction()} {address}
            ");
                core.RD1.writeInt32(-1);
                core.RD2.writeInt32(2);

                core.executeNext();
                core.executeNext();

                Assert.That(core.IP.readUInt32(), Is.EqualTo(address));
            }

            [Test]
            public void TestConNotTaken()
            {
                var address = 0x100;
                var core = createBytomIncB1($@"
                cmp RD1, RD2
                {getInstruction()} {address}
            ");
                core.RD1.writeInt32(1);
                core.RD2.writeInt32(1);

                core.executeNext();
                core.executeNext();

                Assert.That(core.IP.readUInt32(), Is.Not.EqualTo(address));
            }
        }

        public class TestJAE
        {
            public virtual string getInstruction() => "jae";

            [Test]
            public void TestMemTaken()
            {
                var address = 0x100;
                var core = createBytomIncB1($@"
                cmp RD1, RD2
                {getInstruction()} [RD0]
            ");
                core.RD0.writeInt32(address);
                core.RD1.writeInt32(2);
                core.RD2.writeInt32(1);

                core.executeNext();
                core.executeNext();

                Assert.That(core.IP.readUInt32(), Is.EqualTo(address));
            }

            [Test]
            public void TestMemNotTaken()
            {
                var address = 0x100;
                var core = createBytomIncB1($@"
                cmp RD1, RD2
                {getInstruction()} [RD0]
            ");
                core.RD0.writeInt32(address);
                core.RD1.writeInt32(1);
                core.RD2.writeInt32(2);

                core.executeNext();
                core.executeNext();

                Assert.That(core.IP.readUInt32(), Is.Not.EqualTo(address));
            }

            [Test]
            public void TestConTaken()
            {
                var address = 0x100;
                var core = createBytomIncB1($@"
                cmp RD1, RD2
                {getInstruction()} {address}
            ");
                core.RD1.writeInt32(2);
                core.RD2.writeInt32(1);

                core.executeNext();
                core.executeNext();

                Assert.That(core.IP.readUInt32(), Is.EqualTo(address));
            }

            [Test]
            public virtual void TestConNeg()
            {
                var address = 0x100;
                var core = createBytomIncB1($@"
                cmp RD1, RD2
                {getInstruction()} {address}
            ");
                core.RD1.writeInt32(-1);
                core.RD2.writeInt32(2);

                core.executeNext();
                core.executeNext();

                Assert.That(core.IP.readUInt32(), Is.EqualTo(address));
            }

            [Test]
            public void TestConTakenEq()
            {
                var address = 0x100;
                var core = createBytomIncB1($@"
                cmp RD1, RD2
                {getInstruction()} {address}
            ");
                core.RD1.writeInt32(1);
                core.RD2.writeInt32(1);

                core.executeNext();
                core.executeNext();

                Assert.That(core.IP.readUInt32(), Is.EqualTo(address));
            }

            [Test]
            public void TestConNotTaken()
            {
                var address = 0x100;
                var core = createBytomIncB1($@"
                cmp RD1, RD2
                {getInstruction()} {address}
            ");
                core.RD1.writeInt32(1);
                core.RD2.writeInt32(2);

                core.executeNext();
                core.executeNext();

                Assert.That(core.IP.readUInt32(), Is.Not.EqualTo(address));
            }
        }

        public class TestJB
        {
            public virtual string getInstruction() => "jb";

            [Test]
            public void TestMemTaken()
            {
                var address = 0x100;
                var core = createBytomIncB1($@"
                cmp RD1, RD2
                {getInstruction()} [RD0]
            ");
                core.RD0.writeInt32(address);
                core.RD1.writeInt32(1);
                core.RD2.writeInt32(2);

                core.executeNext();
                core.executeNext();

                Assert.That(core.IP.readUInt32(), Is.EqualTo(address));
            }

            [Test]
            public void TestMemNotTaken()
            {
                var address = 0x100;
                var core = createBytomIncB1($@"
                cmp RD1, RD2
                {getInstruction()} [RD0]
            ");
                core.RD0.writeInt32(address);
                core.RD1.writeInt32(1);
                core.RD2.writeInt32(1);

                core.executeNext();
                core.executeNext();

                Assert.That(core.IP.readUInt32(), Is.Not.EqualTo(address));
            }

            [Test]
            public void TestConTaken()
            {
                var address = 0x100;
                var core = createBytomIncB1($@"
                cmp RD1, RD2
                {getInstruction()} {address}
            ");
                core.RD1.writeInt32(1);
                core.RD2.writeInt32(2);

                core.executeNext();
                core.executeNext();

                Assert.That(core.IP.readUInt32(), Is.EqualTo(address));
            }

            [Test]
            public virtual void TestConNeg()
            {
                var address = 0x100;
                var core = createBytomIncB1($@"
                cmp RD1, RD2
                {getInstruction()} {address}
            ");
                core.RD1.writeInt32(1);
                core.RD2.writeInt32(-1);

                core.executeNext();
                core.executeNext();

                Assert.That(core.IP.readUInt32(), Is.EqualTo(address));
            }

            [Test]
            public void TestConNotTaken()
            {
                var address = 0x100;
                var core = createBytomIncB1($@"
                cmp RD1, RD2
                {getInstruction()} {address}
            ");
                core.RD1.writeInt32(1);
                core.RD2.writeInt32(1);

                core.executeNext();
                core.executeNext();

                Assert.That(core.IP.readUInt32(), Is.Not.EqualTo(address));
            }
        }

        public class TestJBE
        {
            public virtual string getInstruction() => "jbe";

            [Test]
            public void TestMemTaken()
            {
                var address = 0x100;
                var core = createBytomIncB1($@"
                cmp RD1, RD2
                {getInstruction()} [RD0]
            ");
                core.RD0.writeInt32(address);
                core.RD1.writeInt32(1);
                core.RD2.writeInt32(1);

                core.executeNext();
                core.executeNext();

                Assert.That(core.IP.readUInt32(), Is.EqualTo(address));
            }

            [Test]
            public void TestMemNotTaken()
            {
                var address = 0x100;
                var core = createBytomIncB1($@"
                cmp RD1, RD2
                {getInstruction()} [RD0]
            ");
                core.RD0.writeInt32(address);
                core.RD1.writeInt32(2);
                core.RD2.writeInt32(1);

                core.executeNext();
                core.executeNext();

                Assert.That(core.IP.readUInt32(), Is.Not.EqualTo(address));
            }

            [Test]
            public void TestConTaken()
            {
                var address = 0x100;
                var core = createBytomIncB1($@"
                cmp RD1, RD2
                {getInstruction()} {address}
            ");
                core.RD1.writeInt32(1);
                core.RD2.writeInt32(2);

                core.executeNext();
                core.executeNext();

                Assert.That(core.IP.readUInt32(), Is.EqualTo(address));
            }

            [Test]
            public virtual void TestConNeg()
            {
                var address = 0x100;
                var core = createBytomIncB1($@"
                cmp RD1, RD2
                {getInstruction()} {address}
            ");
                core.RD1.writeInt32(2);
                core.RD2.writeInt32(-1);

                core.executeNext();
                core.executeNext();

                Assert.That(core.IP.readUInt32(), Is.EqualTo(address));
            }

            [Test]
            public void TestConTakenEq()
            {
                var address = 0x100;
                var core = createBytomIncB1($@"
                cmp RD1, RD2
                {getInstruction()} {address}
            ");
                core.RD1.writeInt32(1);
                core.RD2.writeInt32(1);

                core.executeNext();
                core.executeNext();

                Assert.That(core.IP.readUInt32(), Is.EqualTo(address));
            }

            [Test]
            public void TestConNotTaken()
            {
                var address = 0x100;
                var core = createBytomIncB1($@"
                cmp RD1, RD2
                {getInstruction()} {address}
            ");
                core.RD1.writeInt32(2);
                core.RD2.writeInt32(1);

                core.executeNext();
                core.executeNext();

                Assert.That(core.IP.readUInt32(), Is.Not.EqualTo(address));
            }
        }

        public class TestJGT : TestJA
        {
            public override string getInstruction() => "jgt";

            [Test]
            public override void TestConNeg()
            {
                var address = 0x100;
                var core = createBytomIncB1($@"
                cmp RD1, RD2
                {getInstruction()} {address}
            ");
                core.RD1.writeInt32(-1);
                core.RD2.writeInt32(2);

                core.executeNext();
                core.executeNext();

                Assert.That(core.IP.readUInt32(), Is.Not.EqualTo(address));
            }
        }

        public class TestJGE : TestJAE
        {
            public override string getInstruction() => "jge";

            [Test]
            public override void TestConNeg()
            {
                var address = 0x100;
                var core = createBytomIncB1($@"
                cmp RD1, RD2
                {getInstruction()} {address}
            ");
                core.RD1.writeInt32(-1);
                core.RD2.writeInt32(2);

                core.executeNext();
                core.executeNext();

                Assert.That(core.IP.readUInt32(), Is.Not.EqualTo(address));
            }
        }

        public class TestJLT : TestJB
        {
            public override string getInstruction() => "jlt";

            [Test]
            public override void TestConNeg()
            {
                var address = 0x100;
                var core = createBytomIncB1($@"
                cmp RD1, RD2
                {getInstruction()} {address}
            ");
                core.RD1.writeInt32(1);
                core.RD2.writeInt32(-1);

                core.executeNext();
                core.executeNext();

                Assert.That(core.IP.readUInt32(), Is.Not.EqualTo(address));
            }
        }

        public class TestJLE : TestJBE
        {
            public override string getInstruction() => "jle";

            [Test]
            public override void TestConNeg()
            {
                var address = 0x100;
                var core = createBytomIncB1($@"
                cmp RD1, RD2
                {getInstruction()} {address}
            ");
                core.RD1.writeInt32(2);
                core.RD2.writeInt32(-1);

                core.executeNext();
                core.executeNext();

                Assert.That(core.IP.readUInt32(), Is.Not.EqualTo(address));
            }
        }

        [Test]
        public void TestCallMem()
        {
            var address = 0x100;
            var core = createBytomIncB1($@"
                call [RD0]
            ");
            core.RD0.writeInt32(address);

            core.executeNext();

            Assert.That(core.popUInt32Stack(), Is.EqualTo(4u));
            Assert.That(core.IP.readUInt32(), Is.EqualTo(address));
        }

        [Test]
        public void TestCallCon()
        {
            var address = 0x100;
            var core = createBytomIncB1($@"
                call {address}
            ");

            core.executeNext();

            Assert.That(core.popUInt32Stack(), Is.EqualTo(4u));
            Assert.That(core.IP.readUInt32(), Is.EqualTo(address));
        }

        [Test]
        public void TestRet()
        {
            var address = 8 + 4 + 4 + 4;
            var core = createBytomIncB1($@"
                call {address}
                halt
                halt
                halt
                ret
            ");

            core.executeNext();
            core.executeNext();

            Assert.That(core.IP.readUInt32(), Is.EqualTo(4u));
        }
    }
}
