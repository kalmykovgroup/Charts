using Charts.Api.Application.Interfaces.Repositories.CommonInterfaces;
using Charts.Api.Application.Models;

namespace Charts.Api.Application.Interfaces.Repositories
{ 

    public interface IDatabaseRepository :
   IGetAllRepository<Database>,
   IAddRepository<Database>,
   IUpdateRepository<Database>,
   IDeleteRepository<Database>,
   IGetByIdRepository<Database>
    {
    }
}
