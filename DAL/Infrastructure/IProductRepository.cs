using DAL.Entities;

namespace DAL.Infrastructure
{
    public interface IProductRepository
    {
        // создание без привязки к магазину
        public void Create(Product product);
        
        // получение информации (количество, стоимость/ед.) в конкретном магазине
        public Product Get(Product product);

        // получения словаря id магазина : стоимость товара
        public Dictionary<int, int> GetProductCosts(Product product);


    }
}
