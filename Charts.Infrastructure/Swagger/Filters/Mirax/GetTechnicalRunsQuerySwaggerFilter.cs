using Charts.Infrastructure.Databases.Seeder;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Charts.Infrastructure.Swagger.Filters.Mirax
{ 
    public class GetTechnicalRunsQuerySwaggerFilter : IOperationFilter
    {
        private static readonly string Route = "mirax/technical-runs";

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            SwaggerParams.AddHeader(operation, context, Route, "X-Db", SeederIds.DatabaseId.ToString());
        }
    }
}
