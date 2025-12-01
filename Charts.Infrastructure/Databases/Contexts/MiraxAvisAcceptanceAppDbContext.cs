/*
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Mirax.AvisAcceptanceApp.Models.Entity;
using Mirax.AvisAcceptanceApp.Models.Entity.Configuration;
using Mirax.AvisAcceptanceApp.Models.Entity.ModBusDevice;
using Mirax.AvisAcceptanceApp.Models.Entity.ModBusDevice.PortableDeviceModel;
using Mirax.AvisAcceptanceApp.Service.TechTest.Models;
using Mirax.AvisAcceptanceApp.Service.TemperatureAnalysis.Models;
using Mirax.AvisAcceptanceApp.Share.CopyModels;

namespace Charts.Infrastructure.Databases.Contexts
{
    public class MiraxAvisAcceptanceAppDbContext : DbContext
    {

        public DbSet<ClimateGas> ClimateGasesConfigurations { get; set; }
        public DbSet<ClimateTemperature> ClimateTemperaturesConfigurations { get; set; }
        public DbSet<Gas> Gases { get; set; }
        public DbSet<ClimateTestTimestamp> ClimateTestTimestamps { get; set; }
        public DbSet<ClimateTestSubOperationsTimestamp> ClimateTestSubOperationsTimestamps { get; set; }
        public DbSet<TypeDefinitions> TypeDefinitions { get; set; }
        public DbSet<SenerType> SenerTypes { get; set; }
        public DbSet<TechRunLog> TechRunLogs { get; set; }
        public DbSet<ClimateTestLog> ClimateTestLogs { get; set; }
        public DbSet<PortableDevice> PortableDevices { get; set; }
        public DbSet<Sensor> Sensors { get; set; }
        public DbSet<TechnicalRunToStart> TechnicalRunsToStart { get; set; }
        public DbSet<ClimateTestToStart> ClimateTestsToStart { get; set; }
        public DbSet<ClimateChamberLog> ClimateChamberLogs { get; set; }
        public DbSet<ClimateStepLog> ClimateStepLogs { get; set; }
        public DbSet<GasToClimate> GasToClimate { get; set; }
        public DbSet<SensorConfigurationPreset> SensorConfigurationPresets { get; set; }
        public DbSet<TemperatureTestToRun> TemperatureTestToRuns { get; set; }
        public DbSet<SensorModificationParameters> SensorModificationParameters { get; set; }
        public DbSet<TemperatureAnalysisModel> TemperatureAnalysisModels { get; set; }
        public DbSet<TemperatureData> TemperatureDatas { get; set; }
        public DbSet<DeviceConfigurationPreset> DeviceConfigurationPresets { get; set; }
        public DbSet<SensorEvent> SensorEvents { get; set; }
        public DbSet<GasEvent> GasEvents { get; set; }
        public DbSet<Error> Errors { get; set; } 

        public MiraxAvisAcceptanceAppDbContext(DbContextOptions<MiraxAvisAcceptanceAppDbContext> options) : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            ChangeTracker.AutoDetectChangesEnabled = false;
        }

        // Жёсткий запрет любых записей
        public override int SaveChanges(bool acceptAllChangesOnSuccess) =>
            throw new NotSupportedException("Read-only context");
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken ct = default) =>
            throw new NotSupportedException("Read-only context");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DeviceEntity>()
                .HasDiscriminator<string>("Discriminator")
                .HasValue<PortableDevice>("PortableDevice")
                .HasValue<Sensor>("Sensor");

            modelBuilder.Entity<DeviceEntity>()
                .HasIndex(e => e.CreateDate);

        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<DateTime>()
                .HaveConversion<DateTimeTruncateConverter>();
        }

        /// <summary>
        /// Обрезание милисекунд во времени 
        /// </summary>
        public class DateTimeTruncateConverter : ValueConverter<DateTime, DateTime>
        {
            public DateTimeTruncateConverter()
                : base(
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc).AddTicks(-(v.Ticks % TimeSpan.TicksPerSecond)),
                    v => v
                )
            { }
        }
    }
}
*/
