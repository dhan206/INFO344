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
        public Page(string title, DateTime date, string URL)
        {
            this.PartitionKey = title;
            this.RowKey = Guid.NewGuid().ToString();

            this.Title = title;
            this.Date = date;
            this.URL = URL;
        }

        public Page() { }

        public string Title { get; set; }
        public DateTime Date { get; set; }
        public string URL { get; set; }
    }
}
