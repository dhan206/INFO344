using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class AzureStorageConnection
    {
        private static CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
        private static CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
        private static CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
        public CloudQueue commandQueue = queueClient.GetQueueReference("commandqueue");
        public CloudQueue crawlQueue = queueClient.GetQueueReference("crawlqueue");
        public CloudTable urlTable = tableClient.GetTableReference("urltable");
        public CloudTable dashboardTable = tableClient.GetTableReference("dashboardtable");

        public AzureStorageConnection()
        {
            commandQueue.CreateIfNotExists();
            crawlQueue.CreateIfNotExists();
            urlTable.CreateIfNotExists();
            dashboardTable.CreateIfNotExists();
        }
    }
}

