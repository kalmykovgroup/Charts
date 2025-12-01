namespace Charts.Domain.Interfaces
{
    public interface IRequestDbKeyAccessor
    {
        Guid? DbId { get; } 

        // добавь в интерфейс, чтобы не кастить к реализации
        void Set(Guid? id);
    }

}
