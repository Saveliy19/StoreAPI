using DAL.Entities;

namespace DAL.Infrastructure
{
    public interface IStoreRepository
    {
        // получение списка всех существующих магазинов
        List<Store> GetAll();

        // создание нового магазина
        void Create(Store store);

        // получение полного ассортимента магазина
        Store Get(Store store);

        // Добавляем продукты в магазин
        public void AddProducts(Store store);

        // убираем продукты из магазина
        public int RemoveProducts(Store store);
    }
}
