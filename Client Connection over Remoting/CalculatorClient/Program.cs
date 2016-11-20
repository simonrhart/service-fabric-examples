using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CalculatorService;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace CalculatorClient
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                var calcsvc =
                    ServiceProxy.Create<ICalculatorService>(new Uri("fabric:/CalculatorApplication/CalculatorService"));
                var result = calcsvc.Add(1, 2).Result;
                Console.WriteLine(result);
                Thread.Sleep(2000);
                
            }
        }
    }
}
