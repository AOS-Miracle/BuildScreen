namespace BuildScreen.ContinousIntegration.Persistance
{
    public interface IClientFactory
    {
        IClient CreateClient(ClientConfiguration clientConfiguration);
    }
}
