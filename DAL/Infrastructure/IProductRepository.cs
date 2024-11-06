using DAL.Entities;

namespace DAL.Infrastructure
{
    internal interface IProductRepository
    {
        // создание без привязки к магазину
        public void Create(Product product);
        
        // Добавляем продукты в магазин
        public void AddToStore(Store store);

        // убираем продукты из магазина
        public void RemoveFromStore(Store store);


        // получение информации (количество, стоимость/ед.) в конкретном магазине
        public Product Get(Product product);

        // получения словаря id магазина : стоимость товара
        public Dictionary<int, int> GetProductCosts(Product product);


    }
}
