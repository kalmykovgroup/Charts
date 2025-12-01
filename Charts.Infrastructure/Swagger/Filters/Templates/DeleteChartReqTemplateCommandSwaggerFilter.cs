using Charts.Infrastructure.Databases.Seeder;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Charts.Infrastructure.Swagger.Filters.Templates
{
    public class DeleteChartReqTemplateCommandSwaggerFilter : IOperationFilter
    {
        private static readonly string Route = "templates/delete/{id}";
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        { 
            SwaggerParams.Add(operation, context, Route, "id", SeederIds.CreateChartReqTemplateId.ToString());
            SwaggerParams.AddHeader(operation, context, Route, "X-Db", SeederIds.DatabaseId.ToString());
        }
    }
}
