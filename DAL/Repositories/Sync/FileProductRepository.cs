using DAL.DataBase.Models;
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

        public bool CheckExistence(DAL.Entities.Product product)
        {
            using (var reader = new StreamReader(_productPath))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line == product.Name) return true;
                }
            }

            return false;
        }

        public void Create(DAL.Entities.Product product)
        {
            if (CheckExistence(product)) throw new AlreadyExistException($"Продукт {product.Name} уже существует!");

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

        public DAL.Entities.Product Get(DAL.Entities.Product product)
        {
            if (!CheckExistence(product)) throw new ProductNotExistException($"Продукта {product.Name} не существует!");

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

        public List<DAL.Entities.Product> GetAll()
        {
            var products = new List<DAL.Entities.Product>();

            using (var reader = new StreamReader(_productPath))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();

                    var product = new DAL.Entities.Product() { Name = line };
                    products.Add(product);
                }
            }

            return products;
        }

        public List<int[]> GetStoresSellingProduct(DAL.Entities.Product product)
        {
            if (!CheckExistence(product)) throw new ProductNotExistException($"Продукта {product.Name} не существует!");

            List<int[]> storesSellingProduct = new List<int[]>();
            bool found = false;

            using (var reader = new StreamReader(_storeProductsPath))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = new List<string>(line.Split(','));
                    if (product.Name == values[1])
                    {
                        storesSellingProduct.Add(new int[3] { int.Parse(values[0]), int.Parse(values[2]), int.Parse(values[3]) });
                        found = true;
                    }
                }
            }

            if (!found) throw new ProductUnavailableException($"Продукт {product.Name} нигде не продается!");
            return storesSellingProduct;
        }
    }
}
