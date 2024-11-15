
using DAL.Entities;

namespace DAL.Repositories.Interfaces
{
    public interface IAsyncProductRepository
    {
        public Task Create(Product product);

        // Получение всех продуктов
        public Task<List<Product>> GetAll();

    }
}
