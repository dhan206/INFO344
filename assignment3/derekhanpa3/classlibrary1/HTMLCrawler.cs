using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibrary1;
using HtmlAgilityPack;
using System.Net;

namespace ClassLibrary1
{
    public class HTMLCrawler
    {
        public HTMLCrawler()
        {
            this.VisitedList = new HashSet<string>();
        }

        public HashSet<string> VisitedList { get; set; }
        public Dictionary<string, HashSet<string>> DisallowList { get; set; }
        public List<string> errors = new List<string>();
        private static HtmlDocument htmlDoc = new HtmlDocument();
        private static WebClient webClient = new WebClient();

        public void CrawlPage(string url)
        {
            try
            {
                htmlDoc.LoadHtml(webClient.DownloadString(url + "/index.html"));
                string pageTitle = htmlDoc.DocumentNode.SelectSingleNode("//head/title").InnerText;
                string pageDate = htmlDoc.DocumentNode.SelectSingleNode("//").InnerText;
            } catch (Exception e)
            {
                errors.Add(e.ToString());
            }
        }


    }
}
