using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class NewProduct
    {
        [Required(ErrorMessage = "Product name is required.")]
        public string Name { get; set; }
    }
}
