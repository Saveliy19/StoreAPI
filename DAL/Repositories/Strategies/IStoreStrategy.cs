using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Strategies
{
    public interface IStoreStrategy
    {
        DAL.Entities.Product GetProduct(DAL.Entities.Product product);
        List<int[]> GetStoresSellingProduct(DAL.Entities.Product product);
        List<DAL.Entities.Store> GetStores();
        void AddProductsToStore(DAL.Entities.Store store);
        DAL.Entities.Store CalculateAffordableItems(DAL.Entities.Store store, int cache);
        void CreateProduct(DAL.Entities.Product product);
        void CreateStore(DAL.Entities.Store store);
        int DeleteProductsFromStore(DAL.Entities.Store store);
        List<DAL.Entities.Product> GetProducts();
        DAL.Entities.Store GetStoreAssortment(DAL.Entities.Store store);
    }
}
