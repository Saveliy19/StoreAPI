using DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Strategies
{
    public class AsyncStoreStrategy: IStoreStrategy
    {
        private readonly IAsyncStoreRepository _asyncStoreRepository;
        private readonly IAsyncProductRepository _asyncProductRepository;

        public AsyncStoreStrategy(IAsyncStoreRepository asyncStoreRepository, IAsyncProductRepository asyncProductRepository)
        {
            _asyncStoreRepository = asyncStoreRepository;
            _asyncProductRepository = asyncProductRepository;
        }

        public DAL.Entities.Product GetProduct(DAL.Entities.Product product) => _asyncProductRepository.Get(product).GetAwaiter().GetResult();
        public List<int[]> GetStoresSellingProduct(DAL.Entities.Product product) => _asyncProductRepository.GetStoresSellingProduct(product).GetAwaiter().GetResult();
        public List<DAL.Entities.Store> GetStores() => _asyncStoreRepository.GetAll().GetAwaiter().GetResult();
        public void AddProductsToStore(DAL.Entities.Store store) => _asyncStoreRepository.AddProducts(store).GetAwaiter().GetResult();
        public DAL.Entities.Store CalculateAffordableItems(DAL.Entities.Store store, int cache) 
        {
            try
            {
                var assortment = _asyncStoreRepository.Get(store).GetAwaiter().GetResult();

                foreach (var product in assortment.Products)
                {
                    int count = cache / product.Cost;

                    if (count <= product.Count) product.Count = count;
                }
                return assortment;
            }
            catch (Exception) { throw; }
            
        } 
        public void CreateProduct(DAL.Entities.Product product) => _asyncProductRepository.Create(product).GetAwaiter().GetResult();
        public void CreateStore(DAL.Entities.Store store) => _asyncStoreRepository.Create(store).GetAwaiter().GetResult();
        public int DeleteProductsFromStore(DAL.Entities.Store store) => _asyncStoreRepository.RemoveProducts(store).GetAwaiter().GetResult();
        public List<DAL.Entities.Product> GetProducts() => _asyncProductRepository.GetAll().GetAwaiter().GetResult();
        public DAL.Entities.Store GetStoreAssortment(DAL.Entities.Store store) => _asyncStoreRepository.Get(store).GetAwaiter().GetResult();
    }
}
