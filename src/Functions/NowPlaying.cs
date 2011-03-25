﻿using System.Text.RegularExpressions;

using CwIRC;

namespace MoronBot.Functions
{
    class NowPlaying : Function
    {
        public NowPlaying(MoronBot moronBot)
        {
            Name = GetName();
            Help = "np (<user>)\t\t- Returns your currently playing music (from Last.fm). You can also supply a specific username to check.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;
        }

        public override void GetResponse(BotMessage message, MoronBot moronBot)
        {
            if (Regex.IsMatch(message.Command, "^(np)$", RegexOptions.IgnoreCase))
            {
                if (message.ParameterList.Count > 0)
                {
                    return;
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }
    }
}