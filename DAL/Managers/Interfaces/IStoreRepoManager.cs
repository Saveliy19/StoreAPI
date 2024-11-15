
using DAL.Entities;

namespace DAL.Managers.Interfaces
{
    public interface IStoreRepoManager
    {
        public void CreateStore(Store store);

        // создать продукт
        public void CreateProduct(Product product);

        // завоз товара в магазин
        public void AddProductsToStore(Store store);

        // покупка/списание партии товара в магазине
        public int DeleteProductsFromStore(Store store);

        // сколько каких товаров можно купить в магазине на определенную сумму
        public Store CalculateAffordableItems(Store store, int cache);

        // поиск магазина, где дешевле всего набор товаров
        // public BestPriceLocation GetBestPriceLocation(List<Product> products);

        // получение ассортимента магазина
        public Store GetStoreAssortment(Store store);

        // получение списка магазинов
        public List<Store> GetStores();

        // получение списка всех продуктов
        public List<Product> GetProducts();

        public Dictionary<int, int> GetProductCosts(Product product);
    }
}
