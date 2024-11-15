
using DAL.Entities;

namespace DAL.Repositories.Interfaces
{
    public interface IAsyncProductRepository
    {
        public Task Create(Product product);

        // Получение всех продуктов
        public Task<List<Product>> GetAll();

        // получение информации (количество, стоимость/ед.) в конкретном магазине
        public Task<Product> Get(Product product);

        // получения словаря id магазина : стоимость товара
        public Task<Dictionary<int, int>> GetProductCosts(Product product);

    }
}
