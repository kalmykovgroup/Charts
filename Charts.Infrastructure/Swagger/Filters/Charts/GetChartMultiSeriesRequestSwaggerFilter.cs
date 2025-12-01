using Charts.Domain.Contracts.Charts.Requests;
using Charts.Domain.Contracts.Metadata.Dtos;
using Charts.Domain.Contracts.Template;
using Charts.Domain.Contracts.Template.Dtos;
using Charts.Domain.Contracts.Types;
using Charts.Infrastructure.Databases.Seeder;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Charts.Infrastructure.Swagger.Filters.Charts
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

                    // ��������� �������
                    Entity = new EntityDto("public.DeviceEntity"), // ������� �� ���� �������/�������������
                    TimeField = new FieldDto("CreateDate", "date"),          // ������� �������
                    SelectedFields = new[] { new FieldDto("BatteryVoltage", "double"), new FieldDto("Temperature", "double") },

                    // �������������� �������. ��� Between �������� � ������,
                    // ����� ���������� ������������, ������� ������ � Values ��� �������.
                    Where = new List<FilterClause>
                {
                    new FilterClause(new FieldDto("FactoryNumber", "text"), FilterOp.Eq, "{{deviceId}}"),
                },
                    // ���������������� SQL-�������� � � ���� �� �����
                    Sql = null,

                    // Params: ������� minVolt/maxVolt, ��������� ���� ���� volt
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
