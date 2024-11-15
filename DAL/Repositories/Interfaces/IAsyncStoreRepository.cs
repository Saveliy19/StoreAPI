
using DAL.Entities;

namespace DAL.Repositories.Interfaces
{
    public interface IAsyncStoreRepository
    {
        // создание нового магазина
        Task Create(Store store);

        // список всех магазинов
        Task<List<Store>> GetAll();

        // получение полного ассортимента магазина
        Task<Store> Get(Store store);

        // Добавляем продукты в магазин
        Task AddProducts(Store store);

        // убираем продукты из магазина
        Task<int> RemoveProducts(Store store);
    }
}
