namespace Charts.Domain.Interfaces.Repositories.CommonInterfaces
{
    public interface ISoftDeleteRepository<TEntity> where TEntity : class
    {
        Task SoftDeleteAsync(Guid id, CancellationToken cancellationToken);
    }
}
