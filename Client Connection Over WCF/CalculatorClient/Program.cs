using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CalculatorService;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace CalculatorClient
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Uri serviceName = new Uri("fabric:/CalculatorApplication/CalculatorService");
                ServicePartitionResolver servicePartitionResolver = new ServicePartitionResolver((() => new FabricClient()));
                NetTcpBinding binding = CreateClientConnectionBinding();
                Client calClient =
                    new Client(
                        new WcfCommunicationClientFactory<ICalculatorService>(servicePartitionResolver: servicePartitionResolver, clientBinding: binding),
                        serviceName);
                Console.WriteLine(calClient.Add(3,5).Result);
            }
            // ReSharper disable once FunctionNeverReturns
        }

        private static NetTcpBinding CreateClientConnectionBinding()
        {
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None)
            {
                SendTimeout = TimeSpan.MaxValue,
                ReceiveTimeout = TimeSpan.MaxValue,
                OpenTimeout = TimeSpan.FromSeconds(5),
                CloseTimeout = TimeSpan.FromSeconds(5),
                MaxConnections = int.MaxValue,
                MaxReceivedMessageSize = 1024 * 1024
               
            };
           
            binding.MaxBufferSize = (int)binding.MaxReceivedMessageSize;
            binding.MaxBufferPoolSize = Environment.ProcessorCount * binding.MaxReceivedMessageSize;
            return binding;
        }
    }
}
