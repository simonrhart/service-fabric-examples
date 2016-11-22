using System.Collections.Generic;
using System.Threading.Tasks;
using System.ServiceModel;

namespace Contracts
{
    [ServiceContract]
    public interface IShoppingCartService
    {
        [OperationContract]
        Task AddItem(ShoppingCartItem item);

        [OperationContract]
        Task<IList<KeyValuePair<string, ShoppingCartItem>>> GetItems();
    }
}
