using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class StoreAssortment
    {
        [Required(ErrorMessage = "Store Id is required.")]
        public int StoreId { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "Products list must contain at least one product.")]
        public List<Product> Products { get; set; }
    }
}
