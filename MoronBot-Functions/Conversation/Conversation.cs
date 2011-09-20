﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using CwIRC;
using MBFunctionInterface;
using MBUtilities;

namespace Automatic
{
    public class Conversation : Function
    {
        DateTime lastCheese = DateTime.Now.AddMinutes(-60);

        public Conversation()
        {
            Help = "A set of automatic functions that react to specific keywords/phrases in chat.";
            Type = Types.Regex;
            AccessLevel = AccessLevels.Anyone;
        }

        public override List<IRCResponse> GetResponse(BotMessage message)
        {
            // Cheese in message
            if (Regex.IsMatch(message.MessageString, "cheese", RegexOptions.IgnoreCase))
            {
                if (lastCheese.AddMinutes(60).CompareTo(DateTime.Now) <= 0)
                {
                    lastCheese = DateTime.Now;
                    return new List<IRCResponse>() { new IRCResponse(ResponseType.Do, "loves cheese", message.ReplyTo) };
                }
            }

            // Windmill in message
            if (Regex.IsMatch(message.MessageString, "windmill", RegexOptions.IgnoreCase))
            {
                return new List<IRCResponse>() { 
                    new IRCResponse(ResponseType.Say, "WINDMILLS DO NOT WORK THAT WAY!", message.ReplyTo) };
            }

            // Someone has greeted MoronBot
            Match match = Regex.Match(message.MessageString, @"^('?sup|hi|hey|hello|greetings|bonjour|salut|howdy|'?yo|o?hai|mojn|dongs),?[ ]" + Regex.Escape(Settings.Instance.CurrentNick) + @"([^a-zA-Z0-9_\|`\[\]\^-]|$)", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return new List<IRCResponse>() { 
                    new IRCResponse(ResponseType.Say, match.Value.Split(' ')[0] + " " + message.User.Name, message.ReplyTo) };
            }

            return null;
        }
    }
}
