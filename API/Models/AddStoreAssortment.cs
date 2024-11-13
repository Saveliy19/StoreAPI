using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class AddStoreAssortment
    {
        [Required(ErrorMessage = "Store Id is required.")]
        public int StoreId { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "Products list must contain at least one product.")]
        public List<Product> Products { get; set; }
    }
}
