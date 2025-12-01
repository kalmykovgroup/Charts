using Charts.Domain.Contracts.Metadata.Dtos;
using Charts.Domain.Contracts.Template.Requests;
using Charts.Infrastructure.Databases.Seeder;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Charts.Infrastructure.Swagger.Filters.Templates
{
    public class UpdateChartReqTemplateRequestSwaggerFilter : IOperationFilter
    {
        private static readonly string Route = "templates/update/{id}";
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            SwaggerParams.Add(operation, context, Route, "id", SeederIds.CreateChartReqTemplateId.ToString());

            var request = new UpdateChartReqTemplateRequest()
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
                    Name = "CreateDate"
                },
                SelectedFields = []
            };
            SwaggerParams.Add(operation, context, Route, request);
            SwaggerParams.AddHeader(operation, context, Route, "X-Db", SeederIds.DatabaseId.ToString());
        }
    }
}
