using System.Data.Common;
using System.Text.Json;
using System.Text.Json.Serialization;
using Charts.Api.Extensions;
using Charts.Api.Halpers;
using Charts.Api.Middleware;
using Charts.Application.QueryAndCommands.Metadata.Databases;
using Charts.Domain.Contracts;
using Charts.Domain.Interfaces;
using Charts.Domain.Interfaces.Mirax;
using Charts.Domain.Interfaces.Repositories;
using Charts.Infrastructure.Databases;
using Charts.Infrastructure.Mapper;
using Charts.Infrastructure.Options;
using Charts.Infrastructure.Repositories;
using Charts.Infrastructure.Services;
using Charts.Infrastructure.Startup;
using Charts.Infrastructure.Swagger;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Npgsql;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
// AddCommonSwaggerServices, AddSeederServices
// SQL Server
// SQLite
// MySQL/MariaDB
// Postgres

namespace Charts.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                 .MinimumLevel.Debug() // <-- ВКЛЮЧАЕМ DEBUG ГЛОБАЛЬНО
                 .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                 .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                 .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
                 .Enrich.FromLogContext()
                 .Enrich.WithThreadId()
                 .Enrich.WithEnvironmentName()
                 .Enrich.WithProcessId()
                 .WriteTo.Console(
                     theme: AnsiConsoleTheme.Code,
                     outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}"
                 )
                 .WriteTo.File("logs/app.log", rollingInterval: RollingInterval.Day)
                 .CreateLogger();


            var builder = WebApplication.CreateBuilder(args);
     

            builder.Host.UseWindowsService();

            builder.Host.UseSerilog();

            // ---------- AutoMapper ----------
            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<ChartReqTemplatesProfile>();
                cfg.AddProfile<DatabasesProfile>();
            });

            // ---------- EF/Logging ----------
            var loggerFactory = LoggerFactory.Create(logging =>
            {
                logging.AddConsole()
                       .AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning)
                       .AddFilter("Microsoft.EntityFrameworkCore.Infrastructure", LogLevel.Warning);
            });

            // ---------- UoW ----------
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


            //Repository
            builder.Services.AddScoped<IChartReqTemplateRepository, ChartReqTemplateRepository>();
            builder.Services.AddScoped<IDatabaseRepository, DatabaseRepository>();
            builder.Services.AddScoped<IMiraxRepository, MiraxRepository>();

            // ---------- Default AppDb DataSource (Postgres как meta-хранилище) ----------
            var defaultCs = builder.Configuration.GetConnectionString("DefaultConnection")
                            ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection is missing.");

            var dsb = new NpgsqlDataSourceBuilder(defaultCs);
            dsb.UseLoggerFactory(loggerFactory);
            dsb.EnableDynamicJson(); // если используешь jsonb c динамическим JSON
            var defaultDataSource = dsb.Build();
            builder.Services.AddSingleton(defaultDataSource);

            builder.Services.Configure<HostOptions>(o =>
            {
                o.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
            });

            // --- Readiness флаг и фоновые ретраи старта ---
            builder.Services.AddSingleton<IAppReadiness, AppReadiness>();
            builder.Services.AddHostedService<MetaDbWarmupWorker>();

            // --- HealthChecks ---
            builder.Services.AddHealthChecks()
                .AddCheck<ReadinessHealthCheck>("ready");

            // EF Core контекст для внутренней БД приложения
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(defaultDataSource)
                       .UseLoggerFactory(loggerFactory)
                       .EnableDetailedErrors());

            // ---------- Реестр фабрик провайдеров ----------
            builder.Services.AddSingleton<IDbProviderRegistry>(sp =>
                new DbProviderRegistry(new Dictionary<string, DbProviderFactory>(StringComparer.OrdinalIgnoreCase)
                {
                    ["Npgsql"] = NpgsqlFactory.Instance,
                    ["Microsoft.Data.SqlClient"] = SqlClientFactory.Instance,
                    ["MySqlConnector"] = MySqlConnectorFactory.Instance,
                    ["Sqlite"] = SqliteFactory.Instance,
                })
            );

            // ---------- Реестр подключений (читает из AppDbContext) ----------
            builder.Services.AddSingleton<IDatabaseRegistry, DatabaseRegistry>();

            // ---------- Текущая база из запроса (ID-only) ----------
            builder.Services.AddScoped<ICurrentDb, CurrentDb>();
            builder.Services.AddScoped<IRequestDbKeyAccessor, RequestDbKeyAccessor>();

            // ---------- Middleware (IMiddleware) ----------
            builder.Services.AddTransient<RequestDbKeyMiddleware>();



            // ---------- Сервисы графиков/метаданных ----------  



            // Регистрируем новые
            builder.Services.AddSingleton<IWhereCompiler, PostgresWhereCompiler>();
            builder.Services.AddSingleton<ISqlRequestFactory, PostgresSqlRequestFactory>();
            builder.Services.AddSingleton<ITimeColumnInspector, PostgresTimeColumnInspector>();
            builder.Services.AddSingleton<IRawDataExecutor, PostgresRawDataExecutor>();
            builder.Services.AddSingleton<IChartQueryPlanner, ChartQueryPlanner>();
            builder.Services.AddSingleton<IChartDataService, ChartDataService>();


            // Ридер сырых рядов (stateless). Можно AddScoped, если хочешь унифицировать.
           // builder.Services.AddSingleton<IRawSeriesReader, RawSeriesReader>();

            // Бининг — чистая логика, держим одним инстансом
            builder.Services.AddSingleton<IBucketingService, BucketingService>();

            // Метаданные сущностей/полей
            builder.Services.AddScoped<IEntityMetadataService, EntityMetadataService>();



            builder.Services.AddControllers().AddJsonOptions(o =>
            {
                o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                o.JsonSerializerOptions.Converters.Add(new DateTimeOffsetUtcJsonConverter());
            });

            builder.Services.AddOpenApi();
            builder.Services.AddCommonSwaggerServices();

            // ---------- Options ----------
            builder.Services.Configure<ChartBucketingOptions>(builder.Configuration.GetSection("Charts:ChartBucketing"));
            builder.Services.Configure<ChartDataOptions>(builder.Configuration.GetSection("Charts:Data"));
            builder.Services.Configure<ChartsOptions>(builder.Configuration.GetSection("Charts"));

            builder.Services.Configure<MetaDbOptions>(builder.Configuration.GetSection("MetaDb"));

            // ---------- CORS ----------
            var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontendAndApi", policy =>
                    policy.WithOrigins(allowedOrigins)
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials());
            });

            // ---------- MediatR ----------
            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(
                    typeof(Program).Assembly,
                    typeof(GetAllDatabasesQuery).Assembly   // <— сборка с хендлерами
                );
            });

            // ---------- Сидеры ----------
            builder.Services.AddMemoryCache();
            builder.Services.AddSeederServices(builder.Configuration);

            // Readiness
            builder.Services.AddSingleton<IAppReadiness, AppReadiness>();

            // ===================================================================
            var app = builder.Build();


            app.UseExceptionHandler(errApp =>
            {
                errApp.Run(async context =>
                {
                    var feature = context.Features.Get<IExceptionHandlerPathFeature>();
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "application/json";

                    var payload = new
                    {
                        message = feature?.Error?.Message,
#if DEBUG
                        details = feature?.Error?.ToString()
#endif
                    };
                    await context.Response.WriteAsJsonAsync(payload);
                });
            });

          app.Use(async (ctx, next) =>
             {
                 var ready = ctx.RequestServices.GetRequiredService<IAppReadiness>();
                 var path = ctx.Request.Path;

                 // Пропускаем health/openapi/swagger для проверки состояния
                 if (!ready.Ready
                     && !path.StartsWithSegments("/health")
                     && !path.StartsWithSegments("/swagger")
                     && !path.StartsWithSegments("/openapi"))
                 {
                     ctx.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                     await ctx.Response.WriteAsync("Service is not ready. Try later.");
                     return;
                 }

                 await next();
             });


            // ---------- Dev Swagger ----------
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Charts API v1"));
            }

            // ---------- Pipeline ----------
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("AllowFrontendAndApi");

            // Middleware для dbId
            app.UseMiddleware<RequestDbKeyMiddleware>();

            app.UseAuthentication();
            app.UseAuthorization();

            // ✅ СТАТИКА
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    // Агрессивное кеширование для /assets (JS/CSS с хешами)
                    if (ctx.Context.Request.Path.StartsWithSegments("/assets"))
                    {
                        ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=31536000,immutable");
                    }
                }
            });

            // API endpoints
            app.MapControllers();

            // Health checks
            app.MapGet("d/health/db", async (ICurrentDb currentDb, CancellationToken ct) =>
            {
                await using var con = await currentDb.OpenConnectionAsync(ct);
                await using var cmd = con.CreateCommand();
                cmd.CommandText = "SELECT 1";
                var v = await cmd.ExecuteScalarAsync(ct);
                return Results.Ok(new { dbId = currentDb.Key, ok = v is not null });
            }).WithMetadata(new RequireDbKeyAttribute());

            app.MapGet("d/registry", (IDatabaseRegistry reg) =>
            {
                var items = reg.Snapshot
                    .OrderBy(kv => kv.Key)
                    .Select(kv => new
                    {
                        id = kv.Key,
                        provider = kv.Value.Provider.ToString(),
                        version = kv.Value.DatabaseVersion
                    });
                return Results.Ok(items);
            });

            app.MapPost("d/registry/reload", async (IDatabaseRegistry reg, CancellationToken ct) =>
            {
                await reg.ReloadAsync(ct);
                return Results.Ok(new { ok = true, count = reg.Snapshot.Count });
            });

            app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = _ => false });
            app.MapHealthChecks("/health/ready", new HealthCheckOptions { Predicate = r => r.Name == "ready" });

            // ✅ SPA FALLBACK — всё остальное → index.html для React Router
            app.MapFallbackToFile("index.html");



            app.Map("/error", (HttpContext ctx) =>
            {
                // Можно логировать здесь. Возвращаем ProblemDetails.
                return Results.Problem(statusCode: 500, title: "Server error");
            });


            await app.RunAsync();
        }
    }
}
