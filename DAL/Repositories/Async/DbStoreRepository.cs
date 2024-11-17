using DAL.DataBase;
using DAL.DataBase.Models;
using DAL.Entities;
using DAL.Exceptions;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories.Async
{
    public class DbStoreRepository : IAsyncStoreRepository
    {
        private StoreDbContext _dbContext;

        private IAsyncProductRepository _productRepository;
        public DbStoreRepository(StoreDbContext dbContext, IAsyncProductRepository productRepository) 
        { 
            _dbContext = dbContext;
            _productRepository = productRepository;
        }

        public async Task Create(DAL.Entities.Store store)
        {
            var newStore = new DAL.DataBase.Models.Store { StoreName = store.Name, StoreAddress = store.Address };

            await _dbContext.AddAsync(newStore);

            await _dbContext.SaveChangesAsync();
        }

        public async Task<Entities.Store> Get(Entities.Store store)
        {
            
            var storeProducts = await _dbContext.StoreProducts
                    .Where(sp => sp.StoreId == store.Id)
                    .Include(sp => sp.Store)
                    .ToListAsync();

            var products = storeProducts.Select(sp => new Entities.Product
            {
                Name = sp.ProductName,
                Cost = sp.Price,
                Count = sp.Quantity
            }).ToList();

            store.Products = products;

            return store;
        }

        public async Task<List<Entities.Store>> GetAll()
        {
            List<Entities.Store> stores = new List<Entities.Store>();

            var dbStores = await _dbContext.Stores.ToListAsync();

            foreach (var dbStore in dbStores)
            {
                var store = new DAL.Entities.Store() { Id = dbStore.Id, Name = dbStore.StoreName, Address = dbStore.StoreAddress };
                stores.Add(store);
            }

            return stores;
        }

        public async Task AddProducts(Entities.Store store)
        {
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    foreach (var product in store.Products)
                    {
                        if (!await _productRepository.CheckExistence(product)) { throw new ProductNotExistException($"Продукта {product.Name} не существует!"); }

                        var existingStoreProduct = await _dbContext.StoreProducts
                                                              .FirstOrDefaultAsync(sp => sp.StoreId == store.Id && sp.ProductName == product.Name);

                        if (existingStoreProduct != null)
                        {
                            existingStoreProduct.Quantity += product.Count;
                            existingStoreProduct.Price = product.Cost;
                        }
                        else
                        {
                            var newStoreProduct = new DAL.DataBase.Models.StoreProduct
                            {
                                StoreId = store.Id,
                                ProductName = product.Name, 
                                Quantity = product.Count, 
                                Price = product.Cost
                            };

                            await _dbContext.StoreProducts.AddAsync(newStoreProduct);
                        }
                    }                 

                    await _dbContext.SaveChangesAsync();

                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task<int> RemoveProducts(Entities.Store store)
        {
            int summ = 0;


            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    bool productFound;
                    foreach (var product in store.Products)
                    {
                        productFound = false;


                        var existingStoreProduct = await _dbContext.StoreProducts
                                        .FirstOrDefaultAsync(sp => sp.ProductName == product.Name && sp.StoreId == store.Id);

                        if (existingStoreProduct != null)
                        {
                            if (existingStoreProduct.Quantity < product.Count) throw new ProductUnavailableException($"Продукт {product.Name} не продается в магазине {product.StoreId} в достаточном количестве");
                            summ += product.Count * existingStoreProduct.Price;
                            existingStoreProduct.Quantity -= product.Count;
                            productFound = true;
                        }

                        if (!productFound) throw new ProductUnavailableException($"Продукт {product.Name} не продается в магазине {product.StoreId}");
                    }
                }
                catch (Exception) 
                {
                    await transaction.RollbackAsync();
                    throw;
                }

                
            }      

            return summ;
        }
    }
}
