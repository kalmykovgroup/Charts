using Charts.Infrastructure.Databases.Seeder;

namespace Charts.Api.Extensions
{
    public static class BuilderSeederExtensions
    {
        public static IServiceCollection AddSeederServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ISeeder, DatabasesSeeder>(); 
            services.AddScoped<ISeeder, ChartReqTemplatesSeeder>(); 

            services.AddScoped<SeederPipeline>();

            return services;
        }
    }
}
