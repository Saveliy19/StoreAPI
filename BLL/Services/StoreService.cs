using BLL.DTO;
using BLL.Infrasructure;
using BLL.Mappers;
using DAL.Entities;
using DAL.Infrastructure;

namespace BLL.Services
{
    public class StoreService: IStoreService
    {
        private IStoreRepository _storeRepository;
        private IProductRepository _productRepository;
        private IStoreMapper _storeMapper;

        public StoreService(IStoreRepository storeRepository, IProductRepository productRepository, IStoreMapper storeMapper) 
        {
            _storeRepository = storeRepository;
            _productRepository = productRepository;
            _storeMapper = storeMapper;
        }

        public void AddProductsToStore(BLL.DTO.Store store)
        {
            var dalStore = _storeMapper.MapStore(store);

            _storeRepository.AddProducts(dalStore);
        }

        public void CreateProduct(BLL.DTO.Product product)
        {
            var dalProduct = _storeMapper.MapProduct(product);

            _productRepository.Create(dalProduct);
        }

        public void CreateStore(BLL.DTO.Store store)
        {
            var dalStore = _storeMapper.MapStore(store);

            _storeRepository.Create(dalStore);
        }

        public void DeleteProductsFromStore(BLL.DTO.Store store)
        {
            var dalStore = _storeMapper.MapStore(store);

            _storeRepository.RemoveProducts(dalStore);
        }

        public BLL.DTO.Store CalculateAffordableItems(BLL.DTO.Store store, int cache)
        {

            var assortment = _storeRepository.Get(_storeMapper.MapStore(store));

            foreach (var product in assortment.Products)
            {
                int count = cache / product.Cost;

                if (count <= product.Count) product.Count = count;
            }

            var bllStore = _storeMapper.MapStore(assortment);

            return bllStore;
        }

        public BLL.DTO.BestPriceLocation GetBestPriceLocation(List<DTO.Product> products)
        {

            var dalStores = _storeRepository.GetAll();

            Dictionary<int, int> storeSumm = new Dictionary<int, int>();
            Dictionary<int, int> storeProductsCount = new Dictionary<int, int>();

            // количество позиций продуктов
            int positionsCount = 0;

            foreach (var product in products)
            {
                positionsCount++;
            }

            foreach (var store in dalStores)
            {
                storeSumm[store.Id] = 0;
                storeProductsCount[store.Id] = 0;
            }


            foreach (var product in products)
            {
                var dalProduct = _storeMapper.MapProduct(product);
                var storePrice = _productRepository.GetProductCosts(dalProduct);

                foreach (KeyValuePair<int, int> entry in storePrice)
                {
                    try
                    {
                        storeSumm[entry.Key] += entry.Value * dalProduct.Count;
                        storeProductsCount[entry.Key]++;
                    }
                    
                    catch 
                    {
                        continue;
                    }

                }
            }

            int bestId = 0;
            int bestCost = 0;

            foreach (KeyValuePair<int, int> entry in storeSumm)
            {
                if (storeSumm[entry.Key] <= bestCost && storeProductsCount[entry.Key] == positionsCount)
                {
                    bestCost = storeSumm[entry.Key];
                    bestId = entry.Key;
                }
            }

            var bestStore = new BLL.DTO.Store();

            foreach (var dalStore in dalStores)
            {
                if (dalStore.Id == bestId)
                {
                    bestStore = _storeMapper.MapStore(dalStore);
                    break;
                }
            }
                        
            return new DTO.BestPriceLocation() { PriceSumm = bestCost, Store = bestStore };
        }
    }
}
