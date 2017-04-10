using System;

namespace IRC_Library
{
    public class ArgumentRegexException : ArgumentException
    {
        public ArgumentRegexException(string argument, string regex) : base(argument)
        {
            this.Regex = regex;
        }

        public string Regex
        {
            get;
        }
    }
}