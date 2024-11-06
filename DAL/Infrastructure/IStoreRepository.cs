using DAL.Entities;

namespace DAL.Infrastructure
{
    internal interface IStoreRepository
    {
        // создание нового магазина
        void Create(Store store);

        // получение полного ассортимента магазина
        Store Get(Store store);

        // Добавляем продукты в магазин
        public void AddProducts(Store store);

        // убираем продукты из магазина
        public void RemoveProducts(Store store);
    }
}
