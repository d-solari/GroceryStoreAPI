using System.ComponentModel.DataAnnotations;

namespace GroceryStoreAPI.Dtos
{
    public class CustomerUpdateDto
    {
        [Required]
        public string Name { get; set; }
    }
}
