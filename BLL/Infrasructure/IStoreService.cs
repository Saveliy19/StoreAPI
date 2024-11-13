using BLL.DTO;

namespace BLL.Infrasructure
{
    public interface IStoreService
    {
        // создать магазин
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
        public DTO.BestPriceLocation GetBestPriceLocation(List<DTO.Product> products);
    }
}
