﻿using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

using CwIRC;
using MBFunctionInterface;
using MBUtilities;

namespace Internet
{
    /// <summary>
    /// A Function which returns the english translation of a given sentence from Google Translate.
    /// No longer works because Google restricted access to the translation API.
    /// </summary>
    public class Translate : Function
    {
        public Translate()
        {
            Help = "translate <sentence> - Translates the given sentence to English.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;
        }

        public override List<IRCResponse> GetResponse(BotMessage message)
        {
            if (Regex.IsMatch(message.Command, "^(translate)$", RegexOptions.IgnoreCase))
            {
                if (message.ParameterList.Count > 0)
                {
                    string translatedString;
                    try
                    {
                        string translateTerm = message.Parameters.Replace('"', ' ');
                        translatedString = Gapi.Language.Translator.Translate(translateTerm, Gapi.Language.Language.English);
                    }
                    catch (Gapi.Core.GapiException ex)
                    {
                        Logger.Write(ex.ToString(), Settings.Instance.ErrorFile);
                        translatedString = "Couldn't work out what language you're using.";
                    }
                    return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, translatedString, message.ReplyTo) };
                }
                else
                {
                    return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "Translate what?", message.ReplyTo) };
                }
            }
            else
            {
                return null;
            }
        }
    }
}
