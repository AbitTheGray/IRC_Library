using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;

namespace IRC_Library
{
    public sealed class IRC
    {
        public IRC(IPEndPoint endpoint)
        {
            this.EndPoint = endpoint;

            MessageReceived += (sender, id, message) =>
            {
                if (id.Equals("PRIVMSG", StringComparison.InvariantCultureIgnoreCase) && PrivateMessageReceived != null)
                {
                    int index = message.IndexOf(' ');
                    PrivateMessageReceived(sender, message.Substring(0, index), message.Substring(index + 1));
                }
                if (id.Equals("NOTICE", StringComparison.InvariantCultureIgnoreCase) && PrivateMessageReceived != null)
                {
                    int index = message.IndexOf(' ');
                    NoticeMessageReceived(sender, message.Substring(0, index), message.Substring(index + 1));
                }
            };

            RawMessageReceived += (message) =>
            {
                if (message.StartsWith("PING ", StringComparison.InvariantCultureIgnoreCase))
                    SendRawMessage("PONG " + message.Substring(5));
            };
        }
        public IRC(IPAddress address, int port = 6667) : this(new IPEndPoint(address, port))
        {
        }
        public IRC(string address, int port = 6667) : this(new IPEndPoint(Dns.GetHostAddresses(address)[0], port))
        {
        }

        public IPEndPoint EndPoint
        {
            get;
            private set;
        }

        private TcpClient _client = null;
        private StreamReader _reader = null;
        private StreamWriter _writer = null;

        private Thread t_listener = null;

        public void Connect(string nick, string username, string realName, string password = null)
        {
            if (Connected)
                throw new AlreadyConnectedException();

            if (!Regex.IsMatch(nick, "[A-Za-z0-9]*"))
                throw new ArgumentException(nameof(nick));
            if (!Regex.IsMatch(username, "[A-Za-z0-9]*"))
                throw new ArgumentException(nameof(username));

            _client = new TcpClient();
            _client.Connect(EndPoint);

            _reader = new StreamReader(_client.GetStream());

            t_listener = new Thread(() =>
            {
                while (Connected)
                {
                    try
                    {
                        string line = _reader.ReadLine();
                        if (string.IsNullOrWhiteSpace(line))
                            continue;
                        if (RawMessageReceived != null)
                            RawMessageReceived(line);
                        if (line.StartsWith("ERROR ", StringComparison.InvariantCultureIgnoreCase))
                            Close();

                        if (MessageReceived == null)
                            continue;

                        if (line[0] != ':')
                            continue;
                        line = line.Substring(1);
                        int firstSpace = line.IndexOf(' ');
                        if (firstSpace == -1)
                            continue;
                        Sender sender = Sender.FromIRC(line.Substring(0, firstSpace));
                        line = line.Substring(firstSpace + 1);
                        firstSpace = line.IndexOf(' ');
                        if (firstSpace == -1)
                            continue;
                        string id = line.Substring(0, firstSpace);
                        line = line.Substring(firstSpace + 1);

                        MessageReceived(sender, id, line);
                    }
                    catch
                    {
                        break;
                    }
                }
            });
            t_listener.Start();
            _writer = new StreamWriter(_client.GetStream());

            if (password != null)
                SendRawMessage("PASS " + password);
            SendRawMessage("NICK " + nick);
            SendRawMessage($"USER {username} 8 * :{realName}");
        }

        public bool Connected
        {
            get
            {
                return _client != null && _client.Connected;
            }
        }

        public void Quit(string reason = null)
        {
            if (reason == null)
                SendRawMessage("QUIT");
            else
                SendRawMessage($"QUIT :{reason}");

            Close();
        }

        public void Close()
        {
            if (!Connected)
                throw new NotConnectedException();

            if (ConnectionClosed != null)
                ConnectionClosed();

            _writer.Dispose();
            _reader.Dispose();

            _client.Close();

            _client = null;
        }

        public event EmptyEventHandler ConnectionClosed;
        public delegate void EmptyEventHandler();

        public event RawMessageHandler RawMessageReceived;
        public event RawMessageHandler RawMessageSend;
        public delegate void RawMessageHandler(string message);

        public event MessageHandler MessageReceived;
        public delegate void MessageHandler(Sender sender, string id, string message);

        public event PrivateMessageHandler PrivateMessageReceived;
        public event PrivateMessageHandler NoticeMessageReceived;
        public delegate void PrivateMessageHandler(Sender sender, string channel, string message);

        public void SendRawMessage(string message)
        {
            if (!Connected)
                throw new NotConnectedException();
            _writer.WriteLine(message);
            _writer.Flush();
            if (RawMessageSend != null)
                RawMessageSend(message);
        }



        #region Channel

        public static bool IsChannelName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;
            if (name.Length > 50)
                return false;
            if (name.IndexOfAny(new char[] { (char)7, ' ', ',' }) != -1)
                return false;
            var ch = name[0];
            switch (ch)
            {
                case '&':
                case '#':
                case '+':
                case '!':
                    return true;
                default:
                    return false;
            }
        }

        public void JoinChannel(string channel)
        {
            if (!IsChannelName(channel))
                throw new InvalidChannelNameException(channel);

            SendRawMessage($"JOIN {channel}");
        }
        public void LeaveChannel(string channel)
        {
            if (!IsChannelName(channel))
                throw new InvalidChannelNameException(channel);

            SendRawMessage($"PART {channel}");
        }

        #endregion

        #region Channel Topic

        public void ChangeChannelTopic(string channel, string newTopic)
        {
            if (!IsChannelName(channel))
                throw new InvalidChannelNameException(channel);

            if (string.IsNullOrWhiteSpace(newTopic))
                throw new ArgumentNullException(nameof(newTopic));

            SendRawMessage($"TOPIC {channel} :{newTopic}");
        }
        public void RemoveChannelTopic(string channel)
        {
            if (!IsChannelName(channel))
                throw new InvalidChannelNameException(channel);

            SendRawMessage($"TOPIC {channel} :");
        }
        public void RequestChannelTopic(string channel)
        {
            if (!IsChannelName(channel))
                throw new InvalidChannelNameException(channel);

            SendRawMessage($"TOPIC {channel}");
        }
        public void SetChannelTopicEditable(string channel, bool editable)
        {
            if (!IsChannelName(channel))
                throw new InvalidChannelNameException(channel);

            SendRawMessage(editable ? $"MODE {channel} -t" : $"MODE {channel} +t");
        }

        #endregion

        #region Channel Invites

        public void SetChannelInviteOnly(IRC lib, string channel, bool inviteOnly)
        {
            if (!IsChannelName(channel))
                throw new InvalidChannelNameException(channel);

            lib.SendRawMessage(inviteOnly ? $"MODE {channel} +t" : $"MODE {channel} -t");
        }
        public void InviteToChannel(IRC lib, string channel, string nick)
        {
            if (!IsChannelName(channel))
                throw new InvalidChannelNameException(channel);

            lib.SendRawMessage($"INVITE {nick} {channel}");
        }

        public void AllowJoinOnInviteOnly(IRC lib, string channel, string nick, bool allow = true)
        {
            if (!IsChannelName(channel))
                throw new InvalidChannelNameException(channel);

            lib.SendRawMessage(allow ? $"MODE {channel} +I {nick}" : $"MODE {channel} -I {nick}");
        }

        #endregion

        #region Channel Bad Words Filter

        public void EnableBadWordsFilter(IRC lib, string channel)
        {
            if (!IsChannelName(channel))
                throw new InvalidChannelNameException(channel);

            lib.SendRawMessage($"MODE {channel} +G");
        }

        public void DisableBadWordsFilter(IRC lib, string channel)
        {
            if (!IsChannelName(channel))
                throw new InvalidChannelNameException(channel);

            lib.SendRawMessage($"MODE {channel} -G");
        }

        #endregion

        #region Channel User Permissions

        public void AddChannelOperator(string channel, string nick)
        {
            if (!IsChannelName(channel))
                throw new InvalidChannelNameException(channel);

            SendRawMessage($"MODE {channel} +o {nick}");
        }
        public void RemoveChannelOperator(string channel, string nick)
        {
            if (!IsChannelName(channel))
                throw new InvalidChannelNameException(channel);

            SendRawMessage($"MODE {channel} -o {nick}");
        }

        public void AddChannelHalfOperator(string channel, string nick)
        {
            if (!IsChannelName(channel))
                throw new InvalidChannelNameException(channel);

            SendRawMessage($"MODE {channel} +h {nick}");
        }
        public void RemoveChannelHalfOperator(string channel, string nick)
        {
            if (!IsChannelName(channel))
                throw new InvalidChannelNameException(channel);

            SendRawMessage($"MODE {channel} -h {nick}");
        }

        public void AddChannelVoice(string channel, string nick)
        {
            if (!IsChannelName(channel))
                throw new InvalidChannelNameException(channel);

            SendRawMessage($"MODE {channel} +v {nick}");
        }
        public void RemoveChannelVoice(string channel, string nick)
        {
            if (!IsChannelName(channel))
                throw new InvalidChannelNameException(channel);

            SendRawMessage($"MODE {channel} -v {nick}");
        }

        #endregion
    }
}