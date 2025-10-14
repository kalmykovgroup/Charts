using Charts.Api.Application.Interfaces;

namespace Charts.Api.Application.Contracts
{
    public sealed class RequestDbKeyAccessor : IRequestDbKeyAccessor
    {
        public Guid? DbId { get; private set; } 
        public void Set(Guid? id) { DbId = id; }
    }

}
