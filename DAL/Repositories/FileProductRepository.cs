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

            using (var writer = new StreamWriter(_storeProductsPath))
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
