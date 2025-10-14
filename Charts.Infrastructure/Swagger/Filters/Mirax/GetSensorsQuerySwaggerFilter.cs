using Charts.Api.Swagger;
using Charts.Api.Infrastructure.Databases.Seeder;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Charts.Api.Infrastructure.Swagger.Filters.Mirax
{ 
    public class GetSensorsQuerySwaggerFilter : IOperationFilter
    {
        private static readonly string Route = "mirax/technical-runs/{technicalRunId:guid}/devices/{factoryNumber}/sensors";
        //0b9cfee7-4b28-4210-87cd-0acb9e42c330
        //2507365

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            SwaggerParams.AddHeader(operation, context, Route, "X-Db", SeederIds.DatabaseId.ToString());
            SwaggerParams.AddPath(operation, context, Route, "technicalRunId", "0b9cfee7-4b28-4210-87cd-0acb9e42c330");
            SwaggerParams.AddPath(operation, context, Route, "factoryNumber", "2507365");
        }
    }
}
