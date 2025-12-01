using System.Linq.Expressions;

namespace Charts.Domain.Interfaces.Repositories.CommonInterfaces
{
    public interface IGetByIdRepository<TEntity> where TEntity : class
    {

        Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken, params Expression<Func<TEntity, object>>[] includes);
    }
}
