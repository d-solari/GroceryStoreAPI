using System.ComponentModel.DataAnnotations;

namespace GroceryStoreAPI.Dtos
{
    public class CustomerCreateDto
    {
        [Required]
        public int? Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
