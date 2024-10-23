using Bytom.Assembler.Nodes;

namespace Bytom.Assembler.Tests
{
    public class FrontendTest
    {
        [Test]
        public void TestNOP()
        {
            Frontend frontend = new Frontend();
            var instructions = frontend.parse("NOP").nodes;
            Assert.That(instructions.Count, Is.EqualTo(1));
            Assert.IsInstanceOf<Nop>(instructions[0]);
        }

        [Test]
        public void TestHALT()
        {
            Frontend frontend = new Frontend();
            var instructions = frontend.parse("HALT").nodes;
            Assert.That(instructions.Count, Is.EqualTo(1));
            Assert.IsInstanceOf<Halt>(instructions[0]);
        }

        [TestCase("MOV RD0, RD1", typeof(MovRegReg), 32u)]
        [TestCase("MOV RD0, [RD1]", typeof(MovRegMem), 32u)]
        [TestCase("MOV [RD0], RD1", typeof(MovMemReg), 32u)]
        [TestCase("MOV RD0, 0xFF", typeof(MovRegCon), 64u)]
        [TestCase("MOV RD0, 0.44", typeof(MovRegCon), 64u)]
        [TestCase("MOV RD0, 63", typeof(MovRegCon), 64u)]
        [TestCase("MOV [RD0], 0xFF", typeof(MovMemCon), 64u)]
        [TestCase("MOV [RD0], 255", typeof(MovMemCon), 64u)]
        [TestCase("MOV [RD0], 0.44", typeof(MovMemCon), 64u)]
        public void TestMov(
            string instructionString,
            Type expectedInstructionType,
            uint size
        )
        {
            Frontend frontend = new Frontend();
            var instructions = frontend.parse(instructionString).nodes;
            Assert.That(instructions.Count, Is.EqualTo(1));
            Assert.IsInstanceOf(expectedInstructionType, instructions[0]);
            Assert.That(((Instruction)instructions[0]).GetSizeBits(), Is.EqualTo(size));
        }

        [TestCase("PUSH RD0", typeof(PushReg))]
        [TestCase("PUSH [RD0]", typeof(PushMem))]
        [TestCase("PUSH 0xFF", typeof(PushCon))]
        public void TestPush(
            string instructionString,
            Type expectedInstructionType
        )
        {
            Frontend frontend = new Frontend();
            var instructions = frontend.parse(instructionString).nodes;
            Assert.That(instructions.Count, Is.EqualTo(1));
            Assert.IsInstanceOf(expectedInstructionType, instructions[0]);
        }

        [TestCase("POP RD0", typeof(PopReg))]
        [TestCase("POP [RD0]", typeof(PopMem))]
        public void TestPop(
            string instructionString,
            Type expectedInstructionType
        )
        {
            Frontend frontend = new Frontend();
            var instructions = frontend.parse(instructionString).nodes;
            Assert.That(instructions.Count, Is.EqualTo(1));
            Assert.IsInstanceOf(expectedInstructionType, instructions[0]);
        }

        [Test]
        public void TestAdd()
        {
            Frontend frontend = new Frontend();
            var instructions = frontend.parse("ADD RD0, RD1").nodes;
            Assert.That(instructions.Count, Is.EqualTo(1));
            Assert.IsInstanceOf<Add>(instructions[0]);
        }

        [Test]
        public void TestSub()
        {
            Frontend frontend = new Frontend();
            var instructions = frontend.parse("SUB RD0, RD1").nodes;
            Assert.That(instructions.Count, Is.EqualTo(1));
            Assert.IsInstanceOf<Sub>(instructions[0]);
        }

        [Test]
        public void TestInc()
        {
            Frontend frontend = new Frontend();
            var instructions = frontend.parse("INC RD0").nodes;
            Assert.That(instructions.Count, Is.EqualTo(1));
            Assert.IsInstanceOf<Inc>(instructions[0]);
        }

        [Test]
        public void TestDec()
        {
            Frontend frontend = new Frontend();
            var instructions = frontend.parse("DEC RD0").nodes;
            Assert.That(instructions.Count, Is.EqualTo(1));
            Assert.IsInstanceOf<Dec>(instructions[0]);
        }


        [Test]
        public void TestMul()
        {
            Frontend frontend = new Frontend();
            var instructions = frontend.parse("MUL RD0, RD1").nodes;
            Assert.That(instructions.Count, Is.EqualTo(1));
            Assert.IsInstanceOf<Mul>(instructions[0]);
        }

        [Test]
        public void TestIMul()
        {
            Frontend frontend = new Frontend();
            var instructions = frontend.parse("IMUL RD0, RD1").nodes;
            Assert.That(instructions.Count, Is.EqualTo(1));
            Assert.IsInstanceOf<IMul>(instructions[0]);
        }

        [Test]
        public void TestDiv()
        {
            Frontend frontend = new Frontend();
            var instructions = frontend.parse("DIV RD0, RD1").nodes;
            Assert.That(instructions.Count, Is.EqualTo(1));
            Assert.IsInstanceOf<Div>(instructions[0]);
        }

        [Test]
        public void TestIDiv()
        {
            Frontend frontend = new Frontend();
            var instructions = frontend.parse("IDIV RD0, RD1").nodes;
            Assert.That(instructions.Count, Is.EqualTo(1));
            Assert.IsInstanceOf<IDiv>(instructions[0]);
        }

        [Test]
        public void TestAnd()
        {
            Frontend frontend = new Frontend();
            var instructions = frontend.parse("AND RD0, RD1").nodes;
            Assert.That(instructions.Count, Is.EqualTo(1));
            Assert.IsInstanceOf<And>(instructions[0]);
        }

        [Test]
        public void TestOr()
        {
            Frontend frontend = new Frontend();
            var instructions = frontend.parse("OR RD0, RD1").nodes;
            Assert.That(instructions.Count, Is.EqualTo(1));
            Assert.IsInstanceOf<Or>(instructions[0]);
        }

        [Test]
        public void TestXor()
        {
            Frontend frontend = new Frontend();
            var instructions = frontend.parse("XOR RD0, RD1").nodes;
            Assert.That(instructions.Count, Is.EqualTo(1));
            Assert.IsInstanceOf<Xor>(instructions[0]);
        }

        [Test]
        public void TestNot()
        {
            Frontend frontend = new Frontend();
            var instructions = frontend.parse("NOT RD0").nodes;
            Assert.That(instructions.Count, Is.EqualTo(1));
            Assert.IsInstanceOf<Not>(instructions[0]);
        }

        [Test]
        public void TestShl()
        {
            Frontend frontend = new Frontend();
            var instructions = frontend.parse("SHL RD0, RD1").nodes;
            Assert.That(instructions.Count, Is.EqualTo(1));
            Assert.IsInstanceOf<Shl>(instructions[0]);
        }

        [Test]
        public void TestShr()
        {
            Frontend frontend = new Frontend();
            var instructions = frontend.parse("SHR RD0, RD1").nodes;
            Assert.That(instructions.Count, Is.EqualTo(1));
            Assert.IsInstanceOf<Shr>(instructions[0]);
        }

        [Test]
        public void TestFadd()
        {
            Frontend frontend = new Frontend();
            var instructions = frontend.parse("FADD RD0, RD1").nodes;
            Assert.That(instructions.Count, Is.EqualTo(1));
            Assert.IsInstanceOf<Fadd>(instructions[0]);
        }

        [Test]
        public void TestFsub()
        {
            Frontend frontend = new Frontend();
            var instructions = frontend.parse("FSUB RD0, RD1").nodes;
            Assert.That(instructions.Count, Is.EqualTo(1));
            Assert.IsInstanceOf<Fsub>(instructions[0]);
        }

        [Test]
        public void TestFmul()
        {
            Frontend frontend = new Frontend();
            var instructions = frontend.parse("FMUL RD0, RD1").nodes;
            Assert.That(instructions.Count, Is.EqualTo(1));
            Assert.IsInstanceOf<Fmul>(instructions[0]);
        }

        [Test]
        public void TestFdiv()
        {
            Frontend frontend = new Frontend();
            var instructions = frontend.parse("FDIV RD0, RD1").nodes;
            Assert.That(instructions.Count, Is.EqualTo(1));
            Assert.IsInstanceOf<Fdiv>(instructions[0]);
        }

        [Test]
        public void TestFcmp()
        {
            Frontend frontend = new Frontend();
            var instructions = frontend.parse("FCMP RD0, RD1").nodes;
            Assert.That(instructions.Count, Is.EqualTo(1));
            Assert.IsInstanceOf<Fcmp>(instructions[0]);
        }

        [Test]
        public void TestJmpMem()
        {
            Frontend frontend = new Frontend();
            var instructions = frontend.parse("JMP [RD0]").nodes;
            Assert.That(instructions.Count, Is.EqualTo(1));
            Assert.IsInstanceOf<JmpMem>(instructions[0]);
        }

        [Test]
        public void TestJmpLabel()
        {
            Frontend frontend = new Frontend();
            var instructions = frontend.parse("JMP foo").nodes;
            Assert.That(instructions.Count, Is.EqualTo(1));
            Assert.IsInstanceOf<JmpLabel>(instructions[0]);
        }

        [Test]
        public void TestJeqMem()
        {
            Frontend frontend = new Frontend();
            var instructions = frontend.parse("JEQ [RD0]").nodes;
            Assert.That(instructions.Count, Is.EqualTo(1));
            Assert.IsInstanceOf<JeqMem>(instructions[0]);
        }

        [Test]
        public void TestJeqLabel()
        {
            Frontend frontend = new Frontend();
            var instructions = frontend.parse("JEQ foo").nodes;
            Assert.That(instructions.Count, Is.EqualTo(1));
            Assert.IsInstanceOf<JeqLabel>(instructions[0]);
        }

        [Test]
        public void TestJneMem()
        {
            Frontend frontend = new Frontend();
            var instructions = frontend.parse("JNE [RD0]").nodes;
            Assert.That(instructions.Count, Is.EqualTo(1));
            Assert.IsInstanceOf<JneMem>(instructions[0]);
        }

        [Test]
        public void TestJneLabel()
        {
            Frontend frontend = new Frontend();
            var instructions = frontend.parse("JNE foo").nodes;
            Assert.That(instructions.Count, Is.EqualTo(1));
            Assert.IsInstanceOf<JneLabel>(instructions[0]);
        }

        public class TestJB
        {
            [Test]
            public void TestMem()
            {
                Frontend frontend = new Frontend();
                var instructions = frontend.parse("JB [RD0]").nodes;
                Assert.That(instructions.Count, Is.EqualTo(1));
                Assert.IsInstanceOf<Nodes.JB.Mem>(instructions[0]);
            }

            [Test]
            public void TestCon()
            {
                Frontend frontend = new Frontend();
                var instructions = frontend.parse("JB 0x100").nodes;
                Assert.That(instructions.Count, Is.EqualTo(1));
                Assert.IsInstanceOf<Nodes.JB.Con>(instructions[0]);
            }

            [Test]
            public void TestLabel()
            {
                Frontend frontend = new Frontend();
                var instructions = frontend.parse("JB foo").nodes;
                Assert.That(instructions.Count, Is.EqualTo(1));
                Assert.IsInstanceOf<Nodes.JB.Label>(instructions[0]);
            }
        }

        public class TestJBE
        {
            [Test]
            public void TestMem()
            {
                Frontend frontend = new Frontend();
                var instructions = frontend.parse("JBE [RD0]").nodes;
                Assert.That(instructions.Count, Is.EqualTo(1));
                Assert.IsInstanceOf<Nodes.JBE.Mem>(instructions[0]);
            }

            [Test]
            public void TestCon()
            {
                Frontend frontend = new Frontend();
                var instructions = frontend.parse("JBE 0x100").nodes;
                Assert.That(instructions.Count, Is.EqualTo(1));
                Assert.IsInstanceOf<Nodes.JBE.Con>(instructions[0]);
            }

            [Test]
            public void TestLabel()
            {
                Frontend frontend = new Frontend();
                var instructions = frontend.parse("JBE foo").nodes;
                Assert.That(instructions.Count, Is.EqualTo(1));
                Assert.IsInstanceOf<Nodes.JBE.Label>(instructions[0]);
            }
        }

        public class TestJA
        {
            [Test]
            public void TestMem()
            {
                Frontend frontend = new Frontend();
                var instructions = frontend.parse("JA [RD0]").nodes;
                Assert.That(instructions.Count, Is.EqualTo(1));
                Assert.IsInstanceOf<Nodes.JA.Mem>(instructions[0]);
            }

            [Test]
            public void TestCon()
            {
                Frontend frontend = new Frontend();
                var instructions = frontend.parse("JA 0x100").nodes;
                Assert.That(instructions.Count, Is.EqualTo(1));
                Assert.IsInstanceOf<Nodes.JA.Con>(instructions[0]);
            }

            [Test]
            public void TestLabel()
            {
                Frontend frontend = new Frontend();
                var instructions = frontend.parse("JA foo").nodes;
                Assert.That(instructions.Count, Is.EqualTo(1));
                Assert.IsInstanceOf<Nodes.JA.Label>(instructions[0]);
            }
        }

        public class TestJAE
        {
            [Test]
            public void TestMem()
            {
                Frontend frontend = new Frontend();
                var instructions = frontend.parse("JAE [RD0]").nodes;
                Assert.That(instructions.Count, Is.EqualTo(1));
                Assert.IsInstanceOf<Nodes.JAE.Mem>(instructions[0]);
            }

            [Test]
            public void TestCon()
            {
                Frontend frontend = new Frontend();
                var instructions = frontend.parse("JAE 0x100").nodes;
                Assert.That(instructions.Count, Is.EqualTo(1));
                Assert.IsInstanceOf<Nodes.JAE.Con>(instructions[0]);
            }

            [Test]
            public void TestLabel()
            {
                Frontend frontend = new Frontend();
                var instructions = frontend.parse("JAE foo").nodes;
                Assert.That(instructions.Count, Is.EqualTo(1));
                Assert.IsInstanceOf<Nodes.JAE.Label>(instructions[0]);
            }
        }

        public class TestJLT
        {
            [Test]
            public void TestMem()
            {
                Frontend frontend = new Frontend();
                var instructions = frontend.parse("JLT [RD0]").nodes;
                Assert.That(instructions.Count, Is.EqualTo(1));
                Assert.IsInstanceOf<Nodes.JLT.Mem>(instructions[0]);
            }

            [Test]
            public void TestCon()
            {
                Frontend frontend = new Frontend();
                var instructions = frontend.parse("JLT 0x100").nodes;
                Assert.That(instructions.Count, Is.EqualTo(1));
                Assert.IsInstanceOf<Nodes.JLT.Con>(instructions[0]);
            }

            [Test]
            public void TestLabel()
            {
                Frontend frontend = new Frontend();
                var instructions = frontend.parse("JLT foo").nodes;
                Assert.That(instructions.Count, Is.EqualTo(1));
                Assert.IsInstanceOf<Nodes.JLT.Label>(instructions[0]);
            }
        }

        public class TestJLE
        {
            [Test]
            public void TestMem()
            {
                Frontend frontend = new Frontend();
                var instructions = frontend.parse("JLE [RD0]").nodes;
                Assert.That(instructions.Count, Is.EqualTo(1));
                Assert.IsInstanceOf<Nodes.JLE.Mem>(instructions[0]);
            }

            [Test]
            public void TestCon()
            {
                Frontend frontend = new Frontend();
                var instructions = frontend.parse("JLE 0x100").nodes;
                Assert.That(instructions.Count, Is.EqualTo(1));
                Assert.IsInstanceOf<Nodes.JLE.Con>(instructions[0]);
            }

            [Test]
            public void TestLabel()
            {
                Frontend frontend = new Frontend();
                var instructions = frontend.parse("JLE foo").nodes;
                Assert.That(instructions.Count, Is.EqualTo(1));
                Assert.IsInstanceOf<Nodes.JLE.Label>(instructions[0]);
            }

        }

        public class TestJGT
        {
            [Test]
            public void TestMem()
            {
                Frontend frontend = new Frontend();
                var instructions = frontend.parse("JGT [RD0]").nodes;
                Assert.That(instructions.Count, Is.EqualTo(1));
                Assert.IsInstanceOf<Nodes.JGT.Mem>(instructions[0]);
            }

            [Test]
            public void TestCon()
            {
                Frontend frontend = new Frontend();
                var instructions = frontend.parse("JGT 0x100").nodes;
                Assert.That(instructions.Count, Is.EqualTo(1));
                Assert.IsInstanceOf<Nodes.JGT.Con>(instructions[0]);
            }

            [Test]
            public void TestLabel()
            {
                Frontend frontend = new Frontend();
                var instructions = frontend.parse("JGT foo").nodes;
                Assert.That(instructions.Count, Is.EqualTo(1));
                Assert.IsInstanceOf<Nodes.JGT.Label>(instructions[0]);
            }
        }


        public class TestJGE
        {
            [Test]
            public void TestMem()
            {
                Frontend frontend = new Frontend();
                var instructions = frontend.parse("JGE [RD0]").nodes;
                Assert.That(instructions.Count, Is.EqualTo(1));
                Assert.IsInstanceOf<Nodes.JGE.Mem>(instructions[0]);
            }

            [Test]
            public void TestCon()
            {
                Frontend frontend = new Frontend();
                var instructions = frontend.parse("JGE 0x100").nodes;
                Assert.That(instructions.Count, Is.EqualTo(1));
                Assert.IsInstanceOf<Nodes.JGE.Con>(instructions[0]);
            }

            [Test]
            public void TestLabel()
            {
                Frontend frontend = new Frontend();
                var instructions = frontend.parse("JGE foo").nodes;
                Assert.That(instructions.Count, Is.EqualTo(1));
                Assert.IsInstanceOf<Nodes.JGE.Label>(instructions[0]);
            }
        }

        public class TestCALL
        {
            [Test]
            public void TestMem()
            {
                Frontend frontend = new Frontend();
                var instructions = frontend.parse("CALL [RD0]").nodes;
                Assert.That(instructions.Count, Is.EqualTo(1));
                Assert.IsInstanceOf<Nodes.CALL.Mem>(instructions[0]);
            }

            [Test]
            public void TestCon()
            {
                Frontend frontend = new Frontend();
                var instructions = frontend.parse("CALL 0x100").nodes;
                Assert.That(instructions.Count, Is.EqualTo(1));
                Assert.IsInstanceOf<Nodes.CALL.Con>(instructions[0]);
            }

            [Test]
            public void TestLabel()
            {
                Frontend frontend = new Frontend();
                var instructions = frontend.parse("CALL foo").nodes;
                Assert.That(instructions.Count, Is.EqualTo(1));
                Assert.IsInstanceOf<Nodes.CALL.Label>(instructions[0]);
            }
        }

        [Test]
        public void TestRet()
        {
            Frontend frontend = new Frontend();
            var instructions = frontend.parse("RET").nodes;
            Assert.That(instructions.Count, Is.EqualTo(1));
            Assert.IsInstanceOf<Ret>(instructions[0]);
        }

        [Test]
        public void TestCmp()
        {
            Frontend frontend = new Frontend();
            var instructions = frontend.parse("CMP RD0, RD1").nodes;
            Assert.That(instructions.Count, Is.EqualTo(1));
            Assert.IsInstanceOf<Cmp>(instructions[0]);
        }
    }
}