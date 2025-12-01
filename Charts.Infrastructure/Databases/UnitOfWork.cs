using Charts.Domain.Interfaces;

namespace Charts.Infrastructure.Databases
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _dbContext;

        public UnitOfWork(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken)
        {
            var dbTransaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            return new EfTransactionWrapper(dbTransaction);
        }
    }
}
