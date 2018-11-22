using System;

namespace BuildScreen.ContinousIntegration.Persistance
{
    public interface IBaseClient : IDisposable
    {
        bool IsConnected { get; set; }
    }
}
