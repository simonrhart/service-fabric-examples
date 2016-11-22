using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Client;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Client;

namespace SimpleStoreClient
{
    public class Client : ServicePartitionClient<WcfCommunicationClient<IShoppingCartService>>, IShoppingCartService
    {
        public Client(WcfCommunicationClientFactory<IShoppingCartService> clientFactory, Uri serviceName, ServicePartitionKey customerId)
                : base(clientFactory, serviceName, customerId)
        {
        }

        public Task AddItem(ShoppingCartItem item)
        {
            return this.InvokeWithRetryAsync(client => client.Channel.AddItem(item));
        }

        //public Task DeleteItem(string productName)
        //{
        //    return this.InvokeWithRetryAsync(client => client.Channel.DeleteItem(productName));
        //}

        public Task<IList<KeyValuePair<string, ShoppingCartItem>>> GetItems()
        {
            return this.InvokeWithRetryAsync(client => client.Channel.GetItems());
        }
    }
}
