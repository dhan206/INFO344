using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using System.Configuration;
using ClassLibrary1;

namespace WorkerRole1
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        public override void Run()
        {
            AzureStorageConnection Azure = new AzureStorageConnection();
            HTMLCrawler HtmlCrawler = new HTMLCrawler();
            Dashboard Dashboard = new Dashboard();

            bool crawling = false;
            bool initialized = false;
            while (true)
            {
                CloudQueueMessage commandMessage = Azure.commandQueue.GetMessage();
                if (commandMessage != null)
                {
                    string command = commandMessage.AsString;
                    crawling = (!command.StartsWith("stop"));
                    if (crawling)
                    {
                        Dashboard.updateDashboard(crawling, initialized, 0, string.Empty, string.Empty);
                        string[] urls = command.Replace(",", "").Split(' ');
                        XMLCrawler xmlCrawler = new XMLCrawler();
                        Stopwatch sw = new Stopwatch();
                        sw.Start();
                        //xmlCrawler.CrawlRobots(urls[1]); //crawl cnn
                        xmlCrawler.CrawlRobots(urls[2]); //crawl bleacherreport
                        sw.Stop();
                        var elapsed = sw.Elapsed.ToString();
                        HtmlCrawler.DisallowList = xmlCrawler.DisallowList;
                        initialized = true;
                    }
                    if (!command.Equals("stop and clear"))
                    {
                        Dashboard.updateDashboard(crawling, initialized, 0, string.Empty, string.Empty);
                    }
                    Azure.commandQueue.DeleteMessage(commandMessage);
                }
                if (crawling)
                {
                    CloudQueueMessage crawlMessage = Azure.crawlQueue.GetMessage();
                    if (crawlMessage != null)
                    {
                        HtmlCrawler.CrawlPage(crawlMessage.AsString);
                    }

                    // Adds to the table 10 at a time
                    if(HtmlCrawler.PageBatch.Count == 10)
                    {
                        TableBatchOperation batchOperation = new TableBatchOperation();
                        List<string> lastTenURLS = new List<string>();
                        foreach(Page page in HtmlCrawler.PageBatch)
                        {
                            batchOperation.InsertOrReplace(page);
                            lastTenURLS.Add(page.URL);
                        }
                        Azure.pageTable.ExecuteBatch(batchOperation);
                        Dashboard.updateDashboard(crawling, initialized, HtmlCrawler.PagesCrawled, string.Join(",", lastTenURLS.ToArray()), string.Join(",", HtmlCrawler.Errors.ToArray()));
                        HtmlCrawler.ResetBatch();
                    }
                    Thread.Sleep(100);
                    Azure.crawlQueue.DeleteMessage(crawlMessage);
                }
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // Speed up things
            ServicePointManager.UseNagleAlgorithm = false;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            Trace.TraceInformation("WorkerRole1 has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("WorkerRole1 is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("WorkerRole1 has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following with your own logic.
            while (!cancellationToken.IsCancellationRequested)
            {
                Trace.TraceInformation("Working");
                await Task.Delay(1000);
            }
        }
    }
}
