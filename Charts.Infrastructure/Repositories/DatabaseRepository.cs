using Charts.Api.Application.Interfaces.Repositories;
using Charts.Api.Application.Models;
using Charts.Api.Infrastructure.Databases;

namespace Charts.Api.Infrastructure.Repositories
{
    public class DatabaseRepository(AppDbContext dbContext) : RepositoryBase<Database>(dbContext), IDatabaseRepository
    {
    }
}
