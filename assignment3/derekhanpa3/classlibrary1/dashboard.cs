using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class Dashboard : TableEntity
    {
        public Dashboard()
        {
            this.PartitionKey = "OG";
            this.RowKey = "OG";
        }

        public string State { get; set; }                    //State of web role
        public int CPU { get; set; }                         //Machine CountersL CPU Utilization%
        public int RAM { get; set; }                         //Machine Counters: RAM
        public int Crawled { get; set; }                     //#URLs Crawled
        public List<string> LastTen = new List<string>();    //Last 10 URLs crawled
        public int SizeQueue { get; set; }                   //Size of queue
        public int SizeIndex { get; set; }                   //Size of index
        public List<string> Errors = new List<string>();     //Errors
    }
}