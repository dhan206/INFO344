using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace WebRole1
{
    public class Dashboard : TableEntity
    {
        public Dashboard()
        {
            this.PartitionKey = "OG";
            this.RowKey = "OG";
        }

        public string State { get; set; }           //State of web role (initializing, crawling, or idle)
        public int CPU { get; set; }                //Machine CountersL CPU Utilization%
        public int RAM { get; set; }                //Machine Counters: RAM
        public int? Crawled { get; set; }           //#URLs Crawled
        public string LastTen { get; set; }         //Last 10 URLs crawled
        public int SizeQueue { get; set; }          //Size of queue
        public int? SizeIndex { get; set; }         //Size of index
        public string Errors { get; set; }          //Errors
        public int? NumberOfTitles { get; set; }    //Number of titles in Trie (for query suggestion)
        public string LastTitle { get; set; }       //Last title inserted into Trie

        public void updateTitleStats(int count, string lastWord)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable dashboardTable = tableClient.GetTableReference("dashboardtablepa4");
            dashboardTable.CreateIfNotExists();

            Dashboard updateTitlesDashboard = new Dashboard()
            {
                NumberOfTitles = count,
                LastTitle = lastWord,
                ETag = "*"
            };
            dashboardTable.Execute(TableOperation.Merge(updateTitlesDashboard));
        }
    }
}