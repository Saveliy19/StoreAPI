using BLL.DTO;
using BLL.Exceptions;
using BLL.Infrasructure;
using BLL.Mappers;
using DAL.Entities;
using DAL.Exceptions;
using DAL.Managers.Interfaces;
using DAL.Repositories.Interfaces;

namespace BLL.Services
{
    public class StoreService: IStoreService
    {
        private IStoreRepoManager _storeRepoManager;
        private IStoreMapper _storeMapper;

        public StoreService(IStoreRepoManager storeRepoManager, IStoreMapper storeMapper) 
        {
            _storeRepoManager = storeRepoManager;
            _storeMapper = storeMapper;
        }

        private bool ChechStoreExistence(DTO.Store store)
        {
            bool found = false;

            var dalStores = _storeRepoManager.GetStores();

            foreach (var dalStore in dalStores)
            {
                if (dalStore.Id == store.Id || (dalStore.Name == store.Name && dalStore.Address == store.Address)) { found = true; break; }
            }

            return found;
        }

        public void AddProductsToStore(BLL.DTO.Store store)
        {
            if (!ChechStoreExistence(store)) { throw new StoreNotExistException($"Магазина {store.Id} не существует!"); }

            try
            {
                var dalStore = _storeMapper.MapStore(store);

                _storeRepoManager.AddProductsToStore(dalStore);
            }

            catch (DAL.Exceptions.ProductNotExistException ex) { throw new BLL.Exceptions.ProductNotExistException(ex.Message); }
        }

        public void CreateProduct(BLL.DTO.Product product)
        {
            try
            {
                var dalProduct = _storeMapper.MapProduct(product);

                _storeRepoManager.CreateProduct(dalProduct);
            }
            catch (DAL.Exceptions.AlreadyExistException ex) { throw new BLL.Exceptions.AlreadyExistException(ex.Message); }

            catch (Exception) { throw; }            
        }

        public void CreateStore(BLL.DTO.Store store)
        {
            if (ChechStoreExistence(store)) throw new BLL.Exceptions.AlreadyExistException($"Магазин {store.Name}, расположенный по адресу {store.Address} уже существует!");
            try
            {
                var dalStore = _storeMapper.MapStore(store);

                _storeRepoManager.CreateStore(dalStore);
            }
            // обработка ошибки уже существует
            catch(Exception) { throw; }
        }

        public int DeleteProductsFromStore(BLL.DTO.Store store)
        {
            if (!ChechStoreExistence(store)) { throw new StoreNotExistException($"Магазина {store.Id} не существует!"); }

            var dalStore = _storeMapper.MapStore(store);

            try
            {
                int summ = _storeRepoManager.DeleteProductsFromStore(dalStore);
                return summ;
            }
            catch (DAL.Exceptions.ProductUnavailableException ex) { throw new BLL.Exceptions.ProductUnavailableException(ex.Message); }

            catch (DAL.Exceptions.ProductNotExistException ex) { throw new BLL.Exceptions.ProductNotExistException(ex.Message); }
        }

        public BLL.DTO.Store CalculateAffordableItems(BLL.DTO.Store store, int cache)
        {
            if (!ChechStoreExistence(store)) { throw new StoreNotExistException($"Магазина {store.Id} не существует!"); }

            var assortment = _storeRepoManager.CalculateAffordableItems(_storeMapper.MapStore(store), cache);

            var bllStore = _storeMapper.MapStore(assortment);

            return bllStore;
        }

        public BLL.DTO.BestPriceLocation GetBestPriceLocation(List<DTO.Product> products)
        {
            var dalStores = _storeRepoManager.GetStores();

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

            try
            {
                // Рассчитываем сумму стоимости товаров в каждом магазине
                foreach (var product in products)
                {
                    var dalProduct = _storeMapper.MapProduct(product);
                    var storePrices = _storeRepoManager.GetProductCosts(dalProduct);

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
            }

            catch (DAL.Exceptions.ProductUnavailableException ex) { throw new BLL.Exceptions.ProductUnavailableException(ex.Message); }

            catch (DAL.Exceptions.ProductNotExistException ex) { throw new BLL.Exceptions.ProductNotExistException(ex.Message); }


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

            // Если подходящего магазина не найдено
            if (bestStoreId == -1)
            {
                throw new StoreNotExistException("Не найдено магазина, в котором можно купить все перечисленные товары");
            }

            var bestStore = _storeMapper.MapStore(dalStores.FirstOrDefault(s => s.Id == bestStoreId));
            return new DTO.BestPriceLocation { PriceSumm = lowestCost, Store = bestStore };
        }


        public List<BLL.DTO.Store> GetStores()
        {
            var stores = new List<BLL.DTO.Store>();

            var dalStores = _storeRepoManager.GetStores();

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

            var dalProducts = _storeRepoManager.GetProducts();

            foreach (var product in dalProducts)
            {
                var bllProduct = _storeMapper.MapProduct(product);
                products.Add(bllProduct);
            }

            return products;

        }

        public DTO.Store GetStoreAssortment(DTO.Store store)
        {
            if (!ChechStoreExistence(store)) { throw new StoreNotExistException($"Магазина {store.Id} не существует!"); }

            var dalStore = _storeRepoManager.GetStoreAssortment(_storeMapper.MapStore(store));

            var bllStore = _storeMapper.MapStore(dalStore);

            return bllStore;

        }
    }
}
