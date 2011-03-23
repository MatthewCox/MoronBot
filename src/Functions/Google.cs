﻿using System.Text.RegularExpressions;

using CwIRC;

using Gapi;

namespace MoronBot.Functions
{
    class Google : Function
    {
        public Google(MoronBot moronBot)
        {
            Name = GetName(); ;
            Help = "google/search/find <terms>\t- Gives you the first google search result for a given search term.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;
        }
        
        public override void GetResponse(BotMessage message, MoronBot moronBot)
        {
            if (Regex.IsMatch(message.Command, "^(google|search|find)$", RegexOptions.IgnoreCase))
            {
                if (message.ParameterList.Count > 0)
                {
                    string searchTerm = message.Parameters.Replace("\"", "%22");
                    Gapi.Search.SearchResults searchResults;
                    Gapi.Search.SearchResult searchResult;
                    try
                    {
                        searchResults = Gapi.Search.Searcher.Search(Gapi.Search.SearchType.Web, searchTerm);
                        //Gapi.Search.SearchSafety.Active;
                        //Gapi.Search.S
                        if (searchResults.Items.Length == 0)
                        {
                            moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Say, "No results found for \"" + message.Parameters + "\"!", message.ReplyTo));
                            return;
                        }

                        searchResult = searchResults.Items[0];
                    }
                    catch (System.Exception ex)
                    {
                        moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Say, ex.GetType().ToString() + " | " + ex.Message, message.ReplyTo));
                        return;
                    }
                    string searchContent = Regex.Replace(searchResult.Content, @"<.+?>", "");
                    moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Say, searchContent + " (" + searchResult.Url + ")", message.ReplyTo));
                    return;
                }
                else
                {
                    moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Say, "Search for what?", message.ReplyTo));
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
