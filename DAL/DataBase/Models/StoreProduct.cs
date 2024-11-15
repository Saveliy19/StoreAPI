using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DAL.DataBase.Models
{
    public class StoreProduct
    {
        [Key, Column(Order = 0)]
        public int StoreId { get; set; }

        [Key, Column(Order = 1)]
        [MaxLength(100)]
        public string ProductName { get; set; }

        [Required]
        public int Price { get; set; }

        [Required]
        public int Quantity { get; set; }

        // Навигационные свойства для связей
        public Store Store { get; set; }
        public Product Product { get; set; }
    }
}
