using Charts.Api.Swagger;
using Charts.Api.Application.Contracts.Charts.Requests;
using Charts.Api.Application.Contracts.Metadata.Dtos;
using Charts.Api.Domain.Contracts.Template;
using Charts.Api.Domain.Contracts.Template.Dtos;
using Charts.Api.Domain.Contracts.Types;
using Charts.Api.Infrastructure.Databases.Seeder;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Charts.Api.Infrastructure.Swagger.Filters.Charts
{
    public sealed class GetChartMultiSeriesRequestSwaggerFilter : IOperationFilter
    {
        private static readonly string Route = "charts/multi";

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var request = new GetMultiSeriesRequest
            {
                Template = new ResolvedCharReqTemplate
                {
                    Id = SeederIds.DefaultChartReqTemplateBaseId, 
                    DatabaseId = SeederIds.DatabaseId,


                    ResolvedFromMs = new DateTimeOffset(DateTime.Parse("2025-08-29T15:00:00Z")).ToUnixTimeMilliseconds(),
                    ResolvedToMs = new DateTimeOffset(DateTime.Parse("2025-08-29T17:00:00Z")).ToUnixTimeMilliseconds(),

                    // Ќастройки графика
                    Entity = new EntityDto("public.DeviceEntity"), // помен€й на вашу таблицу/представление
                    TimeField = new FieldDto("CreateDate", "date"),          // колонка времени
                    SelectedFields = new[] { new FieldDto("BatteryVoltage", "double"), new FieldDto("Temperature", "double") },

                    // “ипизированные фильтры. ƒл€ Between значение Ч массив,
                    // здесь используем плейсхолдеры, которые придут в Values при запуске.
                    Where = new List<FilterClause>
                {
                    new FilterClause(new FieldDto("FactoryNumber", "text"), FilterOp.Eq, "{{deviceId}}"),
                },
                    // ѕользовательский SQL-фрагмент Ч в демо не нужен
                    Sql = null,

                    // Params: убираем minVolt/maxVolt, добавл€ем один ключ volt
                    Params = new List<ReadySqlParam>
                    {
                        new ReadySqlParam(
                            Key: "deviceId",
                            Value: "2507633", 
                            Field: new FieldDto("FactoryNumber", "text")
                            { 
                                 SqlParamType = SqlParamType.Text
                            }, 
                            Required: true
                        ),
                    }
                },
                Px = 2200, 
            };

            SwaggerParams.Add(operation, context, Route, request);
            SwaggerParams.AddHeader(operation, context, Route, "X-Db", SeederIds.DatabaseId.ToString());
        }
    }
}
