namespace Charts.Domain.Interfaces
{
    public interface IEntity
    {
        public Guid Id { get; set; }
        // Аудит
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
