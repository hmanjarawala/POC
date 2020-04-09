using CoffeeBean.Mail.Internet;
using CoffeeBean.Mail.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            byte[] bytes = new byte[] { 1, 2, 3, 4, 5 };
            using(Stream s = new MemoryStream(bytes))
            {
                using(StreamReader r = new StreamReader(s))
                {
                    Console.WriteLine(r.Read());
                    Console.WriteLine(r.Read());
                    Console.WriteLine(r.Read());
                    Console.WriteLine(r.Read());
                    r.BaseStream.Seek(r.BaseStream.Position-2, SeekOrigin.Begin);
                    Console.WriteLine(r.Read());
                    Console.WriteLine(r.Read());
                }
            }
        }
    }
}
