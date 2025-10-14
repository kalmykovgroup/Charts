using AutoMapper;
using Charts.Api.Application.Contracts.Template.Dtos;
using Charts.Api.Application.Contracts.Template.Requests;
using Charts.Api.Application.Models;
using Charts.Api.Domain.Contracts.Template;
using Charts.Api.Domain.Contracts.Types;
using Charts.Api.Infrastructure.Extensions;

namespace Charts.Api.Infrastructure.Mapper;

public sealed class ChartReqTemplatesProfile : Profile
{
    public ChartReqTemplatesProfile()
    {
        // Основные маппинги для ChartReqTemplate
        // Если DTO используют long? - просто копируем значения
        CreateMap<ChartReqTemplateDto, ChartReqTemplate>()
            .ReverseMap();

        CreateMap<CreateChartReqTemplateRequest, ChartReqTemplate>();

        CreateMap<UpdateChartReqTemplateRequest, ChartReqTemplate>();

        // Если у вас разные namespace для Domain и Application FilterClause
        CreateMap<FilterClause, FilterClause>()
            .ConstructUsing(src => new FilterClause(
                src.Field,
                MapOpToDomain(src.Op),
                JsonValueNormalizer.ToClr(src.Value)!
            ));

        CreateMap<FilterClause, FilterClause>()
            .ConstructUsing(src => new FilterClause(
                src.Field,
                MapOpToContract(src.Op),
                JsonValueNormalizer.ToClr(src.Value)!
            ));

        // Аналогично для SqlParam если есть разные namespace
        CreateMap<SqlParam, SqlParam>()
            .ConstructUsing(src => new SqlParam(
                src.Key,
                src.Field,
                src.Required,
                JsonValueNormalizer.ToClr(src.Value),
                src.Description,
                JsonValueNormalizer.ToClr(src.DefaultValue)
            ));

        // SqlFilter
        CreateMap<SqlFilter, SqlFilter>()
            .ConstructUsing(src => new SqlFilter(src.WhereSql));

        CreateMap<SqlFilter, SqlFilter>()
            .ConstructUsing(src => new SqlFilter(src.WhereSql));
    }

    private static FilterOp MapOpToDomain(FilterOp op)
        => Enum.Parse<FilterOp>(op.ToString());

    private static FilterOp MapOpToContract(FilterOp op)
        => Enum.Parse<FilterOp>(op.ToString());
}