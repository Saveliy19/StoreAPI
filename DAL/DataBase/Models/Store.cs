using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DAL.DataBase.Models
{
    public class Store
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]  // Автоматическая генерация ID
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string StoreName { get; set; }

        [Required]
        [MaxLength(100)]
        public string StoreAddress { get; set; }

        // Связь "один ко многим" с таблицей StoreProducts
        public List<StoreProduct> StoreProducts { get; set; }
    }
}
