using System.Text.Json;
using Charts.Domain.Contracts.Metadata.Dtos;
using Charts.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Charts.Infrastructure.Databases.Configurations
{

    public sealed class DatabaseConfiguration : IEntityTypeConfiguration<Database>
    {
        // Общие JSON-настройки для сериализации в jsonb
        private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web)
        {
            WriteIndented = false
        };

        // ValueComparer для List<SqlParam>
        private static readonly ValueComparer<List<EntityDto>> EntityDtoListComparer =
            new(
                (a, b) =>
                    (a == null && b == null) ||
                    (a != null && b != null && a.SequenceEqual(b)),
                v => v == null ? 0 : v.Aggregate(0, (h, e) => HashCode.Combine(h, e.GetHashCode())),
                v => v == null ? new List<EntityDto>() : v.ToList()
            );

        public void Configure(EntityTypeBuilder<Database> e)
        {
            e.ToTable("databases");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(128).IsRequired();
            e.Property(x => x.ConnectionString).IsRequired();
            e.Property(x => x.DatabaseStatus).HasConversion<int>().IsRequired();
            e.HasIndex(x => x.Name).IsUnique();

            e.Property(x => x.Entities)
                .HasColumnName("entities")
                .HasColumnType("jsonb")
                .HasConversion(
                    v => v == null ? null : JsonSerializer.Serialize(v, JsonOpts),
                    v => v == null ? new List<EntityDto>() : JsonSerializer.Deserialize<List<EntityDto>>(v, JsonOpts)!)
                .Metadata.SetValueComparer(EntityDtoListComparer);
        }
    }
}
