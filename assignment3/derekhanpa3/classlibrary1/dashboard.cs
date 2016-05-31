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
        public int? Crawled { get; set; }           //#URLs Crawled
        public string LastTen { get; set; }         //Last 10 URLs crawled
        public int SizeQueue { get; set; }          //Size of queue
        public int? SizeIndex { get; set; }         //Size of index
        public string Errors { get; set; }          //Errors
        public int? NumberOfTitles { get; set; }    //Number of titles in Trie (for query suggestion)
        public string LastTitle { get; set; }       //Last title inserted into Trie
        private AzureStorageConnection Azure = new AzureStorageConnection();

        /// <summary>
        /// Update dashboard with new stats after table insertion
        /// </summary>
        /// <param name="crawling">is the web crawler crawling?</param>
        /// <param name="initialized">has it been initialized?</param>
        /// <param name="crawled">how many pages has it crawled?</param>
        /// <param name="lastTen">what are the last ten urls crawled? comma seperated</param>
        /// <param name="errors">what are the errors that came up? comma seperated</param>
        public void updateDashboardNewStats(bool crawling, bool initialized, int crawled, string lastTen, string errors, int addedToTable)
        {
            PerformanceCounter ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            float ramUsage = ramCounter.NextValue();
            CloudQueue temp = Azure.crawlQueue;
            temp.FetchAttributes();

            TableQuery<Dashboard> getDashboard = new TableQuery<Dashboard>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "OG"));
            Dashboard currentDashboard = Azure.dashboardTable.ExecuteQuery(getDashboard).ElementAt(0);
            string state = string.Empty;
            if (crawling && !initialized)
            {
                state = "Initializing";
            }
            else if (crawling && initialized)
            {
                state = "Crawling";
            }
            else
            {
                state = "Idling";
                lastTen = currentDashboard.LastTen;
            }

            Dashboard newDashboard = new Dashboard()
            {
                State = state,
                Crawled = currentDashboard.Crawled + crawled,
                LastTen = lastTen,
                SizeIndex = currentDashboard.SizeIndex + addedToTable,
                Errors = errors,
                ETag = "*"
            };
            Azure.dashboardTable.Execute(TableOperation.Merge(newDashboard));
        }

        /// <summary>
        /// Refreshes the dashboard, used to start first dashboard table entity
        /// </summary>
        public void refreshDashboard()
        {
            PerformanceCounter ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            float ramUsage = ramCounter.NextValue();
            CloudQueue temp = Azure.crawlQueue;
            temp.FetchAttributes();

            if (Azure.dashboardTable.ExecuteQuery(new TableQuery()).Count() == 0)
            {
                Dashboard newDashboard = new Dashboard()
                {
                    State = "Ready to start crawling",
                    CPU = (int)getCPU(),
                    RAM = (int)ramUsage,
                    Crawled = 0,
                    LastTen = string.Empty,
                    SizeQueue = (int)temp.ApproximateMessageCount,
                    SizeIndex = 0,
                    Errors = string.Empty,
                    NumberOfTitles = 0,
                    LastTitle = string.Empty
                };
                Azure.dashboardTable.Execute(TableOperation.InsertOrReplace(newDashboard));
            }
            else
            {
                Dashboard newDashboard = new Dashboard() {
                    CPU = (int)getCPU(),
                    RAM = (int)ramUsage,
                    SizeQueue = (int)temp.ApproximateMessageCount,
                    ETag = "*"
                };
                Azure.dashboardTable.Execute(TableOperation.Merge(newDashboard));
            }
        }

        /// <summary>
        /// gets CPU used
        /// </summary>
        /// <returns>float of CPU Utilization</returns>
        private float getCPU()
        {
            PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            float cpuUsage = cpuCounter.NextValue();
            float totalCPUCount = 0;
            for (int i = 0; i < 10; i++)
            {
                totalCPUCount += cpuCounter.NextValue();
                System.Threading.Thread.Sleep(100);
            }
            return totalCPUCount / 10;
        }
    }
}