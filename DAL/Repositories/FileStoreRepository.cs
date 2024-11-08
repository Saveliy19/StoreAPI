using DAL.Entities;
using DAL.Infrastructure;
using static System.Formats.Asn1.AsnWriter;

namespace DAL.Repositories
{
    public class FileStoreRepository : IStoreRepository
    {
        private string _storePath;
        private string _storeProductsPath;
        private IProductRepository _productRepository;

        public FileStoreRepository(string storePath, string storeProductsPath, IProductRepository productRepository)
        {
            if (!File.Exists(storePath)) File.Create(storePath).Close();
            if (!File.Exists(storeProductsPath)) File.Create(storeProductsPath).Close();
            _storeProductsPath = storeProductsPath;
            _storePath = storePath;
            _productRepository = productRepository;
        }

        public void UpdateInStore(Store store, bool sign)
        {
            // проверяем, что магазин существует
            Create(store);

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
                // удостоверяемя, что продукт с таким именем уже существует
                // или создаем его
                _productRepository.Create(product);

                // Поиск нужного товара
                foreach (var row in storeData)
                {
                    if (row[0] == store.Id.ToString() && row[1] == product.Name)
                    {
                        if (sign)
                        {
                            row[2] = product.Cost.ToString();
                            int currentCount = int.Parse(row[3]);
                            row[3] = (currentCount + product.Count).ToString();
                        }

                        else
                        {
                            int currentCount = int.Parse(row[3]) - product.Count;
                            if (currentCount > 0) row[3] = currentCount.ToString();
                            else row[3] = "0";
                        }

                        return;
                    }
                }

                if (sign)
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

        public void AddProducts(Store store)
        {
            UpdateInStore(store, true);
        }

        public void RemoveProducts(Store store)
        {
            UpdateInStore(store, false);
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

            foreach (var row in storesData)
            {
                if (row[1] == store.Name) return;
            }

            var newStore = new List<string>
            {
                storesData.Count.ToString(),
                store.Name,
                store.Address
            };
            storesData.Add(newStore);

            using (var writer = new StreamWriter(_storeProductsPath))
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
