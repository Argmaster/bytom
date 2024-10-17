using System.Collections.Generic;

namespace Bytom.Hardware.CPU
{
    public class Package
    {
        public List<Core> cores { get; set; }

        public Package(List<Core> cores_)
        {
            this.cores = cores_;
        }
    }
}
