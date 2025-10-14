using Charts.Api.Application.Interfaces;
using Charts.Api.Infrastructure.Databases;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Charts.Api.Infrastructure.Repositories
{
    public abstract class RepositoryBase<TEntity>(AppDbContext dbContext) where TEntity : class, IEntity
    {
        protected readonly AppDbContext DbContext = dbContext;
        protected readonly DbSet<TEntity> DbSet = dbContext.Set<TEntity>();

        public virtual Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            entity.Id = entity.Id == Guid.Empty ? Guid.CreateVersion7() : entity.Id;
            DbSet.Add(entity);
            return Task.CompletedTask;
        }


        public virtual Task AddRangeAsync(List<TEntity> entities, CancellationToken cancellationToken = default)
        {
            foreach (var entity in entities)
            {
                entity.Id = entity.Id == Guid.Empty ? Guid.CreateVersion7() : entity.Id;
            }

            DbSet.AddRange(entities);
            return Task.CompletedTask;
        }

        public virtual async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await DbSet.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
            if (entity == null)
                throw new InvalidOperationException($"Entity of type {typeof(TEntity).Name} with Id {id} not found.");

            DbSet.Remove(entity);
        }

        public virtual Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return DbSet.AnyAsync(e => e.Id == id, cancellationToken);
        }

        public virtual async Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken, Expression<Func<TEntity, bool>>? filter = null, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = DbSet.Where(e => !e.IsDeleted);

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.ToListAsync(cancellationToken);
        }

        public virtual async Task<TEntity?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken,
            params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = DbSet.AsQueryable();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted, cancellationToken);
        }

        public virtual async Task SoftDeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var entity = await DbSet.FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted, cancellationToken);
            if (entity == null) return;

            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
        }

        public virtual Task UpdateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            DbSet.Update(entity);
            return Task.CompletedTask;
        }
    }
}
