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
                [
                    new Nop()
                ]
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
                [
                    new Halt()
                ]
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
                [
                    new MovRegReg(
                        new Register(RegisterID.RD0),
                        new Register(RegisterID.RD1)
                    )
                ]
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
                [
                    new MovRegMem(
                        new Register(RegisterID.RD0),
                        new MemoryAddress(RegisterID.RD1)
                    )
                ]
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
                [
                    new MovMemReg(
                        new MemoryAddress(RegisterID.RD0),
                        new Register(RegisterID.RD1)
                    )
                ]
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
                [
                    new MovRegCon(
                        new Register(RegisterID.RD0),
                        new ConstantInt(constant_value)
                    )
                ]
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
                [
                    new MovMemCon(
                        new MemoryAddress(RegisterID.RD0),
                        new ConstantInt(constant_value)
                    )
                ]
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
                [
                    new MovMemCon(
                        new MemoryAddress(RegisterID.RD0),
                        new ConstantFloat(constant_value)
                    )
                ]
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
                [
                    new PushReg(
                        new Register(RegisterID.RD0)
                    )
                ]
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
                [
                    new PushMem(
                        new MemoryAddress(RegisterID.RD0)
                    )
                ]
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
                [
                    new PopReg(
                        new Register(RegisterID.RD0)
                    )
                ]
            );
            var machine_code = code.ToMachineCode();
            Assert.That(machine_code.Count, Is.EqualTo(4));

            var decoder = new InstructionDecoder(machine_code.ToArray());

            Assert.That(decoder.GetOpCode(), Is.EqualTo(OpCode.PopReg));
            Assert.That(decoder.GetFirstRegisterID(), Is.EqualTo(RegisterID.RD0));
            Assert.That(decoder.GetSecondRegisterID(), Is.EqualTo(RegisterID.NO_REGISTER));
        }

        [Test]
        public void TestSwap()
        {
            Backend backend = new Backend();
            var code = backend.compile(
                [
                    new Swap(
                        new Register(RegisterID.RD0),
                        new Register(RegisterID.RD1)
                    )
                ]
            );
            var machine_code = code.ToMachineCode();
            Assert.That(machine_code.Count, Is.EqualTo(4));

            var decoder = new InstructionDecoder(machine_code.ToArray());

            Assert.That(decoder.GetOpCode(), Is.EqualTo(OpCode.Swap));
            Assert.That(decoder.GetFirstRegisterID(), Is.EqualTo(RegisterID.RD0));
            Assert.That(decoder.GetSecondRegisterID(), Is.EqualTo(RegisterID.RD1));
        }


        [Test]
        public void TestAdd()
        {
            Backend backend = new Backend();
            var code = backend.compile(
                [
                    new Add(
                        new Register(RegisterID.RD0),
                        new Register(RegisterID.RD1)
                    )
                ]
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
                [
                    new Sub(
                        new Register(RegisterID.RD0),
                        new Register(RegisterID.RD1)
                    )
                ]
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
                [
                    new Inc(
                        new Register(RegisterID.RD0)
                    )
                ]
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
                [
                    new Dec(
                        new Register(RegisterID.RD0)
                    )
                ]
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
                [
                    new Mul(
                        new Register(RegisterID.RD0),
                        new Register(RegisterID.RD1)
                    )
                ]
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
                [
                    new IMul(
                        new Register(RegisterID.RD0),
                        new Register(RegisterID.RD1)
                    )
                ]
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
                [
                    new Div(
                        new Register(RegisterID.RD0),
                        new Register(RegisterID.RD1)
                    )
                ]
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
                [
                    new IDiv(
                        new Register(RegisterID.RD0),
                        new Register(RegisterID.RD1)
                    )
                ]
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
                [
                    new And(
                        new Register(RegisterID.RD0),
                        new Register(RegisterID.RD1)
                    )
                ]
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
                [
                    new Or(
                        new Register(RegisterID.RD0),
                        new Register(RegisterID.RD1)
                    )
                ]
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
                [
                    new Xor(
                        new Register(RegisterID.RD0),
                        new Register(RegisterID.RD1)
                    )
                ]
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
                [
                    new Not(
                        new Register(RegisterID.RD0)
                    )
                ]
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
                [
                    new Shl(
                        new Register(RegisterID.RD0),
                        new Register(RegisterID.RD1)
                    )
                ]
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
                [
                    new Shr(
                        new Register(RegisterID.RD0),
                        new Register(RegisterID.RD1)
                    )
                ]
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
                [
                    new Fadd(
                        new Register(RegisterID.RD0),
                        new Register(RegisterID.RD1)
                    )
                ]
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
                [
                    new Fsub(
                        new Register(RegisterID.RD0),
                        new Register(RegisterID.RD1)
                    )
                ]
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
                [
                    new Fmul(
                        new Register(RegisterID.RD0),
                        new Register(RegisterID.RD1)
                    )
                ]
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
                [
                    new Fdiv(
                        new Register(RegisterID.RD0),
                        new Register(RegisterID.RD1)
                    )
                ]
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
                [
                    new Fcmp(
                        new Register(RegisterID.RD0),
                        new Register(RegisterID.RD1)
                    )
                ]
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
                [
                    new JmpMem(
                        new MemoryAddress(RegisterID.RD0)
                    )
                ]
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
                [
                    new JmpCon(
                        new ConstantInt(0xFF)
                    )
                ]
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
                [
                    new LabelNode("end"),
                    new JmpLabel(
                        new Label("end")
                    )
                ]
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
            [
                new JeqMem(
                    new MemoryAddress(RegisterID.RD0)
                )
            ]
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
            [
                new JeqCon(
                    new ConstantInt(0xFF)
                )
            ]
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
            [
                new LabelNode("end"),
                new JeqLabel(
                    new Label("end")
                )
            ]
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
            new List<Node>
            {
                new JneMem(
                    new MemoryAddress(RegisterID.RD0)
                )
            }
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
            new List<Node>
            {
                new JneCon(
                    new ConstantInt(0xFF)
                )
            }
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
            new List<Node>
            {
                new LabelNode("end"),
                new JneLabel(
                    new Label("end")
                )
            }
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

        [Test]
        public void TestJltMem()
        {
            Backend backend = new Backend();
            var code = backend.compile(
            new List<Node>
            {
                new JltMem(
                    new MemoryAddress(RegisterID.RD0)
                )
            }
            );
            var machine_code = code.ToMachineCode();
            Assert.That(machine_code.Count, Is.EqualTo(4));

            var decoder = new InstructionDecoder(machine_code.ToArray());

            Assert.That(decoder.GetOpCode(), Is.EqualTo(OpCode.JltMem));
            Assert.That(decoder.GetFirstRegisterID(), Is.EqualTo(RegisterID.RD0));
            Assert.That(decoder.GetSecondRegisterID(), Is.EqualTo(RegisterID.NO_REGISTER));
        }

        [Test]
        public void TestJltCon()
        {
            Backend backend = new Backend();
            var code = backend.compile(
            new List<Node>
            {
                new JltCon(
                    new ConstantInt(0xFF)
                )
            }
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
        public void TestJltLabel()
        {
            Backend backend = new Backend();
            var code = backend.compile(
            new List<Node>
            {
                new LabelNode("end"),
                new JltLabel(
                    new Label("end")
                )
            }
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

        [Test]
        public void TestJleMem()
        {
            Backend backend = new Backend();
            var code = backend.compile(
            new List<Node>
            {
                new JleMem(
                    new MemoryAddress(RegisterID.RD0)
                )
            }
            );
            var machine_code = code.ToMachineCode();
            Assert.That(machine_code.Count, Is.EqualTo(4));

            var decoder = new InstructionDecoder(machine_code.ToArray());

            Assert.That(decoder.GetOpCode(), Is.EqualTo(OpCode.JleMem));
            Assert.That(decoder.GetFirstRegisterID(), Is.EqualTo(RegisterID.RD0));
            Assert.That(decoder.GetSecondRegisterID(), Is.EqualTo(RegisterID.NO_REGISTER));
        }

        [Test]
        public void TestJleCon()
        {
            Backend backend = new Backend();
            var code = backend.compile(
            new List<Node>
            {
                new JleCon(
                    new ConstantInt(0xFF)
                )
            }
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
        public void TestJleLabel()
        {
            Backend backend = new Backend();
            var code = backend.compile(
            new List<Node>
            {
                new LabelNode("end"),
                new JleLabel(
                    new Label("end")
                )
            }
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

        [Test]
        public void TestJgeMem()
        {
            Backend backend = new Backend();
            var code = backend.compile(
            new List<Node>
            {
                new JgeMem(
                    new MemoryAddress(RegisterID.RD0)
                )
            }
            );
            var machine_code = code.ToMachineCode();
            Assert.That(machine_code.Count, Is.EqualTo(4));

            var decoder = new InstructionDecoder(machine_code.ToArray());

            Assert.That(decoder.GetOpCode(), Is.EqualTo(OpCode.JgeMem));
            Assert.That(decoder.GetFirstRegisterID(), Is.EqualTo(RegisterID.RD0));
            Assert.That(decoder.GetSecondRegisterID(), Is.EqualTo(RegisterID.NO_REGISTER));
        }

        [Test]
        public void TestJgeCon()
        {
            Backend backend = new Backend();
            var code = backend.compile(
            new List<Node>
            {
                new JgeCon(
                    new ConstantInt(0xFF)
                )
            }
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
        public void TestJgeLabel()
        {
            Backend backend = new Backend();
            var code = backend.compile(
            new List<Node>
            {
                new LabelNode("end"),
                new JgeLabel(
                    new Label("end")
                )
            }
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

        [Test]
        public void TestJgtMem()
        {
            Backend backend = new Backend();
            var code = backend.compile(
            new List<Node>
            {
                new JgtMem(
                    new MemoryAddress(RegisterID.RD0)
                )
            }
            );
            var machine_code = code.ToMachineCode();
            Assert.That(machine_code.Count, Is.EqualTo(4));

            var decoder = new InstructionDecoder(machine_code.ToArray());

            Assert.That(decoder.GetOpCode(), Is.EqualTo(OpCode.JgtMem));
            Assert.That(decoder.GetFirstRegisterID(), Is.EqualTo(RegisterID.RD0));
            Assert.That(decoder.GetSecondRegisterID(), Is.EqualTo(RegisterID.NO_REGISTER));
        }

        [Test]
        public void TestJgtCon()
        {
            Backend backend = new Backend();
            var code = backend.compile(
            new List<Node>
            {
                new JgtCon(
                    new ConstantInt(0xFF)
                )
            }
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
        public void TestJgtLabel()
        {
            Backend backend = new Backend();
            var code = backend.compile(
            new List<Node>
            {
                new LabelNode("end"),
                new JgtLabel(
                    new Label("end")
                )
            }
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

        [Test]
        public void TestCallMem()
        {
            Backend backend = new Backend();
            var code = backend.compile(
            new List<Node>
            {
            new CallMem(
                new MemoryAddress(RegisterID.RD0)
            )
            }
            );
            var machine_code = code.ToMachineCode();
            Assert.That(machine_code.Count, Is.EqualTo(4));

            var decoder = new InstructionDecoder(machine_code.ToArray());

            Assert.That(decoder.GetOpCode(), Is.EqualTo(OpCode.CallMem));
            Assert.That(decoder.GetFirstRegisterID(), Is.EqualTo(RegisterID.RD0));
            Assert.That(decoder.GetSecondRegisterID(), Is.EqualTo(RegisterID.NO_REGISTER));
        }

        [Test]
        public void TestCallCon()
        {
            Backend backend = new Backend();
            var code = backend.compile(
            new List<Node>
            {
            new CallCon(
            new ConstantInt(0xFF)
            )
            }
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
        public void TestCallLabel()
        {
            Backend backend = new Backend();
            var code = backend.compile(
            new List<Node>
            {
            new LabelNode("start"),
            new CallLabel(
                new Label("start")
            )
            }
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

        [Test]
        public void TestRet()
        {
            Backend backend = new Backend();
            var code = backend.compile(
            new List<Node>
            {
                new Ret()
            }
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
            new List<Node>
            {
                new Cmp(
                    new Register(RegisterID.RD0),
                    new Register(RegisterID.RD1)
                )
            }
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
    }
}