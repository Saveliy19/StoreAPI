using DAL.Infrastructure;

namespace BLL.Services
{
    public class StoreService
    {
        private IStoreRepository _storeRepository;
        private IProductRepository _productRepository;

        public StoreService(IStoreRepository storeRepository, IProductRepository productRepository) 
        {
            _storeRepository = storeRepository;
            _productRepository = productRepository;
        }

        // создать магазин
        public void CreateStore()
        {

        }

        // создать продукт
        public void CreateProduct()
        {

        }

        // завоз товара в магазин
        public void AddProductsToStore()
        {

        }

        // покупка/списание партии товара в магазине
        public void DeleteProductsFromStore() 
        {

        }

        // сколько каких товаров можно купить в магазине на определенную сумму
        public Dictionary<string, int> CalculateAffordableItems()
        {

        }

        // поиск магазина, где дешевле всего набор товаров
        public int GetBestPriceLocation()
        {

        }

    }
}
