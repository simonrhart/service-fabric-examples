using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Client;

namespace SimpleStoreClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Uri ServiceName = new Uri("fabric:/SimpleStoreApplication/ShoppingCartService");

            ServicePartitionResolver serviceResolver = new ServicePartitionResolver(() => new FabricClient());
            NetTcpBinding binding = CreateClientConnectionBinding();
            ServicePartitionKey partitionKey = new ServicePartitionKey(0);
                Client shoppingClient =
                 new Client(
                     new WcfCommunicationClientFactory<IShoppingCartService>(servicePartitionResolver: serviceResolver, clientBinding: binding),
                     ServiceName, partitionKey);

                shoppingClient.AddItem(new ShoppingCartItem
                {
                    ProductName = "XBOX ONE",
                    UnitPrice = 329.0,
                    Amount = 2
                }).Wait();
            shoppingClient.AddItem(new ShoppingCartItem
            {
                ProductName = "XBOX 360",
                UnitPrice = 299,
                Amount = 1
            }).Wait();
            PrintPartition(shoppingClient);
            var list = shoppingClient.GetItems().Result;
          
           
        }
        private static NetTcpBinding CreateClientConnectionBinding()
        {
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None)
            {
                SendTimeout = TimeSpan.MaxValue,
                ReceiveTimeout = TimeSpan.MaxValue,
                OpenTimeout = TimeSpan.FromSeconds(5),
                CloseTimeout = TimeSpan.FromSeconds(5),
                MaxReceivedMessageSize = 1024 * 1024
            };
            binding.MaxBufferSize = (int)binding.MaxReceivedMessageSize;
            binding.MaxBufferPoolSize = Environment.ProcessorCount * binding.MaxReceivedMessageSize;

            return binding;
        }
        private static void PrintPartition(Client client)
        {
            ResolvedServicePartition partition;
            if (client.TryGetLastResolvedServicePartition(out partition))
            {
                Console.WriteLine("Partition ID: " + partition.Info.Id);
            }
        }
    }
}
