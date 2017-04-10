using System;
using System.Text.RegularExpressions;

namespace IRC_Library
{
    public sealed class Sender
    {
        public static Sender FromIRC(string fromIRC)
        {
            if (string.IsNullOrWhiteSpace(fromIRC))
                throw new ArgumentNullException("fromIRC");

            int at = fromIRC.IndexOf('@');

            if (at == -1)
            {
                if (fromIRC.IndexOf('!') == -1)
                {
                    return new Sender(null, null, fromIRC);
                }
                else
                {
                    string[] names = fromIRC.Split('!');
                    if (names.Length > 2 || names.Length == 0)
                        throw new ArgumentRegexException("fromIRC");
                    if (names.Length == 1)
                        return new Sender(names[0], null, null);
                    else
                        return new Sender(names[0], names[1], null);
                }
            }
            else
            {
                string host = fromIRC.Substring(at + 1);
                string front = fromIRC.Substring(0, at);

                string[] names = front.Split('!');
                if (names.Length > 2 || names.Length == 0)
                    throw new ArgumentRegexException("fromIRC");
                if (names.Length == 1)
                    return new Sender(names[0], null, host);
                else
                    return new Sender(names[0], names[1], host);
            }
        }
        public Sender(string nick, string user, string host)
        {
            if (nick != null && !Regex.IsMatch(nick, "[A-Za-z0-9]*"))
                throw new ArgumentRegexException("nick");
            this.Nick = nick;

            if (user != null && !Regex.IsMatch(user, "[A-Za-z0-9]*"))
                throw new ArgumentRegexException("user");
            this.User = user;

            if (host != null && !Regex.IsMatch(host, "[A-Za-z0-9]*"))
                throw new ArgumentRegexException("host");
            this.Host = host;
        }

        public string Nick
        {
            get;
            private set;
        }

        public string User
        {
            get;
            private set;
        }

        public string Host
        {
            get;
            private set;
        }

        public override string ToString()
        {
            return string.Format("{0}!{1}@{2}", Nick, User, Host);
        }

        public override int GetHashCode()
        {
            return unchecked(Nick.GetHashCode() + User.GetHashCode() + Host.GetHashCode());
        }

        public override bool Equals(object obj)
        {
            if (obj is Sender)
            {
                Sender sender = (Sender)obj;
                return sender.Nick == this.Nick && sender.User == this.User && sender.Host == this.Host;
            }
            else
                return false;
        }
    }
}