using Charts.Domain.Interfaces;

namespace Charts.Domain.Contracts
{
    public abstract class BaseEntity : IEntity
    {
        // Аудит 
        public Guid Id { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public Guid? CreatedBy { get; set; }

        public DateTimeOffset? UpdatedAt { get; set; }

        public Guid? UpdatedBy { get; set; }

        // Мягкое удаление
        public bool IsDeleted { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; } 

    }
}
