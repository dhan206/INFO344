using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Services;
using ClassLibrary1;

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
        public static AzureStorageConnection Azure = new AzureStorageConnection();

        [WebMethod]
        public string StartCrawling()
        {
            CloudQueueMessage messageStart = new CloudQueueMessage("Start: http://www.cnn.com, http://bleacherreport.com");
            Azure.commandQueue.AddMessage(messageStart);
            return "The crawler has started";
        }

        [WebMethod]
        public string StopCrawling()
        {
            CloudQueueMessage stopMessage = new CloudQueueMessage("stop");
            Azure.commandQueue.AddMessage(stopMessage);
            return "The crawler has been stopped.";
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
