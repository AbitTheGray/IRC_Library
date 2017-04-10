using System;

namespace IRC_Library
{
    public class ChannelUser
    {
        internal ChannelUser(IRC library, Channel channel, string username)
        {
            if (library == null)
                throw new ArgumentNullException(nameof(library));
            this.Library = library;

            this.Channel = channel;
            this.Username = username;
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
                    Library.AddChannelOperator(Channel.ToString(), Username);
                else
                    Library.RemoveChannelOperator(Channel.ToString(), Username);
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
                    Library.AddChannelHalfOperator(Channel.ToString(), Username);
                else
                    Library.RemoveChannelHalfOperator(Channel.ToString(), Username);
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
                    Library.AddChannelVoice(Channel.ToString(), Username);
                else
                    Library.RemoveChannelVoice(Channel.ToString(), Username);
            }
        }
    }
}