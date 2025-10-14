using Charts.Api.Swagger;
using Charts.Api.Infrastructure.Databases.Seeder;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Charts.Api.Infrastructure.Swagger.Filters.Databases
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
