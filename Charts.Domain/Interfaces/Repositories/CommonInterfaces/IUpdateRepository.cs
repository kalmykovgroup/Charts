namespace Charts.Api.Application.Interfaces.Repositories.CommonInterfaces
{
    public interface IUpdateRepository<TEntity> where TEntity : class
    {
        Task UpdateAsync(TEntity entity, CancellationToken cancellationToken);
    }
}
