﻿using System.Collections.Generic;
using System.Text.RegularExpressions;

using CwIRC;
using MBFunctionInterface;
using MBUtilities;

namespace Utility
{
    public class RemoveTell : Function
    {
        public RemoveTell()
        {
            Help = "r(emove)tell <message text> - Removes a message from the message database, if it matches <message text> and was sent by you.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;
        }

        public override List<IRCResponse> GetResponse(BotMessage message)
        {
            if (Regex.IsMatch(message.Command, "^(r(emove)?tell)$", RegexOptions.IgnoreCase))
            {
                if (message.ParameterList.Count > 0)
                {
                    foreach (string user in Tell.MessageMap.Keys)
                    {
                        int index = Tell.MessageMap[user].FindIndex(s =>
                            s.From.ToLowerInvariant() == message.User.Name.ToLowerInvariant() &&
                            Regex.IsMatch(s.Message, ".*" + message.Parameters + ".*", RegexOptions.IgnoreCase));

                        if (index >= 0)
                        {
                            List<IRCResponse> response = new List<IRCResponse>();
                            response.Add(new IRCResponse(ResponseType.Say, "Message \"" +
                                Tell.MessageMap[user][index].Message +
                                "\", sent on date \"" +
                                Tell.MessageMap[user][index].SentDate +
                                "\" to \"" +
                                user +
                                "\" removed from the message database!", message.ReplyTo));
                            Tell.MessageMap[user].RemoveAt(index);
                            Tell.WriteMessages();
                            return response;
                        }
                    }
                    
                    return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "No message sent by you containing \"" + message.Parameters + "\" found in the message database!", message.ReplyTo) };
                }
                else
                {
                    return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "You didn't specify a message to remove!", message.ReplyTo) };
                }
            }

            return null;
        }
    }
}