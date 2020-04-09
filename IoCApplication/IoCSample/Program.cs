using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IoCSample
{
    class Program
    {
        static void Main(string[] args)
        {
            string fileDirectory = @"C:\Windows\Temp";

            IoCClass obj = new IoCClass() { FileDirectory = fileDirectory };

            // 1. Constructor Injection.
            obj.WriteLogUsingConstructorInjection("Hi, hetu how r u?");

            // 2. Setter Injection.
            obj.WriteLogUsingSetterInjection("What r u doing dear?");

            // 3. Interface Injection.
            obj.WriteLogUsingInterfaceInjection("Is everything Ok na??");

            // 4. Service Locator Injection.
            obj.WriteLogUsingServiceInjection("Hope everything is alright");

            // 5. Generic-Type Injection.
            obj.WriteLogUsingGenericTypeInjection("Hey, i should leave now. Bye");

            Console.Read();
        }
    }
}
