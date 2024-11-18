using DAL.Repositories.Interfaces;

namespace DAL.Repositories.Strategies
{
    public class SyncStoreStrategy: IStoreStrategy
    {
        private readonly ISyncStoreRepository _syncStoreRepository;
        private readonly ISyncProductRepository _syncProductRepository;

        public SyncStoreStrategy(ISyncStoreRepository syncStoreRepository, ISyncProductRepository syncProductRepository)
        {
            _syncStoreRepository = syncStoreRepository;
            _syncProductRepository = syncProductRepository;
        }

        public DAL.Entities.Product GetProduct(DAL.Entities.Product product) => _syncProductRepository.Get(product);
        public List<int[]> GetStoresSellingProduct(DAL.Entities.Product product) => _syncProductRepository.GetStoresSellingProduct(product);
        public List<DAL.Entities.Store> GetStores() => _syncStoreRepository.GetAll();
        public void AddProductsToStore(DAL.Entities.Store store) => _syncStoreRepository.AddProducts(store);
        public DAL.Entities.Store CalculateAffordableItems(DAL.Entities.Store store, int cache) 
        {
            try
            {
                var assortment = _syncStoreRepository.Get(store);

                foreach (var product in assortment.Products)
                {
                    int count = cache / product.Cost;

                    if (count <= product.Count) product.Count = count;
                }
                return assortment;
            }
            catch (Exception) { throw; }
        }
        public void CreateProduct(DAL.Entities.Product product) => _syncProductRepository.Create(product);
        public void CreateStore(DAL.Entities.Store store) => _syncStoreRepository.Create(store);
        public int DeleteProductsFromStore(DAL.Entities.Store store) => _syncStoreRepository.RemoveProducts(store);
        public List<DAL.Entities.Product> GetProducts() => _syncProductRepository.GetAll();
        public DAL.Entities.Store GetStoreAssortment(DAL.Entities.Store store) => _syncStoreRepository.Get(store);

    }
}
