﻿using System.Collections.Generic;
using System.IO;
using System.Net;
using System;
using System.Text.RegularExpressions;
using System.Xml;

using CwIRC;
using MBFunctionInterface;
using MBUtilities;
using MBUtilities.Channel;

namespace Internet
{
    /// <summary>
    /// A Function which checks if RSS feeds have updated, and returns a link to the oldest new item if they have.
    /// </summary>
    public class RSSChecker : Function
    {
        public struct Feed
        {
            public string URL;
            public DateTime LastUpdate;

            public Feed(string url, DateTime lastUpdate)
            {
                URL = url;
                LastUpdate = lastUpdate;
            }
        }

        public static Dictionary<string, Feed> FeedMap = new Dictionary<string, Feed>();

        public RSSChecker()
        {
            Help = "Automatic function, scans RSS feeds and reports new items in the channel.";
            Type = Types.Regex;
            AccessLevel = AccessLevels.Anyone;

            Feed homestuck = new Feed();
            homestuck.URL = "http://www.mspaintadventures.com/rss/rss.xml";
            homestuck.LastUpdate = DateTime.Now;
            FeedMap.Add("Homestuck", homestuck);

            //LoadFeeds(Settings.Instance.Server + ".Feeds.xml");
        }

        ~RSSChecker()
        {
            //SaveFeeds(Settings.Instance.Server + ".Feeds.xml");
        }

        public override List<IRCResponse> GetResponse(BotMessage message)
        {
            //foreach (KeyValuePair<string, Feed> feed in FeedMap)
            //{


            URL.WebPage feedPage;
            try
            {
                feedPage = URL.FetchURL(FeedMap["Homestuck"].URL);
            }
            catch (System.Exception ex)
            {
                Logger.Write(ex.Message, Settings.Instance.ErrorFile);
                return null;
            }

            XmlDocument feedDoc = new XmlDocument();

            feedDoc.LoadXml(feedPage.Page);

            XmlNode firstItem = feedDoc.SelectSingleNode(@"/rss/channel/item");

            DateTime newestDate = new DateTime();
            DateTime.TryParse(firstItem.SelectSingleNode("pubDate").FirstChild.Value, out newestDate);

            if (newestDate > FeedMap["Homestuck"].LastUpdate)
            {
                XmlNode oldestNew = feedDoc.SelectSingleNode(@"/rss/channel/item");

                int numUpdates = 0;

                foreach (XmlNode item in feedDoc.SelectNodes(@"/rss/channel/item"))
                {
                    DateTime itemDate = new DateTime();
                    DateTime.TryParse(item.SelectSingleNode("pubDate").FirstChild.Value, out itemDate);

                    if (itemDate > FeedMap["Homestuck"].LastUpdate)
                    {
                        oldestNew = item;
                        numUpdates++;
                    }
                    else
                    {
                        break;
                    }
                }

                FeedMap["Homestuck"] = new Feed(FeedMap["Homestuck"].URL, newestDate);

                string itemTitle = oldestNew.SelectSingleNode("title").FirstChild.Value;
                string itemLink = ChannelList.EvadeChannelLinkBlock(message, oldestNew.SelectSingleNode("link").FirstChild.Value);

                return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "Homestuck has updated, " + numUpdates + " new pages! New ones start here: " + itemTitle + " (" + itemLink + ")", message.ReplyTo) };
            }
            //}
            return null;
        }

        //public void SaveFeeds(string fileName)
        //{
        //    XmlWriterSettings xws = new XmlWriterSettings();
        //    xws.Indent = true;
        //    xws.NewLineOnAttributes = true;
        //    using (XmlWriter writer = XmlWriter.Create(fileName, xws))
        //    {
        //        writer.WriteStartDocument();
        //        writer.WriteStartElement("Links");

        //        foreach (KeyValuePair<string, string> link in AccountMap)
        //        {
        //            writer.WriteStartElement("Link");

        //            writer.WriteElementString("IRCName", link.Key);
        //            writer.WriteElementString("LastFMName", link.Value);

        //            writer.WriteEndElement();
        //        }

        //        writer.WriteEndElement();
        //        writer.WriteEndDocument();
        //    }
        //}

        //public void LoadFeeds(string fileName)
        //{
        //    if (!File.Exists(fileName))
        //        return;

        //    XmlDocument doc = new XmlDocument();
        //    doc.Load(new StreamReader(File.OpenRead(fileName)));
        //    XmlNode root = doc.DocumentElement;

        //    foreach (XmlNode linkNode in root.SelectNodes(@"/Links/Link"))
        //    {
        //        string IRCName = linkNode.SelectSingleNode("IRCName").FirstChild.Value;
        //        string LastFMName = linkNode.SelectSingleNode("LastFMName").FirstChild.Value;

        //        AccountMap.Add(IRCName, LastFMName);
        //    }
        //}
    }
}
