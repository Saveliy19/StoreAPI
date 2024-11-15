using DAL.DataBase;
using DAL.DataBase.Models;
using DAL.Entities;
using DAL.Exceptions;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories.Async
{
    public class DbProductRepository : IAsyncProductRepository
    {

        private readonly StoreDbContext _context;

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

        public async Task<Entities.Product> Get(Entities.Product product)
        {
            var storeProduct = await _context.StoreProducts.
                Where(sp => sp.StoreId == product.StoreId && sp.ProductName == product.Name)
                .FirstOrDefaultAsync();

            if (storeProduct == null)
            {
                throw new ProductNotExistException($"Продукта {product.Name} не продается в магазине {product.StoreId}");
            }

            product.Count = storeProduct.Quantity;
            product.Cost = storeProduct.Price;

            return product;
        }

        public async Task<List<DAL.Entities.Product>> GetAll()
        {
            List<DAL.Entities.Product> products = new List<DAL.Entities.Product>();

            var dbProducts = await _context.Products.ToListAsync();

            foreach (var item in dbProducts)
            {
                var product = new DAL.Entities.Product(){ Name = item.Name};
                products.Add(product);
            }

            return products;
        }

        public async Task<Dictionary<int, int>> GetProductCosts(Entities.Product product)
        {
            Dictionary<int, int> storePrice = new Dictionary<int, int>();

            var storesProducts = await _context.StoreProducts
                                                .Where(sp => sp.ProductName == product.Name)
                                                .Include(sp => sp.Store)
                                                .ToListAsync();

            foreach (var storeProduct in storesProducts)
            {
                storePrice[storeProduct.StoreId] = storeProduct.Price;
            }

            return storePrice;
        }

    }
}
