using System;

namespace IRC_Library
{
    public class ArgumentRegexException : ArgumentException
    {
        public ArgumentRegexException(string argument) : base(argument)
        {
        }
    }
}