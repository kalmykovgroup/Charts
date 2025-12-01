using Charts.Domain.Interfaces.Repositories;
using Charts.Domain.Models;
using Charts.Infrastructure.Databases;

namespace Charts.Infrastructure.Repositories
{
    public class DatabaseRepository(AppDbContext dbContext) : RepositoryBase<Database>(dbContext), IDatabaseRepository
    {
    }
}
