namespace DAL.Exceptions
{
    public class ProductNotExistException: Exception
    {
        public ProductNotExistException(string message) : base(message) { }
    }
}
