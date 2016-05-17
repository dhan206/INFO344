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
using System.Web.Script.Services;
using System.Threading;

namespace WebRole1
{
    /// <summary>
    /// Summary description for Admin
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class Admin : System.Web.Services.WebService
    {
        public static AzureStorageConnection Azure = new AzureStorageConnection();

        [WebMethod]
        public string StartCrawling()
        {
            Dashboard firstDashboard = new Dashboard();
            firstDashboard.loadFirstDashboard();
            Azure.commandQueue.AddMessage(new CloudQueueMessage("Start: http://www.cnn.com, http://bleacherreport.com"));
            return "The crawler has started";
        }

        [WebMethod]
        public string StopCrawling()
        {
            Azure.commandQueue.AddMessage(new CloudQueueMessage("stop"));
            return "The crawler has been stopped.";
        }

        [WebMethod]
        public string ClearIndex()
        {
            Azure.commandQueue.AddMessage(new CloudQueueMessage("stop and clear"));
            Thread.Sleep(2000); //wait for 2 seconds so the stop message is received
            Azure.crawlQueue.Clear();
            Azure.commandQueue.Clear();
            Azure.dashboardTable.DeleteIfExists();
            Azure.pageTable.DeleteIfExists();
            return "Everything has been cleared.";
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetPageTitle(string url)
        {
            var input = url.Trim();
            try
            {
                TableQuery<Page> getTitleWithURL = new TableQuery<Page>()
                    .Where(TableQuery.GenerateFilterCondition("URL", QueryComparisons.Equal, input));
                Page returnedPage = Azure.pageTable.ExecuteQuery(getTitleWithURL).ElementAt(0);
                return returnedPage.Title;
            } catch (Exception e)
            {
                return "Sorry, there are no pages with the url: '" + url + "' in my table";
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public List<string> RefreshDashboard()
        {
            var results = new List<string>();
            try
            {
                TableQuery<Dashboard> getDashboard = new TableQuery<Dashboard>()
                    .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "OG"));
                Dashboard currentDashboard = Azure.dashboardTable.ExecuteQuery(getDashboard).ElementAt(0);
                results.Add(currentDashboard.State);
                results.Add(currentDashboard.CPU.ToString());
                results.Add(currentDashboard.RAM.ToString());
                results.Add(currentDashboard.Crawled.ToString());
                results.Add(currentDashboard.LastTen);
                results.Add(currentDashboard.SizeQueue.ToString());
                results.Add(currentDashboard.SizeIndex.ToString());
                results.Add(currentDashboard.Errors);
            } catch (Exception e)
            {
                return new List<string> { "Error, dashboard does not currently exist" };
            }
            return results;
        }
    }
}
