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

            bool continute = false;
            while (true)
            {
                CloudQueueMessage getCommandMessage = Azure.commandQueue.GetMessage();
                //CloudQueueMessage getCrawlMessage = Azure.crawlQueue.GetMessage();
                if (getCommandMessage != null)
                {
                    string command = getCommandMessage.AsString;
                    continute = (command != "stop"); //either start or stop
                    string[] urls = command.Replace(",", "").Split(' ');
                    XMLCrawler cnnCrawler = new XMLCrawler();
                    cnnCrawler.CrawlRobots(urls[1]);
                    XMLCrawler brCrawler = new XMLCrawler();
                    brCrawler.CrawlRobots(urls[2]);
                    Azure.commandQueue.DeleteMessage(getCommandMessage);
                }
                //if (continute)
                //{ 
                //    if (getCrawlMessage != null)
                //    {

                //    }
                //    Thread.Sleep(50);
                //    Azure.crawlQueue.DeleteMessage(getCrawlMessage);
                //}
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

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
