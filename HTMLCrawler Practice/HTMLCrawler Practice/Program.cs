using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HTMLCrawler_Practice
{
    class Program
    {
        public static HtmlDocument doc = new HtmlDocument();
        public static WebClient webc = new WebClient();
        public static string[] urls = new string[] { "http://www.cnn.com/2016/05/14/us/air-show-crash-georgia-atlanta-area/index.html", "http://www.bleacherreport.com" };


        static void Main(string[] args)
        {
            foreach (string u in urls)
            {
                string content = webc.DownloadString(u);
                doc.LoadHtml(content);
                int count = 0;
                Console.WriteLine(doc.DocumentNode.SelectSingleNode("//head/title").InnerText);
                foreach(HtmlNode meta in doc.DocumentNode.SelectNodes("//head/meta"))
                {
                    string name = meta.GetAttributeValue("name", string.Empty);
                    if (name == "lastmod")
                    {
                        Console.WriteLine(meta.GetAttributeValue("content", string.Empty));
                    }
                }
                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//a[@href]"))
                {
                    string hrefValue = link.GetAttributeValue("href", string.Empty);
                    if (hrefValue.StartsWith("/"))
                    {
                        Uri uri = new Uri(u);
                        hrefValue = uri.Host + hrefValue;
                    }
                    Console.WriteLine(hrefValue);
                    count++;
                }
                Console.WriteLine("Count: " + count + "*************************************************************************************************************");
            }
            Console.WriteLine(DateTime.Compare(Convert.ToDateTime("2016-04-30T23:58:58-05:00"), Convert.ToDateTime("03/01/2016")));

            Console.Read();

        }
    }
}
