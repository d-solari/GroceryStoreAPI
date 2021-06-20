using System.Collections.Generic;
using System.Threading.Tasks;
using GroceryStoreAPI.Models;

namespace GroceryStoreAPI.Data
{
    public interface IGroceryStoreAPIJsonProvider
    {
        Task<IEnumerable<Customer>> DeserializeJson();
        Task SerializeJson(IEnumerable<Customer> customers);
        Task Add(Customer customer);
        Task Update(Customer customer);
    }
}
