using System.Linq.Expressions;

namespace Charts.Api.Application.Interfaces.Repositories.CommonInterfaces
{
    public interface IGetAllRepository<TEntity> where TEntity : class
    {
        Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken,
            Expression<Func<TEntity, bool>>? filter = null, params Expression<Func<TEntity, object>>[] includes);
    }
}
