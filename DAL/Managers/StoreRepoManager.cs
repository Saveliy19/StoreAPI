
using DAL.Entities;
using DAL.Managers.Interfaces;
using DAL.Repositories.Async;
using DAL.Repositories.Interfaces;

namespace DAL.Managers
{
    public class StoreRepoManager: IStoreRepoManager
    {
        private readonly bool _useAsync;

        private readonly ISyncStoreRepository? _syncStoreRepository;
        private readonly ISyncProductRepository? _syncProductRepository;

        private readonly IAsyncStoreRepository? _asyncStoreRepository;
        private readonly IAsyncProductRepository? _asyncProductRepository;

        // Конструктор для синхронного режима
        public StoreRepoManager(ISyncProductRepository syncProductRepository, ISyncStoreRepository syncStoreRepository)
        {
            _useAsync = false;
            _syncStoreRepository = syncStoreRepository;
            _syncProductRepository = syncProductRepository;
        }

        // Конструктор для асинхронного режима
        public StoreRepoManager(IAsyncProductRepository asyncProductRepository, IAsyncStoreRepository asyncStoreRepository)
        {
            _useAsync = true;
            _asyncStoreRepository = asyncStoreRepository;
            _asyncProductRepository = asyncProductRepository;
        }

        public Dictionary<int, int> GetProductCosts(Product product)
        {
            Dictionary<int, int> productCosts = new Dictionary<int, int>();

            try
            {
                if (!_useAsync) productCosts = _syncProductRepository.GetProductCosts(product);

                else productCosts = _asyncProductRepository.GetProductCosts(product).GetAwaiter().GetResult();

                return productCosts;
            }
            catch (Exception) { throw; }            
        }

        public List<Store> GetStores()
        {
            var stores = new List<Store>();

            try
            {
                if (!_useAsync) { stores = _syncStoreRepository.GetAll(); }

                else { stores = _asyncStoreRepository.GetAll().GetAwaiter().GetResult(); }

                return stores;
            }
            catch (Exception) { throw; }            
        }

        public void AddProductsToStore(Store store)
        {
            try
            {
                if (!_useAsync) _syncStoreRepository.AddProducts(store);

                else _asyncStoreRepository.AddProducts(store).GetAwaiter().GetResult();
            }
            catch (Exception) { throw; }            
        }

        public Store CalculateAffordableItems(Store store, int cache)
        {
            var assortment = new Store();

            try
            {
                if (!_useAsync) assortment = _syncStoreRepository.Get(store);
                else assortment = _asyncStoreRepository.Get(store).GetAwaiter().GetResult();

                foreach (var product in assortment.Products)
                {
                    int count = cache / product.Cost;

                    if (count <= product.Count) product.Count = count;
                }

                return assortment;
            }
            catch (Exception) { throw; }          
        }

        public void CreateProduct(Product product)
        {
            try
            {
                if (!_useAsync)
                {
                    _syncProductRepository.Create(product);
                    return;
                }

                _asyncProductRepository.Create(product).GetAwaiter().GetResult(); ;
            }
            catch (Exception) { throw; }
        }

        public void CreateStore(Store store)
        {
            try
            {
                if (!_useAsync) _syncStoreRepository.Create(store);

                else _asyncStoreRepository.Create(store).GetAwaiter().GetResult(); ;
            }
            catch (Exception) { throw; }
            
        }

        public int DeleteProductsFromStore(Store store)
        {
            int summ = 0;

            try
            {
                if (!_useAsync) summ = _syncStoreRepository.RemoveProducts(store);

                else summ = _asyncStoreRepository.RemoveProducts(store).GetAwaiter().GetResult(); ;

                return summ;
            }
            catch (Exception) { throw; }            
        }

        public List<Product> GetProducts()
        {
            var products = new List<Product>();

            try
            {
                if (!_useAsync) products = _syncProductRepository.GetAll();

                else products = _asyncProductRepository.GetAll().GetAwaiter().GetResult(); ;

                return products;
            }
            catch (Exception) { throw; }            
        }

        public Store GetStoreAssortment(Store store)
        {
            var assortment = store;

            try
            {
                if (!_useAsync) assortment = _syncStoreRepository.Get(store);

                else assortment = _asyncStoreRepository.Get(store).GetAwaiter().GetResult(); ;

                return assortment;
            }
            catch (Exception) { throw; }        
        }
    }
}
