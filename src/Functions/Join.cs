﻿using System.Text.RegularExpressions;

using CwIRC;

namespace MoronBot.Functions
{
    class Join : Function
    {
        public Join(MoronBot moronBot)
        {
            Name = GetName();
            Help = "join <channel>\t\t- Joins the specified channel.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;
        }
        
        public override IRCResponse GetResponse(BotMessage message, MoronBot moronBot)
        {
            if (Regex.IsMatch(message.Command, "^(join)$", RegexOptions.IgnoreCase))
            {
                if (message.ParameterList.Count > 0)
                {
                    string output = "";
                    foreach (string parameter in message.ParameterList)
                    {
                        output += "JOIN ";
                        if (!parameter.StartsWith("#"))
                        {
                            output += "#";
                        }
                        output += parameter + "\r\n";
                    }
                    return new IRCResponse(ResponseType.Raw, output, "");
                }
                else
                {
                    return new IRCResponse(ResponseType.Say, message.User.Name + ", you didn't say where I should join.", message.ReplyTo);
                }
            }
            else
            {
                return null;
            }
        }
    }
}
