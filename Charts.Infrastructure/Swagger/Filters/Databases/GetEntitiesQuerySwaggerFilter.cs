using Charts.Infrastructure.Databases.Seeder;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Charts.Infrastructure.Swagger.Filters.Databases
{
    public class GetEntitiesQuerySwaggerFilter : IOperationFilter
    {
        private static readonly string Route = "metadata/database/entities";

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            SwaggerParams.AddHeader(operation, context, Route, "X-Db", SeederIds.DatabaseId.ToString());
        }
    }
   
}
