using Charts.Api.Application.Contracts.Metadata.Dtos;
using Charts.Api.Application.Models;
using Charts.Api.Domain.Contracts.Template;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;
using System.Text.Json;

namespace Charts.Api.Infrastructure.Databases.Configurations
{
    public sealed class ChartReqTemplateConfiguration : IEntityTypeConfiguration<ChartReqTemplate>
    {
        // Общие JSON-настройки для сериализации в jsonb
        private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web)
        {
            WriteIndented = false
        };

        // ValueComparer для List<FilterClause>
        private static readonly ValueComparer<List<FilterClause>> FilterListComparer =
            new(
                (a, b) =>
                    (a == null && b == null) ||
                    (a != null && b != null && a.SequenceEqual(b)),
                v => v == null ? 0 : v.Aggregate(0, (h, e) => HashCode.Combine(h, e.GetHashCode())),
                v => v == null ? new List<FilterClause>() : v.ToList()
            );

        // ValueComparer для List<SqlParam>
        private static readonly ValueComparer<List<SqlParam>> ParamListComparer =
            new(
                (a, b) =>
                    (a == null && b == null) ||
                    (a != null && b != null && a.SequenceEqual(b)),
                v => v == null ? 0 : v.Aggregate(0, (h, e) => HashCode.Combine(h, e.GetHashCode())),
                v => v == null ? new List<SqlParam>() : v.ToList()
            );

        // ValueComparer для FieldDto[]
        private static readonly ValueComparer<FieldDto[]> FieldsComparer =
            new(
                (a, b) =>
                    (a == null && b == null) ||
                    (a != null && b != null && a.SequenceEqual(b)),
                v => v == null ? 0 : v.Aggregate(0, (h, e) => HashCode.Combine(h, e.GetHashCode())),
                v => v == null ? Array.Empty<FieldDto>() : v.ToArray()
            );

        public void Configure(EntityTypeBuilder<ChartReqTemplate> b)
        {
            // ====== Table/Key
            b.ToTable("chart_req_templates");
            b.HasKey(x => x.Id);

            // ====== Scalar columns
            b.Property(x => x.Name)
                .HasMaxLength(200)
                .IsRequired();

            b.Property(x => x.Description)
                .HasMaxLength(1000);

            // ====== JSON: Entity (EntityDto) → jsonb
            b.Property(x => x.Entity)
                .HasColumnName("entity")
                .HasColumnType("jsonb")
                .IsRequired()
                .HasConversion(
                    v => JsonSerializer.Serialize(v, JsonOpts),
                    v => JsonSerializer.Deserialize<EntityDto>(v, JsonOpts)!);

            // ====== JSON: TimeField (FieldDto) → jsonb
            b.Property(x => x.TimeField)
                .HasColumnName("time_field")
                .HasColumnType("jsonb")
                .IsRequired()
                .HasConversion(
                    v => JsonSerializer.Serialize(v, JsonOpts),
                    v => JsonSerializer.Deserialize<FieldDto>(v, JsonOpts)!);

            // ====== JSON: Fields (FieldDto[]) → jsonb
            b.Property(x => x.SelectedFields)
                .HasColumnName("fields")
                .HasColumnType("jsonb")
                .IsRequired()
                .HasConversion(
                    v => JsonSerializer.Serialize(v, JsonOpts),
                    v => JsonSerializer.Deserialize<FieldDto[]>(v, JsonOpts)!)
                .Metadata.SetValueComparer(FieldsComparer);

            // ====== Relations
            b.HasOne(x => x.Database)
                .WithMany(d => d.ChartReqTemplates) // Updated to reference the collection
                .HasForeignKey(x => x.DatabaseId)
                .OnDelete(DeleteBehavior.Restrict);

            b.Property(x => x.OriginalFromMs)
            .HasColumnName("from")
            .HasColumnType("timestamp with time zone")
            .HasConversion(
                // long? → DateTime? (для сохранения в БД)
                v => v.HasValue
                    ? DateTimeOffset.FromUnixTimeMilliseconds(v.Value).UtcDateTime
                    : (DateTime?)null,
                // DateTime? → long? (для чтения из БД)
                v => v.HasValue
                    ? new DateTimeOffset(v.Value, TimeSpan.Zero).ToUnixTimeMilliseconds()
                    : (long?)null
            );

            b.Property(x => x.OriginalToMs)
                .HasColumnName("to")
                .HasColumnType("timestamp with time zone")
                .HasConversion(
                    // long? → DateTime? (для сохранения в БД)
                    v => v.HasValue
                        ? DateTimeOffset.FromUnixTimeMilliseconds(v.Value).UtcDateTime
                        : (DateTime?)null,
                    // DateTime? → long? (для чтения из БД)
                    v => v.HasValue
                        ? new DateTimeOffset(v.Value, TimeSpan.Zero).ToUnixTimeMilliseconds()
                        : (long?)null
                );

            // ====== JSON: Where (List<FilterClause>) → jsonb
            b.Property(x => x.Where)
                .HasColumnName("where")
                .HasColumnType("jsonb")
                .HasConversion(
                    v => v == null ? null : JsonSerializer.Serialize(v, JsonOpts),
                    v => v == null ? null : JsonSerializer.Deserialize<List<FilterClause>>(v, JsonOpts)!)
                .Metadata.SetValueComparer(FilterListComparer);

            // ====== SQL: Sql (SqlFilter -> только WhereSql) → text (sql_where)
            b.Property(x => x.Sql)
                .HasColumnName("sql_where")
                .HasColumnType("text")
                .HasConversion(
                    v => v == null ? null : v.WhereSql,
                    v => v == null ? null : new SqlFilter(v)
                );

            // ====== JSON: Params (List<SqlParam>) → jsonb
            b.Property(x => x.Params)
                .HasColumnName("params")
                .HasColumnType("jsonb")
                .HasConversion(
                    v => v == null ? null : JsonSerializer.Serialize(v, JsonOpts),
                    v => v == null ? null : JsonSerializer.Deserialize<List<SqlParam>>(v, JsonOpts)!)
                .Metadata.SetValueComparer(ParamListComparer);

            // ====== Indexes / Constraints
            b.HasIndex(x => new { x.DatabaseId, x.Name })
                .IsUnique()
                .HasDatabaseName("ux_chart_template_dbid_name");

            b.HasIndex(x => new { x.DatabaseId, x.Entity, x.TimeField })
                .HasDatabaseName("ix_chart_template_dbid_entity_time");
        }
    }
}