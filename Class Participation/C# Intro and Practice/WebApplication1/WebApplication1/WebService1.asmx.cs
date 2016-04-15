using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace WebApplication1
{
    /// <summary>
    /// Summary description for WebService1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WebService1 : System.Web.Services.WebService
    {
        public static int counter = 0;
        [WebMethod]
        public string HelloWorld(string name)
        {
            return "Hello world " + name + " " + counter++;
        }

        [WebMethod]
        public string ReadFile()
        {
            string filename = HttpContext.Current.Server.MapPath("~") + "/output.txt";
            using (StreamWriter sw = new StreamWriter(filename))
            {
                for (int i = 0; i < 10; i++)
                {
                    sw.WriteLine(i);
                }
            }

            string output = "";
            using (StreamReader sr = new StreamReader(filename))
            {
                while (sr.EndOfStream == false)
                {
                    string line = sr.ReadLine();
                    output += " " + line;
                }
            }
            return output;
        }

        
        [WebMethod]
        public string OutputConfig()
        {
            return ConfigurationManager.AppSettings["derek"];
        }

        [WebMethod]
        public int[] OddNumbers(int n)
        {
            List<int> oddNumbers = new List<int>();
            for (int i = 0; i < n; i++)
            {
                if(i % 2 == 1)
                {
                    oddNumbers.Add(i);
                }
            }
            return oddNumbers.ToArray();
        }
    }
}
