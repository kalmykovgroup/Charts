using Charts.Api.Swagger;
using Charts.Api.Infrastructure.Databases.Seeder;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Charts.Api.Infrastructure.Swagger.Filters.Databases
{
    public class GetEntityFieldsQuerySwaggerFilter : IOperationFilter
    {
        private static readonly string Route = "metadata/database/fields";
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            SwaggerParams.AddHeader(operation, context, Route, "X-Db", SeederIds.DatabaseId.ToString());
            SwaggerParams.Add(operation, context, Route, "entity", "BatteryVoltage");
        }
    }
}
