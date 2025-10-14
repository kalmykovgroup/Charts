namespace Charts.Api.Infrastructure.Databases.Seeder
{
    public static class SeederIds
    {
        public static Guid DatabaseId { get; set; } = Guid.Parse("77777777-0000-0000-0000-000000000011");
        public static Guid CreateChartReqTemplateId { get; set; } = Guid.Parse("77777777-0000-0000-0000-000000000000");
        public static Guid TestDefaultChartReqTemplateBaseId { get; set; } = Guid.Parse("77777777-0000-0000-0000-000000000000");
        public static Guid TestDefaultChartReqTemplateSensorId { get; set; } = Guid.Parse("77777777-0000-0000-0000-000000000001");
        public static Guid DefaultChartReqTemplateBaseId { get; set; } = Guid.Parse("77777777-0000-0000-0000-000000000222");
        public static Guid DefaultChartReqTemplateSensorId { get; set; } = Guid.Parse("77777777-0000-0000-0000-000000000223");
    }
}
