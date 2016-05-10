using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace WebRole1
{
    /// <summary>
    /// Summary description for Admin
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Admin : System.Web.Services.WebService
    { 

        [WebMethod]
        public bool StartCrawling()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference("derekhanpa3queue");
            queue.CreateIfNotExists();

            CloudQueueMessage messageCNN = new CloudQueueMessage("http://www.cnn.com");
            CloudQueueMessage messageBR = new CloudQueueMessage("http://www.bleacherreport.com");
            queue.AddMessage(messageCNN);
            queue.AddMessage(messageBR);
            return true;
        }

        [WebMethod]
        public bool StopCrawling()
        {

            return true;
        }

        [WebMethod]
        public bool ClearIndex()
        {

            return true;
        }

        [WebMethod]
        public string GetPageTitle()
        {
            string title = "";
            return title;
        }
    }
}
