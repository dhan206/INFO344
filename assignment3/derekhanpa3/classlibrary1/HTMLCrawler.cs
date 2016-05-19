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
            this.PagesCrawled = 0;
        }

        public HashSet<string> VisitedList { get; set; }
        public Dictionary<string, HashSet<string>> DisallowList { get; set; }
        public List<string> Errors = new List<string>();
        public string BatchID { get; set; }
        public List<Page> PageBatch { get; set; }
        public int PagesCrawled { get; set; }

        private static HtmlDocument HtmlDoc = new HtmlDocument();
        private static WebClient WebClient = new WebClient();
        private AzureStorageConnection Azure = new AzureStorageConnection();
        
        /// <summary>
        /// Crawls the page to retrieve urls, title, and date
        /// </summary>
        /// <param name="url"></param>
        public void CrawlPage(string url)
        {
            if (!url.Contains("http")) {
                url = "http://" + url;
            }
            try
            {
                Uri uri = new Uri(url);
                if (!VisitedList.Contains(url))
                {
                    this.PagesCrawled++;
                    VisitedList.Add(url);
                    if (DisallowList.ContainsKey(getDomain(uri)))
                    {
                        if (uri.Segments.Count() == 1 || !DisallowList[getDomain(uri)].Contains("/" + uri.Segments[1].Remove(uri.Segments[1].Length - 1)))
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
                                        break;
                                    }
                                }
                                foreach (HtmlNode link in HtmlDoc.DocumentNode.SelectNodes("//a[@href]"))
                                {
                                    string hrefValue = link.GetAttributeValue("href", string.Empty);
                                    if (hrefValue.StartsWith("/") || !hrefValue.Contains("http") || !hrefValue.Contains(".com"))
                                    {
                                        hrefValue = uri.Host + hrefValue;
                                    }
                                    if (!VisitedList.Contains(hrefValue) && !hrefValue.Contains("javascript:void"))
                                    {
                                        Azure.crawlQueue.AddMessageAsync(new CloudQueueMessage(hrefValue));
                                    }
                                }
                                PageBatch.Add(new Page(pageTitle, pageDate, url, BatchID));
                            }
                        }
                    }
                }
            } catch (Exception e)
            {
                Errors.Add(e.Message + "*" + url);
            }   
        }

        /// <summary>
        /// Resets local variables after each batch insertion
        /// </summary>
        public void ResetBatch()
        {
            this.PagesCrawled = 0;
            this.PageBatch.Clear();
            this.Errors.Clear();
            this.BatchID = Guid.NewGuid().ToString();
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
