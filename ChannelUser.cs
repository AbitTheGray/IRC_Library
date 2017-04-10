using System;

namespace IRC_Library
{
    public class ChannelUser
    {
        public ChannelUser(IRC library, Channel channel, string username)
        {
            if (library == null)
                throw new ArgumentNullException("library");
            this.Library = library;
        }

        public IRC Library
        {
            get;
            private set;
        }

        public Channel Channel
        {
            get;
            private set;
        }

        public string Username
        {
            get;
            private set;
        }


        internal bool _operator = false;
        public bool Operator
        {
            get
            {
                return _operator;
            }
            set
            {
                if (value)
                    Util.AddChannelOperator(Library, Channel.ToString(), Username);
                else
                    Util.RemoveChannelOperator(Library, Channel.ToString(), Username);
            }
        }


        internal bool _halfoperator = false;
        public bool HalfOperator
        {
            get
            {
                return _halfoperator;
            }
            set
            {
                if (value)
                    Util.AddChannelHalfOperator(Library, Channel.ToString(), Username);
                else
                    Util.RemoveChannelHalfOperator(Library, Channel.ToString(), Username);
            }
        }


        internal bool _voice = false;
        public bool Voice
        {
            get
            {
                return _voice;
            }
            set
            {
                if (value)
                    Util.AddChannelVoice(Library, Channel.ToString(), Username);
                else
                    Util.RemoveChannelVoice(Library, Channel.ToString(), Username);
            }
        }
    }
}