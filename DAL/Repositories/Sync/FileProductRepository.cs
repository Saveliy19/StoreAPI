using DAL.Entities;
using DAL.Exceptions;
using DAL.Repositories.Interfaces;
using static System.Formats.Asn1.AsnWriter;

namespace DAL.Repositories.Sync
{
    public class FileProductRepository : ISyncProductRepository
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

            using (var writer = new StreamWriter(_productPath))
            {
                foreach (var row in productsData)
                {
                    writer.WriteLine(string.Join(",", row));
                }
            }
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

            throw new ProductNotExistException($"Продукта {product.Name} не продается в магазине {product.StoreId}");
        }

        public Dictionary<int, int> GetProductCosts(Product product)
        {
            bool found = false;
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
                        found = true;
                    }
                }
            }

            if (!found) throw new ProductUnavailableException($"Продукт {product.Name} нигде не продается!");

            return productCosts;
        }

        public List<Product> GetAll()
        {
            var products = new List<Product>();

            using (var reader = new StreamReader(_productPath))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();

                    var product = new Product() { Name = line };
                    products.Add(product);
                }
            }

            return products;
        }


    }
}
