namespace BLL.Exceptions
{
    public class StoreNotExistException: Exception
    {
        public StoreNotExistException(string message) : base(message) { }
    }
}
