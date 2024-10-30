
using System.Collections.Concurrent;

namespace Bytom.Hardware.Tests
{
    public class TestMemory : Memory
    {
        public TestMemory(
            uint capacity_bytes,
            uint bandwidth_bytes
        ) : base(
            capacity_bytes: capacity_bytes,
            write_latency_cycles: 1,
            read_latency_cycles: 1,
            bandwidth_bytes: bandwidth_bytes,
            clock: new Clock(1000)
        )
        { }
    }


    public class DebugMemory : Memory
    {
        public DebugMemory(
            uint capacity_bytes,
            uint bandwidth_bytes
        ) : base(
            capacity_bytes: capacity_bytes,
            write_latency_cycles: 1,
            read_latency_cycles: 1,
            bandwidth_bytes: bandwidth_bytes,
            clock: new Clock(1000)
        )
        { }

        public override void powerOnInit()
        { }
    }

    public class GenericMemoryTests<MemT, DebugMemT>
        where MemT : Memory
        where DebugMemT : Memory
    {
        public static MemoryT createMemory<MemoryT>(uint capacity = 128, uint bandwidth_bytes = 1)
        {
            return (MemoryT)Activator.CreateInstance(
                typeof(MemoryT),
                capacity,
                bandwidth_bytes
            )!;
        }

        public static Memory createMemory(uint capacity = 128, uint bandwidth_bytes = 1)
        {
            return new Memory(
                capacity_bytes: capacity,
                write_latency_cycles: 1,
                read_latency_cycles: 1,
                bandwidth_bytes: bandwidth_bytes,
                new Clock(1000)
            );
        }

        [Test]
        public void TestPushIoPowerOff()
        {
            var memory = createMemory<MemT>();
            Assert.Throws<InvalidOperationException>(
                () => memory!.pushIoMessage(new WriteMessage(Address.zero, 1))
            );
        }

        [Test]
        public void TestPowerOn()
        {
            var memory = createMemory<MemT>();
            memory!.powerOn(null);
            Assert.That(memory!.getPowerStatus(), Is.EqualTo(PowerStatus.ON));
        }

        [Test]
        public void TestManualPowerCycle()
        {
            var memory = createMemory<MemT>();
            memory!.powerOn(null);
            Assert.That(memory!.getPowerStatus(), Is.EqualTo(PowerStatus.ON));
            memory!.powerOff();
            Assert.That(memory!.getPowerStatus(), Is.EqualTo(PowerStatus.OFF));
            Assert.That(memory!.thread!.IsAlive, Is.False);
        }

        [Test]
        public void TestDisposePowerOff()
        {
            using (var memory = createMemory<MemT>())
            {
                Assert.That(memory!.getPowerStatus(), Is.EqualTo(PowerStatus.OFF));
            }
        }

        [Test]
        public void TestAutoPowerOff()
        {
            Thread? thread_handle;
            using (var memory = createMemory<MemT>())
            {
                memory!.powerOn(null);
                thread_handle = memory.thread;
                Assert.That(memory!.getPowerStatus(), Is.EqualTo(PowerStatus.ON));
            }
            Assert.That(thread_handle!.IsAlive, Is.False);
        }

        [Test]
        public void TestWriteScheduleIo()
        {
            var capacity = 128u;
            var memory = createMemory<DebugMemT>(capacity);

            memory!.powerOn(null);
            memory!.address_range = new AddressRange(new Address(0), capacity);
            scheduleWriteIo(capacity, memory);

            Assert.That(memory.io_queue.Count, Is.EqualTo(capacity));
        }

        private static void scheduleWriteIo(uint capacity, Memory memory)
        {
            for (var i = 0; i < capacity; i++)
            {
                memory!.pushIoMessage(new WriteMessage(new Address(i), 1));
            }
        }

        [Test]
        public void TestWriteStartRunningIo()
        {
            var capacity = 128u;
            var memory = createMemory<DebugMemT>(capacity);

            memory!.powerOn(null);
            memory!.address_range = new AddressRange(new Address(0), capacity);
            scheduleWriteIo(capacity, memory);

            Assert.That(memory.io_queue.Count, Is.EqualTo(capacity));

            memory!.tick();
            Assert.That(memory.io_queue.Count, Is.EqualTo(capacity - 1));
            Assert.That(memory.tasks_running.Count, Is.EqualTo(1));
        }

        [Test]
        public void TestWriteManualTickAll()
        {
            var capacity = 128u;
            var memory = createMemory<DebugMemT>(capacity);

            memory!.powerOn(null);
            memory!.address_range = new AddressRange(new Address(0), capacity);
            scheduleWriteIo(capacity, memory);

            Assert.That(memory.io_queue.Count, Is.EqualTo(capacity));

            for (var i = 0; i < capacity * 2; i++)
            {
                memory!.tick();
            }
            Assert.That(memory.io_queue.Count, Is.EqualTo(0));
            Assert.That(memory.tasks_running.Count, Is.EqualTo(0));

            for (var i = 0; i < capacity; i++)
            {
                Assert.That(memory.memory[i], Is.EqualTo(1));
            }
        }

        [Test]
        public void TestWriteRunAuto()
        {
            var capacity = 128u;
            var memory = createMemory<DebugMemT>(capacity);

            memory!.powerOn(null);
            memory!.address_range = new AddressRange(new Address(0), capacity);
            scheduleWriteIo(capacity, memory);

            memory!.startWorkerThread();

            Thread.Sleep(2_000);
            Assert.That(memory!.isDone(), Is.True);
            Assert.That(memory.io_queue.Count, Is.EqualTo(0));
            Assert.That(memory.tasks_running.Count, Is.EqualTo(0));

            for (var i = 0; i < capacity; i++)
            {
                Assert.That(memory.memory[i], Is.EqualTo(1));
            }

            memory!.powerOff();
            Assert.That(memory!.getPowerStatus(), Is.EqualTo(PowerStatus.OFF));
            Assert.That(memory!.thread!.IsAlive, Is.False);
        }

        [Test]
        public void TestReadScheduleIo()
        {
            var capacity = 128u;
            var memory = createMemory<DebugMemT>(capacity);

            memory!.powerOn(null);
            memory!.address_range = new AddressRange(new Address(0), capacity);
            var writeBackBuffer = new ConcurrentQueue<WriteMessage>();

            for (var i = 0; i < capacity; i++)
            {
                memory!.pushIoMessage(new ReadMessage(new Address(i), writeBackBuffer));
            }

            Assert.That(memory.io_queue.Count, Is.EqualTo(capacity));
            Assert.That(memory.tasks_running.Count, Is.EqualTo(0));
            Assert.That(writeBackBuffer.Count, Is.EqualTo(0));
        }

        [Test]
        public void TestReadManualIo()
        {
            var capacity = 128u;
            var memory = createMemory<DebugMemT>(capacity);

            memory!.powerOn(null);
            memory!.address_range = new AddressRange(new Address(0), capacity);
            var writeBackBuffer = new ConcurrentQueue<WriteMessage>();

            for (var i = 0; i < capacity; i++)
            {
                memory!.pushIoMessage(new ReadMessage(new Address(i), writeBackBuffer));
            }
            for (var i = 0; i < capacity * 2; i++)
            {
                memory!.tick();
            }

            Assert.That(memory.io_queue.Count, Is.EqualTo(0));
            Assert.That(memory.tasks_running.Count, Is.EqualTo(0));
            Assert.That(writeBackBuffer.Count, Is.EqualTo(capacity));

            for (var i = 0; i < capacity; i++)
            {
                Assert.That(writeBackBuffer.TryDequeue(out var message), Is.True);
                Assert.That(message!.data, Is.EqualTo(0));
            }
        }

        [Test]
        public void TestReadAutoIo()
        {
            var capacity = 128u;
            var memory = createMemory<DebugMemT>(capacity);

            memory!.powerOn(null);
            memory!.address_range = new AddressRange(new Address(0), capacity);
            var writeBackBuffer = new ConcurrentQueue<WriteMessage>();

            for (var i = 0; i < capacity; i++)
            {
                memory!.pushIoMessage(new ReadMessage(new Address(i), writeBackBuffer));
            }

            memory!.startWorkerThread();

            Thread.Sleep(2_000);
            Assert.That(memory!.isDone(), Is.True);

            Assert.That(memory.io_queue.Count, Is.EqualTo(0));
            Assert.That(writeBackBuffer.Count, Is.EqualTo(capacity));

            for (var i = 0; i < capacity; i++)
            {
                Assert.That(writeBackBuffer.TryDequeue(out var message), Is.True);
                Assert.That(message!.data, Is.EqualTo(0));
            }

            memory!.powerOff();
            Assert.That(memory!.getPowerStatus(), Is.EqualTo(PowerStatus.OFF));
            Assert.That(memory!.thread!.IsAlive, Is.False);
        }

        [Test]
        public void TestReadAutoIoNonDebug()
        {
            var capacity = 128u;
            var memory = createMemory<MemT>(capacity);

            memory!.powerOn(null);
            memory!.address_range = new AddressRange(new Address(0), capacity);
            var writeBackBuffer = new ConcurrentQueue<WriteMessage>();

            for (var i = 0; i < capacity; i++)
            {
                memory!.pushIoMessage(new ReadMessage(new Address(i), writeBackBuffer));
            }

            Thread.Sleep(2_000);
            Assert.That(memory!.isDone(), Is.True);

            Assert.That(memory.io_queue.Count, Is.EqualTo(0));
            Assert.That(writeBackBuffer.Count, Is.EqualTo(capacity));

            for (var i = 0; i < capacity; i++)
            {
                Assert.That(writeBackBuffer.TryDequeue(out var message), Is.True);
                Assert.That(message!.data, Is.EqualTo(0));
            }

            memory!.powerOff();
            Assert.That(memory!.getPowerStatus(), Is.EqualTo(PowerStatus.OFF));
            Assert.That(memory!.thread!.IsAlive, Is.False);
        }
    }

    public class MemoryTests : GenericMemoryTests<TestMemory, DebugMemory>
    { }
}
