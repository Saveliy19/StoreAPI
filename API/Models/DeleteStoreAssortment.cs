using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class DeleteStoreAssortment
    {

        [Required]
        [MinLength(1, ErrorMessage = "Products list must contain at least one product.")]
        public List<PurchaseProduct> Products { get; set; }
    }
}
