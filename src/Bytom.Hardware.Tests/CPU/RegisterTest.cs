using Bytom.Hardware.CPU;

namespace Bytom.Hardware.Tests
{
    public class RegisterTest
    {
        [TestCase(1u, 1u, 1)]
        [TestCase(uint.MaxValue, uint.MaxValue, -1)]
        [TestCase(63u, 63u, 63)]
        [TestCase(255u, 255u, 255)]
        [TestCase(uint.MaxValue / 2, uint.MaxValue / 2, int.MaxValue)]
        [TestCase(uint.MaxValue / 2 + 1, uint.MaxValue / 2 + 1, int.MinValue)]
        public void TestIntegerRW(uint value, uint expected_unsigned_value, int expected_signed_value)
        {
            Register32 register = new Register32(value);

            Assert.That(register.readUInt32(), Is.EqualTo(expected_unsigned_value));
            Assert.That(register.readInt32(), Is.EqualTo(expected_signed_value));
        }
    }
}