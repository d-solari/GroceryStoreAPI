using System.Collections.Generic;
using System.Threading.Tasks;
using GroceryStoreAPI.Models;

namespace GroceryStoreAPI.Data
{
    public interface IGroceryStoreAPIRepo
    {
        Task<IEnumerable<Customer>> GetAllCustomers();
        Task<Customer> GetCustomerById(int id);
        Task CreateCustomer(Customer customer);
        Task UpdateCustomer(Customer customer);
    }
}
