﻿using System.Collections.Generic;
using System.Text.RegularExpressions;

using CwIRC;

namespace MoronBot.Functions.Bot
{
    class Leave : Function
    {
        public Leave()
        {
            Help = "leave/gtfo [<channel>]\t- Leaves the current channel, or the one specified.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;

            AccessList.Add("Tyranic-Moron");
        }
        
        public override List<IRCResponse> GetResponse(BotMessage message)
        {
            if (Regex.IsMatch(message.Command, "^(leave|gtfo)$", RegexOptions.IgnoreCase))
            {
                if (message.ParameterList.Count > 0)
                {
                    return new List<IRCResponse>() { new IRCResponse(ResponseType.Raw, "PART " + message.ReplyTo + " :" + message.Parameters, "") };
                }
                else
                {
                    return new List<IRCResponse>() { new IRCResponse(ResponseType.Raw, "PART " + message.ReplyTo + " :" + Settings.Instance.LeaveMessage, "") };
                }
            }
            return null;
        }
    }
}
