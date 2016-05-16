using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibrary1;
using HtmlAgilityPack;
using System.Net;
using Microsoft.WindowsAzure.Storage.Queue;

namespace ClassLibrary1
{
    public class HTMLCrawler
    {
        public HTMLCrawler()
        {
            this.VisitedList = new HashSet<string>();
            this.PageBatch = new List<Page>();
            this.BatchID = Guid.NewGuid().ToString();
        }

        public HashSet<string> VisitedList { get; set; }
        public Dictionary<string, HashSet<string>> DisallowList { get; set; }
        public List<string> errors = new List<string>();
        public string BatchID { get; set; }
        public List<Page> PageBatch { get; set; }
        private static HtmlDocument HtmlDoc = new HtmlDocument();
        private static WebClient WebClient = new WebClient();
        private AzureStorageConnection Azure = new AzureStorageConnection();
        
        public void CrawlPage(string url)
        {
            Uri uri = new Uri(url);
            if (!DisallowList[getDomain(uri)].Contains("/" + uri.Segments[1].Remove(uri.Segments[1].Length - 1)) && !VisitedList.Contains(url))
            {
                VisitedList.Add(url);
                try
                {
                    var htmlpage = WebClient.DownloadString(url);
                    if (htmlpage.Contains("<!DOCTYPE html>")) //if its an html page
                    {
                        HtmlDoc.LoadHtml(htmlpage);
                        string pageTitle = HtmlDoc.DocumentNode.SelectSingleNode("//head/title").InnerText;
                        string pageDate = string.Empty;
                        foreach (HtmlNode meta in HtmlDoc.DocumentNode.SelectNodes("//head/meta"))
                        {
                            string name = meta.GetAttributeValue("name", string.Empty);
                            if (name == "lastmod")
                            {
                                pageDate = meta.GetAttributeValue("content", string.Empty);
                            }
                        }

                        foreach (HtmlNode link in HtmlDoc.DocumentNode.SelectNodes("//a[@href]"))
                        {
                            string hrefValue = link.GetAttributeValue("href", string.Empty);
                            if (hrefValue.StartsWith("/"))
                            {
                                hrefValue = uri.Host + hrefValue;
                            }
                            if (!VisitedList.Contains(hrefValue))
                            {
                                Azure.crawlQueue.AddMessageAsync(new CloudQueueMessage(hrefValue));
                            }
                        }
                        PageBatch.Add(new Page(pageTitle, pageDate, url, BatchID));
                    }
                }
                catch (Exception e)
                {
                    errors.Add(e.ToString() + " : " + url);
                }
            }
        }

        private string getDomain(Uri uri)
        {
            //gets the domain names to ignore the subdomain
            return uri.Host.Split('.')[uri.Host.Split('.').Length - 2];
        }
    }
}
