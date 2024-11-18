using BLL.DTO;
using BLL.Exceptions;
using BLL.Infrasructure;
using BLL.Mappers;
using DAL.Entities;
using DAL.Exceptions;
using DAL.Managers.Interfaces;
using DAL.Repositories.Interfaces;
using System.Linq.Expressions;

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
            Dictionary<int, int> storeAmount = new Dictionary<int, int>();
            Dictionary<int, int> storePositionsCount = new Dictionary<int, int>();

            try
            {
                foreach (var product in products)
                {
                    var dalProduct = _storeMapper.MapProduct(product);
                    var storesSellingProduct = _storeRepoManager.GetStoresSellingProduct(dalProduct);
                    foreach (var store in storesSellingProduct)
                    {
                        if (store[2] >= product.Quantity)
                        {
                            if (storeAmount.ContainsKey(store[0]))
                            {
                                storeAmount[store[0]] += store[1] * product.Quantity;
                                storePositionsCount[store[0]]++;
                            }
                            else
                            {
                                storeAmount[store[0]] = store[1] * product.Quantity;
                                storePositionsCount[store[0]] = 1;
                            }
                        }
                    }
                }
            }
            catch (DAL.Exceptions.ProductNotExistException ex) { throw new BLL.Exceptions.ProductNotExistException(ex.Message); }
            catch (DAL.Exceptions.ProductUnavailableException ex) { throw new BLL.Exceptions.ProductUnavailableException(ex.Message); }


            BLL.DTO.BestPriceLocation cheapestLocation = new BLL.DTO.BestPriceLocation();
            int minAmount = int.MaxValue;
            int cheapestId = -1;


            foreach (var entry in storePositionsCount)
            {
                if (entry.Value == products.Count && storeAmount[entry.Key] <= minAmount)
                {
                    cheapestId = entry.Key;
                    minAmount = storeAmount[entry.Key];
                }
            }

            if (cheapestId == -1) throw new StoreNotExistException("Не существует магазина, в котором есть все продукты в необходимом количестве!");

            var bestStore = _storeMapper.MapStore(_storeRepoManager.GetStores().FirstOrDefault(s => s.Id == cheapestId));

            cheapestLocation.PriceSumm = minAmount;
            cheapestLocation.Store = bestStore;

            return cheapestLocation;

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
