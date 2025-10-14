namespace Charts.Api.Application.Interfaces.Repositories.CommonInterfaces
{ 
    public interface IAddRangeRepository<TEntity> where TEntity : class
    {
        Task AddRangeAsync(List<TEntity> entities, CancellationToken cancellationToken);
    }
}
