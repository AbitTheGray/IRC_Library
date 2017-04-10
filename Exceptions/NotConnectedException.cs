using System;

namespace IRC_Library
{
    public class NotConnectedException : Exception
    {
        public NotConnectedException() : base("Not Connected to IRC Server")
        {
        }
    }
}