using System;
using System.Collections.Concurrent;
using System.Threading;


namespace Bytom.Hardware
{

    public class MessageReceiver
    {
        public AddressRange? address_range { get; set; }
        protected Clock clock { get; }
        protected ConcurrentQueue<WriteMessage> write_queue { get; }
        protected ConcurrentQueue<ReadMessage> read_queue { get; }
        protected MemoryController? memory_controller;
        protected PowerStatus power_status = PowerStatus.OFF;
        protected bool requested_power_off = false;
        protected Thread? thread = null;

        public MessageReceiver(Clock clock)
        {
            this.clock = clock;
            write_queue = new ConcurrentQueue<WriteMessage>();
            read_queue = new ConcurrentQueue<ReadMessage>();
        }

        public void pushRead(ReadMessage message)
        {
            read_queue.Enqueue(message);
        }

        public void pushWrite(WriteMessage message)
        {
            write_queue.Enqueue(message);
        }

        public virtual void powerOn(MemoryController controller)
        {
            if (power_status == PowerStatus.ON)
            {
                throw new InvalidOperationException("Memory is already powered on");
            }
            power_status = PowerStatus.STARTING;
            memory_controller = controller;
            powerOnInit();
            power_status = PowerStatus.ON;
        }

        public virtual void powerOnInit()
        {
            beforeThreadStart();
            thread = new Thread(messageReceiverThread);
            thread.Start();
        }

        public virtual void beforeThreadStart()
        {
        }

        public virtual void messageReceiverThread()
        {
            while (!requested_power_off)
            {
                receiveTick();
            }
            requested_power_off = false;
            power_status = PowerStatus.OFF;
        }

        public virtual void receiveTick()
        {
            clock.waitForCycles(1);
            if (write_queue.TryDequeue(out var write_message))
            {
                write(write_message);
            }
            if (read_queue.TryDequeue(out var read_message))
            {
                read(read_message);
            }
        }

        public virtual void write(WriteMessage message)
        {
            throw new NotImplementedException();
        }

        public virtual void read(ReadMessage message)
        {
            throw new NotImplementedException();
        }

        public virtual void powerOff()
        {
            if (power_status == PowerStatus.OFF)
            {
                throw new InvalidOperationException("Memory is already powered off");
            }
            power_status = PowerStatus.STOPPING;
            // Wait for message buffers to be empty.
            while (write_queue.Count > 0 || read_queue.Count > 0)
            { }
            powerOffTeardown();
            powerOffCheck();
        }

        public virtual void powerOffTeardown()
        {
            requested_power_off = true;
            thread?.Join();
        }
        public virtual void powerOffCheck()
        {
            if (power_status != PowerStatus.OFF)
            {
                throw new InvalidOperationException("Memory failed to power off");
            }
        }

        public PowerStatus getPowerStatus()
        {
            return power_status;
        }

        public bool isInMyAddressRange(Address address)
        {
            return address_range?.contains(address) ?? false;
        }
    }
}