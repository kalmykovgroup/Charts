using Charts.Infrastructure.Databases.Seeder;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Charts.Infrastructure.Swagger.Filters.Mirax
{ 
    public class GetPortableDevicesQuerySwaggerFilter : IOperationFilter
    {
        private static readonly string Route = "mirax/technical-runs/{technicalRunId:guid}/devices";
        //0b9cfee7-4b28-4210-87cd-0acb9e42c330
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            SwaggerParams.AddHeader(operation, context, Route, "X-Db", SeederIds.DatabaseId.ToString()); 
            SwaggerParams.AddPath(operation, context, Route, "technicalRunId", "0b9cfee7-4b28-4210-87cd-0acb9e42c330");
        }
    }
}
