
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

        public Task<List<int[]>> GetStoresSellingProduct(Product product);

        public Task<bool> CheckExistence(Product product);

    }
}
