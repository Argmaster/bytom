using Bytom.Hardware.CPU;

namespace Bytom.Hardware.Tests
{
    public class RegisterTest
    {
        [TestCase(1, 1u, 1)]
        [TestCase(-1, uint.MaxValue, -1)]
        [TestCase(63, 63u, 63)]
        [TestCase(255, 255u, 255)]
        [TestCase(int.MaxValue, uint.MaxValue / 2, int.MaxValue)]
        [TestCase(int.MinValue, uint.MaxValue / 2 + 1, int.MinValue)]
        public void TestIntegerRW(int value, uint expected_unsigned_value, int expected_signed_value)
        {
            Register32 register = new Register32(value);

            Assert.That(register.ReadUInt32(), Is.EqualTo(expected_unsigned_value));
            Assert.That(register.ReadInt32(), Is.EqualTo(expected_signed_value));
        }
    }
}