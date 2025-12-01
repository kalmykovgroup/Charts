using Charts.Domain.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace Charts.Infrastructure.Databases
{
    public class EfTransactionWrapper : ITransaction
    {
        private readonly IDbContextTransaction _transaction;

        public EfTransactionWrapper(IDbContextTransaction transaction)
        {
            _transaction = transaction;
        }

        public Task CommitAsync(CancellationToken cancellationToken)
        {
            return _transaction.CommitAsync(cancellationToken);
        }

        public Task RollbackAsync(CancellationToken cancellationToken)
        {
            return _transaction.RollbackAsync(cancellationToken);
        }

        public ValueTask DisposeAsync()
        {
            return _transaction.DisposeAsync();
        }

        public void Dispose()
        {
            _transaction.Dispose();
        }
    }
}
