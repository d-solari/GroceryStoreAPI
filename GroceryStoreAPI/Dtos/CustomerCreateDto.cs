using System.ComponentModel.DataAnnotations;

namespace GroceryStoreAPI.Dtos
{
    public class CustomerCreateDto
    {
        [Required]
        public string Name { get; set; }
    }
}
