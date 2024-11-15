using System.ComponentModel.DataAnnotations;

namespace DAL.DataBase.Models
{
    public class Product
    {
        [Key]
        [MaxLength(100)]
        public string Name { get; set; }

        public List<StoreProduct> StoreProducts { get; set; }
    }
}
