using Charts.Api.Infrastructure.Databases.Seeder;

namespace Charts.Api.Infrastructure.Databases.Seed
{
    public class SeederPipeline
    {
        private readonly IEnumerable<ISeeder> _seeders;

        public SeederPipeline(IEnumerable<ISeeder> seeders)
        {
            // Автоматически сортируем по порядку
            _seeders = seeders.OrderBy(s => s.Order);
        }

        public async Task ExecuteAsync(IServiceProvider serviceProvider)
        {
            foreach (var seeder in _seeders)
            {
                var seederName = seeder.GetType().Name;
                Console.WriteLine($"Running seeder: {seederName}...");
                await seeder.SeedAsync(serviceProvider);
            }
        }
    }
}
