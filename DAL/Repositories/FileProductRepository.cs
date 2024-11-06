using DAL.Entities;
using DAL.Infrastructure;
using static System.Formats.Asn1.AsnWriter;

namespace DAL.Repositories
{
    internal class FileProductRepository : IProductRepository
    {
        private string _productPath;
        private string _storeProductsPath;
        public FileProductRepository(string productPath, string storeProductsPath)
        {
            if (!File.Exists(productPath)) File.Create(productPath).Close();
            if (!File.Exists(storeProductsPath)) File.Create(storeProductsPath).Close();
            _productPath = productPath;
            _storeProductsPath = storeProductsPath;
        }

        public void Create(Product product)
        {
            List<string> productsData = new List<string>();

            using (var reader = new StreamReader(_productPath))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    productsData.Add(line);
                }
            }

            foreach (var row in productsData)
            {
                if (row == product.Name) return;
            }

            productsData.Add(product.Name);
        }

        public void UpdateInStore(Store store, bool sign)
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
                // удостоверяемя, что продукт с таким именем уже существует
                // или создаем его
                Create(product);

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

        public void AddToStore(Store store)
        {
            UpdateInStore(store, true);
        }

        public void RemoveFromStore(Store store)
        {
            UpdateInStore(store, false);
        }

        public Product Get(Product product)
        {
            // Считываем строки в список строк
            List<List<string>> storeData = new List<List<string>>();

            using (var reader = new StreamReader(_storeProductsPath))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = new List<string>(line.Split(','));
                    if (product.StoreId == int.Parse(values[0]) && product.Name == values[1])
                    {
                        product.Cost = int.Parse(values[2]);
                        product.Count = int.Parse(values[3]);
                        return product;
                    }
                }
            }

            throw new Exception("Такого продукта не существует!");
        }

        public Dictionary<int, int> GetProductCosts(Product product)
        {
            Dictionary<int, int> productCosts = new Dictionary<int, int>();
            using (var reader = new StreamReader(_storeProductsPath))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = new List<string>(line.Split(','));
                    if (product.Name == values[1])
                    {
                        productCosts[int.Parse(values[0])] = int.Parse(values[2]);
                    }
                }
            }

            return productCosts;
        }

        
    }
}
