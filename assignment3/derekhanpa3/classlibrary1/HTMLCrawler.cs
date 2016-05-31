using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibrary1;
using HtmlAgilityPack;
using System.Net;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using System.Text.RegularExpressions;

namespace ClassLibrary1
{
    public class HTMLCrawler
    {
        public HTMLCrawler()
        {
            this.VisitedList = new HashSet<string>();
            this.PagesCrawled = 0;
            this.Errors = new Queue<string>();
            this.LastTen = new Queue<string>();
        }

        public HashSet<string> VisitedList { get; set; }
        public Dictionary<string, HashSet<string>> DisallowList { get; set; }
        public Queue<string> Errors { get; set; }
        public Queue<string> LastTen { get; set; }
        public int PagesCrawled { get; set; }
        public int AddedToTable { get; set; }

        private static HtmlDocument HtmlDoc = new HtmlDocument();
        private static WebClient WebClient = new WebClient();
        private AzureStorageConnection Azure = new AzureStorageConnection();

        /// <summary>
        /// Crawls the page to retrieve urls, title, and date
        /// </summary>
        /// <param name="url"></param>
        public void CrawlPage(string url)
        {
            this.AddedToTable = 0;
            this.PagesCrawled = 0;
            if (!url.Contains("http"))
            {
                url = "http://" + url;
            }
            Uri uri = new Uri(url);
            if (DisallowList.ContainsKey(getDomain(uri)))
            {
                if (uri.Segments.Count() == 1 || !DisallowList[getDomain(uri)].Contains("/" + uri.Segments[1].Remove(uri.Segments[1].Length - 1)))
                {
                    //HttpRequest create = new HttpRequest(url); 
                    var htmlpage = WebClient.DownloadString(url);
                    if (htmlpage.Contains("<!DOCTYPE html>")) //if its an html page
                    {
                        this.PagesCrawled = 1;
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
                            if (hrefValue.StartsWith("/") || !hrefValue.Contains("http") && !hrefValue.Contains(".com"))
                            {
                                hrefValue = uri.Host + hrefValue;
                            }
                            if (!VisitedList.Contains(hrefValue) && !hrefValue.Contains("javascript:"))
                            {
                                Azure.crawlQueue.AddMessageAsync(new CloudQueueMessage(hrefValue));
                                VisitedList.Add(hrefValue);
                            }
                        }

                        if (LastTen.Count == 10)
                        {
                            LastTen.Dequeue();
                        }
                        LastTen.Enqueue(url);
                        string cleanTitle = Regex.Replace(pageTitle, @"[^0-9a-zA-Z\s]+", string.Empty);
                        foreach (string word in cleanTitle.Split(' '))
                        {
                            if (word != "") {
                                TableOperation insertPage = TableOperation.InsertOrReplace(new Page(word.ToLower(), pageTitle, pageDate, url));
                                Azure.pageTable.Execute(insertPage);
                                AddedToTable++;
                            }
                        }
                    }
                }
            }
        }

        public void AddError(string error)
        {
            if (this.Errors.Count == 50)
            {
                Errors.Dequeue();
            }
            Errors.Enqueue(error);
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
