namespace Charts.Domain.Interfaces.Repositories.CommonInterfaces
{
    public interface IAddRepository<TEntity> where TEntity : class
    {
        Task AddAsync(TEntity entity, CancellationToken cancellationToken);
    }
}
