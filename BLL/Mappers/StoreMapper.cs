using BLL.Infrasructure;

namespace BLL.Mappers
{
    internal class StoreMapper: IStoreMapper
    {
        public DAL.Entities.Product MapProduct(BLL.DTO.Product product)
        {
            var dalProduct = new DAL.Entities.Product
            {
                Name = product.Name,
                Cost = product.Cost,
                Count = product.Quantity
            };

            return dalProduct;
        }

        public DTO.Product MapProduct(DAL.Entities.Product product)
        {
            var bllProduct = new DTO.Product
            {
                Name = product.Name,
                Cost = product.Cost,
                Quantity = product.Count
            };

            return bllProduct;
        }

        public DAL.Entities.Store MapStore(BLL.DTO.Store store)
        {
            var dalProducts = new List<DAL.Entities.Product>();

            foreach (var product in store.Products)
            {
                var dalProduct = MapProduct(product);

                dalProducts.Add(dalProduct);
            }

            var dalStore = new DAL.Entities.Store
            {
                Id = store.Id,
                Name = store.Name,
                Address = store.Address,
                Products = dalProducts
            };

            return dalStore;
        }

        public DTO.Store MapStore(DAL.Entities.Store store)
        {
            var bllProducts = new List<DTO.Product>();

            foreach (var product in store.Products)
            {
                var bllProduct = MapProduct(product);

                bllProducts.Add(bllProduct);
            }

            var bllStore = new DTO.Store
            {
                Id = store.Id,
                Name = store.Name,
                Address = store.Address,
                Products = bllProducts
            };

            return bllStore;
        }
    }
}
