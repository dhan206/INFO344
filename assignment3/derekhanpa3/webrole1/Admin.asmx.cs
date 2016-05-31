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
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

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
        public static Dashboard Dashboard = new Dashboard();
        public static Dictionary<string, Tuple<List<Page>, DateTime>> cache = new Dictionary<string, Tuple<List<Page>, DateTime>>();

        /// <summary>
        /// Starts the webcrawler
        /// </summary>
        /// <returns>Confirmation that the crawler has started.</returns>
        [WebMethod]
        public string StartCrawling()
        {
            Azure = new AzureStorageConnection();
            Azure.commandQueue.AddMessage(new CloudQueueMessage("Start: http://www.cnn.com, http://bleacherreport.com"));
            return "The crawler has started";
        }

        /// <summary>
        /// Stops the webcrawler
        /// </summary>
        /// <returns>Confirmation that the crawler has stopped</returns>
        [WebMethod]
        public string StopCrawling()
        {
            Azure.commandQueue.AddMessage(new CloudQueueMessage("stop"));
            return "The crawler has been stopped.";
        }

        /// <summary>
        /// Clears all of the queues and tables
        /// </summary>
        /// <returns>Confirmation that the tables and queues have been cleared.</returns>
        [WebMethod]
        public string ClearIndex()
        {
            Azure.commandQueue.AddMessage(new CloudQueueMessage("stop and clear"));
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var loop = true;
            while (loop)
            {
                if (sw.ElapsedMilliseconds > 10000)
                {
                    Azure.crawlQueue.DeleteIfExists();
                    Azure.commandQueue.DeleteIfExists();
                    Azure.dashboardTable.DeleteIfExists();
                    Azure.pageTable.DeleteIfExists();
                    loop = false;
                }
            }
            return "Everything has been cleared.";
        }

        /// <summary>
        /// Gets the page title of the given url
        /// </summary>
        /// <param name="url"></param>
        /// <returns>Page title of given url</returns>
        [WebMethod]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public void GetPage(string callback, string query)
        {
            var finalResults = new List<Page>();
            bool searchTable = true;
            if (cache == null || cache.Count > 100)
            {
                cache = new Dictionary<string, Tuple<List<Page>, DateTime>>();
            }

            if (cache.ContainsKey(query))
            {
                if(cache[query].Item2.AddMinutes(10) < DateTime.Now) {
                    finalResults = cache[query].Item1;
                    searchTable = false;
                }
            }

            try
            {
                if (searchTable)
                {
                    //Queries page table for titles containing word(s) from the query
                    var queryResults = new List<Page>();
                    var queryWords = Regex.Replace(query, @"[^0-9a-zA-Z\s]+", string.Empty).Split(' ');
                    foreach (string word in queryWords)
                    {
                        TableQuery<Page> getPagesWithTitle = new TableQuery<Page>()
                            .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, word));
                        foreach (Page item in Azure.pageTable.ExecuteQuery(getPagesWithTitle))
                        {
                            queryResults.Add(item);
                        }
                    }

                    //Ranks the results based on query word occurances and date (newest first)
                    var orderedResults = queryResults
                        .GroupBy(x => x.RowKey)
                        .Select(x => new Tuple<int, Page>(x.ToList().Count(), x.ElementAt(0)))
                        .OrderByDescending(x => x.Item1)
                        .ThenByDescending(x => x.Item2.Date)
                        .ToList();

                    for (int i = 0; i < Math.Min(orderedResults.Count, 20); i++)
                    {
                        finalResults.Add(orderedResults[i].Item2);
                    }

                    //caches the results
                    cache[query] = new Tuple<List<Page>, DateTime>(finalResults, DateTime.Now);
                }
                    
            } catch (Exception e)
            {
                finalResults = new List<Page>();
            }

            string jsonData = (new JavaScriptSerializer()).Serialize(finalResults);
            string response = callback + "(" + jsonData + ")";

            Context.Response.Clear();
            Context.Response.ContentType = "application/json";
            Context.Response.Write(response);
            Context.Response.End();
        }

        /// <summary>
        /// Refreshes the dashboard
        /// </summary>
        /// <returns>List of dashboard stats</returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public List<string> RefreshDashboard()
        {
            Azure = new AzureStorageConnection();
            var results = new List<string>();
            Dashboard.refreshDashboard();
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
                results.Add(currentDashboard.NumberOfTitles.ToString());
                results.Add(currentDashboard.LastTitle);
            } catch (Exception e)
            {
                return new List<string> { "Error, dashboard does not currently exist" };
            }
            return results;
        }
    }
}
