namespace Charts.Api.Application.Interfaces
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken);
    }
}
