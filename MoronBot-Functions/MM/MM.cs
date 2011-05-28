﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

using CwIRC;
using MBFunctionInterface;

using MBUtilities;

namespace Fun
{
    public class MM : Function
    {
        Random rand = new Random();
        List<string> mmList = new List<string>();

        public MM()
        {
            Help = "MM (<number>) - Returns a random \"Thing Players can no longer do in an MM game DM'd by Xela'\", or a specific one if you add a number.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;

            // Read mm.txt into mmList
            // I copied th  public class from Welch.cs
            StreamReader mmFile = new StreamReader(File.OpenRead(Path.Combine(Settings.Instance.DataPath, "mm.txt")));

            while (!mmFile.EndOfStream)
            {
                mmList.Add(mmFile.ReadLine());
            }

            mmFile.Close();
        }

        ~MM()
        {
            StreamWriter mmFile = new StreamWriter(File.OpenWrite(Path.Combine(Settings.Instance.DataPath, "mm.txt")));
            foreach (string s in mmList)
            {
                mmFile.Write(s + "\n");
            }
            mmFile.Close();
        }

        public override List<IRCResponse> GetResponse(BotMessage message)
        {
            if (Regex.IsMatch(message.Command, "^(mm)$", RegexOptions.IgnoreCase))
            {
                // Specific thing requested
                if (message.ParameterList.Count > 0)
                {
                    if (message.ParameterList[0].ToLower() == "add") // Adding something to the MM list
                    {
                        string msg = message.Parameters.Substring(message.ParameterList[0].Length + 1);
                        int index = mmList.Count + 1;
                        mmList.Add(index + ". " + msg);
                        return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "Message added at index " + index, message.ReplyTo) };
                    }
                    else if (message.ParameterList[0] == "list") // Post the list to pastebin, give link
                    {
                        string list = "";
                        foreach (string item in mmList)
                        {
                            list += item + "\n";
                        }
                        string url = URL.Pastebin(list, "M&M List", "10M", "text", "1");
                        return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "M&M list posted: " + url + " (link expires in 10 mins)", message.ReplyTo) };
                    }
                    else
                    {
                        int number = 0;
                        if (Int32.TryParse(message.ParameterList[0], out number))
                        {
                            number -= 1;
                            if (number >= 0 && number < mmList.Count)
                            {
                                return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, mmList[number], message.ReplyTo) };
                            }
                        }
                        // Number too large or small, or not a number at all
                        return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "Invalid number, range is 1-" + mmList.Count, message.ReplyTo) };
                    }
                }
                // No specific thing requested
                else
                {
                    // Return a random thing
                    return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, mmList[rand.Next(mmList.Count)], message.ReplyTo) };
                }
            }
            else
            {
                return null;
            }
        }
    }
}