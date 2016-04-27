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
        public void BuildTrie()
        {
            if (File.Exists(filename) && data.isEmpty)
            {
                using (StreamReader sr = new StreamReader(filename))
                {
                    var brk = false;
                    //var lastWord = "a";
                    var count = 0;
                    var check = 0;
                    while (!sr.EndOfStream && !brk)
                    {
                        try
                        {
                            //quickly builds to 750mb
                            if (GC.GetTotalMemory(false) <= (750 * 1024 * 1024))
                            {
                                data.AddWord(sr.ReadLine());
                                count++;
                            }
                            else
                            {
                                if (check < 1000)
                                {
                                    //var line = sr.ReadLine();
                                    data.AddWord(sr.ReadLine());
                                    count++;
                                    //lastWord = line;
                                }
                                else
                                {
                                    //ensures that 50mb is free for later use
                                    using (MemoryFailPoint mem = new MemoryFailPoint(50))
                                    {
                                        check = 0;
                                        var line = sr.ReadLine();
                                        data.AddWord(line);
                                        mem.Dispose(); //free up memory
                                        count++;
                                        //lastWord = line;
                                    }
                                }
                                check++;
                            }
                        }
                        catch (InsufficientMemoryException e)
                        {
                            //ignores exception and breaks out of building the trie
                            brk = true;
                            break;
                        }
                    }
                }
            }
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
            //initialization
            if(data.isEmpty)
            {
                if (File.Exists(filename))
                {
                    BuildTrie();
                }
                else
                {
                    DownloadWiki();
                }
            }

            //empty string returns empty results
            if (string.IsNullOrEmpty(prefix))
            {
                return new List<string>();
            }

            if (data.isEmpty)
            {
                return new List<string> { "We're sorry, our setup is has not yet completed. Please try again later." };
            }

            return data.GetWords(prefix);
        }

        /// <summary>
        /// downloads the wiki title text file from the Azure Blob and saves it to the 
        /// filename path
        /// </summary>
        /// <returns>if the filename exists or has been created (successful download)</returns>
        [WebMethod]
        public bool DownloadWiki()
        {
            //file already exists, no need to redownload
            if (File.Exists(filename))
            {
                return true;
            }

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
            return File.Exists(filename);
        }
    }
}
