﻿using System.Collections.Generic;
using System.Text.RegularExpressions;

using CwIRC;
using MBFunctionInterface;
using MBUtilities;
using MBUtilities.Channel;

namespace Bot
{
    public class Ignore : Function
    {
        public Ignore()
        {
            Help = "ignore <user(s)> - Tells MoronBot to ignore the specified user(s). UserList functions will still work, however (TellAuto, for instance).";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;
        }

        public override List<IRCResponse> GetResponse(BotMessage message)
        {
            if (Regex.IsMatch(message.Command, "^(ignore)$", RegexOptions.IgnoreCase))
            {
                if (message.TargetType == IRCMessage.TargetTypes.CHANNEL && ChannelList.UserIsAnyOp(message.User.Name, message.ReplyTo))
                {
                    foreach (string parameter in message.ParameterList)
                    {
                        if (!Settings.Instance.IgnoreList.Contains(parameter.ToUpper()))
                        {
                            Settings.Instance.IgnoreList.Add(parameter.ToUpper());
                        }
                    }
                    return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, message.Parameters + (message.ParameterList.Count > 1 ? " are " : " is ") + "now ignored.", message.ReplyTo) };
                }
                else
                {
                    return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "You need to be an operator to make me ignore people :P", message.ReplyTo) };
                }
            }
            else
            {
                return null;
            }
        }
    }

    public class Unignore : Function
    {
        public Unignore()
        {
            Help = "unignore <user(s)> - Tells MoronBot to unignore the specified user(s).";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;
        }

        public override List<IRCResponse> GetResponse(BotMessage message)
        {
            if (Regex.IsMatch(message.Command, "^(unignore)$", RegexOptions.IgnoreCase))
            {
                if (message.TargetType == IRCMessage.TargetTypes.CHANNEL && ChannelList.UserIsAnyOp(message.User.Name, message.ReplyTo))
                {
                    foreach (string parameter in message.ParameterList)
                    {
                        if (Settings.Instance.IgnoreList.Contains(parameter.ToUpper()))
                        {
                            Settings.Instance.IgnoreList.Remove(parameter.ToUpper());
                        }
                    }
                    return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, message.Parameters + (message.ParameterList.Count > 1 ? " are " : " is ") + "no longer ignored.", message.ReplyTo) };
                }
                else
                {
                    return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "You need to be an operator to make me unignore people :P", message.ReplyTo) };
                }
            }
            else
            {
                return null;
            }
        }
    }
}
