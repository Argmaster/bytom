using Bytom.Hardware.CPU;
using System.Collections;

namespace Bytom.Hardware.CPU.Tests
{
    public class AluTests
    {

        [TestCase(ALUOperationType.ADD, int.MaxValue, int.MaxValue, -2, false, true, true)]
        [TestCase(ALUOperationType.ADD, 0, 0, 0, true, false, false)]
        [TestCase(ALUOperationType.ADD, 534216, 34245, 568461, false, false, false)]
        [TestCase(ALUOperationType.ADD, -1, 1, 0, true, false, false)]
        [TestCase(ALUOperationType.ADD, -1, -1, -2, false, true, false)]
        [TestCase(ALUOperationType.SUB, int.MinValue, 1, int.MaxValue, false, false, true)]
        [TestCase(ALUOperationType.SUB, 1, 1, 0, true, false, false)]
        [TestCase(ALUOperationType.SUB, -10, 10, -20, false, true, false)]
        [TestCase(ALUOperationType.SUB, int.MaxValue, int.MinValue, -1, false, true, true)]
        [TestCase(ALUOperationType.UNSIGNED_MUL, 1, 0, 0, true, false, false)]
        [TestCase(ALUOperationType.UNSIGNED_MUL, int.MaxValue, int.MaxValue, 1, false, false, true)]
        [TestCase(ALUOperationType.UNSIGNED_MUL, int.MinValue, int.MaxValue, -2147483648, false, true, true)]
        [TestCase(ALUOperationType.UNSIGNED_MUL, 56, -33, -1848, false, true, true)]
        [TestCase(ALUOperationType.UNSIGNED_DIV, 1, 0, 0, false, false, false)]
        [TestCase(ALUOperationType.UNSIGNED_DIV, 10, 2, 5, false, false, false)]
        [TestCase(ALUOperationType.UNSIGNED_DIV, -10, 2, 2147483643, false, false, false)]
        [TestCase(ALUOperationType.UNSIGNED_DIV, -10, -2, 0, true, false, false)]
        [TestCase(ALUOperationType.SIGNED_DIV, 1, 0, 0, false, false, false)]
        [TestCase(ALUOperationType.SIGNED_DIV, 10, 2, 5, false, false, false)]
        [TestCase(ALUOperationType.SIGNED_DIV, -10, 2, -5, false, true, false)]
        [TestCase(ALUOperationType.SIGNED_DIV, 10, -2, -5, false, true, false)]
        [TestCase(ALUOperationType.AND, 0b0110_0110, 0b0000_1111, 0b0000_0110, false, false, false)]
        [TestCase(ALUOperationType.OR, 0b0110_0110, 0b0000_1111, 0b0110_1111, false, false, false)]
        [TestCase(ALUOperationType.XOR, 0b0110_0110, 0b0000_1111, 0b0110_1001, false, false, false)]
        [TestCase(ALUOperationType.SHL, 1, 64, 0, true, false, true)]
        [TestCase(ALUOperationType.SHL, 1, 32, 0, true, false, true)]
        [TestCase(ALUOperationType.SHL, 1, 31, -2147483648, false, true, false)]
        [TestCase(ALUOperationType.SHR, 1, 64, 0, true, false, true)]
        [TestCase(ALUOperationType.SHR, 1, 32, 0, true, false, true)]
        [TestCase(ALUOperationType.SHR, -2147483648, 31, 1, false, false, false)]
        public void TestArithmeticSigned(
            ALUOperationType op_type,
            int left,
            int right,
            int expected,
            bool zero,
            bool negative,
            bool overflow
        )
        {
            var alu = new ALU();
            var left_reg = new Register32(0);
            var right_reg = new Register32(0);
            var ccr = new ConditionCodeRegister();

            left_reg.writeInt32(left);
            right_reg.writeInt32(right);

            ALUOperation32 alu_op = new ALUOperation32(
                op_type,
                left_reg,
                right_reg,
                ccr
            );

            saturate(alu.execute(alu_op));

            Assert.That(left_reg.readInt32(), Is.EqualTo(expected));

            Assert.That(ccr.readBit(0), Is.EqualTo(zero));
            Assert.That(ccr.readBit(2), Is.EqualTo(negative));
            Assert.That(ccr.readBit(3), Is.EqualTo(overflow));
        }

        [TestCase(ALUOperationType.ADD, uint.MaxValue, 1u, uint.MinValue, true, true)]
        [TestCase(ALUOperationType.ADD, 1u, 1u, 2u, false, false)]
        [TestCase(ALUOperationType.ADD, uint.MaxValue, 64324u, 64323u, false, true)]
        [TestCase(ALUOperationType.ADD, 45u, 78u, 123u, false, false)]
        [TestCase(ALUOperationType.ADD, 78u, 45u, 123u, false, false)]
        [TestCase(ALUOperationType.SUB, uint.MaxValue, 1u, uint.MaxValue - 1u, false, false)]
        [TestCase(ALUOperationType.SUB, 45u, 78u, 4294967263u, false, true)]
        [TestCase(ALUOperationType.SUB, 78u, 45u, 33u, false, false)]
        [TestCase(ALUOperationType.UNSIGNED_MUL, 1u, 0u, 0u, true, false)]
        [TestCase(ALUOperationType.UNSIGNED_MUL, uint.MaxValue, uint.MaxValue, 1u, false, true)]
        [TestCase(ALUOperationType.UNSIGNED_MUL, uint.MinValue, uint.MaxValue, 0u, true, false)]
        [TestCase(ALUOperationType.UNSIGNED_MUL, 56u, 33u, 1848u, false, false)]
        [TestCase(ALUOperationType.SIGNED_MUL, 1u, 0u, 0u, true, false)]
        [TestCase(ALUOperationType.SIGNED_MUL, uint.MaxValue, uint.MaxValue, 1u, false, true)]
        [TestCase(ALUOperationType.SIGNED_MUL, uint.MinValue, uint.MaxValue, 0u, true, false)]
        [TestCase(ALUOperationType.SIGNED_MUL, 56u, 33u, 1848u, false, false)]
        [TestCase(ALUOperationType.UNSIGNED_DIV, 1u, 0u, 0u, false, false)]
        [TestCase(ALUOperationType.UNSIGNED_DIV, 10u, 2u, 5u, false, false)]
        [TestCase(ALUOperationType.UNSIGNED_DIV, 2u, 10u, 0u, true, false)]
        [TestCase(ALUOperationType.AND, 0b0110_0110u, 0b0000_1111u, 0b0000_0110u, false, false)]
        [TestCase(ALUOperationType.OR, 0b0110_0110u, 0b0000_1111u, 0b0110_1111u, false, false)]
        [TestCase(ALUOperationType.XOR, 0b0110_0110u, 0b0000_1111u, 0b0110_1001u, false, false)]
        public void TestArithmeticUnsigned(
            ALUOperationType op_type,
            uint left,
            uint right,
            uint expected,
            bool zero,
            bool carry
        )
        {
            var alu = new ALU();
            var left_reg = new Register32(0);
            var right_reg = new Register32(0);
            var ccr = new ConditionCodeRegister();

            left_reg.writeUInt32(left);
            right_reg.writeUInt32(right);

            ALUOperation32 alu_op = new ALUOperation32(
                op_type,
                left_reg,
                right_reg,
                ccr
            );

            saturate(alu.execute(alu_op));

            Assert.That(left_reg.readUInt32(), Is.EqualTo(expected));

            Assert.That(ccr.readBit(0), Is.EqualTo(zero));
            Assert.That(ccr.readBit(1), Is.EqualTo(carry));
        }

        static void saturate(IEnumerable enumerable)
        {
            foreach (var _ in enumerable)
            { }
        }
    }
}
