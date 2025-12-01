using Charts.Domain.Contracts.Metadata.Dtos;
using Charts.Domain.Contracts.Template;
using Charts.Domain.Contracts.Types;
using Charts.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Charts.Infrastructure.Databases.Seeder
{
    public class ChartReqTemplatesSeeder : ISeeder
    {
        public int Order => 2;

        public async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // если шаблоны уже есть — ничего не делаем
            if (await db.ChartReqTemplates.AnyAsync())
                return;

            // возьмём первую зарегистрированную базу из meta-БД 
            var database = await db.Databases.Where(x => x.Name == "mirax").FirstOrDefaultAsync();
            if (database is null) return; // ещё нет подключений — пропускаем сид

          

            var tpl = new ChartReqTemplate
            {
                Id = SeederIds.DefaultChartReqTemplateBaseId,
                Name = "Сенсор",
                Description = "Получаем данные по сенсору, который мы укажем на фильтр + параметр",
                DatabaseId = database.Id,
                VisualOrder = 0, 

                // Настройки графика
                Entity = new EntityDto("public.DeviceEntity"), // поменяй на вашу таблицу/представление
                TimeField = new FieldDto("CreateDate", "date")
                {
                    SqlParamType = SqlParamType.Date
                },          // колонка времени
                SelectedFields = new[] { 
                    new FieldDto("Concentration", "double", 0)
                    {
                        SqlParamType = SqlParamType.Double
                    } 
                },

                // Типизированные фильтры. Для Between значение — массив,
                // здесь используем плейсхолдеры, которые придут в Values при запуске.
                Where = new List<FilterClause>
                {
                    new FilterClause(new FieldDto("FactoryNumber", "text"), FilterOp.Eq, "{{deviceId}}"),
                    new FilterClause(new FieldDto("TechnicalRunToStartId", "uuid"), FilterOp.Eq, "{{technicalRunToStartId}}"),
                    new FilterClause(new FieldDto("ChannelNumber", "integer"), FilterOp.Eq, "{{channelNumber}}"),
                },
                // Пользовательский SQL-фрагмент — в демо не нужен
                Sql = null,

                // Params: убираем minVolt/maxVolt, добавляем один ключ volt
                Params = new List<SqlParam>
                {
                    new SqlParam(
                        Key: "deviceId",
                        Value: null,
                        Description: "Заводской номер",
                        Field: new FieldDto("FactoryNumber", "text")
                        {
                            Type = "text",
                            SqlParamType = SqlParamType.Text,
                            VisualOrder = 0,
                        },
                        DefaultValue: "2507434",

                        Required: true
                    ),
                    new SqlParam(
                        Key: "technicalRunToStartId",
                        Value: null,
                        Description: "Испытание",
                        Field: new FieldDto("TechnicalRunToStartId", "uuid")
                        {
                            Type = "uuid",
                            SqlParamType = SqlParamType.Uuid,
                            VisualOrder = 1,

                        },
                        DefaultValue: "371ff58e-9879-4b3a-8271-a75543e48b7d",

                        Required: true
                    ),
                    new SqlParam(
                        Key: "channelNumber",
                        Value: null,
                        Description: "Канал",
                        Field: new FieldDto("ChannelNumber", "integer")
                        {
                            Type = "integer",
                            SqlParamType = SqlParamType.Int,

                        },
                        DefaultValue: "0",

                        Required: true
                    ),
                }
            };

            var tpl2 = new ChartReqTemplate
            {
                Id = SeederIds.DefaultChartReqTemplateSensorId,
                Name = "Базовые графики avis",
                Description = "",
                DatabaseId = database.Id,

                VisualOrder = 1,

                // Настройки графика
                Entity = new EntityDto("public.DeviceEntity"), // поменяй на вашу таблицу/представление
                TimeField = new FieldDto("CreateDate", "date")
                {
                    SqlParamType = SqlParamType.Date
                },          // колонка времени
                SelectedFields = new[] {
                    new FieldDto("BatteryVoltage", "double", 0){
                         SqlParamType = SqlParamType.Double
                    } ,

                     new FieldDto("BatteryLevel", "double", 1){
                         SqlParamType = SqlParamType.Double
                    } ,

                     new FieldDto("Temperature", "double", 2){
                         SqlParamType = SqlParamType.Double
                    } ,
                },

                // Типизированные фильтры. Для Between значение — массив,
                // здесь используем плейсхолдеры, которые придут в Values при запуске.
                Where = new List<FilterClause>
                {
                    new FilterClause(new FieldDto("FactoryNumber", "text"), FilterOp.Eq, "{{deviceId}}"), 
                    new FilterClause(new FieldDto("TechnicalRunToStartId", "uuid"), FilterOp.Eq, "{{technicalRunToStartId}}"),
                },
                // Пользовательский SQL-фрагмент — в демо не нужен
                Sql = null,

                // Params: убираем minVolt/maxVolt, добавляем один ключ volt
                Params = new List<SqlParam>
                {
                    new SqlParam(
                        Key: "deviceId",
                        Value: null,
                        Description: "Заводской номер",
                        Field: new FieldDto("FactoryNumber", "text")
                        {
                            Type = "text",
                            SqlParamType = SqlParamType.Text,

                        },
                        DefaultValue: "2507434",

                        Required: true
                    ), 
                    new SqlParam(
                        Key: "technicalRunToStartId",
                        Value: null,
                        Description: "Испытание",
                        Field: new FieldDto("TechnicalRunToStartId", "uuid")
                        {
                            Type = "uuid",
                            SqlParamType = SqlParamType.Uuid,

                        },
                        DefaultValue: "371ff58e-9879-4b3a-8271-a75543e48b7d",

                        Required: true
                    ),
                }
            };

            db.ChartReqTemplates.Add(tpl);
            db.ChartReqTemplates.Add(tpl2);
            await db.SaveChangesAsync();
        }
    }
}
