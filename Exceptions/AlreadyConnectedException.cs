using System;

namespace IRC_Library
{
    public class AlreadyConnectedException : Exception
    {
        public AlreadyConnectedException() : base("Already connected exception")
        {
        }
    }
}