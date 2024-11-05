using DAL.Entities;
using DAL.Infrastructure;

namespace DAL.Repositories
{
    internal class DbStoreRepository : IRepository<Store>
    {
        void IRepository<Store>.Create(Store entity)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Store> IRepository<Store>.Find(Func<Store, bool> predicate)
        {
            throw new NotImplementedException();
        }

        Store IRepository<Store>.Get(string name)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Store> IRepository<Store>.GetAll()
        {
            throw new NotImplementedException();
        }

        void IRepository<Store>.Update(Store entity)
        {
            throw new NotImplementedException();
        }
    }
}
