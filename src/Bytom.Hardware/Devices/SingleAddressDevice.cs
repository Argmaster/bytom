namespace Bytom.Hardware
{
    public class SingleAddressDevice : Device
    {
        public AddressRange? address_range { get; set; }

        public SingleAddressDevice(uint max_tasks_running, Clock clock)
            : base(max_tasks_running, clock)
        { }

        public override void powerOffTeardown()
        {
            base.powerOffTeardown();
            address_range = null;
        }

        public override bool isInMyAddressRange(Address address)
        {
            return address_range?.contains(address) ?? false;
        }
    }
}