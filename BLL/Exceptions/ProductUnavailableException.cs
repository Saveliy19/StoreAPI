namespace BLL.Exceptions
{
    public class ProductUnavailableException : Exception
    {
        public ProductUnavailableException(string message) : base(message) { }
    }
}
