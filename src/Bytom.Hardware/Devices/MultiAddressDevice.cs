using System.Collections.Generic;

namespace Bytom.Hardware
{
    public class MultiAddressDevice : Device
    {
        public List<AddressRange> address_ranges { get; set; }

        public MultiAddressDevice(uint max_tasks_running, Clock clock)
            : base(max_tasks_running, clock)
        {
            address_ranges = new List<AddressRange>();
        }

        public override bool isInMyAddressRange(Address address)
        {
            return address_ranges.Exists(range => range.contains(address));
        }
    }
}