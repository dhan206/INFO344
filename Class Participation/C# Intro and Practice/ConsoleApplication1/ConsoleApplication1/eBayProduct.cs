using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class eBayProduct
    {
        public string Name { get; private set; }
        public double Price { get; set; }

        public eBayProduct()
        {
            this.Name = "unknown";
            this.Price = 0.0;
        }

        public eBayProduct(string name, double price)
        {
            this.Name = name;
            this.Price = price;
        }

        public void SetName(string newName)
        {
            this.Name = newName;
        }

        public static List<eBayProduct> GetDefaultProducts()
        {
            List<eBayProduct> products = new List<eBayProduct>();
            products.Add(new eBayProduct("iMac", 2000));
            products.Add(new eBayProduct("Win7", 100));
            products.Add(new eBayProduct("iPhone", 750.99));
            return products;
        }
    }
}
