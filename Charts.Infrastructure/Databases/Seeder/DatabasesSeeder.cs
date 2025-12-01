using Charts.Domain.Contracts.Types;
using Charts.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Charts.Infrastructure.Databases.Seeder
{
    public class DatabasesSeeder : ISeeder
    { 
        public int Order => 1;

        public async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            if (await dbContext.Databases.AnyAsync())
                return; 
          
            await dbContext.Databases.AddRangeAsync([
                    new Database()
                    {
                        Id =  Guid.NewGuid(),
                        Name = "default",
                        DatabaseVersion = "",
                        Description = "PostgreSQLConnectionLocal",
                        ConnectionString = "Host=localhost;Port=5432;Username=postgres;Password=koval007;Database=test; Include Error Detail=true;Pooling=false;Timeout=300;CommandTimeout=300",
                        Status = EntityStatus.Active,
                    },
                    new Database()
                    {
                        Id = SeederIds.DatabaseId,
                        Name = "mirax",
                        DatabaseVersion = "",
                        ConnectionString = "Host=127.0.0.1;Port=5454;Database=mirax;Username=postgres;Password=postgres;Pooling=true;Timeout=5;KeepAlive=30;Ssl Mode=Disable",
                        Status = EntityStatus.Active,
                    },
                    new Database()
                    {
                        Id = Guid.NewGuid(),
                        Name = "scenarios",
                        DatabaseVersion = "",
                        ConnectionString = "Host=localhost;Port=5432;Database=mirax;Username=postgres;Password=postgres;",
                        Status = EntityStatus.Active,
                    }
                ]);
            await dbContext.SaveChangesAsync();
        }
    }
}
