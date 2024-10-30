using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;


namespace Bytom.Hardware
{
    public class MessageReceiver : IDisposable
    {
        public AddressRange? address_range { get; set; }
        public Clock clock { get; }

        // Queue for receiving read/write messages.
        public ConcurrentQueue<IoMessage> io_queue { get; }
        // List of tasks which were taken from the queue and primed to run.
        public List<IEnumerator> tasks_running { get; }
        // Maximum number of tasks that can be running at once enforced during
        // dequeuing of read/write messages.
        public uint max_tasks_running { get; }

        public MemoryController? memory_controller;
        public PowerStatus power_status = PowerStatus.OFF;
        public bool requested_power_off = false;
        public Thread? thread = null;

        public MessageReceiver(uint max_tasks_running, Clock clock)
        {
            this.clock = clock;
            io_queue = new ConcurrentQueue<IoMessage>();
            tasks_running = new List<IEnumerator>();
            this.max_tasks_running = max_tasks_running;
        }

        public void Dispose()
        {
            if (power_status == PowerStatus.ON && !requested_power_off)
            {
                powerOff();
            }
        }

        public void pushIoMessage(IoMessage message)
        {
            if (power_status != PowerStatus.ON)
            {
                throw new InvalidOperationException("Memory is not powered on");
            }
            io_queue.Enqueue(message);
        }

        public bool isDone()
        {
            return io_queue.IsEmpty && tasks_running.Count == 0;
        }

        public virtual void powerOn(MemoryController? controller)
        {
            if (power_status == PowerStatus.ON)
            {
                throw new InvalidOperationException("Memory is already powered on");
            }
            if (power_status == PowerStatus.STARTING)
            {
                return;
            }
            power_status = PowerStatus.STARTING;
            memory_controller = controller;
            powerOnInit();
            power_status = PowerStatus.ON;
        }

        public virtual void powerOnInit()
        {
            beforeThreadStart();
            startWorkerThread();
        }

        public virtual void beforeThreadStart()
        {
        }

        public void startWorkerThread()
        {
            thread = new Thread(messageReceiverThread);
            thread.Start();
        }

        public virtual void messageReceiverThread()
        {
            while (!requested_power_off)
            {
                using (clock.startTick())
                {
                    tick();
                }
            }
            requested_power_off = false;
            power_status = PowerStatus.OFF;
        }

        public virtual void tick()
        {
            runQueuedTasks();
            advanceRunningTasks();
        }

        private void advanceRunningTasks()
        {
            var tasks_to_remove = new List<IEnumerator>();

            foreach (var task in tasks_running)
            {
                if (!task.MoveNext())
                {
                    tasks_to_remove.Add(task);
                }
            }

            foreach (var task in tasks_to_remove)
            {
                tasks_running.Remove(task);
            }
        }

        private void runQueuedTasks()
        {
            while (tasks_running.Count < max_tasks_running)
            {
                if (io_queue.TryDequeue(out var message))
                {
                    if (message is WriteMessage)
                        tasks_running.Add(
                            write((WriteMessage)message).GetEnumerator()
                        );
                    else if (message is ReadMessage)
                        tasks_running.Add(
                            read((ReadMessage)message).GetEnumerator()
                        );
                    else
                        throw new InvalidOperationException("Unknown message type");
                }
                else
                {
                    break;
                }
            }
        }

        public virtual IEnumerable write(WriteMessage message)
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerable read(ReadMessage message)
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
            while (!isDone())
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