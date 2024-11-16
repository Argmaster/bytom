using Bytom.Assembler.Nodes;
using Bytom.Assembler.Operands;
using Bytom.Hardware.CPU;
using Bytom.Tools;

namespace Bytom.Assembler.Tests
{

    public class BackendTest
    {
        [Test]
        public void TestNop()
        {
            Backend backend = new Backend();
            var code = backend.compile(
                new AbstractSyntaxTree(
                    [
                        new Nop()
                    ]
                )
            );
            var machine_code = code.ToMachineCode();
            Assert.That(machine_code.Count, Is.EqualTo(4));

            var decoder = new InstructionDecoder(machine_code.ToArray());

            Assert.That(decoder.GetOpCode(), Is.EqualTo(OpCode.Nop));
        }

        [Test]
        public void TestHalt()
        {
            Backend backend = new Backend();
            var code = backend.compile(
                new AbstractSyntaxTree(
                    [
                        new Halt()
                    ]
                )
            );
            var machine_code = code.ToMachineCode();
            Assert.That(machine_code.Count, Is.EqualTo(4));

            var decoder = new InstructionDecoder(machine_code.ToArray());

            Assert.That(decoder.GetOpCode(), Is.EqualTo(OpCode.Halt));
        }

        [Test]
        public void TestMovRegReg()
        {
            Backend backend = new Backend();
            var code = backend.compile(
                new AbstractSyntaxTree(
                    [
                        new Nodes.MovRegReg(
                            new OpRegister(RegisterID.RD0),
                            new OpRegister(RegisterID.RD1)
                        )
                    ]
                )
            );
            var machine_code = code.ToMachineCode();
            Assert.That(machine_code.Count, Is.EqualTo(4));

            var decoder = new InstructionDecoder(machine_code.ToArray());

            Assert.That(decoder.GetOpCode(), Is.EqualTo(OpCode.MovRegReg));
            Assert.That(decoder.GetFirstRegisterID(), Is.EqualTo(RegisterID.RD0));
            Assert.That(decoder.GetSecondRegisterID(), Is.EqualTo(RegisterID.RD1));
        }

        [Test]
        public void TestMovRegMem()
        {
            Backend backend = new Backend();
            var code = backend.compile(
                new AbstractSyntaxTree(
                    [
                        new MovRegMem(
                            new OpRegister(RegisterID.RD0),
                            new OpMemoryAddress(RegisterID.RD1)
                        )
                    ]
                )
            );
            var machine_code = code.ToMachineCode();
            Assert.That(machine_code.Count, Is.EqualTo(4));

            var decoder = new InstructionDecoder(machine_code.ToArray());

            Assert.That(decoder.GetOpCode(), Is.EqualTo(OpCode.MovRegMem));
            Assert.That(decoder.GetFirstRegisterID(), Is.EqualTo(RegisterID.RD0));
            Assert.That(decoder.GetSecondRegisterID(), Is.EqualTo(RegisterID.RD1));
        }

        [Test]
        public void TestMovMemReg()
        {
            Backend backend = new Backend();
            var code = backend.compile(
                new AbstractSyntaxTree(
                    [
                        new MovMemReg(
                            new OpMemoryAddress(RegisterID.RD0),
                            new OpRegister(RegisterID.RD1)
                        )
                    ]
                )
            );
            var machine_code = code.ToMachineCode();
            Assert.That(machine_code.Count, Is.EqualTo(4));
        }

        [Test]
        public void TestMovRegCon()
        {
            Backend backend = new Backend();
            int constant_value = 0xFF;

            var code = backend.compile(
                new AbstractSyntaxTree(
                    [
                        new MovRegCon(
                            new OpRegister(RegisterID.RD0),
                            new OpConstantInt(constant_value)
                        )
                    ]
                )
            );
            var machine_code = code.ToMachineCode();
            Assert.That(machine_code.Count, Is.EqualTo(8));

            var decoder = new InstructionDecoder(
                machine_code.GetRange(0, 4).ToArray()
            );

            Assert.That(decoder.GetOpCode(), Is.EqualTo(OpCode.MovRegCon));
            Assert.That(decoder.GetFirstRegisterID(), Is.EqualTo(RegisterID.RD0));
            Assert.That(decoder.GetSecondRegisterID(), Is.EqualTo(RegisterID.NO_REGISTER));

            var constant = Serialization.Int32FromBytesBigEndian(
                machine_code.GetRange(4, 4).ToArray()
            );
            Assert.That(constant, Is.EqualTo(constant_value));
        }

        [Test]
        public void TestMovMemConI32()
        {
            Backend backend = new Backend();
            int constant_value = 0xFF;
            var code = backend.compile(
                new AbstractSyntaxTree(
                    [
                        new MovMemCon(
                            new OpMemoryAddress(RegisterID.RD0),
                            new OpConstantInt(constant_value)
                        )
                    ]
                )
            );
            var machine_code = code.ToMachineCode();
            Assert.That(machine_code.Count, Is.EqualTo(8));

            var decoder = new InstructionDecoder(
                machine_code.GetRange(0, 4).ToArray()
            );

            Assert.That(decoder.GetOpCode(), Is.EqualTo(OpCode.MovMemCon));
            Assert.That(decoder.GetFirstRegisterID(), Is.EqualTo(RegisterID.RD0));
            Assert.That(decoder.GetSecondRegisterID(), Is.EqualTo(RegisterID.NO_REGISTER));

            var constant = Serialization.Int32FromBytesBigEndian(
                machine_code.GetRange(4, 4).ToArray()
            );
            Assert.That(constant, Is.EqualTo(constant_value));
        }

        [Test]
        public void TestMovMemConF32()
        {
            Backend backend = new Backend();

            var constant_value = 0.5f;
            var code = backend.compile(
                new AbstractSyntaxTree(
                    [
                        new MovMemCon(
                            new OpMemoryAddress(RegisterID.RD0),
                            new OpConstantFloat(constant_value)
                        )
                    ]
                )
            );
            var machine_code = code.ToMachineCode();
            Assert.That(machine_code.Count, Is.EqualTo(8));

            var decoder = new InstructionDecoder(
                machine_code.GetRange(0, 4).ToArray()
            );

            Assert.That(decoder.GetOpCode(), Is.EqualTo(OpCode.MovMemCon));
            Assert.That(decoder.GetFirstRegisterID(), Is.EqualTo(RegisterID.RD0));
            Assert.That(decoder.GetSecondRegisterID(), Is.EqualTo(RegisterID.NO_REGISTER));

            var constant = Serialization.Float32FromBytesBigEndian(
                machine_code.GetRange(4, 4).ToArray()
            );
            Assert.That(constant, Is.EqualTo(constant_value));
        }

        [Test]
        public void TestPushReg()
        {
            Backend backend = new Backend();
            var code = backend.compile(
                new AbstractSyntaxTree(
                    [
                        new PushReg(
                            new OpRegister(RegisterID.RD0)
                        )
                    ]
                )
            );
            var machine_code = code.ToMachineCode();
            Assert.That(machine_code.Count, Is.EqualTo(4));

            var decoder = new InstructionDecoder(machine_code.ToArray());

            Assert.That(decoder.GetOpCode(), Is.EqualTo(OpCode.PushReg));
            Assert.That(decoder.GetFirstRegisterID(), Is.EqualTo(RegisterID.RD0));
            Assert.That(decoder.GetSecondRegisterID(), Is.EqualTo(RegisterID.NO_REGISTER));
        }

        [Test]
        public void TestPushMem()
        {
            Backend backend = new Backend();
            var code = backend.compile(
                new AbstractSyntaxTree(
                    [
                        new PushMem(
                            new OpMemoryAddress(RegisterID.RD0)
                        )
                    ]
                )
            );
            var machine_code = code.ToMachineCode();
            Assert.That(machine_code.Count, Is.EqualTo(4));

            var decoder = new InstructionDecoder(machine_code.ToArray());

            Assert.That(decoder.GetOpCode(), Is.EqualTo(OpCode.PushMem));
            Assert.That(decoder.GetFirstRegisterID(), Is.EqualTo(RegisterID.RD0));
            Assert.That(decoder.GetSecondRegisterID(), Is.EqualTo(RegisterID.NO_REGISTER));
        }

        [Test]
        public void TestPopReg()
        {
            Backend backend = new Backend();
            var code = backend.compile(
                new AbstractSyntaxTree(
                    [
                        new PopReg(
                            new OpRegister(RegisterID.RD0)
                        )
                    ]
                )
            );
            var machine_code = code.ToMachineCode();
            Assert.That(machine_code.Count, Is.EqualTo(4));

            var decoder = new InstructionDecoder(machine_code.ToArray());

            Assert.That(decoder.GetOpCode(), Is.EqualTo(OpCode.PopReg));
            Assert.That(decoder.GetFirstRegisterID(), Is.EqualTo(RegisterID.RD0));
            Assert.That(decoder.GetSecondRegisterID(), Is.EqualTo(RegisterID.NO_REGISTER));
        }

        [Test]
        public void TestAdd()
        {
            Backend backend = new Backend();
            var code = backend.compile(
                new AbstractSyntaxTree(
                    [
                        new Add(
                            new OpRegister(RegisterID.RD0),
                            new OpRegister(RegisterID.RD1)
                        )
                    ]
                )
            );
            var machine_code = code.ToMachineCode();
            Assert.That(machine_code.Count, Is.EqualTo(4));

            var decoder = new InstructionDecoder(machine_code.ToArray());

            Assert.That(decoder.GetOpCode(), Is.EqualTo(OpCode.Add));
            Assert.That(decoder.GetFirstRegisterID(), Is.EqualTo(RegisterID.RD0));
            Assert.That(decoder.GetSecondRegisterID(), Is.EqualTo(RegisterID.RD1));
        }


        [Test]
        public void TestSub()
        {
            Backend backend = new Backend();
            var code = backend.compile(
                new AbstractSyntaxTree(
                    [
                        new Sub(
                            new OpRegister(RegisterID.RD0),
                            new OpRegister(RegisterID.RD1)
                        )
                    ]
                )
            );
            var machine_code = code.ToMachineCode();
            Assert.That(machine_code.Count, Is.EqualTo(4));

            var decoder = new InstructionDecoder(machine_code.ToArray());

            Assert.That(decoder.GetOpCode(), Is.EqualTo(OpCode.Sub));
            Assert.That(decoder.GetFirstRegisterID(), Is.EqualTo(RegisterID.RD0));
            Assert.That(decoder.GetSecondRegisterID(), Is.EqualTo(RegisterID.RD1));
        }


        [Test]
        public void TestInc()
        {
            Backend backend = new Backend();
            var code = backend.compile(
                new AbstractSyntaxTree(
                    [
                        new Inc(
                            new OpRegister(RegisterID.RD0)
                        )
                    ]
                )
            );
            var machine_code = code.ToMachineCode();
            Assert.That(machine_code.Count, Is.EqualTo(4));

            var decoder = new InstructionDecoder(machine_code.ToArray());

            Assert.That(decoder.GetOpCode(), Is.EqualTo(OpCode.Inc));
            Assert.That(decoder.GetFirstRegisterID(), Is.EqualTo(RegisterID.RD0));
            Assert.That(decoder.GetSecondRegisterID(), Is.EqualTo(RegisterID.NO_REGISTER));
        }

        [Test]
        public void TestDec()
        {
            Backend backend = new Backend();
            var code = backend.compile(
                new AbstractSyntaxTree(
                    [
                        new Dec(
                            new OpRegister(RegisterID.RD0)
                        )
                    ]
                )
            );
            var machine_code = code.ToMachineCode();
            Assert.That(machine_code.Count, Is.EqualTo(4));

            var decoder = new InstructionDecoder(machine_code.ToArray());

            Assert.That(decoder.GetOpCode(), Is.EqualTo(OpCode.Dec));
            Assert.That(decoder.GetFirstRegisterID(), Is.EqualTo(RegisterID.RD0));
            Assert.That(decoder.GetSecondRegisterID(), Is.EqualTo(RegisterID.NO_REGISTER));
        }


        [Test]
        public void TestMul()
        {
            Backend backend = new Backend();
            var code = backend.compile(
                new AbstractSyntaxTree(
                    [
                        new Mul(
                            new OpRegister(RegisterID.RD0),
                            new OpRegister(RegisterID.RD1)
                        )
                    ]
                )
            );
            var machine_code = code.ToMachineCode();
            Assert.That(machine_code.Count, Is.EqualTo(4));

            var decoder = new InstructionDecoder(machine_code.ToArray());

            Assert.That(decoder.GetOpCode(), Is.EqualTo(OpCode.Mul));
            Assert.That(decoder.GetFirstRegisterID(), Is.EqualTo(RegisterID.RD0));
            Assert.That(decoder.GetSecondRegisterID(), Is.EqualTo(RegisterID.RD1));
        }

        [Test]
        public void TestIMul()
        {
            Backend backend = new Backend();
            var code = backend.compile(
                new AbstractSyntaxTree(
                    [
                        new IMul(
                            new OpRegister(RegisterID.RD0),
                            new OpRegister(RegisterID.RD1)
                        )
                    ]
                )
            );
            var machine_code = code.ToMachineCode();
            Assert.That(machine_code.Count, Is.EqualTo(4));

            var decoder = new InstructionDecoder(machine_code.ToArray());

            Assert.That(decoder.GetOpCode(), Is.EqualTo(OpCode.IMul));
            Assert.That(decoder.GetFirstRegisterID(), Is.EqualTo(RegisterID.RD0));
            Assert.That(decoder.GetSecondRegisterID(), Is.EqualTo(RegisterID.RD1));
        }


        [Test]
        public void TestDiv()
        {
            Backend backend = new Backend();
            var code = backend.compile(
                new AbstractSyntaxTree(
                    [
                        new Div(
                            new OpRegister(RegisterID.RD0),
                            new OpRegister(RegisterID.RD1)
                        )
                    ]
                )
            );
            var machine_code = code.ToMachineCode();
            Assert.That(machine_code.Count, Is.EqualTo(4));

            var decoder = new InstructionDecoder(machine_code.ToArray());

            Assert.That(decoder.GetOpCode(), Is.EqualTo(OpCode.Div));
            Assert.That(decoder.GetFirstRegisterID(), Is.EqualTo(RegisterID.RD0));
            Assert.That(decoder.GetSecondRegisterID(), Is.EqualTo(RegisterID.RD1));
        }


        [Test]
        public void TestIDiv()
        {
            Backend backend = new Backend();
            var code = backend.compile(
                new AbstractSyntaxTree(
                    [
                        new IDiv(
                            new OpRegister(RegisterID.RD0),
                            new OpRegister(RegisterID.RD1)
                        )
                    ]
                )
            );
            var machine_code = code.ToMachineCode();
            Assert.That(machine_code.Count, Is.EqualTo(4));

            var decoder = new InstructionDecoder(machine_code.ToArray());

            Assert.That(decoder.GetOpCode(), Is.EqualTo(OpCode.IDiv));
            Assert.That(decoder.GetFirstRegisterID(), Is.EqualTo(RegisterID.RD0));
            Assert.That(decoder.GetSecondRegisterID(), Is.EqualTo(RegisterID.RD1));
        }


        [Test]
        public void TestAnd()
        {
            Backend backend = new Backend();
            var code = backend.compile(
                new AbstractSyntaxTree(
                    [
                        new And(
                            new OpRegister(RegisterID.RD0),
                            new OpRegister(RegisterID.RD1)
                        )
                    ]
                )
            );
            var machine_code = code.ToMachineCode();
            Assert.That(machine_code.Count, Is.EqualTo(4));

            var decoder = new InstructionDecoder(machine_code.ToArray());

            Assert.That(decoder.GetOpCode(), Is.EqualTo(OpCode.And));
            Assert.That(decoder.GetFirstRegisterID(), Is.EqualTo(RegisterID.RD0));
            Assert.That(decoder.GetSecondRegisterID(), Is.EqualTo(RegisterID.RD1));
        }


        [Test]
        public void TestOr()
        {
            Backend backend = new Backend();
            var code = backend.compile(
                new AbstractSyntaxTree(
                    [
                        new Or(
                            new OpRegister(RegisterID.RD0),
                            new OpRegister(RegisterID.RD1)
                        )
                    ]
                )
            );
            var machine_code = code.ToMachineCode();
            Assert.That(machine_code.Count, Is.EqualTo(4));

            var decoder = new InstructionDecoder(machine_code.ToArray());

            Assert.That(decoder.GetOpCode(), Is.EqualTo(OpCode.Or));
            Assert.That(decoder.GetFirstRegisterID(), Is.EqualTo(RegisterID.RD0));
            Assert.That(decoder.GetSecondRegisterID(), Is.EqualTo(RegisterID.RD1));
        }


        [Test]
        public void TestXor()
        {
            Backend backend = new Backend();
            var code = backend.compile(
                new AbstractSyntaxTree(
                    [
                        new Xor(
                            new OpRegister(RegisterID.RD0),
                            new OpRegister(RegisterID.RD1)
                        )
                    ]
                )
            );
            var machine_code = code.ToMachineCode();
            Assert.That(machine_code.Count, Is.EqualTo(4));

            var decoder = new InstructionDecoder(machine_code.ToArray());

            Assert.That(decoder.GetOpCode(), Is.EqualTo(OpCode.Xor));
            Assert.That(decoder.GetFirstRegisterID(), Is.EqualTo(RegisterID.RD0));
            Assert.That(decoder.GetSecondRegisterID(), Is.EqualTo(RegisterID.RD1));
        }


        [Test]
        public void TestNot()
        {
            Backend backend = new Backend();
            var code = backend.compile(
                new AbstractSyntaxTree(
                    [
                        new Not(
                            new OpRegister(RegisterID.RD0)
                        )
                    ]
                )
            );
            var machine_code = code.ToMachineCode();
            Assert.That(machine_code.Count, Is.EqualTo(4));

            var decoder = new InstructionDecoder(machine_code.ToArray());

            Assert.That(decoder.GetOpCode(), Is.EqualTo(OpCode.Not));
            Assert.That(decoder.GetFirstRegisterID(), Is.EqualTo(RegisterID.RD0));
            Assert.That(decoder.GetSecondRegisterID(), Is.EqualTo(RegisterID.NO_REGISTER));
        }


        [Test]
        public void TestShl()
        {
            Backend backend = new Backend();
            var code = backend.compile(
                new AbstractSyntaxTree(
                    [
                        new Shl(
                            new OpRegister(RegisterID.RD0),
                            new OpRegister(RegisterID.RD1)
                        )
                    ]
                )
            );
            var machine_code = code.ToMachineCode();
            Assert.That(machine_code.Count, Is.EqualTo(4));

            var decoder = new InstructionDecoder(machine_code.ToArray());

            Assert.That(decoder.GetOpCode(), Is.EqualTo(OpCode.Shl));
            Assert.That(decoder.GetFirstRegisterID(), Is.EqualTo(RegisterID.RD0));
            Assert.That(decoder.GetSecondRegisterID(), Is.EqualTo(RegisterID.RD1));
        }


        [Test]
        public void TestShr()
        {
            Backend backend = new Backend();
            var code = backend.compile(
                new AbstractSyntaxTree(
                    [
                        new Shr(
                            new OpRegister(RegisterID.RD0),
                            new OpRegister(RegisterID.RD1)
                        )
                    ]
                )
            );
            var machine_code = code.ToMachineCode();
            Assert.That(machine_code.Count, Is.EqualTo(4));

            var decoder = new InstructionDecoder(machine_code.ToArray());

            Assert.That(decoder.GetOpCode(), Is.EqualTo(OpCode.Shr));
            Assert.That(decoder.GetFirstRegisterID(), Is.EqualTo(RegisterID.RD0));
            Assert.That(decoder.GetSecondRegisterID(), Is.EqualTo(RegisterID.RD1));
        }


        [Test]
        public void TestFadd()
        {
            Backend backend = new Backend();
            var code = backend.compile(
                new AbstractSyntaxTree(
                    [
                        new Fadd(
                            new OpRegister(RegisterID.RD0),
                            new OpRegister(RegisterID.RD1)
                        )
                    ]
                )
            );
            var machine_code = code.ToMachineCode();
            Assert.That(machine_code.Count, Is.EqualTo(4));

            var decoder = new InstructionDecoder(machine_code.ToArray());

            Assert.That(decoder.GetOpCode(), Is.EqualTo(OpCode.Fadd));
            Assert.That(decoder.GetFirstRegisterID(), Is.EqualTo(RegisterID.RD0));
            Assert.That(decoder.GetSecondRegisterID(), Is.EqualTo(RegisterID.RD1));
        }


        [Test]
        public void TestFsub()
        {
            Backend backend = new Backend();
            var code = backend.compile(
                new AbstractSyntaxTree(
                    [
                        new Fsub(
                            new OpRegister(RegisterID.RD0),
                            new OpRegister(RegisterID.RD1)
                        )
                    ]
                )
            );
            var machine_code = code.ToMachineCode();
            Assert.That(machine_code.Count, Is.EqualTo(4));

            var decoder = new InstructionDecoder(machine_code.ToArray());

            Assert.That(decoder.GetOpCode(), Is.EqualTo(OpCode.Fsub));
            Assert.That(decoder.GetFirstRegisterID(), Is.EqualTo(RegisterID.RD0));
            Assert.That(decoder.GetSecondRegisterID(), Is.EqualTo(RegisterID.RD1));
        }


        [Test]
        public void TestFmul()
        {
            Backend backend = new Backend();
            var code = backend.compile(
                new AbstractSyntaxTree(
                    [
                        new Fmul(
                            new OpRegister(RegisterID.RD0),
                            new OpRegister(RegisterID.RD1)
                        )
                    ]
                )
            );
            var machine_code = code.ToMachineCode();
            Assert.That(machine_code.Count, Is.EqualTo(4));

            var decoder = new InstructionDecoder(machine_code.ToArray());

            Assert.That(decoder.GetOpCode(), Is.EqualTo(OpCode.Fmul));
            Assert.That(decoder.GetFirstRegisterID(), Is.EqualTo(RegisterID.RD0));
            Assert.That(decoder.GetSecondRegisterID(), Is.EqualTo(RegisterID.RD1));
        }


        [Test]
        public void TestFdiv()
        {
            Backend backend = new Backend();
            var code = backend.compile(
                new AbstractSyntaxTree(
                    [
                        new Fdiv(
                            new OpRegister(RegisterID.RD0),
                            new OpRegister(RegisterID.RD1)
                        )
                    ]
                )
            );
            var machine_code = code.ToMachineCode();
            Assert.That(machine_code.Count, Is.EqualTo(4));

            var decoder = new InstructionDecoder(machine_code.ToArray());

            Assert.That(decoder.GetOpCode(), Is.EqualTo(OpCode.Fdiv));
            Assert.That(decoder.GetFirstRegisterID(), Is.EqualTo(RegisterID.RD0));
            Assert.That(decoder.GetSecondRegisterID(), Is.EqualTo(RegisterID.RD1));
        }


        [Test]
        public void TestFcmp()
        {
            Backend backend = new Backend();
            var code = backend.compile(
                new AbstractSyntaxTree(
                    [
                        new Fcmp(
                            new OpRegister(RegisterID.RD0),
                            new OpRegister(RegisterID.RD1)
                        )
                    ]
                )
            );
            var machine_code = code.ToMachineCode();
            Assert.That(machine_code.Count, Is.EqualTo(4));

            var decoder = new InstructionDecoder(machine_code.ToArray());

            Assert.That(decoder.GetOpCode(), Is.EqualTo(OpCode.Fcmp));
            Assert.That(decoder.GetFirstRegisterID(), Is.EqualTo(RegisterID.RD0));
            Assert.That(decoder.GetSecondRegisterID(), Is.EqualTo(RegisterID.RD1));
        }

        [Test]
        public void TestJmpMem()
        {
            Backend backend = new Backend();
            var code = backend.compile(
                new AbstractSyntaxTree(
                    [
                        new JmpMem(
                            new OpMemoryAddress(RegisterID.RD0)
                        )
                    ]
                )
            );
            var machine_code = code.ToMachineCode();
            Assert.That(machine_code.Count, Is.EqualTo(4));

            var decoder = new InstructionDecoder(machine_code.ToArray());

            Assert.That(decoder.GetOpCode(), Is.EqualTo(OpCode.JmpMem));
            Assert.That(decoder.GetFirstRegisterID(), Is.EqualTo(RegisterID.RD0));
        }

        [Test]
        public void TestJmpCon()
        {
            Backend backend = new Backend();
            var code = backend.compile(
                new AbstractSyntaxTree(
                    [
                        new JmpCon(
                            new OpConstantInt(0xFF)
                        )
                    ]
                )
            );
            var machine_code = code.ToMachineCode();
            Assert.That(machine_code.Count, Is.EqualTo(8));

            var decoder = new InstructionDecoder(
                machine_code.GetRange(0, 4).ToArray()
            );

            Assert.That(decoder.GetOpCode(), Is.EqualTo(OpCode.JmpCon));
            Assert.That(decoder.GetFirstRegisterID(), Is.EqualTo(RegisterID.NO_REGISTER));
            Assert.That(decoder.GetSecondRegisterID(), Is.EqualTo(RegisterID.NO_REGISTER));

            var constant = Serialization.Int32FromBytesBigEndian(
                machine_code.GetRange(4, 4).ToArray()
            );
            Assert.That(constant, Is.EqualTo(0xFF));
        }

        [Test]
        public void TestJmpLabel()
        {
            Backend backend = new Backend();
            var code = backend.compile(
                new AbstractSyntaxTree(
                    [
                        new LabelNode("end"),
                        new JmpLabel(
                            new OpLabel("end")
                        )
                    ]
                )
            );
            var machine_code = code.ToMachineCode();
            Assert.That(machine_code.Count, Is.EqualTo(8));

            var decoder = new InstructionDecoder(
                machine_code.GetRange(0, 4).ToArray()
            );

            Assert.That(decoder.GetOpCode(), Is.EqualTo(OpCode.JmpCon));
            Assert.That(decoder.GetFirstRegisterID(), Is.EqualTo(RegisterID.NO_REGISTER));
            Assert.That(decoder.GetSecondRegisterID(), Is.EqualTo(RegisterID.NO_REGISTER));

            var constant = Serialization.Int32FromBytesBigEndian(
                machine_code.GetRange(4, 4).ToArray()
            );
            Assert.That(constant, Is.EqualTo(0));
        }

        [Test]
        public void TestJeqMem()
        {
            Backend backend = new Backend();
            var code = backend.compile(
                new AbstractSyntaxTree(
                    [
                        new JeqMem(
                            new OpMemoryAddress(RegisterID.RD0)
                        )
                    ]
                )
            );
            var machine_code = code.ToMachineCode();
            Assert.That(machine_code.Count, Is.EqualTo(4));

            var decoder = new InstructionDecoder(machine_code.ToArray());

            Assert.That(decoder.GetOpCode(), Is.EqualTo(OpCode.JeqMem));
            Assert.That(decoder.GetFirstRegisterID(), Is.EqualTo(RegisterID.RD0));
            Assert.That(decoder.GetSecondRegisterID(), Is.EqualTo(RegisterID.NO_REGISTER));
        }

        [Test]
        public void TestJeqCon()
        {
            Backend backend = new Backend();
            var code = backend.compile(
                new AbstractSyntaxTree(
                    [
                        new JeqCon(
                            new OpConstantInt(0xFF)
                        )
                    ]
                )
            );
            var machine_code = code.ToMachineCode();
            Assert.That(machine_code.Count, Is.EqualTo(8));

            var decoder = new InstructionDecoder(
            machine_code.GetRange(0, 4).ToArray()
            );

            Assert.That(decoder.GetOpCode(), Is.EqualTo(OpCode.JeqCon));
            Assert.That(decoder.GetFirstRegisterID(), Is.EqualTo(RegisterID.NO_REGISTER));
            Assert.That(decoder.GetSecondRegisterID(), Is.EqualTo(RegisterID.NO_REGISTER));

            var constant = Serialization.Int32FromBytesBigEndian(
            machine_code.GetRange(4, 4).ToArray()
            );
            Assert.That(constant, Is.EqualTo(0xFF));
        }

        [Test]
        public void TestJeqLabel()
        {
            Backend backend = new Backend();
            var code = backend.compile(
                new AbstractSyntaxTree(
                    [
                        new LabelNode("end"),
                        new JeqLabel(
                            new OpLabel("end")
                        )
                    ]
                )
            );
            var machine_code = code.ToMachineCode();
            Assert.That(machine_code.Count, Is.EqualTo(8));

            var decoder = new InstructionDecoder(
            machine_code.GetRange(0, 4).ToArray()
            );

            Assert.That(decoder.GetOpCode(), Is.EqualTo(OpCode.JeqCon));
            Assert.That(decoder.GetFirstRegisterID(), Is.EqualTo(RegisterID.NO_REGISTER));
            Assert.That(decoder.GetSecondRegisterID(), Is.EqualTo(RegisterID.NO_REGISTER));

            var constant = Serialization.Int32FromBytesBigEndian(
            machine_code.GetRange(4, 4).ToArray()
            );
            Assert.That(constant, Is.EqualTo(0));
        }

        [Test]
        public void TestJneMem()
        {
            Backend backend = new Backend();
            var code = backend.compile(
                new AbstractSyntaxTree(
                    [
                        new JneMem(
                            new OpMemoryAddress(RegisterID.RD0)
                        )
                    ]
                )
            );
            var machine_code = code.ToMachineCode();
            Assert.That(machine_code.Count, Is.EqualTo(4));

            var decoder = new InstructionDecoder(machine_code.ToArray());

            Assert.That(decoder.GetOpCode(), Is.EqualTo(OpCode.JneMem));
            Assert.That(decoder.GetFirstRegisterID(), Is.EqualTo(RegisterID.RD0));
            Assert.That(decoder.GetSecondRegisterID(), Is.EqualTo(RegisterID.NO_REGISTER));
        }

        [Test]
        public void TestJneCon()
        {
            Backend backend = new Backend();
            var code = backend.compile(
                new AbstractSyntaxTree(
                    [
                        new JneCon(
                            new OpConstantInt(0xFF)
                        )
                    ]
                )
            );
            var machine_code = code.ToMachineCode();
            Assert.That(machine_code.Count, Is.EqualTo(8));

            var decoder = new InstructionDecoder(
            machine_code.GetRange(0, 4).ToArray()
            );

            Assert.That(decoder.GetOpCode(), Is.EqualTo(OpCode.JneCon));
            Assert.That(decoder.GetFirstRegisterID(), Is.EqualTo(RegisterID.NO_REGISTER));
            Assert.That(decoder.GetSecondRegisterID(), Is.EqualTo(RegisterID.NO_REGISTER));

            var constant = Serialization.Int32FromBytesBigEndian(
            machine_code.GetRange(4, 4).ToArray()
            );
            Assert.That(constant, Is.EqualTo(0xFF));
        }

        [Test]
        public void TestJneLabel()
        {
            Backend backend = new Backend();
            var code = backend.compile(
                new AbstractSyntaxTree(
                    [
                        new LabelNode("end"),
                        new JneLabel(
                            new OpLabel("end")
                        )
                    ]
                )
            );
            var machine_code = code.ToMachineCode();
            Assert.That(machine_code.Count, Is.EqualTo(8));

            var decoder = new InstructionDecoder(
            machine_code.GetRange(0, 4).ToArray()
            );

            Assert.That(decoder.GetOpCode(), Is.EqualTo(OpCode.JneCon));
            Assert.That(decoder.GetFirstRegisterID(), Is.EqualTo(RegisterID.NO_REGISTER));
            Assert.That(decoder.GetSecondRegisterID(), Is.EqualTo(RegisterID.NO_REGISTER));

            var constant = Serialization.Int32FromBytesBigEndian(
            machine_code.GetRange(4, 4).ToArray()
            );
            Assert.That(constant, Is.EqualTo(0));
        }

        public class TestJLT
        {
            [Test]
            public void TestMem()
            {
                Backend backend = new Backend();
                var code = backend.compile(
                new AbstractSyntaxTree(
                    [
                        new Nodes.JLT.Mem(
                        new OpMemoryAddress(RegisterID.RD0)
                    )
                    ]
                )
                );
                var machine_code = code.ToMachineCode();
                Assert.That(machine_code.Count, Is.EqualTo(4));

                var decoder = new InstructionDecoder(machine_code.ToArray());

                Assert.That(decoder.GetOpCode(), Is.EqualTo(OpCode.JltMem));
                Assert.That(decoder.GetFirstRegisterID(), Is.EqualTo(RegisterID.RD0));
                Assert.That(decoder.GetSecondRegisterID(), Is.EqualTo(RegisterID.NO_REGISTER));
            }

            [Test]
            public void TestCon()
            {
                Backend backend = new Backend();
                var code = backend.compile(
                    new AbstractSyntaxTree(
                        [
                            new Nodes.JLT.Con(
                            new OpConstantInt(0xFF)
                        )
                        ]
                    )
                );
                var machine_code = code.ToMachineCode();
                Assert.That(machine_code.Count, Is.EqualTo(8));

                var decoder = new InstructionDecoder(
                machine_code.GetRange(0, 4).ToArray()
                );

                Assert.That(decoder.GetOpCode(), Is.EqualTo(OpCode.JltCon));
                Assert.That(decoder.GetFirstRegisterID(), Is.EqualTo(RegisterID.NO_REGISTER));
                Assert.That(decoder.GetSecondRegisterID(), Is.EqualTo(RegisterID.NO_REGISTER));

                var constant = Serialization.Int32FromBytesBigEndian(
                machine_code.GetRange(4, 4).ToArray()
                );
                Assert.That(constant, Is.EqualTo(0xFF));
            }

            [Test]
            public void TestLabel()
            {
                Backend backend = new Backend();
                var code = backend.compile(
                    new AbstractSyntaxTree(
                        [
                            new LabelNode("end"),
                        new Nodes.JLT.Label(
                            new OpLabel("end")
                        )
                        ]
                    )
                );
                var machine_code = code.ToMachineCode();
                Assert.That(machine_code.Count, Is.EqualTo(8));

                var decoder = new InstructionDecoder(
                machine_code.GetRange(0, 4).ToArray()
                );

                Assert.That(decoder.GetOpCode(), Is.EqualTo(OpCode.JltCon));
                Assert.That(decoder.GetFirstRegisterID(), Is.EqualTo(RegisterID.NO_REGISTER));
                Assert.That(decoder.GetSecondRegisterID(), Is.EqualTo(RegisterID.NO_REGISTER));

                var constant = Serialization.Int32FromBytesBigEndian(
                machine_code.GetRange(4, 4).ToArray()
                );
                Assert.That(constant, Is.EqualTo(0));
            }
        }

        public class TestJLE
        {
            [Test]
            public void TestMem()
            {
                Backend backend = new Backend();
                var code = backend.compile(
                    new AbstractSyntaxTree(
                        [
                            new Nodes.JLE.Mem(
                                new OpMemoryAddress(RegisterID.RD0)
                            )
                        ]
                    )
                );
                var machine_code = code.ToMachineCode();
                Assert.That(machine_code.Count, Is.EqualTo(4));

                var decoder = new InstructionDecoder(machine_code.ToArray());

                Assert.That(decoder.GetOpCode(), Is.EqualTo(OpCode.JleMem));
                Assert.That(decoder.GetFirstRegisterID(), Is.EqualTo(RegisterID.RD0));
                Assert.That(decoder.GetSecondRegisterID(), Is.EqualTo(RegisterID.NO_REGISTER));
            }

            [Test]
            public void TestCon()
            {
                Backend backend = new Backend();
                var code = backend.compile(
                    new AbstractSyntaxTree(
                        [
                            new Nodes.JLE.Con(
                                new OpConstantInt(0xFF)
                            )
                        ]
                    )
                );
                var machine_code = code.ToMachineCode();
                Assert.That(machine_code.Count, Is.EqualTo(8));

                var decoder = new InstructionDecoder(
                machine_code.GetRange(0, 4).ToArray()
                );

                Assert.That(decoder.GetOpCode(), Is.EqualTo(OpCode.JleCon));
                Assert.That(decoder.GetFirstRegisterID(), Is.EqualTo(RegisterID.NO_REGISTER));
                Assert.That(decoder.GetSecondRegisterID(), Is.EqualTo(RegisterID.NO_REGISTER));

                var constant = Serialization.Int32FromBytesBigEndian(
                machine_code.GetRange(4, 4).ToArray()
                );
                Assert.That(constant, Is.EqualTo(0xFF));
            }

            [Test]
            public void TestLabel()
            {
                Backend backend = new Backend();
                var code = backend.compile(
                    new AbstractSyntaxTree(
                        [
                            new LabelNode("end"),
                            new Nodes.JLE.Label(
                                new OpLabel("end")
                            )
                        ]
                    )
                );
                var machine_code = code.ToMachineCode();
                Assert.That(machine_code.Count, Is.EqualTo(8));

                var decoder = new InstructionDecoder(
                machine_code.GetRange(0, 4).ToArray()
                );

                Assert.That(decoder.GetOpCode(), Is.EqualTo(OpCode.JleCon));
                Assert.That(decoder.GetFirstRegisterID(), Is.EqualTo(RegisterID.NO_REGISTER));
                Assert.That(decoder.GetSecondRegisterID(), Is.EqualTo(RegisterID.NO_REGISTER));

                var constant = Serialization.Int32FromBytesBigEndian(
                machine_code.GetRange(4, 4).ToArray()
                );
                Assert.That(constant, Is.EqualTo(0));
            }
        }

        public class TestJGT
        {
            [Test]
            public void TestMem()
            {
                Backend backend = new Backend();
                var code = backend.compile(
                    new AbstractSyntaxTree(
                        [
                            new Nodes.JGT.Mem(
                            new OpMemoryAddress(RegisterID.RD0)
                        )
                        ]
                    )
                );
                var machine_code = code.ToMachineCode();
                Assert.That(machine_code.Count, Is.EqualTo(4));

                var decoder = new InstructionDecoder(machine_code.ToArray());

                Assert.That(decoder.GetOpCode(), Is.EqualTo(OpCode.JgtMem));
                Assert.That(decoder.GetFirstRegisterID(), Is.EqualTo(RegisterID.RD0));
                Assert.That(decoder.GetSecondRegisterID(), Is.EqualTo(RegisterID.NO_REGISTER));
            }

            [Test]
            public void TestCon()
            {
                Backend backend = new Backend();
                var code = backend.compile(
                    new AbstractSyntaxTree(
                        [
                            new Nodes.JGT.Con(
                            new OpConstantInt(0xFF)
                        )
                        ]
                    )
                );
                var machine_code = code.ToMachineCode();
                Assert.That(machine_code.Count, Is.EqualTo(8));

                var decoder = new InstructionDecoder(
                machine_code.GetRange(0, 4).ToArray()
                );

                Assert.That(decoder.GetOpCode(), Is.EqualTo(OpCode.JgtCon));
                Assert.That(decoder.GetFirstRegisterID(), Is.EqualTo(RegisterID.NO_REGISTER));
                Assert.That(decoder.GetSecondRegisterID(), Is.EqualTo(RegisterID.NO_REGISTER));

                var constant = Serialization.Int32FromBytesBigEndian(
                machine_code.GetRange(4, 4).ToArray()
                );
                Assert.That(constant, Is.EqualTo(0xFF));
            }

            [Test]
            public void TestLabel()
            {
                Backend backend = new Backend();
                var code = backend.compile(
                    new AbstractSyntaxTree(
                        [
                            new LabelNode("end"),
                        new Nodes.JGT.Label(
                            new OpLabel("end")
                        )
                        ]
                    )
                );
                var machine_code = code.ToMachineCode();
                Assert.That(machine_code.Count, Is.EqualTo(8));

                var decoder = new InstructionDecoder(
                machine_code.GetRange(0, 4).ToArray()
                );

                Assert.That(decoder.GetOpCode(), Is.EqualTo(OpCode.JgtCon));
                Assert.That(decoder.GetFirstRegisterID(), Is.EqualTo(RegisterID.NO_REGISTER));
                Assert.That(decoder.GetSecondRegisterID(), Is.EqualTo(RegisterID.NO_REGISTER));

                var constant = Serialization.Int32FromBytesBigEndian(
                machine_code.GetRange(4, 4).ToArray()
                );
                Assert.That(constant, Is.EqualTo(0));
            }
        }

        public class TestJGE
        {
            [Test]
            public void TestMem()
            {
                Backend backend = new Backend();
                var code = backend.compile(

                new AbstractSyntaxTree(
                    [
                        new Nodes.JGE.Mem(
                        new OpMemoryAddress(RegisterID.RD0)
                    )
                    ]
                )
                );
                var machine_code = code.ToMachineCode();
                Assert.That(machine_code.Count, Is.EqualTo(4));

                var decoder = new InstructionDecoder(machine_code.ToArray());

                Assert.That(decoder.GetOpCode(), Is.EqualTo(OpCode.JgeMem));
                Assert.That(decoder.GetFirstRegisterID(), Is.EqualTo(RegisterID.RD0));
                Assert.That(decoder.GetSecondRegisterID(), Is.EqualTo(RegisterID.NO_REGISTER));
            }

            [Test]
            public void TestCon()
            {
                Backend backend = new Backend();
                var code = backend.compile(
                    new AbstractSyntaxTree(
                        [
                            new Nodes.JGE.Con(
                                new OpConstantInt(0xFF)
                            )
                        ]
                    )
                );
                var machine_code = code.ToMachineCode();
                Assert.That(machine_code.Count, Is.EqualTo(8));

                var decoder = new InstructionDecoder(
                machine_code.GetRange(0, 4).ToArray()
                );

                Assert.That(decoder.GetOpCode(), Is.EqualTo(OpCode.JgeCon));
                Assert.That(decoder.GetFirstRegisterID(), Is.EqualTo(RegisterID.NO_REGISTER));
                Assert.That(decoder.GetSecondRegisterID(), Is.EqualTo(RegisterID.NO_REGISTER));

                var constant = Serialization.Int32FromBytesBigEndian(
                machine_code.GetRange(4, 4).ToArray()
                );
                Assert.That(constant, Is.EqualTo(0xFF));
            }

            [Test]
            public void TestLabel()
            {
                Backend backend = new Backend();
                var code = backend.compile(
                    new AbstractSyntaxTree(
                        [
                            new LabelNode("end"),
                            new Nodes.JGE.Label(
                                new OpLabel("end")
                            )
                        ]
                    )
                );
                var machine_code = code.ToMachineCode();
                Assert.That(machine_code.Count, Is.EqualTo(8));

                var decoder = new InstructionDecoder(
                machine_code.GetRange(0, 4).ToArray()
                );

                Assert.That(decoder.GetOpCode(), Is.EqualTo(OpCode.JgeCon));
                Assert.That(decoder.GetFirstRegisterID(), Is.EqualTo(RegisterID.NO_REGISTER));
                Assert.That(decoder.GetSecondRegisterID(), Is.EqualTo(RegisterID.NO_REGISTER));

                var constant = Serialization.Int32FromBytesBigEndian(
                machine_code.GetRange(4, 4).ToArray()
                );
                Assert.That(constant, Is.EqualTo(0));
            }
        }


        public class TestCALL
        {
            [Test]
            public void TestMem()
            {
                Backend backend = new Backend();
                var code = backend.compile(
                    new AbstractSyntaxTree(
                        [
                            new Nodes.CALL.Mem(
                                new OpMemoryAddress(RegisterID.RD0)
                            )
                        ]
                    )
                );
                var machine_code = code.ToMachineCode();
                Assert.That(machine_code.Count, Is.EqualTo(4));

                var decoder = new InstructionDecoder(machine_code.ToArray());

                Assert.That(decoder.GetOpCode(), Is.EqualTo(OpCode.CallMem));
                Assert.That(decoder.GetFirstRegisterID(), Is.EqualTo(RegisterID.RD0));
                Assert.That(decoder.GetSecondRegisterID(), Is.EqualTo(RegisterID.NO_REGISTER));
            }

            [Test]
            public void TestCon()
            {
                Backend backend = new Backend();
                var code = backend.compile(
                    new AbstractSyntaxTree(
                        [
                            new Nodes.CALL.Con(
                                new OpConstantInt(0xFF)
                            )
                        ]
                    )
                );
                var machine_code = code.ToMachineCode();
                Assert.That(machine_code.Count, Is.EqualTo(8));

                var decoder = new InstructionDecoder(
                machine_code.GetRange(0, 4).ToArray()
                );

                Assert.That(decoder.GetOpCode(), Is.EqualTo(OpCode.CallCon));
                Assert.That(decoder.GetFirstRegisterID(), Is.EqualTo(RegisterID.NO_REGISTER));
                Assert.That(decoder.GetSecondRegisterID(), Is.EqualTo(RegisterID.NO_REGISTER));

                var constant = Serialization.Int32FromBytesBigEndian(
                machine_code.GetRange(4, 4).ToArray()
                );
                Assert.That(constant, Is.EqualTo(0xFF));
            }

            [Test]
            public void TestLabel()
            {
                Backend backend = new Backend();
                var code = backend.compile(
                    new AbstractSyntaxTree(
                        [
                            new LabelNode("start"),
                            new Nodes.CALL.Label(
                                new OpLabel("start")
                            )
                        ]
                    )
                );
                var machine_code = code.ToMachineCode();
                Assert.That(machine_code.Count, Is.EqualTo(8));

                var decoder = new InstructionDecoder(
                machine_code.GetRange(0, 4).ToArray()
                );

                Assert.That(decoder.GetOpCode(), Is.EqualTo(OpCode.CallCon));
                Assert.That(decoder.GetFirstRegisterID(), Is.EqualTo(RegisterID.NO_REGISTER));
                Assert.That(decoder.GetSecondRegisterID(), Is.EqualTo(RegisterID.NO_REGISTER));

                var constant = Serialization.Int32FromBytesBigEndian(
                machine_code.GetRange(4, 4).ToArray()
                );
                Assert.That(constant, Is.EqualTo(0));
            }
        }


        [Test]
        public void TestRet()
        {
            Backend backend = new Backend();
            var code = backend.compile(
                new AbstractSyntaxTree(
                    [
                        new Ret()
                    ]
                )
            );
            var machine_code = code.ToMachineCode();
            Assert.That(machine_code.Count, Is.EqualTo(4));

            var decoder = new InstructionDecoder(machine_code.ToArray());

            Assert.That(decoder.GetOpCode(), Is.EqualTo(OpCode.Ret));
            Assert.That(decoder.GetFirstRegisterID(), Is.EqualTo(RegisterID.NO_REGISTER));
            Assert.That(decoder.GetSecondRegisterID(), Is.EqualTo(RegisterID.NO_REGISTER));
        }

        [Test]
        public void TestCmp()
        {
            Backend backend = new Backend();
            var code = backend.compile(
                new AbstractSyntaxTree(
                    [
                        new Cmp(
                            new OpRegister(RegisterID.RD0),
                            new OpRegister(RegisterID.RD1)
                        )
                    ]
                )
            );
            var machine_code = code.ToMachineCode();
            Assert.That(machine_code.Count, Is.EqualTo(4));

            var decoder = new InstructionDecoder(machine_code.ToArray());

            Assert.That(decoder.GetOpCode(), Is.EqualTo(OpCode.Cmp));
            Assert.That(decoder.GetFirstRegisterID(), Is.EqualTo(RegisterID.RD0));
            Assert.That(decoder.GetSecondRegisterID(), Is.EqualTo(RegisterID.RD1));
        }

        [Test]
        public void TestLabelJump()
        {
            Frontend frontend = new Frontend();
            var instructions = frontend.parse(@"
            foo:
                mov rd0, 0xFF
                jmp end
            end:
                halt
            ");

            Backend backend = new Backend();
            var code = backend.compile(instructions);

            Assert.That(code.ToAssembly(), Is.EqualTo(@"mov RD0, 255
jmp 16
halt
"
            ));
        }

        [Test]
        public void TestInt()
        {
            var frontend = new Frontend();
            var instructions = frontend.parse(@"int RD0");

            Backend backend = new Backend();
            var code = backend.compile(instructions);

            Assert.That(code.ToAssembly(), Is.EqualTo("int RD0\n"));
        }

        [Test]
        public void TestIRet()
        {
            var frontend = new Frontend();
            var instructions = frontend.parse(@"iret");

            Backend backend = new Backend();
            var code = backend.compile(instructions);

            Assert.That(code.ToAssembly(), Is.EqualTo("iret\n"));
        }

        [Test]
        public void TestCpuId()
        {
            var frontend = new Frontend();
            var instructions = frontend.parse(@"cpuid RD0");

            Backend backend = new Backend();
            var code = backend.compile(instructions);

            Assert.That(code.ToAssembly(), Is.EqualTo("cpuid RD0\n"));
        }
    }
}