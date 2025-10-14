using AutoMapper;
using Charts.Api.Application.Contracts.Metadata.Dtos;
using Charts.Api.Application.Contracts.Metadata.Requests;
using Charts.Api.Application.Models;

namespace Charts.Api.Infrastructure.Mapper
{
    public sealed class DatabasesProfile : Profile
    {
        public DatabasesProfile()
        {
            CreateMap<Database, DatabaseDto>().ReverseMap();
            CreateMap<CreateDatabaseRequest, Database>();
            CreateMap<UpdateDatabaseRequest, Database>();
       
        }
    }
}
