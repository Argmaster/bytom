using Bytom.Assembler.Instructions;
using Bytom.Assembler.Operands;

namespace Bytom.Assembler.Tests
{
    public class FrontendTest
    {
        [Test]
        public void TestNOP()
        {
            Frontend frontend = new Frontend();
            var instructions = frontend.parse("NOP");
            Assert.That(1 == instructions.Count);
            Assert.IsInstanceOf<Nop>(instructions[0]);
        }

        [Test]
        public void TestHALT()
        {
            Frontend frontend = new Frontend();
            var instructions = frontend.parse("HALT");
            Assert.That(1 == instructions.Count);
            Assert.IsInstanceOf<Halt>(instructions[0]);
        }

        [TestCase("MOV RD0, RD1", typeof(MovRegReg))]
        [TestCase("MOV RD0, [RD1]", typeof(MovRegMem))]
        [TestCase("MOV [RD0], RD1", typeof(MovMemReg))]
        [TestCase("MOV RD0, 0xFF", typeof(MovRegCon))]
        [TestCase("MOV RD0, 0.44", typeof(MovRegCon))]
        [TestCase("MOV [RD0], 0xFF", typeof(MovMemCon))]
        [TestCase("MOV [RD0], 0.44", typeof(MovMemCon))]
        public void TestMov(
            string instructionString,
            Type expectedInstructionType
        )
        {
            Frontend frontend = new Frontend();
            var instructions = frontend.parse(instructionString);
            Assert.That(1 == instructions.Count);
            Assert.IsInstanceOf(expectedInstructionType, instructions[0]);
        }
    }
}