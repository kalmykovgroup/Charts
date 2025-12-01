using Charts.Domain.Interfaces.Repositories.CommonInterfaces;
using Charts.Domain.Models;

namespace Charts.Domain.Interfaces.Repositories
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
