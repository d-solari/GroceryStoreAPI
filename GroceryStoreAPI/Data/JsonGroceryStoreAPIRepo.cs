using GroceryStoreAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GroceryStoreAPI.Data
{
    public class JsonGroceryStoreAPIRepo : IGroceryStoreAPIRepo
    {
        private readonly IGroceryStoreAPIJsonProvider _jsonProvider;

        public JsonGroceryStoreAPIRepo(IGroceryStoreAPIJsonProvider jsonProvider)
        {
            _jsonProvider = jsonProvider ?? throw new ArgumentNullException(nameof(jsonProvider));
        }

        public async Task<IEnumerable<Customer>> GetAllCustomers()
        {   
            return await _jsonProvider.DeserializeJson();
        }

        public async Task<Customer> GetCustomerById(int id)
        {
            var customers = await _jsonProvider.DeserializeJson();

            return customers.FirstOrDefault(c => c.Id == id);
        }
        public async Task CreateCustomer(Customer customer)
        {
            _ = customer ?? throw new ArgumentNullException(nameof(customer));

            await _jsonProvider.Add(customer);
        }

        public async Task UpdateCustomer(Customer customer)
        {
            _ = customer ?? throw new ArgumentNullException(nameof(customer));

            await _jsonProvider.Update(customer);
        }
    }
}
