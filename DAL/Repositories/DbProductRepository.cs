using DAL.Entities;
using DAL.Infrastructure;

namespace DAL.Repositories
{
    internal class DbProductRepository : IRepository<Product>
    {
        void IRepository<Product>.Create(Product entity)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Product> IRepository<Product>.Find(Func<Product, bool> predicate)
        {
            throw new NotImplementedException();
        }

        Product IRepository<Product>.Get(string name)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Product> IRepository<Product>.GetAll()
        {
            throw new NotImplementedException();
        }

        void IRepository<Product>.Update(Product entity)
        {
            throw new NotImplementedException();
        }
    }
}
