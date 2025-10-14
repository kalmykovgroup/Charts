namespace Charts.Api.Domain.Interfaces
{
    public interface IAppReadiness
    {
        bool Ready { get; }
        void SetReady();
        void SetNotReady();
    }
}
