using DAL.Entities;
using DAL.Infrastructure;

namespace DAL.Repositories
{
    internal class FileStoreRepository : IStoreRepository
    {
        private string _storePath;
        private IProductRepository _productRepository;

        public FileStoreRepository(string storePath, IProductRepository productRepository)
        {
            if (!File.Exists(storePath)) File.Create(storePath).Close();
            _storePath = storePath;
            _productRepository = productRepository;
        }

    }
}
