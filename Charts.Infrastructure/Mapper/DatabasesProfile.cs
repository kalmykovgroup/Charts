using AutoMapper;
using Charts.Domain.Contracts.Metadata.Dtos;
using Charts.Domain.Contracts.Metadata.Requests;
using Charts.Domain.Models;

namespace Charts.Infrastructure.Mapper
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
