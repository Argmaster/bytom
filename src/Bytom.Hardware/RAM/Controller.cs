using System.Collections.Generic;


namespace Bytom.Hardware.RAM
{
    public class Controller
    {
        public List<Stick> sticks { get; set; }

        public Controller(List<Stick> sticks)
        {
            this.sticks = sticks;
        }
    }
}