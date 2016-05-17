using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
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
        public int Crawled { get; set; }            //#URLs Crawled
        public string LastTen { get; set; }         //Last 10 URLs crawled
        public int SizeQueue { get; set; }          //Size of queue
        public int SizeIndex { get; set; }          //Size of index
        public string Errors { get; set; }          //Errors
        private AzureStorageConnection Azure = new AzureStorageConnection();


        public void updateDashboard(bool crawling, bool initialized, int crawled, string lastTen, string errors)
        {
            TableQuery<Dashboard> getDashboard = new TableQuery<Dashboard>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "OG"));
            Dashboard currentDashboard = Azure.dashboardTable.ExecuteQuery(getDashboard).ElementAt(0);
            PerformanceCounter ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            float ramUsage = ramCounter.NextValue();
            float cpuUsage = cpuCounter.NextValue();
            CloudQueue temp = Azure.crawlQueue;
            temp.FetchAttributes();

            string state = string.Empty;
            if (crawling && !initialized)
            {
                state = "Initializing";
            } else if (crawling && initialized)
            {
                state = "Crawling";
            } else
            {
                state = "Idle";
            }

            Dashboard newDashboard = new Dashboard() {
                State = state,
                CPU = (int)cpuUsage,
                RAM = (int)ramUsage,
                Crawled = currentDashboard.Crawled + crawled,
                LastTen = lastTen,
                SizeQueue = (int)temp.ApproximateMessageCount,
                SizeIndex = Azure.pageTable.ExecuteQuery(new TableQuery()).Count(),
                Errors = errors + "," + currentDashboard.Errors
            };
            Azure.dashboardTable.Execute(TableOperation.InsertOrReplace(newDashboard));
        }

        public void loadFirstDashboard()
        {
            Dashboard firstDashboard = new Dashboard()
            {
                State = "First",
                CPU = 0,
                RAM = 0,
                Crawled = 0,
                LastTen = string.Empty,
                SizeQueue = 0,
                SizeIndex = 0,
                Errors = string.Empty
            };
            Azure.dashboardTable.Execute(TableOperation.InsertOrReplace(firstDashboard));
        }
    }
}