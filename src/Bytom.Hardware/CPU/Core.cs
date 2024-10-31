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

        public Register32 RD0;
        public Register32 RD1;
        public Register32 RD2;
        public Register32 RD3;
        public Register32 RD4;
        public Register32 RD5;
        public Register32 RD6;
        public Register32 RD7;
        public Register32 RD8;
        public Register32 RD9;
        public Register32 RDA;
        public Register32 RDB;
        public Register32 RDC;
        public Register32 RDD;
        public Register32 RDE;
        public Register32 RDF;

        public ConditionCodeRegister CCR;
        public Register32 CR0;
        public Register32 STP;
        public Register32 FBP;
        public Register32 VATTA;
        public Register32 IDT;
        public Register32 IRA;
        public Register32 IP;
        public Register32 TRA;
        public Register32 TDTA;
        public Register32 KERNEL_STP;
        public Register32 KERNEL_FBP;
        public Register32 KERNEL_IP;

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

            RD0 = new Register32(0);
            RD1 = new Register32(0);
            RD2 = new Register32(0);
            RD3 = new Register32(0);
            RD4 = new Register32(0);
            RD5 = new Register32(0);
            RD6 = new Register32(0);
            RD7 = new Register32(0);
            RD8 = new Register32(0);
            RD9 = new Register32(0);
            RDA = new Register32(0);
            RDB = new Register32(0);
            RDC = new Register32(0);
            RDD = new Register32(0);
            RDE = new Register32(0);
            RDF = new Register32(0);

            CCR = new ConditionCodeRegister();
            CR0 = new Register32(0b10000000_00000000_00000000_00000000);
            STP = new Register32(0);
            FBP = new Register32(0);
            VATTA = new Register32(0);
            IDT = new Register32(0);
            IRA = new Register32(0);
            IP = new Register32(0);
            TRA = new Register32(0);
            TDTA = new Register32(0);
            KERNEL_STP = new Register32(0);
            KERNEL_FBP = new Register32(0);
            KERNEL_IP = new Register32(0);


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
        public Register32 vInstruction = new Register32(0);
        public Register32 vInstructionSize = new Register32(0);
        public Register32 vRD0 = new Register32(0);
        public Register32 vRD1 = new Register32(0);
        public Register32 vRD2 = new Register32(0);
        public Register32 vRD3 = new Register32(0);
        public List<MicroOp> pipeline = new List<MicroOp>();
        IEnumerator? currentMicroOp;
        List<uint> interrupt_queue = new List<uint>();

        public void primeMicroOpDecoding()
        {
            pushMicroOp(new ReadMemory(IP, vInstruction));
            pushMicroOp(new InstructionDecode(vInstruction));
        }
        public void executeMicroOp()
        {
            if (currentMicroOp?.MoveNext() ?? false)
            {
                return;
            }
            if (pipeline.Count > 0)
            {
                currentMicroOp = pipeline.First().execute(this).GetEnumerator();
                pipeline.RemoveAt(0);
            }
        }
        public void pushMicroOp(MicroOp microOp)
        {
            pipeline.Add(microOp);
        }

        public bool isKernelMode()
        {
            return CR0.readBit(31);
        }

        public void setKernelModeBit(bool value)
        {
            CR0.writeBit(31, value);
        }

        public bool isVirtualMemoryEnabled()
        {
            return CR0.readBit(0);
        }

        public void setVirtualMemoryEnabledBit(bool value)
        {
            CR0.writeBit(0, value);
        }

        public virtual void powerOn(Package package)
        {
            if (power_status == PowerStatus.ON)
            {
                throw new System.Exception("Core is already powered on");
            }
            power_status = PowerStatus.STARTING;
            this.package = package;
            powerOnInit();
            power_status = PowerStatus.ON;
        }

        public virtual void powerOnInit()
        {
            foreach (var reg in registers.Values)
            {
                reg.reset();
            }
            var end_address = GetMemoryController().getRamAddressRange().end_address.ToUInt32();
            STP.writeUInt32(end_address);
            FBP.writeUInt32(end_address);

            thread = new Thread(executionLoop);
            thread.Start();
        }

        public IoController GetMemoryController()
        {
            return package!.motherboard!.controller;
        }

        public virtual void executionLoop()
        {
            primeMicroOpDecoding();
            while (!requested_power_off)
            {
                executeMicroOp();
                clock.waitForCycles(1);
            }
            requested_power_off = false;
            if (power_status == PowerStatus.ON)
            {
                throw new System.Exception("Incorrect core power state");
            }
        }

        public virtual void powerOff()
        {
            if (power_status == PowerStatus.OFF)
            {
                throw new System.Exception("Core is already powered off");
            }
            power_status = PowerStatus.STOPPING;
            powerOffTeardown();
            power_status = PowerStatus.OFF;
        }

        public virtual void powerOffTeardown()
        {
            requested_power_off = true;
            pipeline.Clear();
            while (power_status == PowerStatus.ON)
            {
                clock.waitForCycles(1);
            }
            thread!.Join();
            thread = null;
            package = null;
        }

        public PowerStatus getPowerStatus()
        {
            return power_status;
        }
    }
}