 
using Charts.Api.Application.Contracts.Metadata.Dtos;
using Charts.Api.Domain.Contracts.Types;
using Charts.Api.Infrastructure.Databases;
using Charts.Api.Infrastructure.Utils;
using System.Data;
using System.Data.Common;
using Charts.Api.Application.Interfaces;

namespace Charts.Api.Infrastructure.Services;



public sealed class EntityMetadataService : IEntityMetadataService
{
    private static Exception NotImpl(DbProviderType p, string feature) =>
        new NotSupportedException($"{feature}: нужно реализовать для {p}.");

    // Универсальная помощь по созданию команд/параметров
    private static DbCommand Cmd(DbConnection con, string sql)
    {
        var c = con.CreateCommand();
        c.CommandText = sql;
        c.CommandType = CommandType.Text;
        return c;
    }
    private static void P(DbCommand c, string name, object? value, DbType type = DbType.String)
    {
        var p = c.CreateParameter();
        p.ParameterName = name;
        p.Value = value ?? DBNull.Value;
        p.DbType = type;
        c.Parameters.Add(p);
    }

    // ------------------- ПУБЛИЧНЫЕ МЕТОДЫ -------------------

    /// <summary>
    /// Вернёт все сущности СРАЗУ С ПОЛЯМИ.
    /// </summary>
    public async Task<IReadOnlyList<EntityDto>> GetEntitiesAsync(
        DbConnection con, DbProviderType provider, CancellationToken ct)
        => provider switch
        {
            DbProviderType.PostgreSql => await GetEntitiesWithFieldsPgAsync(con, ct),
            _ => throw NotImpl(provider, "Получение списка сущностей с полями")
        };

    /// <summary>
    /// Вернёт одну сущность, схему/таблицу и её поля.
    /// </summary>
    public async Task<(EntityDto Entity, string Schema, string Table, IReadOnlyList<FieldDto> Fields)>
        GetEntityFieldsAsync(DbConnection con, DbProviderType provider, string entity, CancellationToken ct)
        => provider switch
        {
            DbProviderType.PostgreSql => await GetEntityFieldsPgAsync(con, entity, ct),
            _ => throw NotImpl(provider, "Получение полей сущности")
        };

    public (string Schema, string Table, string TimeColumn, string ValueColumn)
        ValidateAndResolveColumns((string Schema, string Table, IReadOnlyList<FieldDto> Fields) meta, string? timeField, string valueField)
    {
        FieldDto timeMeta = timeField is { Length: > 0 }
            ? ResolveField(meta.Fields, timeField)
            : meta.Fields.FirstOrDefault(f => f.IsTime.Value)
              ?? throw new InvalidOperationException("Time field not specified and not found");

        if (!timeMeta.IsTime.HasValue)
            throw new InvalidOperationException($"Field '{timeMeta.Name}' is not a time column");

        var valMeta = ResolveField(meta.Fields, valueField);
        if (!valMeta.IsNumeric.HasValue)
            throw new InvalidOperationException($"Field '{valMeta.Name}' is not numeric");

        return (meta.Schema, meta.Table, timeMeta.Name, valMeta.Name);
    }

    // ------------------- POSTGRES: ВСЕ СУЩНОСТИ СРАЗУ С ПОЛЯМИ -------------------

    private static async Task<IReadOnlyList<EntityDto>> GetEntitiesWithFieldsPgAsync(
        DbConnection con, CancellationToken ct)
    {
        const string sql = @"
select
    n.nspname                                         as schema,
    c.relname                                         as table,
    a.attname                                         as column_name,
    format_type(a.atttypid, a.atttypmod)              as data_type,
    coalesce(bt.typcategory, t.typcategory)::text     as base_cat
from pg_catalog.pg_class c
join pg_catalog.pg_namespace n on n.oid = c.relnamespace
join pg_catalog.pg_attribute a on a.attrelid = c.oid and a.attnum > 0 and not a.attisdropped
join pg_catalog.pg_type t on t.oid = a.atttypid
left join pg_catalog.pg_type bt on t.typtype = 'd' and bt.oid = t.typbasetype
where n.nspname not in ('pg_catalog','information_schema')
  and c.relkind in ('r','v','m','p','f')
order by n.nspname, c.relname, a.attnum;";

        var map = new Dictionary<string, List<FieldDto>>(StringComparer.Ordinal);
        await using var cmd = Cmd(con, sql);
        await using var rdr = await cmd.ExecuteReaderAsync(ct);

        while (await rdr.ReadAsync(ct))
        {
            var schema = rdr.GetString(0);
            var table = rdr.GetString(1);
            var col = rdr.GetString(2);
            var type = rdr.GetString(3);
            var baseCat = rdr.GetString(4);
            var cat = baseCat.Length > 0 ? baseCat[0] : '?';

            var key = $"{schema}.{table}";

            if (!map.TryGetValue(key, out var list))
            {
                list = new List<FieldDto>(32);
                map[key] = list;
            }

            list.Add(new FieldDto(
                name: col,
                type: type,
                sqlParamType: SqlParamTypeMapper.MapDbTypeToParam(DbProviderType.PostgreSql, type),
                isNumeric: cat == 'N',
                isTime: cat == 'D'
            ));
        }

        // Сформировать итоговые DTO
        var result = map
            .OrderBy(kv => kv.Key, StringComparer.Ordinal)
            .Select(kv => new EntityDto(kv.Key, kv.Value))
            .ToList();

        return result;
    }


    // ------------------- POSTGRES: ОДНА СУЩНОСТЬ С ПОЛЯМИ -------------------

    private static async Task<(EntityDto Entity, string Schema, string Table, IReadOnlyList<FieldDto> Fields)>
        GetEntityFieldsPgAsync(DbConnection con, string entity, CancellationToken ct)
    {
        // 1) Разбор входа и попытка резолва фактических schema/table
        var (schemaInput, tableInput) = DbIdentifiers.ParseEntityName(entity, "public");

        var resolved = await ResolveEntityNamePgAsync(con, schemaInput, tableInput, ct);
        if (resolved is null)
            throw new InvalidOperationException($"Relation not found: {schemaInput}.{tableInput}");
        var (schema, table) = resolved.Value;

        // 2) Прочитать колонки одной таблицы/представления
        const string colsSql = @"
select
  a.attname                                      as column_name,
  format_type(a.atttypid, a.atttypmod)          as data_type,
  coalesce(bt.typname, t.typname)               as base_typname,
  coalesce(bt.typcategory, t.typcategory)::text as base_cat
from pg_catalog.pg_attribute a
join pg_catalog.pg_class c on c.oid = a.attrelid
join pg_catalog.pg_namespace n on n.oid = c.relnamespace
join pg_catalog.pg_type t on t.oid = a.atttypid
left join pg_catalog.pg_type bt on t.typtype = 'd' and bt.oid = t.typbasetype
where n.nspname = @schema and c.relname = @table
  and a.attnum > 0 and not a.attisdropped
order by a.attnum;";

        var fields = new List<FieldDto>(64);
        await using (var cmd = Cmd(con, colsSql))
        {
            P(cmd, "schema", schema, DbType.String);
            P(cmd, "table", table, DbType.String);

            await using var rdr = await cmd.ExecuteReaderAsync(ct);
            while (await rdr.ReadAsync(ct))
            {
                var name = rdr.GetString(0);
                var dataType = rdr.GetString(1);
                var baseCat = rdr.GetString(3);
                var cat = baseCat.Length > 0 ? baseCat[0] : '?';

                fields.Add(new FieldDto(
                    name: name,
                    type: dataType,
                    isNumeric: cat == 'N',
                    isTime: cat == 'D',
                    sqlParamType: SqlParamTypeMapper.MapDbTypeToParam(DbProviderType.PostgreSql, dataType)
                ));
            }
        }

        var dto = new EntityDto($"{schema}.{table}", fields);
        return (dto, schema, table, fields);
    }

    // ------------------- ВСПОМОГАТЕЛЬНОЕ -------------------

    private static async Task<(string Schema, string Table)?> ResolveEntityNamePgAsync(
        DbConnection con, string schema, string table, CancellationToken ct)
    {
        // точное имя в кавычках
        if (await ToRegclassExistsPgAsync(con, $"\"{schema}\".\"{table}\"", ct)) return (schema, table);

        // всё в нижнем регистре
        var sLower = schema.ToLowerInvariant();
        var tLower = table.ToLowerInvariant();
        if (await ToRegclassExistsPgAsync(con, $"{sLower}.{tLower}", ct)) return (sLower, tLower);

        // "мягкий" поиск: без подчёркиваний, нечувствителен к регистру
        const string fuzzySql = @"
select n.nspname, c.relname
from pg_catalog.pg_class c
join pg_catalog.pg_namespace n on n.oid = c.relnamespace
where n.nspname not in ('pg_catalog','information_schema')
  and replace(lower(c.relname),'_','') = replace(lower(@t),'_','')
  and replace(lower(n.nspname),'_','') = replace(lower(@s),'_','')
limit 1;";
        await using (var cmd = Cmd(con, fuzzySql))
        {
            P(cmd, "s", schema, DbType.String);
            P(cmd, "t", table, DbType.String);
            await using var rdr = await cmd.ExecuteReaderAsync(ct);
            if (await rdr.ReadAsync(ct))
                return (rdr.GetString(0), rdr.GetString(1));
        }

        // найти любую подходящую таблицу по имени (приоритет public)
        const string findAny = @"
select n.nspname, c.relname
from pg_catalog.pg_class c
join pg_catalog.pg_namespace n on n.oid = c.relnamespace
where n.nspname not in ('pg_catalog','information_schema')
  and replace(lower(c.relname),'_','') = replace(lower(@t),'_','')
order by case when n.nspname='public' then 0 else 1 end, n.nspname
limit 1;";
        await using (var cmd2 = Cmd(con, findAny))
        {
            P(cmd2, "t", table, DbType.String);
            await using var rdr = await cmd2.ExecuteReaderAsync(ct);
            if (await rdr.ReadAsync(ct))
                return (rdr.GetString(0), rdr.GetString(1));
        }

        return null;
    }

    private static async Task<bool> ToRegclassExistsPgAsync(
        DbConnection con, string regclassText, CancellationToken ct)
    {
        await using var cmd = Cmd(con, "select to_regclass(@r) is not null");
        P(cmd, "r", regclassText, DbType.String);
        var obj = await cmd.ExecuteScalarAsync(ct);
        return obj is bool b && b;
    }

    private static FieldDto ResolveField(IReadOnlyList<FieldDto> fields, string name)
    {
        // точный матч
        var f = fields.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.Ordinal));
        if (f is not null) return f;

        // без учёта регистра
        f = fields.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));
        if (f is not null) return f;

        // "мягкий" матч: убираем подчёркивания и приводим к lower
        static string Norm(string s) => s.Replace("_", "", StringComparison.Ordinal).ToLowerInvariant();
        var target = Norm(name);
        f = fields.FirstOrDefault(x => Norm(x.Name) == target);

        return f ?? throw new InvalidOperationException($"Поле '{name}' не найдено");
    } 

}


