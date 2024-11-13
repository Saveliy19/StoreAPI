using DAL.Entities;
using DAL.Infrastructure;

namespace DAL.Repositories
{
    internal class DbProductRepository : IProductRepository
    {
        public void Create(Product product)
        {
            throw new NotImplementedException();
        }

        public Product Get(Product product)
        {
            throw new NotImplementedException();
        }

        public List<Product> GetAll()
        {
            throw new NotImplementedException();
        }

        public Dictionary<int, int> GetProductCosts(Product product)
        {
            throw new NotImplementedException();
        }
    }
}
