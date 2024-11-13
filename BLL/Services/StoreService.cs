using BLL.DTO;
using BLL.Infrasructure;
using BLL.Mappers;
using DAL.Entities;
using DAL.Exceptions;
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

        public int DeleteProductsFromStore(BLL.DTO.Store store)
        {
            var dalStore = _storeMapper.MapStore(store);

            try
            {
                int summ = _storeRepository.RemoveProducts(dalStore);
                return summ;
            }
            catch (ProductUnavailableException) { throw; }
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

            // Используем Dictionary для хранения общей суммы и количества товаров по каждому магазину
            Dictionary<int, int> storeSums = new Dictionary<int, int>();
            Dictionary<int, int> storeProductCounts = new Dictionary<int, int>();

            // Общее количество позиций продуктов, которые нужно купить
            int positionsCount = products.Count;

            // Инициализируем суммы и счетчики для всех магазинов
            foreach (var store in dalStores)
            {
                storeSums[store.Id] = 0;
                storeProductCounts[store.Id] = 0;
            }

            // Рассчитываем сумму стоимости товаров в каждом магазине
            foreach (var product in products)
            {
                var dalProduct = _storeMapper.MapProduct(product);
                var storePrices = _productRepository.GetProductCosts(dalProduct);

                foreach (var entry in storePrices)
                {
                    int storeId = entry.Key;
                    int pricePerUnit = entry.Value;

                    // Добавляем сумму за этот товар и увеличиваем счётчик товаров, доступных в магазине
                    if (storeSums.ContainsKey(storeId))
                    {
                        storeSums[storeId] += pricePerUnit * dalProduct.Count;
                        storeProductCounts[storeId]++;
                    }
                }
            }

            // Поиск магазина с наименьшей стоимостью, учитывая только те магазины, где доступны все товары
            int bestStoreId = -1;
            int lowestCost = int.MaxValue;

            foreach (var storeId in storeSums.Keys)
            {
                if (storeProductCounts[storeId] == positionsCount && storeSums[storeId] < lowestCost)
                {
                    lowestCost = storeSums[storeId];
                    bestStoreId = storeId;
                }
            }

            // Если подходящего магазина не найдено, возвращаем null или можно бросить исключение, если это уместно
            if (bestStoreId == -1)
            {
                return null;
            }

            // Находим данные для лучшего магазина и возвращаем результат
            var bestStore = _storeMapper.MapStore(dalStores.FirstOrDefault(s => s.Id == bestStoreId));
            return new DTO.BestPriceLocation { PriceSumm = lowestCost, Store = bestStore };
        }


        public List<BLL.DTO.Store> GetStores()
        {
            var stores = new List<BLL.DTO.Store>();

            var dalStores = _storeRepository.GetAll();

            foreach (var store in dalStores) 
            {
                var bllStore = _storeMapper.MapStore(store);
                stores.Add(bllStore);
            }

            return stores;
        }

        public List<BLL.DTO.Product> GetProducts()
        {
            var products = new List<BLL.DTO.Product>();

            var dalProducts = _productRepository.GetAll();

            foreach (var product in dalProducts)
            {
                var bllProduct = _storeMapper.MapProduct(product);
                products.Add(bllProduct);
            }

            return products;

        }


    }
}
