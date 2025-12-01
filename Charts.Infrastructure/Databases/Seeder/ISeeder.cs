namespace Charts.Infrastructure.Databases.Seeder
{
    public interface ISeeder
    {
        /// <summary>
        /// Порядок выполнения сидера (чем меньше — тем раньше).
        /// </summary>
        int Order { get; }

        /// <summary>
        /// Выполнить сидирование.
        /// </summary>
        Task SeedAsync(IServiceProvider serviceProvider);
    }
}
