using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using Contracts;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace ShoppingCartService
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class ShoppingCartService : StatefulService, IShoppingCartService
    {
        public ShoppingCartService(StatefulServiceContext context)
            : base(context)
        { }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            //new

            return new[]
           {
                new ServiceReplicaListener(context =>
                    new WcfCommunicationListener<IShoppingCartService>(wcfServiceObject: this, serviceContext: context,
                        endpointResourceName: "ServiceEndpoint", listenerBinding: this.CreateListenerBinding()))
            };

        }


        private NetTcpBinding CreateListenerBinding()
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
        
        public async Task AddItem(ShoppingCartItem item)
        {
            var cart = await StateManager.GetOrAddAsync<IReliableDictionary<string, ShoppingCartItem>>("myCart");
            using (var tx = this.StateManager.CreateTransaction())
            {
                await cart.AddOrUpdateAsync(tx, item.ProductName, item, (k, v) => item);
                await tx.CommitAsync();
            }
        }

        public async Task<IList<KeyValuePair<string, ShoppingCartItem>>> GetItems()
        {
            var result = new List<KeyValuePair<string, ShoppingCartItem>>();

            IReliableDictionary<string, ShoppingCartItem> reliableDictionary =
                await StateManager.GetOrAddAsync<IReliableDictionary<string, ShoppingCartItem>>("myCart");

            using (ITransaction tx = StateManager.CreateTransaction())
            {
                IAsyncEnumerable<KeyValuePair<string, ShoppingCartItem>> asyncEnumerable = await reliableDictionary.CreateEnumerableAsync(tx);
                using (IAsyncEnumerator<KeyValuePair<string, ShoppingCartItem>> asyncEnumerator = asyncEnumerable.GetAsyncEnumerator())
                {
                    while (await asyncEnumerator.MoveNextAsync(CancellationToken.None))
                    {
                        result.Add(asyncEnumerator.Current);
                    }
                }
            }
            return result;

        }
    }
}
