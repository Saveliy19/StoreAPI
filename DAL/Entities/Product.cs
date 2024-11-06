using DAL.Infrastructure;

namespace DAL.Entities
{
    internal class Product: IEntity
    {
        public string Name { get; set; }

        public int Count { get; set; }

        public int Cost { get; set; }

        public int StoreId { get; set; }
    }
}
