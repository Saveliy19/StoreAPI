namespace DAL.Infrastructure
{
    public interface IRepository<T> where T : IEntity
    {
        IEnumerable<T> GetAll();

        T Get(string name);

        void Create(T entity);
        void Update(T entity);

        IEnumerable<T> Find(Func<T, bool> predicate);

    }
}
