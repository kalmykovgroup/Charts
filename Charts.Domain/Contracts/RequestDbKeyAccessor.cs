using Charts.Domain.Interfaces;

namespace Charts.Domain.Contracts
{
    public sealed class RequestDbKeyAccessor : IRequestDbKeyAccessor
    {
        public Guid? DbId { get; private set; } 
        public void Set(Guid? id) { DbId = id; }
    }

}
