using Charts.Api.Swagger;
using Charts.Api.Application.Contracts.Metadata.Dtos;
using Charts.Api.Application.Contracts.Template.Requests;
using Charts.Api.Domain.Contracts.Template;
using Charts.Api.Domain.Contracts.Types;
using Charts.Api.Infrastructure.Databases.Seeder;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Charts.Api.Swagger.Filters.Templates
{
    public class CreateChartReqTemplateRequestSwaggerFilter : IOperationFilter
    {
        private static readonly string Route = "templates/create";
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var request = new CreateChartReqTemplateRequest()
            {
                Id = SeederIds.CreateChartReqTemplateId,
                Name = "Тестовый шаблон",
                Description = "",
                DatabaseId = SeederIds.DatabaseId,
                Entity = new EntityDto()
                {
                    Name = "DeviceEntity"
                },
                TimeField = new FieldDto()
                {
                    Name = "CreateDate",
                    Type = "timestamp with time zone"
                },
                Where = new List<FilterClause>()
                {
                    new FilterClause(new FieldDto()
                    {
                        Name = "FactoryNumber",
                    Type = "string"
                    }, FilterOp.Eq, "{{key}}")
                },
                Params = []
                
            }; 
             

        SwaggerParams.Add(operation, context, Route, request);
        SwaggerParams.AddHeader(operation, context, Route, "X-Db", SeederIds.DatabaseId.ToString());
        }
    }
}
