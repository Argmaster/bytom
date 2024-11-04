using Bytom.Assembler.Nodes;
using Bytom.Assembler.Operands;
using Bytom.Tools;

namespace Bytom.Hardware.CPU.Tests
{
    public class InstructionDecodeTests
    {
        Core? core;

        [SetUp]
        public void Setup()
        {
            core = new Core(0, 1000);
        }

        [Test]
        public void TestDecodeNop()
        {
            var machine_code = new Nop().ToMachineCode();
            var instruction = new Register32(Serialization.UInt32FromBytesBigEndian(machine_code));
            var instruction_decode = new InstructionDecode(instruction);

            Itertools.exhaust(instruction_decode.execute(core!));

            Assert.That(core!.pipeline.Count, Is.EqualTo(4));
            checkUpdateInstructionPointer();
            checkContinueExecution();
        }

        private void checkUpdateInstructionPointer(int offset = 0)
        {
            Assert.That(core!.pipeline[offset], Is.TypeOf<WriteRegister>());
            var write_register = (WriteRegister)core.pipeline[offset];
            Assert.That(write_register.destination.name, Is.EqualTo(core.vInstructionSize.name));
            Assert.That(write_register.source, Is.EqualTo(Serialization.UInt32ToBytesBigEndian(4)));

            Assert.That(core.pipeline[offset + 1], Is.TypeOf<ALUOperation32>());
            var alu_operation = (ALUOperation32)core.pipeline[offset + 1];
            Assert.That(alu_operation.operation, Is.EqualTo(ALUOperationType.ADD));
            Assert.That(alu_operation.left.name, Is.EqualTo(core.IP.name));
            Assert.That(alu_operation.right.name, Is.EqualTo(core.vInstructionSize.name));
        }

        private void checkContinueExecution()
        {
            var read_memory_index = core!.pipeline.Count - 2;
            Assert.That(core.pipeline[read_memory_index], Is.TypeOf<ReadMemory>());
            var read_memory = (ReadMemory)core.pipeline[read_memory_index];
            Assert.That(read_memory.storage.name, Is.EqualTo(core.vInstruction.name));
            Assert.That(read_memory.address.name, Is.EqualTo(core.IP.name));


            var instruction_decode_index = core.pipeline.Count - 1;
            Assert.That(core.pipeline[instruction_decode_index], Is.TypeOf<InstructionDecode>());
            var instruction_decode_next = (InstructionDecode)core.pipeline[instruction_decode_index];
            Assert.That(instruction_decode_next.source.name, Is.EqualTo(core.vInstruction.name));
        }

        [Test]
        public void TestDecodeMovRegReg()
        {
            var machine_code = new MovRegReg(
                new OpRegister(RegisterID.RD0),
                new OpRegister(RegisterID.RD1)
            ).ToMachineCode();
            var instruction = new Register32(Serialization.UInt32FromBytesBigEndian(machine_code));
            var instruction_decode = new InstructionDecode(instruction);

            Itertools.exhaust(instruction_decode.execute(core!));

            Assert.That(core!.pipeline.Count, Is.EqualTo(5));
            checkUpdateInstructionPointer();

            Assert.That(core.pipeline[2], Is.TypeOf<CopyRegister>());
            var copy_register = (CopyRegister)core.pipeline[2];
            Assert.That(copy_register.destination.name, Is.EqualTo(core.RD0.name));
            Assert.That(copy_register.source.name, Is.EqualTo(core.RD1.name));

            checkContinueExecution();
        }

        [Test]
        public void TestMovRegMem()
        {
            var machine_code = new MovRegMem(
                new OpRegister(RegisterID.RD0),
                new OpMemoryAddress(RegisterID.RD1)
            ).ToMachineCode();
            var instruction = new Register32(Serialization.UInt32FromBytesBigEndian(machine_code));
            var instruction_decode = new InstructionDecode(instruction);

            Itertools.exhaust(instruction_decode.execute(core!));

            Assert.That(core!.pipeline.Count, Is.EqualTo(5));
            checkUpdateInstructionPointer();

            Assert.That(core.pipeline[2], Is.TypeOf<ReadMemory>());
            var read_memory = (ReadMemory)core.pipeline[2];
            Assert.That(read_memory.storage, Is.EqualTo(core.RD0));
            Assert.That(read_memory.address, Is.EqualTo(core.RD1));

            checkContinueExecution();
        }

        [Test]
        public void TestMovMemReg()
        {
            var machine_code = new MovMemReg(
                new OpMemoryAddress(RegisterID.RD0),
                new OpRegister(RegisterID.RD1)
            ).ToMachineCode();
            var instruction = new Register32(Serialization.UInt32FromBytesBigEndian(machine_code));
            var instruction_decode = new InstructionDecode(instruction);

            Itertools.exhaust(instruction_decode.execute(core!));

            Assert.That(core!.pipeline.Count, Is.EqualTo(5));
            checkUpdateInstructionPointer();

            Assert.That(core.pipeline[2], Is.TypeOf<WriteMemory>());
            var write_memory = (WriteMemory)core.pipeline[2];
            Assert.That(write_memory.address, Is.EqualTo(core.RD0));
            Assert.That(write_memory.storage, Is.EqualTo(core.RD1));

            checkContinueExecution();
        }

        [Test]
        public void TestMovRegCon()
        {
            var machine_code = new MovRegCon(
                new OpRegister(RegisterID.RD0),
                new OpConstantInt(1)
            ).ToMachineCode();
            var instruction = new Register32(Serialization.UInt32FromBytesBigEndian(
                machine_code.Take(4).ToArray()
            ));
            var instruction_decode = new InstructionDecode(instruction);

            Itertools.exhaust(instruction_decode.execute(core!));

            Assert.That(core!.pipeline.Count, Is.EqualTo(8));
            checkUpdateInstructionPointer();
            checkReadInstructionConstant();

            Assert.That(core.pipeline[5], Is.TypeOf<CopyRegister>());
            var copy_register = (CopyRegister)core.pipeline[5];
            Assert.That(copy_register.destination.name, Is.EqualTo(core.RD0.name));
            Assert.That(copy_register.source.name, Is.EqualTo(core.vRD0.name));

            checkContinueExecution();
        }

        private void checkReadInstructionConstant(int offset = 2)
        {
            Assert.That(core!.pipeline[offset], Is.TypeOf<ReadMemory>());
            var read_memory = (ReadMemory)core.pipeline[offset];
            Assert.That(read_memory.storage.name, Is.EqualTo(core.vRD0.name));
            Assert.That(read_memory.address.name, Is.EqualTo(core.IP.name));

            checkUpdateInstructionPointer(offset + 1);
        }

        [Test]
        public void TestMovMemCon()
        {
            var machine_code = new MovMemCon(
                new OpMemoryAddress(RegisterID.RD0),
                new OpConstantInt(1)
            ).ToMachineCode();
            var instruction = new Register32(Serialization.UInt32FromBytesBigEndian(
                machine_code.Take(4).ToArray()
            ));
            var instruction_decode = new InstructionDecode(instruction);

            Itertools.exhaust(instruction_decode.execute(core!));

            Assert.That(core!.pipeline.Count, Is.EqualTo(8));
            checkUpdateInstructionPointer();
            checkReadInstructionConstant();

            Assert.That(core.pipeline[5], Is.TypeOf<WriteMemory>());
            var write_memory = (WriteMemory)core.pipeline[5];
            Assert.That(write_memory.address.name, Is.EqualTo(core.RD0.name));
            Assert.That(write_memory.storage.name, Is.EqualTo(core.vRD0.name));

            checkContinueExecution();
        }
    }
}