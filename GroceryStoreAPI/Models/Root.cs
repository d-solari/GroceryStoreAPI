using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GroceryStoreAPI.Models
{
    public class Root
    {
        [JsonPropertyName("customers")]
        public IList<Customer> Customers { get; set; }
    }
}
