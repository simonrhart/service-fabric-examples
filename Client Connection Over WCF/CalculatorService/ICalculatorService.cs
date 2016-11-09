using System.ServiceModel;
using System.Threading.Tasks;

namespace CalculatorService
{
    [ServiceContract(Namespace = "http://tempuri.org/calculator")]
    public interface ICalculatorService
    {
        [OperationContract]
        Task<string> Add(int a, int b);
        [OperationContract]
        Task<string> Subtract(int a, int b);
    }
}
