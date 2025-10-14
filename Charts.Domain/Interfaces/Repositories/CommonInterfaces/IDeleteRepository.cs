namespace Charts.Api.Application.Interfaces.Repositories.CommonInterfaces
{
    public interface IDeleteRepository<TEntity> where TEntity : class
    {
        Task DeleteAsync(Guid id, CancellationToken cancellationToken);
    }
}
