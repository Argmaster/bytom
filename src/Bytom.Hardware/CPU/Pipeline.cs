


using System.Collections.Generic;
using System.Diagnostics;

namespace Bytom.Hardware.CPU
{
    public class Pipeline
    {
        public List<MicroOp> microOps = new List<MicroOp>();
        public int Count => microOps.Count;

        public void pushBack(MicroOp microOp)
        {
            microOps.Add(microOp);
        }

        public void pushFront(MicroOp microOp)
        {
            microOps.Insert(0, microOp);
        }

        public bool isEmpty()
        {
            return microOps.Count == 0;
        }

        public MicroOp popFront()
        {
            Debug.Assert(microOps.Count > 0);
            MicroOp microOp = microOps[0];
            microOps.RemoveAt(0);
            return microOp;
        }

        public void flush()
        {
            microOps.Clear();
        }
    }
}