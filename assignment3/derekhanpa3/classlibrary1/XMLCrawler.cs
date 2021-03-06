﻿using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ClassLibrary1
{
    public class XMLCrawler
    {

        public XMLCrawler()
        {
            Azure = new AzureStorageConnection();
            SitemapQueue = new Queue<string>();
            DisallowList = new Dictionary<string, HashSet<string>>();
            this.VisitedList = new HashSet<string>();
            this.localQueue = new Queue<string>();
        }

        private AzureStorageConnection Azure;
        private Queue<string> SitemapQueue;
        public Dictionary<string,HashSet<string>> DisallowList { get; private set; }
        public Queue<string> localQueue { get; set; }
        public HashSet<string> VisitedList { get; set; }

        /// <summary>
        /// Crawls robots.txt with given url (eg http://ww.cnn.com)
        /// </summary>
        /// <param name="url"></param>
        public void CrawlRobots(string url)
        {
            WebClient webClient = new WebClient();
            string content = webClient.DownloadString(url + "/robots.txt");
            string[] lines = content.Split('\n');

            Uri uri = new Uri(url);
            DisallowList.Add(getDomain(uri), new HashSet<string>());

            string site = "Sitemap: ";
            string disallow = "Disallow: ";
            foreach(string line in lines)
            {
                if(line.StartsWith(site))
                {
                    Uri sitemapUri = new Uri(line.Substring(line.IndexOf(site) + site.Length));

                    //filters for NBA only if the host is bleacherreport
                    if ( !(sitemapUri.Host == "bleacherreport.com" && !sitemapUri.AbsolutePath.ToLower().Contains("nba")) )
                    {
                        SitemapQueue.Enqueue(sitemapUri.AbsoluteUri);
                    }
                } else if (line.StartsWith(disallow))
                {
                    DisallowList[getDomain(uri)].Add(line.Substring(line.IndexOf(disallow) + disallow.Length));
                }
            }
            crawlSitemaps();
        }

        /// <summary>
        /// Crawls the sitemaps
        /// </summary>
        private void crawlSitemaps()
        {
            //restriction date: march 1st
            DateTime restriction = Convert.ToDateTime(ConfigurationManager.AppSettings["RestrictionDate"]);

            while(SitemapQueue.Count != 0)
             {
                XmlDocument xml = new XmlDocument();
                xml.Load(SitemapQueue.Dequeue());
                foreach (XmlNode node in xml.DocumentElement.ChildNodes)
                {
                    if (node["lastmod"] != null)
                    {
                        if (Convert.ToDateTime(node["lastmod"].InnerText) > restriction)
                        {
                            addToQueue(node);
                        }
                    } else
                    {
                        addToQueue(node);
                    }
                }
            }
        }

        /// <summary>
        /// Adds found urls/xmls to appropriate queues
        /// </summary>
        /// <param name="node"></param>
        private void addToQueue(XmlNode node)
        {
            if (node.Name == "sitemap")
            {
                SitemapQueue.Enqueue(node["loc"].InnerText);
            }
            else
            {
                Azure.crawlQueue.AddMessageAsync(new CloudQueueMessage(node["loc"].InnerText));
                VisitedList.Add(node["loc"].InnerText);
            }
        }
       
        /// <summary>
        /// Gets the domain, exlcuding sub-domains
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        private string getDomain(Uri uri)
        {
            //gets the domain names to ignore the subdomain
            return uri.Host.Split('.')[uri.Host.Split('.').Length - 2];
        }
    
    }
}
