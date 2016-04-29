using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Threading.Tasks;
using System.Text;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Configuration;
using System.Web.Script.Services;
using System.Diagnostics;
using System.Web.Script.Serialization;
using System.Runtime;

namespace dhanINFO344PA2
{
    /// <summary>
    /// Summary description for WebService1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class WebService1 : System.Web.Services.WebService
    {
        //static Trie to store query suggestions
        public static Trie data = new Trie();

        //static filepath to download and write to or read and build from
        public static string filename = HttpContext.Current.Server.MapPath("~") + "/localWikiFinal.txt";

        /// <summary>
        /// builds a trie from the provided filename
        /// </summary>
        [WebMethod]
        public string BuildTrie()
        {
            if (!data.isEmpty)
            {
                data = new Trie();
            }

            int count = 0;
            string lastWord = "a";
            if (File.Exists(filename))
            {
                using (StreamReader sr = new StreamReader(filename))
                {
                    var brk = false;
                    var check = 0;
                    while (!sr.EndOfStream && !brk)
                    {
                        var line = sr.ReadLine();
                        if (check < 5000)
                        {
                            data.AddWord(line);
                        }
                        else
                        {
                            try
                            {
                                //ensures that 50mb is free for later use
                                using (new MemoryFailPoint(50))
                                {
                                    check = 0;
                                    data.AddWord(line);
                                }
                            }
                            catch (Exception e)
                            {
                                return "Last word added: '" + lastWord + "'. Count: " + count + ". Trie built: " + !data.isEmpty + ". Total Memory Used: " + GC.GetTotalMemory(true);
                            }
                        }
                        count++;
                        lastWord = line;
                        check++;
                    }
                }
            }
            return "Last word added: '" + lastWord + "'. Count: " + count + ". Trie built: " + !data.isEmpty + ". Total Memory Used: " + GC.GetTotalMemory(true);
        }

        /// <summary>
        /// searches the trie for words/phrases that start with the passed prefix
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns>a list of words/phrases the start with prefix</returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public List<string> SearchTrie(string prefix)
        {
            //empty string returns empty results
            if (string.IsNullOrEmpty(prefix))
            {
                return new List<string>();
            }

            return data.GetWords(prefix);
        }

        /// <summary>
        /// downloads the wiki title text file from the Azure Blob and saves it to the 
        /// filename path
        /// </summary>
        /// <returns>if the filename exists or has been created (successful download)</returns>
        [WebMethod]
        public string DownloadWiki()
        {
            //connect to Azure
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                ConfigurationManager.AppSettings["StorageConnectionString"]);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("info344pa2");
            if (container.Exists())
            {
                foreach (IListBlobItem item in container.ListBlobs(null, false))
                {
                    if (!(item is CloudBlockBlob))
                    {
                        continue;
                    }
                    CloudBlockBlob blob = (CloudBlockBlob)item;
                    blob.DownloadToFile(filename, FileMode.Create);
                }
            }
            return "Wiki has been downloaded: " + File.Exists(filename);
        }
    }
}
