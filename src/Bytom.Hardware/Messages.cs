using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Bytom.Hardware
{
    public interface IoMessage
    {
        public Address address { get; }
    }

    public class WriteMessage : IoMessage
    {
        public Address address { get; }
        public byte data { get; }
        public WriteMessage(Address address, byte data)
        {
            this.address = address;
            this.data = data;
        }
    }

    public class ReadMessage : IoMessage
    {
        public Address address { get; }
        public ConcurrentQueue<WriteMessage> writeBackQueue { get; }
        public ReadMessage(Address address, ConcurrentQueue<WriteMessage> writeBackQueue)
        {
            this.address = address;
            this.writeBackQueue = writeBackQueue;
        }
    }
}