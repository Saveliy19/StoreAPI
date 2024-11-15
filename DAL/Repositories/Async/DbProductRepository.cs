using DAL.DataBase;
using DAL.DataBase.Models;
using DAL.Entities;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories.Async
{
    public class DbProductRepository : IAsyncProductRepository
    {

        private readonly DbContext _context;

        public DbProductRepository(StoreDbContext context)
        {
            _context = context;
        }

        public async Task Create(DAL.Entities.Product product)
        {
            var newProduct = new DAL.DataBase.Models.Product { Name = product.Name };

            // Асинхронно добавляем продукт
            await _context.AddAsync(newProduct);

            // Сохраняем изменения
            await _context.SaveChangesAsync();
        }
    }
}
