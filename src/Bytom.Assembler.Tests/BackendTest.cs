using Bytom.Assembler.Instructions;
using Bytom.Assembler.Operands;

namespace Bytom.Assembler.Tests
{

    public class BackendTest
    {
        [Test]
        public void TestMovRegReg()
        {
            Backend backend = new Backend();
            var code = backend.compile(
                [
                    new MovRegReg(
                        new Register(RegisterName.RD0),
                        new Register(RegisterName.RD1)
                    )
                ]
            );
            var machine_code = code.ToMachineCode();
            Assert.That(machine_code.Count, Is.EqualTo(4));

            var instruction = Serialization.Uint32FromBytesBigEndian(machine_code.ToArray());
            var op_code = instruction & Serialization.Mask(16);
            var second_operand_type = (instruction >> 12) & Serialization.Mask(2);
            var first_operand_type = (instruction >> 14) & Serialization.Mask(2);
            var second_register = (instruction >> 16) & Serialization.Mask(6);
            var first_register = (instruction >> (16 + 6)) & Serialization.Mask(6);

            Assert.That(op_code, Is.EqualTo(MovRegReg.code));

            Assert.That(first_operand_type, Is.EqualTo((uint)OperandType.REGISTER));
            Assert.That(first_register, Is.EqualTo((uint)RegisterName.RD0));

            Assert.That(second_operand_type, Is.EqualTo((uint)OperandType.REGISTER));
            Assert.That(second_register, Is.EqualTo((uint)RegisterName.RD1));
        }

        [Test]
        public void TestMovRegMem()
        {
            Backend backend = new Backend();
            var code = backend.compile(
                [
                    new MovRegMem(
                        new Register(RegisterName.RD0),
                        new MemoryAddress(RegisterName.RD1)
                    )
                ]
            );
            var machine_code = code.ToMachineCode();
            Assert.That(machine_code.Count, Is.EqualTo(4));

            var instruction = Serialization.Uint32FromBytesBigEndian(machine_code.ToArray());
            var op_code = instruction & Serialization.Mask(16);
            var second_operand_type = (instruction >> 12) & Serialization.Mask(2);
            var first_operand_type = (instruction >> 14) & Serialization.Mask(2);
            var second_register = (instruction >> 16) & Serialization.Mask(6);
            var first_register = (instruction >> (16 + 6)) & Serialization.Mask(6);

            Assert.That(op_code, Is.EqualTo(MovRegMem.code));

            Assert.That(first_operand_type, Is.EqualTo((uint)OperandType.REGISTER));
            Assert.That(first_register, Is.EqualTo((uint)RegisterName.RD0));

            Assert.That(second_operand_type, Is.EqualTo((uint)OperandType.MEMORY_ADDRESS));
            Assert.That(second_register, Is.EqualTo((uint)RegisterName.RD1));
        }

        [Test]
        public void TestMovMemReg()
        {
            Backend backend = new Backend();
            var code = backend.compile(
                [
                    new MovMemReg(
                        new MemoryAddress(RegisterName.RD0),
                        new Register(RegisterName.RD1)
                    )
                ]
            );
            var machine_code = code.ToMachineCode();
            Assert.That(machine_code.Count, Is.EqualTo(4));

            var instruction = Serialization.Uint32FromBytesBigEndian(machine_code.ToArray());
            var op_code = instruction & Serialization.Mask(16);
            var second_operand_type = (instruction >> 12) & Serialization.Mask(2);
            var first_operand_type = (instruction >> 14) & Serialization.Mask(2);
            var second_register = (instruction >> 16) & Serialization.Mask(6);
            var first_register = (instruction >> (16 + 6)) & Serialization.Mask(6);

            Assert.That(op_code, Is.EqualTo(MovMemReg.code));

            Assert.That(first_operand_type, Is.EqualTo((uint)OperandType.MEMORY_ADDRESS));
            Assert.That(first_register, Is.EqualTo((uint)RegisterName.RD0));

            Assert.That(second_operand_type, Is.EqualTo((uint)OperandType.REGISTER));
            Assert.That(second_register, Is.EqualTo((uint)RegisterName.RD1));
        }

        [Test]
        public void TestMovRegCon()
        {
            Backend backend = new Backend();
            var code = backend.compile(
                [
                    new MovRegCon(
                        new Register(RegisterName.RD0),
                        new ConstantInt(0xFF)
                    )
                ]
            );
            var machine_code = code.ToMachineCode();
            Assert.That(machine_code.Count, Is.EqualTo(8));

            var instruction = Serialization.Uint32FromBytesBigEndian(
                machine_code.GetRange(0, 4).ToArray()
            );
            var op_code = instruction & Serialization.Mask(16);
            var second_operand_type = (instruction >> 12) & Serialization.Mask(2);
            var first_operand_type = (instruction >> 14) & Serialization.Mask(2);
            var second_register = (instruction >> 16) & Serialization.Mask(6);
            var first_register = (instruction >> (16 + 6)) & Serialization.Mask(6);

            var constant = Serialization.Int32FromBytesBigEndian(
                machine_code.GetRange(4, 4).ToArray()
            );

            Assert.That(op_code, Is.EqualTo(MovRegCon.code));

            Assert.That(first_operand_type, Is.EqualTo((uint)OperandType.REGISTER));
            Assert.That(first_register, Is.EqualTo((uint)RegisterName.RD0));

            Assert.That(second_operand_type, Is.EqualTo((uint)OperandType.CONSTANT));
            Assert.That(second_register, Is.EqualTo(0));

            Assert.That(constant, Is.EqualTo(0xFF));
        }

        [Test]
        public void TestMovMemConI32()
        {
            Backend backend = new Backend();
            var code = backend.compile(
                [
                    new MovMemCon(
                        new MemoryAddress(RegisterName.RD0),
                        new ConstantInt(0xFF)
                    )
                ]
            );
            var machine_code = code.ToMachineCode();
            Assert.That(machine_code.Count, Is.EqualTo(8));

            var instruction = Serialization.Uint32FromBytesBigEndian(
                machine_code.GetRange(0, 4).ToArray()
            );
            var op_code = instruction & Serialization.Mask(16);
            var second_operand_type = (instruction >> 12) & Serialization.Mask(2);
            var first_operand_type = (instruction >> 14) & Serialization.Mask(2);
            var second_register = (instruction >> 16) & Serialization.Mask(6);
            var first_register = (instruction >> (16 + 6)) & Serialization.Mask(6);

            var constant = Serialization.Int32FromBytesBigEndian(
                machine_code.GetRange(4, 4).ToArray()
            );

            Assert.That(op_code, Is.EqualTo(MovMemCon.code));

            Assert.That(first_operand_type, Is.EqualTo((uint)OperandType.MEMORY_ADDRESS));
            Assert.That(first_register, Is.EqualTo((uint)RegisterName.RD0));

            Assert.That(second_operand_type, Is.EqualTo((uint)OperandType.CONSTANT));
            Assert.That(second_register, Is.EqualTo(0));

            Assert.That(constant, Is.EqualTo(0xFF));
        }

        [Test]
        public void TestMovMemConF32()
        {
            Backend backend = new Backend();
            var code = backend.compile(
                [
                    new MovMemCon(
                        new MemoryAddress(RegisterName.RD0),
                        new ConstantFloat(0.5f)
                    )
                ]
            );
            var machine_code = code.ToMachineCode();
            Assert.That(machine_code.Count, Is.EqualTo(8));

            var instruction = Serialization.Uint32FromBytesBigEndian(
                machine_code.GetRange(0, 4).ToArray()
            );
            var op_code = instruction & Serialization.Mask(16);
            var constant = Serialization.Float32FromBytesBigEndian(
                machine_code.GetRange(4, 4).ToArray()
            );

            Assert.That(op_code, Is.EqualTo(MovMemCon.code));
            Assert.That(constant, Is.EqualTo(0.5f));
        }
        [Test]
        public void TestPushReg()
        {
            Backend backend = new Backend();
            var code = backend.compile(
                [
                    new PushReg(
                        new Register(RegisterName.RD0)
                    )
                ]
            );
            var machine_code = code.ToMachineCode();
            Assert.That(machine_code.Count, Is.EqualTo(4));

            var instruction = Serialization.Uint32FromBytesBigEndian(machine_code.ToArray());
            var op_code = instruction & Serialization.Mask(16);
            var second_operand_type = (instruction >> 12) & Serialization.Mask(2);
            var first_operand_type = (instruction >> 14) & Serialization.Mask(2);
            var second_register = (instruction >> 16) & Serialization.Mask(6);
            var first_register = (instruction >> (16 + 6)) & Serialization.Mask(6);

            Assert.That(op_code, Is.EqualTo(PushReg.code));

            Assert.That(first_operand_type, Is.EqualTo((uint)OperandType.REGISTER));
            Assert.That(first_register, Is.EqualTo((uint)RegisterName.RD0));

            Assert.That(second_operand_type, Is.EqualTo(0));
            Assert.That(second_register, Is.EqualTo(0));
        }

        [Test]
        public void TestPushMem()
        {
            Backend backend = new Backend();
            var code = backend.compile(
                [
                    new PushMem(
                        new MemoryAddress(RegisterName.RD0)
                    )
                ]
            );
            var machine_code = code.ToMachineCode();
            Assert.That(machine_code.Count, Is.EqualTo(4));

            var instruction = Serialization.Uint32FromBytesBigEndian(machine_code.ToArray());
            var op_code = instruction & Serialization.Mask(16);
            var second_operand_type = (instruction >> 12) & Serialization.Mask(2);
            var first_operand_type = (instruction >> 14) & Serialization.Mask(2);
            var second_register = (instruction >> 16) & Serialization.Mask(6);
            var first_register = (instruction >> (16 + 6)) & Serialization.Mask(6);

            Assert.That(op_code, Is.EqualTo(PushMem.code));

            Assert.That(first_operand_type, Is.EqualTo((uint)OperandType.MEMORY_ADDRESS));
            Assert.That(first_register, Is.EqualTo((uint)RegisterName.RD0));

            Assert.That(second_operand_type, Is.EqualTo(0));
            Assert.That(second_register, Is.EqualTo(0));
        }

        [Test]
        public void TestPushCon()
        {
            Backend backend = new Backend();
            var code = backend.compile(
                [
                    new PushCon(
                        new ConstantInt(0xFF)
                    )
                ]
            );
            var machine_code = code.ToMachineCode();
            Assert.That(machine_code.Count, Is.EqualTo(8));

            var instruction = Serialization.Uint32FromBytesBigEndian(
                machine_code.GetRange(0, 4).ToArray()
            );
            var op_code = instruction & Serialization.Mask(16);
            var second_operand_type = (instruction >> 12) & Serialization.Mask(2);
            var first_operand_type = (instruction >> 14) & Serialization.Mask(2);
            var second_register = (instruction >> 16) & Serialization.Mask(6);
            var first_register = (instruction >> (16 + 6)) & Serialization.Mask(6);

            var constant = Serialization.Int32FromBytesBigEndian(
                machine_code.GetRange(4, 4).ToArray()
            );

            Assert.That(op_code, Is.EqualTo(PushCon.code));

            Assert.That(first_operand_type, Is.EqualTo((uint)OperandType.CONSTANT));
            Assert.That(first_register, Is.EqualTo(0));

            Assert.That(second_operand_type, Is.EqualTo(0));
            Assert.That(second_register, Is.EqualTo(0));

            Assert.That(constant, Is.EqualTo(0xFF));
        }

        [Test]
        public void TestPopReg()
        {
            Backend backend = new Backend();
            var code = backend.compile(
                [
                    new PopReg(
                        new Register(RegisterName.RD0)
                    )
                ]
            );
            var machine_code = code.ToMachineCode();
            Assert.That(machine_code.Count, Is.EqualTo(4));

            var instruction = Serialization.Uint32FromBytesBigEndian(machine_code.ToArray());
            var op_code = instruction & Serialization.Mask(16);
            var second_operand_type = (instruction >> 12) & Serialization.Mask(2);
            var first_operand_type = (instruction >> 14) & Serialization.Mask(2);
            var second_register = (instruction >> 16) & Serialization.Mask(6);
            var first_register = (instruction >> (16 + 6)) & Serialization.Mask(6);

            Assert.That(op_code, Is.EqualTo(PopReg.code));

            Assert.That(first_operand_type, Is.EqualTo((uint)OperandType.REGISTER));
            Assert.That(first_register, Is.EqualTo((uint)RegisterName.RD0));

            Assert.That(second_operand_type, Is.EqualTo(0));
            Assert.That(second_register, Is.EqualTo(0));
        }

        [Test]
        public void TestSwap()
        {
            Backend backend = new Backend();
            var code = backend.compile(
                [
                    new Swap(
                        new Register(RegisterName.RD0),
                        new Register(RegisterName.RD1)
                    )
                ]
            );
            var machine_code = code.ToMachineCode();
            Assert.That(machine_code.Count, Is.EqualTo(4));

            var instruction = Serialization.Uint32FromBytesBigEndian(machine_code.ToArray());
            var op_code = instruction & Serialization.Mask(16);
            var second_operand_type = (instruction >> 12) & Serialization.Mask(2);
            var first_operand_type = (instruction >> 14) & Serialization.Mask(2);
            var second_register = (instruction >> 16) & Serialization.Mask(6);
            var first_register = (instruction >> (16 + 6)) & Serialization.Mask(6);

            Assert.That(op_code, Is.EqualTo(Swap.code));

            Assert.That(first_operand_type, Is.EqualTo((uint)OperandType.REGISTER));
            Assert.That(first_register, Is.EqualTo((uint)RegisterName.RD0));

            Assert.That(second_operand_type, Is.EqualTo((uint)OperandType.REGISTER));
            Assert.That(second_register, Is.EqualTo((uint)RegisterName.RD1));
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
push RDE
push RDF
mov RDF, IP
mov RDE, 24
add RDF, RDE
pop RDE
jmp [RDF]
halt
"
            ));
        }
    }
}