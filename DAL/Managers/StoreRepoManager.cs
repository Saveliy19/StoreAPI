
using DAL.Entities;
using DAL.Exceptions;
using DAL.Managers.Interfaces;
using DAL.Repositories.Async;
using DAL.Repositories.Interfaces;
using DAL.Repositories.Strategies;

namespace DAL.Managers
{
    public class StoreRepoManager: IStoreRepoManager
    {
        private IStoreStrategy _strategy;

        public StoreRepoManager(ISyncProductRepository syncProductRepository, ISyncStoreRepository syncStoreRepository)
        {
            _strategy = new SyncStoreStrategy(syncStoreRepository, syncProductRepository);
        }

        public StoreRepoManager(IAsyncProductRepository asyncProductRepository, IAsyncStoreRepository asyncStoreRepository)
        {
            _strategy = new AsyncStoreStrategy(asyncStoreRepository, asyncProductRepository);
        }


        public void SetStrategy(IStoreStrategy newStrategy)
        {
            _strategy = newStrategy;
        }

        public bool IsStockAvailable(Product product)
        {
            try
            {
                var quantity = product.Count;
                product = _strategy.GetProduct(product);
                return product.Count >= quantity;
            }
            catch (ProductUnavailableException)
            {
                return false;
            }
        }

        public List<int[]> GetStoresSellingProduct(Product product) => _strategy.GetStoresSellingProduct(product);

        public List<Store> GetStores() => _strategy.GetStores();

        public void AddProductsToStore(Store store) => _strategy.AddProductsToStore(store);

        public Store CalculateAffordableItems(Store store, int cache) => _strategy.CalculateAffordableItems(store, cache);

        public void CreateProduct(Product product) => _strategy.CreateProduct(product);

        public void CreateStore(Store store) => _strategy.CreateStore(store);

        public int DeleteProductsFromStore(Store store) => _strategy.DeleteProductsFromStore(store);

        public List<Product> GetProducts() => _strategy.GetProducts();

        public Store GetStoreAssortment(Store store) => _strategy.GetStoreAssortment(store);
    }
}
