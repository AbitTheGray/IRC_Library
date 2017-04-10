using System;
namespace IRC_Library
{
    public class InvalidChannelNameException : Exception
    {
        public InvalidChannelNameException(string channel)
        {
        }

        public string Channel
        {
            get;
        }
    }
}