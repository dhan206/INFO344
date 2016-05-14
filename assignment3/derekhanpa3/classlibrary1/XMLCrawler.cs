using Microsoft.WindowsAzure.Storage.Queue;
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
        private AzureStorageConnection Azure;
        private Queue<string> SitemapQueue;
        public HashSet<string> DisallowList { get; private set; }

        public XMLCrawler()
        {
            Azure = new AzureStorageConnection();
            SitemapQueue = new Queue<string>();
            DisallowList = new HashSet<string>();
        }

        public void CrawlRobots(string url)
        {
            WebClient webClient = new WebClient();
            string content = webClient.DownloadString(url + "/robots.txt");
            string[] lines = content.Split('\n');
            string site = "Sitemap: ";
            string disallow = "Disallow: ";
            foreach(string line in lines)
            {
                if(line.StartsWith(site))
                {
                    SitemapQueue.Enqueue(line.Substring(line.IndexOf(site) + site.Length));
                } else if (line.StartsWith(disallow))
                {
                    DisallowList.Add(line.Substring(line.IndexOf(disallow) + disallow.Length));
                }
            }
            CrawlSitemaps();
        }

        private void CrawlSitemaps()
        {
            //restriction date: march 1st
            DateTime restriction = Convert.ToDateTime(ConfigurationManager.AppSettings["RestrictionDate"]);

            while(SitemapQueue.Count != 0)
             {
                XmlDocument xml = new XmlDocument();
                xml.Load(SitemapQueue.Dequeue());
                foreach (XmlNode node in xml.DocumentElement.ChildNodes)
                {
                    if (node["lastmod"] != null) //if date is specified
                    {
                        if (DateTime.Compare(restriction, Convert.ToDateTime(node["lastmod"].InnerText)) == -1)
                        {
                            AddToQueue(node);
                        }

                    } else
                    {
                        AddToQueue(node);
                    }
                }
            }
        }

        private void AddToQueue(XmlNode node)
        {
            if (node.Name == "sitemap")
            {
                SitemapQueue.Enqueue(node["loc"].InnerText);
            }
            else if (node.Name == "url")
            {
                Uri uri = new Uri(node["loc"].InnerText);
                if (!DisallowList.Contains("/" + uri.Segments[1].Remove(uri.Segments[1].Length - 1)))
                {
                      Azure.crawlQueue.AddMessage(new CloudQueueMessage(node["loc"].InnerText));
                }
            }
        }

    
    }
}
