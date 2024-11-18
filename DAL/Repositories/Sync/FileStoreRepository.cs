using DAL.Entities;
using DAL.Exceptions;
using DAL.Repositories.Interfaces;
using static System.Formats.Asn1.AsnWriter;

namespace DAL.Repositories.Sync
{
    public class FileStoreRepository : ISyncStoreRepository
    {
        private string _storePath;
        private string _storeProductsPath;
        private ISyncProductRepository _productRepository;

        public FileStoreRepository(string storePath, string storeProductsPath, ISyncProductRepository productRepository)
        {
            if (!File.Exists(storePath)) File.Create(storePath).Close();
            if (!File.Exists(storeProductsPath)) File.Create(storeProductsPath).Close();
            _storeProductsPath = storeProductsPath;
            _storePath = storePath;
            _productRepository = productRepository;
        }

        public void AddProducts(Store store)
        {
            {
                // Считываем строки в список строк
                List<List<string>> storeData = new List<List<string>>();

                using (var reader = new StreamReader(_storeProductsPath))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = new List<string>(line.Split(','));
                        storeData.Add(values);
                    }
                }

                // Обработка товаров магазина
                foreach (var product in store.Products)
                {
                    // удостоверяемся, что продукт с таким именем уже существует
                    if (!_productRepository.CheckExistence(product)) throw new ProductNotExistException($"Продукта ${product.Name} не существует!");

                    bool found = false;

                    // Поиск нужного товара
                    foreach (var row in storeData)
                    {
                        if (row[0] == store.Id.ToString() && row[1] == product.Name)
                        {
                            // Добавляем товар
                            row[2] = product.Cost.ToString();
                            int currentCount = int.Parse(row[3]);
                            row[3] = (currentCount + product.Count).ToString();
                            found = true;
                            break;
                        }
                    }

                    // Если товар не найден, добавляем новый
                    if (!found)
                    {
                        var newRow = new List<string>
                        {
                            store.Id.ToString(),
                            product.Name,
                            product.Cost.ToString(),
                            product.Count.ToString()
                        };
                        storeData.Add(newRow);
                    }
                }

                // Перезапись данных в файл
                using (var writer = new StreamWriter(_storeProductsPath))
                {
                    foreach (var row in storeData)
                    {
                        writer.WriteLine(string.Join(",", row));
                    }
                }
            }
        }

        public int RemoveProducts(Store store)
        {
            int summ = 0;

            // Считываем строки в список строк
            List<List<string>> storeData = new List<List<string>>();

            using (var reader = new StreamReader(_storeProductsPath))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = new List<string>(line.Split(','));
                    storeData.Add(values);
                }
            }

            foreach (var product in store.Products)
            {
                if (!_productRepository.CheckExistence(product)) throw new ProductNotExistException($"Продукта ${product.Name} не существует!");
                bool productFound = false;

                foreach (var row in storeData)
                {

                    if (row[0] == store.Id.ToString() && row[1] == product.Name)
                    {
                        productFound = true;
                        int currentCount = int.Parse(row[3]) - product.Count;
                        if (currentCount > 0)
                        {
                            summ += product.Count * int.Parse(row[2]);
                            row[3] = currentCount.ToString();
                        }
                        else throw new ProductUnavailableException($"Продукт {product.Name} недоступен в нужном количестве");
                        break;
                    }
                }

                if (!productFound) throw new ProductUnavailableException($"Продукт {product.Name} не продается в магазине {product.StoreId}");
            }

            // Перезапись данных в файл
            using (var writer = new StreamWriter(_storeProductsPath))
            {
                foreach (var row in storeData)
                {
                    writer.WriteLine(string.Join(",", row));
                }
            }

            return summ;
        }

        public void Create(Store store)
        {
            List<List<string>> storesData = new List<List<string>>();

            using (var reader = new StreamReader(_storePath))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = new List<string>(line.Split(','));
                    storesData.Add(values);
                }
            }

            var newStore = new List<string>
            {
                storesData.Count.ToString(),
                store.Name,
                store.Address
            };
            storesData.Add(newStore);

            using (var writer = new StreamWriter(_storePath))
            {
                foreach (var row in storesData)
                {
                    writer.WriteLine(string.Join(",", row));
                }
            }
        }

        public Store Get(Store store)
        {
            List<Product> storeProducts = new List<Product>();

            using (var reader = new StreamReader(_storeProductsPath))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = new List<string>(line.Split(','));
                    if (int.Parse(values[0]) == store.Id)
                    {
                        Product product = new Product();
                        product.Name = values[1];
                        product.Cost = int.Parse(values[2]);
                        product.Count = int.Parse(values[3]);
                        storeProducts.Add(product);

                    }
                }
            }

            store.Products = storeProducts;
            return store;
        }

        public List<Store> GetAll()
        {
            var stores = new List<Store>();

            List<List<string>> storesData = new List<List<string>>();

            using (var reader = new StreamReader(_storePath))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = new List<string>(line.Split(','));
                    storesData.Add(values);
                }
            }

            if (storesData.Count > 0)
            {
                foreach (var row in storesData)
                {
                    var store = new Store();

                    store.Id = int.Parse(row[0]);
                    store.Name = row[1];
                    store.Address = row[2];
                    stores.Add(store);
                }
            }


            return stores;
        }
    }
}
