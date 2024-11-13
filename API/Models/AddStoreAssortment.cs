using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class AddStoreAssortment
    {
        [Required]
        [MinLength(1, ErrorMessage = "Products list must contain at least one product.")]
        public List<Product> Products { get; set; }
    }
}
