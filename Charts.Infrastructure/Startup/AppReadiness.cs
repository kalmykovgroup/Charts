using Charts.Domain.Interfaces;

namespace Charts.Infrastructure.Startup
{
    public sealed class AppReadiness : IAppReadiness
    {
        private volatile bool _ready;
        public bool Ready => _ready;
        public void SetReady() => _ready = true;
        public void SetNotReady() => _ready = false;
    }
}
