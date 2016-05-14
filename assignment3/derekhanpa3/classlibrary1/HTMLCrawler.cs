using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class HTMLCrawler
    {
        public HTMLCrawler()
        {
            this.VisitedLinks = new HashSet<string>();
            this.DisallowLinks = new HashSet<string>();
        }

        public HashSet<string> VisitedLinks { get; set; }
        public HashSet<string> DisallowLinks { get; set; }

        public string Crawl(string url)
        {
            return "123";
        }

    }
}
