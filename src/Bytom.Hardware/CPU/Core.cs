using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Bytom.Tools;

namespace Bytom.Hardware.CPU
{
    public enum RegisterID
    {
        NO_REGISTER = 0b00_0000,
        RD0 = 0b00_0001,
        RD1 = 0b00_0010,
        RD2 = 0b00_0011,
        RD3 = 0b00_0100,
        RD4 = 0b00_0101,
        RD5 = 0b00_0110,
        RD6 = 0b00_0111,
        RD7 = 0b00_1000,
        RD8 = 0b00_1001,
        RD9 = 0b00_1010,
        RDA = 0b00_1011,
        RDB = 0b00_1100,
        RDC = 0b00_1101,
        RDD = 0b00_1110,
        RDE = 0b00_1111,
        RDF = 0b01_0000,
        CCR = 0b01_1111,
        CR0 = 0b10_0000,
        STP = 0b10_0100,
        FBP = 0b10_0101,
        VATTA = 0b10_0110,
        IDT = 0b10_0111,
        IRA = 0b10_1000,
        IP = 0b10_1001,
        TRA = 0b10_1010, // Task Descriptor Address (address of currently running task)
        TDTA = 0b10_1011, // Task Descriptor Table Address
        KERNEL_STP = 0b11_1010,
        KERNEL_FBP = 0b11_1011,
        KERNEL_IP = 0b11_1100,
    }

    public class Core
    {
        public uint core_id { get; }
        public uint clock_speed_hz { get; }
        public PowerStatus power_status;
        public bool requested_power_off;
        public Clock clock { get; }
        public Package? package;
        public Thread? thread;

        public Register32 RD0 = new Register32(0, name: "RD0");
        public Register32 RD1 = new Register32(0, name: "RD1");
        public Register32 RD2 = new Register32(0, name: "RD2");
        public Register32 RD3 = new Register32(0, name: "RD3");
        public Register32 RD4 = new Register32(0, name: "RD4");
        public Register32 RD5 = new Register32(0, name: "RD5");
        public Register32 RD6 = new Register32(0, name: "RD6");
        public Register32 RD7 = new Register32(0, name: "RD7");
        public Register32 RD8 = new Register32(0, name: "RD8");
        public Register32 RD9 = new Register32(0, name: "RD9");
        public Register32 RDA = new Register32(0, name: "RDA");
        public Register32 RDB = new Register32(0, name: "RDB");
        public Register32 RDC = new Register32(0, name: "RDC");
        public Register32 RDD = new Register32(0, name: "RDD");
        public Register32 RDE = new Register32(0, name: "RDE");
        public Register32 RDF = new Register32(0, name: "RDF");

        public ConditionCodeRegister CCR = new ConditionCodeRegister(name: "CCR");
        public Register32 CR0 = new Register32(0b10000000_00000000_00000000_00000000, name: "CR0");
        public Register32 STP = new Register32(0, name: "STP");
        public Register32 FBP = new Register32(0, name: "FBP");
        public Register32 VATTA = new Register32(0, name: "VATTA");
        public Register32 IDT = new Register32(0, name: "IDT");
        public Register32 IRA = new Register32(0, name: "IRA");
        public Register32 IP = new Register32(0, name: "IP");
        public Register32 TRA = new Register32(0, name: "TRA");
        public Register32 TDTA = new Register32(0, name: "TDTA");
        public Register32 KERNEL_STP = new Register32(0, name: "KERNEL_STP");
        public Register32 KERNEL_FBP = new Register32(0, name: "KERNEL_FBP");
        public Register32 KERNEL_IP = new Register32(0, name: "KERNEL_IP");
        // Register containing the instruction to be executed after it was fetched from
        // memory.
        public Register32 vInstruction = new Register32(0, name: "vInstruction");
        // Register containing the size of the instruction to be executed, used
        // to offset the instruction pointer.
        public Register32 vInstructionSize = new Register32(0, name: "vInstructionSize");
        // Hidden intermediate register 0 used by micro-ops.
        public Register32 vRD0 = new Register32(0, name: "vRD0");
        // Hidden intermediate register 1 used by micro-ops.
        public Register32 vRD1 = new Register32(0, name: "vRD1");
        // Hidden intermediate register 2 used by micro-ops.
        public Register32 vRD2 = new Register32(0, name: "vRD2");
        // Hidden intermediate register 3 used by micro-ops.
        public Register32 vRD3 = new Register32(0, name: "vRD3");
        public Pipeline kernel_pipeline = new Pipeline();
        public Pipeline user_pipeline = new Pipeline();
        public Pipeline interrupt_pipeline = new Pipeline();
        // Micro-op that was primed and currently is being executed.
        // Execution of micro-op can take multiple cycles, for example if this is
        // a blocking memory read operation.
        public IEnumerator? currentMicroOp;
        // Interrupt currently being processed by the core.
        // If null then no interrupt is being processed.
        public uint? interrupt = null;

        public Dictionary<RegisterID, Register32> registers;

        public Core(
            uint core_id,
            uint clock_speed_hz
        )
        {
            this.core_id = core_id;
            this.clock_speed_hz = clock_speed_hz;

            power_status = PowerStatus.OFF;
            clock = new Clock(clock_speed_hz);

            registers = new Dictionary<RegisterID, Register32>{
                { RegisterID.RD0, RD0 },
                { RegisterID.RD1, RD1 },
                { RegisterID.RD2, RD2 },
                { RegisterID.RD3, RD3 },
                { RegisterID.RD4, RD4 },
                { RegisterID.RD5, RD5 },
                { RegisterID.RD6, RD6 },
                { RegisterID.RD7, RD7 },
                { RegisterID.RD8, RD8 },
                { RegisterID.RD9, RD9 },
                { RegisterID.RDA, RDA },
                { RegisterID.RDB, RDB },
                { RegisterID.RDC, RDC },
                { RegisterID.RDD, RDD },
                { RegisterID.RDE, RDE },
                { RegisterID.RDF, RDF },
                { RegisterID.CCR, CCR },
                { RegisterID.CR0, CR0 },
                { RegisterID.STP, STP },
                { RegisterID.FBP, FBP },
                { RegisterID.VATTA, VATTA },
                { RegisterID.IDT, IDT },
                { RegisterID.IRA, IRA },
                { RegisterID.IP, IP },
                { RegisterID.TRA, TRA },
                { RegisterID.TDTA, TDTA },
                { RegisterID.KERNEL_STP, KERNEL_STP },
                { RegisterID.KERNEL_FBP, KERNEL_FBP },
                { RegisterID.KERNEL_IP, KERNEL_IP },
            };
        }

        public Pipeline getPipeline()
        {
            if (interrupt != null)
            {
                return interrupt_pipeline;
            }
            return isKernelMode() ? kernel_pipeline : user_pipeline;
        }

        public void primeMicroOpDecoding()
        {
            pushMicroOp(new ReadMemory(IP, vInstruction));
            pushMicroOp(new InstructionDecode(vInstruction));
        }

        // Execute the next tick on the current micro-op or if it's finished then
        // start execution of the next micro-op in the pipeline.
        public void executeMicroOp()
        {
            if (interrupt == null)
            {
                if (package?.interruptQueue.TryDequeue(out var temp_interrupt) ?? false)
                {
                    getPipeline().pushBack(new EnterInterruptHandler(temp_interrupt));
                }
            }
            if (currentMicroOp?.MoveNext() ?? false)
            {
                return;
            }
            if (!getPipeline().isEmpty())
            {
                currentMicroOp = getPipeline().popFront().execute(this).GetEnumerator();
            }
        }
        // Push a micro-op to the pipeline.
        public void pushMicroOp(MicroOp microOp)
        {
            getPipeline().pushBack(microOp);
        }
        // Get the kernel mode bit from the control register 0.
        public bool isKernelMode()
        {
            return CR0.readBit(31);
        }
        // Set the kernel mode bit in the control register 0.
        public void setKernelModeBit(bool value)
        {
            CR0.writeBit(31, value);
        }
        // Get the virtual memory enabled bit from the control register 0.
        public bool isVirtualMemoryEnabled()
        {
            return CR0.readBit(0);
        }
        // Set the virtual memory enabled bit in the control register 0.
        public void setVirtualMemoryEnabledBit(bool value)
        {
            CR0.writeBit(0, value);
        }
        // Method for starting the core and initializing it's execution loop.
        public virtual void powerOn(Package package)
        {
            if (power_status == PowerStatus.ON)
            {
                throw new Exception("Core is already powered on");
            }
            power_status = PowerStatus.STARTING;
            this.package = package;
            powerOnInit();
            power_status = PowerStatus.ON;
        }
        // Function invoked during initialization phase of core after the signal to
        // power on is received.
        public virtual void powerOnInit()
        {
            foreach (var reg in registers.Values)
            {
                reg.reset();
            }
            var ram_end_address = GetMemoryController().getRamAddressRange().end_address.ToUInt32();
            STP.writeUInt32(ram_end_address);
            FBP.writeUInt32(ram_end_address);

            KERNEL_STP.writeUInt32(ram_end_address);
            KERNEL_FBP.writeUInt32(ram_end_address);

            var firmware_address = GetMemoryController().getFirmwareAddressRange().base_address.ToUInt32();
            IP.writeUInt32(firmware_address);

            thread = new Thread(executionLoop);
            thread.Start();
        }
        // Get reference to the memory controller of the motherboard or throw an exception.
        public IoController GetMemoryController()
        {
            return package!.motherboard!.controller;
        }
        // Function used as main execution loop of the thread representing running core.
        public virtual void executionLoop()
        {
            while (!requested_power_off)
            {
                executeMicroOp();
                clock.waitForCycles(1);
            }
            requested_power_off = false;
            if (power_status == PowerStatus.ON)
            {
                throw new Exception("Incorrect core power state");
            }
        }
        // Method for stopping the core. It will wait for the current instruction to finish
        // executing and then stop the core.
        public virtual void powerOff()
        {
            if (power_status == PowerStatus.OFF)
            {
                throw new Exception("Core is already powered off");
            }
            power_status = PowerStatus.STOPPING;
            powerOffTeardown();
            power_status = PowerStatus.OFF;
        }
        // Perform cleanup of the core after power off signal was received.
        public virtual void powerOffTeardown()
        {
            requested_power_off = true;

            currentMicroOp = null;
            user_pipeline.flush();
            kernel_pipeline.flush();

            while (power_status == PowerStatus.ON)
            {
                clock.waitForCycles(1);
            }
            thread!.Join();
            thread = null;
            package = null;
        }
        // Get the current power status of the core.
        public PowerStatus getPowerStatus()
        {
            return power_status;
        }
    }
}