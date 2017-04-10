using System;
using System.Collections.Generic;

namespace IRC_Library
{
    public class Channel
    {
        internal Channel(IRC library, string name)
        {
            if (library == null)
                throw new ArgumentNullException(nameof(library));
            this.Library = library;

            if (name.Length == 0 || name[0] != '#')
                throw new ArgumentRegexException(nameof(name), "^(#)");
            this._name = name;
        }

        public IRC Library
        {
            get;
            private set;
        }

        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
        }

        private List<ChannelUser> _users;
        public ChannelUser[] Users
        {
            get
            {
                return _users.ToArray();
            }
        }

        public override string ToString()
        {
            return Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is Channel)
                return ((Channel)obj).Name == this.Name;
            else
                return false;
        }
    }
}