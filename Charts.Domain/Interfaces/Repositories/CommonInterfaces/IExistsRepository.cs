namespace Charts.Api.Application.Interfaces.Repositories.CommonInterfaces
{
    public interface IExistsRepository<TEntity> where TEntity : class
    {
        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken);
    }
}
