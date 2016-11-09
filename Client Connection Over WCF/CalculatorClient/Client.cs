using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CalculatorService;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Client;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Client;

namespace CalculatorClient
{
    public class Client : ServicePartitionClient<WcfCommunicationClient<ICalculatorService>>, ICalculatorService
    {
        public Client(ICommunicationClientFactory<WcfCommunicationClient<ICalculatorService>> communicationClientFactory, Uri serviceUri) : base(communicationClientFactory, serviceUri)
        {
        }

        public Task<string> Add(int a, int b)
        {
            return InvokeWithRetryAsync(client => client.Channel.Add(a, b));
        }

        public Task<string> Subtract(int a, int b)
        {
            return InvokeWithRetryAsync(client => client.Channel.Subtract(a, b));
        }
    }
}
