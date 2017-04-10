using System;

namespace IRC_Library
{
    public static class Util
    {
        public static void Quit(this IRC lib, string reason = null)
        {
            if (reason == null)
                lib.SendRawMessage("QUIT");
            else
                lib.SendRawMessage($"QUIT :{reason}");
        }

        #region Channel

        public static void JoinChannel(this IRC lib, string channel)
        {
            if (!channel.StartsWith("#"))
                channel = '#' + channel;
            lib.SendRawMessage($"JOIN {channel}");
        }
        public static void LeaveChannel(this IRC lib, string channel)
        {
            if (!channel.StartsWith("#"))
                channel = '#' + channel;
            lib.SendRawMessage($"PART {channel}");
        }

        #endregion

        #region Channel Topic

        public static void ChangeChannelTopic(this IRC lib, string channel, string newTopic)
        {
            if (!channel.StartsWith("#"))
                channel = '#' + channel;

            if (string.IsNullOrWhiteSpace(newTopic))
                throw new ArgumentNullException(nameof(newTopic));

            lib.SendRawMessage($"TOPIC {channel} :{newTopic}");
        }
        public static void RemoveChannelTopic(this IRC lib, string channel)
        {
            if (!channel.StartsWith("#"))
                channel = '#' + channel;

            lib.SendRawMessage($"TOPIC {channel} :");
        }
        public static void RequestChannelTopic(this IRC lib, string channel)
        {
            if (!channel.StartsWith("#"))
                channel = '#' + channel;

            lib.SendRawMessage($"TOPIC {channel}");
        }
        public static void SetChannelTopicEditable(this IRC lib, string channel, bool editable)
        {
            if (!channel.StartsWith("#"))
                channel = '#' + channel;

            lib.SendRawMessage(editable ? $"MODE {channel} -t" : $"MODE {channel} +t");
        }

        #endregion

        #region Channel Invites

        public static void SetChannelInviteOnly(this IRC lib, string channel, bool inviteOnly)
        {
            if (!channel.StartsWith("#"))
                channel = '#' + channel;

            lib.SendRawMessage(inviteOnly ? $"MODE {channel} +t" : $"MODE {channel} -t");
        }
        public static void InviteToChannel(this IRC lib, string channel, string nick)
        {
            if (!channel.StartsWith("#"))
                channel = '#' + channel;

            lib.SendRawMessage($"INVITE {nick} {channel}");
        }

        public static void AllowJoinOnInviteOnly(this IRC lib, string channel, string nick, bool allow = true)
        {
            if (!channel.StartsWith("#"))
                channel = '#' + channel;

            lib.SendRawMessage(allow ? $"MODE {channel} +I {nick}" : $"MODE {channel} -I {nick}");
        }

        #endregion

        #region Channel Bad Words Filter

        public static void EnableBadWordsFilter(this IRC lib, string channel)
        {
            if (!channel.StartsWith("#"))
                channel = '#' + channel;

            lib.SendRawMessage($"MODE {channel} +G");
        }

        public static void DisableBadWordsFilter(this IRC lib, string channel)
        {
            if (!channel.StartsWith("#"))
                channel = '#' + channel;

            lib.SendRawMessage($"MODE {channel} -G");
        }

        #endregion

        #region Channel Permissions

        public static void AddChannelOperator(this IRC lib, string channel, string nick)
        {
            if (!channel.StartsWith("#"))
                channel = '#' + channel;

            lib.SendRawMessage($"MODE {channel} +o {nick}");
        }
        public static void RemoveChannelOperator(this IRC lib, string channel, string nick)
        {
            if (!channel.StartsWith("#"))
                channel = '#' + channel;

            lib.SendRawMessage($"MODE {channel} -o {nick}");
        }

        public static void AddChannelHalfOperator(this IRC lib, string channel, string nick)
        {
            if (!channel.StartsWith("#"))
                channel = '#' + channel;

            lib.SendRawMessage($"MODE {channel} +h {nick}");
        }
        public static void RemoveChannelHalfOperator(this IRC lib, string channel, string nick)
        {
            if (!channel.StartsWith("#"))
                channel = '#' + channel;

            lib.SendRawMessage($"MODE {channel} -h {nick}");
        }

        public static void AddChannelVoice(this IRC lib, string channel, string nick)
        {
            if (!channel.StartsWith("#"))
                channel = '#' + channel;

            lib.SendRawMessage($"MODE {channel} +v {nick}");
        }
        public static void RemoveChannelVoice(this IRC lib, string channel, string nick)
        {
            if (!channel.StartsWith("#"))
                channel = '#' + channel;

            lib.SendRawMessage($"MODE {channel} -v {nick}");
        }

        #endregion
    }
}