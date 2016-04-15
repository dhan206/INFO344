using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            eBayProduct p = new eBayProduct();
            Console.WriteLine(p.Name + ": " + p.Price);
            p.SetName("Macbook Pro Retina");
            p.Price = 10.00;
            //p.Name = "doesn't work";
            Console.WriteLine(p.Name + ": " + p.Price);
            Console.ReadLine();
        }
    }
}
