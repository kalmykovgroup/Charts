namespace Charts.Api.Application.Interfaces
{
    public interface ITransaction : IAsyncDisposable, IDisposable
    {
        Task CommitAsync(CancellationToken cancellationToken);
        Task RollbackAsync(CancellationToken cancellationToken);
    }
}
