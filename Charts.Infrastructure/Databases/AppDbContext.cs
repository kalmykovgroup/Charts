using System.Text;
using Charts.Domain.Contracts;
using Charts.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Charts.Infrastructure.Databases
{
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// Конструктор контекста данных.
        /// </summary>
        /// <param name="options">Параметры DbContext, передаваемые из DI (например, строка подключения).</param>
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {

        }

        public DbSet<Database> Databases { get; set; } 
        public DbSet<ChartReqTemplate> ChartReqTemplates { get; set; } 
     
        /// <summary>
        /// Конфигурация модели при построении (применяется при миграциях и инициализации контекста).
        /// Здесь задаются:
        /// - глобальные фильтры
        /// - snake_case конвенция
        /// - ValueConverters
        /// - связи между сущностями
        /// </summary>
        /// <param name="modelBuilder">Построитель модели EF Core.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

          

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
          

            

            // Snake_case после конфигураций
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                entity.SetTableName(ToSnakeCase(entity.GetTableName()!));
                foreach (var property in entity.GetProperties())
                {
                    property.SetColumnName(ToSnakeCase(property.Name));
                }
                foreach (var key in entity.GetKeys())
                {
                    key.SetName(ToSnakeCase(key.GetName()!));
                }
                foreach (var fk in entity.GetForeignKeys())
                {
                    fk.SetConstraintName(ToSnakeCase(fk.GetConstraintName()!));
                }
                foreach (var index in entity.GetIndexes())
                {
                    index.SetDatabaseName(ToSnakeCase(index.GetDatabaseName()!));
                }
            }

        }

        /// <summary>
        /// Переопределение SaveChanges для автоматического применения CreatedAt/UpdatedAt и soft delete.
        /// </summary>
        /// <returns>Количество затронутых записей.</returns>
        public override int SaveChanges()
        {
            ApplyAudit();
            return base.SaveChanges();
        }

        /// <summary>
        /// Переопределение SaveChangesAsync для автоматического применения CreatedAt/UpdatedAt и soft delete (асинхронно).
        /// </summary>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <returns>Количество затронутых записей.</returns>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyAudit();
            return await base.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Применение автоматического аудита (CreatedAt, UpdatedAt) и soft delete для отслеживаемых сущностей.
        /// </summary>
        private void ApplyAudit()
        {
            var now = DateTime.UtcNow;

            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = now;
                        entry.Entity.UpdatedAt = now;
                        break;

                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = now;
                        break;

                    case EntityState.Deleted:
                        // Применение мягкого удаления
                        entry.State = EntityState.Modified;
                        entry.Entity.IsDeleted = true;
                        entry.Entity.DeletedAt = now;
                        // entry.Entity.DeletedBy = "system"; // Можно установить текущего пользователя
                        break;
                }
            }
        }

        /// <summary>
        /// Преобразование строки в формат snake_case (используется для имен таблиц, полей, индексов и т.п.).
        /// </summary>
        /// <param name="input">Исходная строка.</param>
        /// <returns>Преобразованная строка.</returns>
        private static string ToSnakeCase(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            var sb = new StringBuilder();
            var state = SnakeCaseState.Start;

            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                if (char.IsUpper(c))
                {
                    switch (state)
                    {
                        case SnakeCaseState.Upper:
                            bool hasNext = (i + 1 < input.Length);
                            if (i > 0 && hasNext && char.IsLower(input[i + 1]))
                                sb.Append('_');
                            break;
                        case SnakeCaseState.Lower:
                        case SnakeCaseState.Start:
                            if (i > 0)
                                sb.Append('_');
                            break;
                    }
                    sb.Append(char.ToLowerInvariant(c));
                    state = SnakeCaseState.Upper;
                }
                else if (c == '_')
                {
                    sb.Append('_');
                    state = SnakeCaseState.Start;
                }
                else
                {
                    sb.Append(c);
                    state = SnakeCaseState.Lower;
                }
            }

            return sb.ToString();
        }

        private enum SnakeCaseState
        {
            Start,
            Lower,
            Upper
        }

    }
}
