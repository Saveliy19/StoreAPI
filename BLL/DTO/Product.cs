using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTO
{
    public class Product
    {
        public string Name { get; set; }

        public int Cost { get; set; }

        public int Quantity { get; set; }

        public int StoreId { get; set; }
    }
}
