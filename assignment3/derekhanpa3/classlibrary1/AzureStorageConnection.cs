using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class AzureStorageConnection
    {
        private static CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
        private static CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
        private static CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
        public CloudQueue commandQueue = queueClient.GetQueueReference("commandqueuepa3");
        public CloudQueue crawlQueue = queueClient.GetQueueReference("crawlqueuepa3");
        public CloudTable pageTable = tableClient.GetTableReference("pagetablepa3");
        public CloudTable dashboardTable = tableClient.GetTableReference("dashboardtablepa3");
        public ServicePoint tableServicePoint = ServicePointManager.FindServicePoint(storageAccount.TableEndpoint);
        public ServicePoint queueServicePoint = ServicePointManager.FindServicePoint(storageAccount.QueueEndpoint);


        public AzureStorageConnection()
        {
            commandQueue.CreateIfNotExists();
            crawlQueue.CreateIfNotExists();
            pageTable.CreateIfNotExists();
            dashboardTable.CreateIfNotExists();
            tableServicePoint.UseNagleAlgorithm = false;
            queueServicePoint.UseNagleAlgorithm = false;
        }
    }
}

