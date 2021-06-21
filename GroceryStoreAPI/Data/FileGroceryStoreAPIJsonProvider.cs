using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using GroceryStoreAPI.Models;
using Microsoft.Extensions.Configuration;

namespace GroceryStoreAPI.Data
{
    public class FileGroceryStoreAPIJsonProvider : IGroceryStoreAPIJsonProvider
    {
        private readonly IConfiguration _config;
        private readonly string _jsonFile;

        public FileGroceryStoreAPIJsonProvider(IConfiguration config)
        {
            _config = config;
            _jsonFile = Path.Combine(Directory.GetCurrentDirectory(), _config.GetValue<string>("JsonFilename"));
            if (!File.Exists(_jsonFile)) throw new FileNotFoundException("Json database file not found.", _jsonFile);
        }
        public async Task<IEnumerable<Customer>> DeserializeJson()
        {
            using FileStream openStream = File.OpenRead(_jsonFile);

            var root = await JsonSerializer.DeserializeAsync<Root>(openStream);

            return root.Customers;
        }

        public async Task SerializeJson(IEnumerable<Customer> customers)
        {
            var root = new Root {Customers = (IList<Customer>)customers};

            using FileStream createStream = File.Create(_jsonFile);

            await JsonSerializer.SerializeAsync(createStream, root);

            await createStream.DisposeAsync();
        }

        public async Task Add(Customer customer)
        {
            _ = customer ?? throw new ArgumentNullException(nameof(customer));

            var customers = await DeserializeJson();

            var newId = customers.Max(c => c.Id) + 1;
            customer.Id = newId;
            customers = customers.Append(customer).ToList();

            await SerializeJson(customers);
        }

        public async Task Update(Customer customer)
        {
            _ = customer ?? throw new ArgumentNullException(nameof(customer));

            var customers = await DeserializeJson();

            var updateCustomer = customers.FirstOrDefault(c => c.Id == customer.Id);
            
            if (updateCustomer != null)
            {
                updateCustomer.Name = customer.Name;   
            }
            
            await SerializeJson(customers);
        }
    }
}
