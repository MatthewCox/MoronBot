﻿using System.Text.RegularExpressions;

using CwIRC;

using Bitly;
using System.Collections.Generic;

namespace MoronBot.Functions.GitHub
{
    class LastCommit : Function
    {
        public LastCommit()
        {
            Help = "(last)commit/(last)change <url>\t\t- Gives you a shortened version of a url, via bit.ly";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;
        }
        
        public override List<IRCResponse> GetResponse(BotMessage message)
        {
            if (Regex.IsMatch(message.Command, "^((last)?(commit|change))$", RegexOptions.IgnoreCase))
            {
                Utilities.URL.WebPage commitFeed = Utilities.URL.FetchURL("https://github.com/Tyranic-Moron/MoronBot/commits/master.atom");

                MatchCollection commitMessages = Regex.Matches(commitFeed.Page, @"width:81ex'>(.+)?\&lt;/pre>", RegexOptions.IgnoreCase);
                return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "Last Commit Message: " + commitMessages[0].Groups[1].Value.Replace("\n", " | "), message.ReplyTo) };

            }
            else
            {
                return null;
            }
        }

        public void FetchCommit(string url)
        {
        }
    }
}
