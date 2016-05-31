using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class Page : TableEntity
    {
        public Page(string word, string title, string date, string url)
        {
            this.PartitionKey = word;
            this.RowKey = Uri.EscapeDataString(url);

            this.Title = title;
            this.Date = date;
            this.URL = url;
        }

        public Page() { }

        public string Title { get; set; }
        public string Date { get; set; }
        public string URL { get; set; }
    }
}
