namespace BLL.Infrasructure
{
    public interface IStoreMapper
    {
        public DAL.Entities.Product MapProduct(BLL.DTO.Product product);


        public DAL.Entities.Store MapStore(BLL.DTO.Store store);

        public BLL.DTO.Store MapStore(DAL.Entities.Store store);

        public BLL.DTO.Product MapProduct(DAL.Entities.Product product);
    }
}
