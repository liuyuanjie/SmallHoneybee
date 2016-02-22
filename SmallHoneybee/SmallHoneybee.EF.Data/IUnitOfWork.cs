namespace SmallHoneybee.EF.Data
{
    public interface IUnitOfWork
    {
        TRepository GetRepository<TRepository>() where TRepository : class;
        void Commit();
    }
}
