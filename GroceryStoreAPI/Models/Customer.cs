using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GroceryStoreAPI.Models
{
    public class Customer
    {
        [Required]
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [Required]
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
