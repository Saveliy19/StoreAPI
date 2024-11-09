using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class NewStore
    {
        [Required(ErrorMessage = "Store name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Store address is required.")]
        public string Address { get; set; }
    }
}
