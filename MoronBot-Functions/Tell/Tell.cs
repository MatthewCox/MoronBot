﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

using CwIRC;
using MBFunctionInterface;
using MBUtilities;

namespace Utility
{
    /// <summary>
    /// A Function which allows users to send messages to each other, even when the intended recipient is offline.
    /// </summary>
    public class Tell : Function
    {
        public struct TellMessage
        {
            public string From;
            public string SentDate;
            public string Message;
        }

        struct UserDateNumber
        {
            public string User { get; set; }
            public DateTime MessageTime { get; set; }
            public uint Number { get; set; }
        }

        public static Dictionary<string, List<TellMessage>> MessageMap = new Dictionary<string, List<TellMessage>>();

        List<UserDateNumber> userDateNumberList = new List<UserDateNumber>();

        public Tell()
        {
            Help = "tell <user> <message> - Tells the specified user the specified message, when they next speak.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;

            ReadMessages();
        }

        ~Tell()
        {
            WriteMessages();
        }

        public override List<IRCResponse> GetResponse(BotMessage message)
        {
            if (Regex.IsMatch(message.Command, "^(tell)$", RegexOptions.IgnoreCase))
            {
                int i = userDateNumberList.FindIndex(s => s.User == message.User.Name);

                if (i < 0)
                {
                    userDateNumberList.Add(new UserDateNumber() { User = message.User.Name, MessageTime = DateTime.Now, Number = 0 });
                    i = userDateNumberList.FindIndex(s => s.User == message.User.Name);
                }

                UserDateNumber userDateNumber = userDateNumberList[i];

                if (userDateNumber.MessageTime.AddMinutes(5).CompareTo(DateTime.Now) <= 0)
                {
                    userDateNumber.MessageTime = DateTime.Now;
                    userDateNumber.Number = 0;
                }
                else
                {
                    if (userDateNumber.Number < 3)
                    {
                        userDateNumber.MessageTime = DateTime.Now;
                        userDateNumber.Number++;
                        userDateNumberList[i] = userDateNumber;
                    }
                    else
                    {
                        return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "You've already sent 3 messages in the last 5 minutes, slow down!", message.ReplyTo) };
                    }
                }

                if (message.ParameterList.Count > 1)
                {
                    string to = WildcardToRegex(message.ParameterList[0]);
                    string msg = message.Parameters.Substring(message.ParameterList[0].Length + 1);
                    if (msg.Trim(' ').Length > 0)
                    {
                        if (!MessageMap.ContainsKey(to))
                        {
                            MessageMap.Add(to, new List<TellMessage>());
                        }
                        TellMessage tellMessage = new TellMessage();
                        tellMessage.From = message.User.Name;
                        tellMessage.SentDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss (UTC zz)");
                        tellMessage.Message = StringUtils.ReplaceNewlines(StringUtils.StripIRCFormatChars(msg), " | ");
                        MessageMap[to].Add(tellMessage);
                        WriteMessages();
                        return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "Ok, I'll tell " + message.ParameterList[0] + " that when they next speak.", message.ReplyTo) };
                    }
                }

                if (message.ParameterList.Count > 0)
                {
                    return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "You didn't give a message for me to tell " + message.ParameterList[0], message.ReplyTo) };
                }
                return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "Tell who what?", message.ReplyTo) };
            }
            return null;
        }

        public static void WriteMessages()
        {
            string fileName = Path.Combine(Settings.Instance.DataPath, Settings.Instance.Server + string.Format("{0}TellMessages.xml", Path.DirectorySeparatorChar));

            FileUtils.CreateDirIfNotExists(fileName);

            XmlWriterSettings xws = new XmlWriterSettings();
            xws.Indent = true;
            xws.NewLineOnAttributes = true;
            using (XmlWriter writer = XmlWriter.Create(fileName, xws))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Users");

                foreach (KeyValuePair<string, List<TellMessage>> userMessages in MessageMap)
                {
                    writer.WriteStartElement("User");
                    writer.WriteElementString("Name", userMessages.Key);

                    writer.WriteStartElement("Messages");

                    foreach (TellMessage tellMessage in userMessages.Value)
                    {
                        writer.WriteStartElement("Message");

                        writer.WriteElementString("Text", tellMessage.Message);
                        writer.WriteElementString("From", tellMessage.From);
                        writer.WriteElementString("Date", tellMessage.SentDate);

                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement();
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        void ReadMessages()
        {
            string fileName = Path.Combine(Settings.Instance.DataPath, Settings.Instance.Server + string.Format("{0}TellMessages.xml", Path.DirectorySeparatorChar));

            if (!File.Exists(fileName))
                return;

            XmlDocument doc = new XmlDocument();
            doc.Load(new StreamReader(File.OpenRead(fileName)));
            XmlNode root = doc.DocumentElement;

            foreach (XmlNode userNode in root.SelectNodes(@"/Users/User"))
            {
                string user = userNode.SelectSingleNode("Name").FirstChild.Value;

                List<TellMessage> tellMessages = new List<TellMessage>();

                foreach (XmlNode messageNode in userNode.SelectNodes(@"Messages/Message"))
                {
                    TellMessage tellMessage = new TellMessage();
                    tellMessage.Message = messageNode.SelectSingleNode("Text").FirstChild.Value;
                    tellMessage.From = messageNode.SelectSingleNode("From").FirstChild.Value;
                    tellMessage.SentDate = messageNode.SelectSingleNode("Date").FirstChild.Value;

                    tellMessages.Add(tellMessage);
                }

                MessageMap.Add(user, tellMessages);
            }
        }

        public static string WildcardToRegex(string pattern)
        {
            return "^" + Regex.Escape(pattern).
                Replace("\\*", ".*").
                Replace("\\?", ".").
                Replace("\\(", "(").
                Replace("\\)", ")").
                Replace("/", "|") + "$";
        }
    }
}
