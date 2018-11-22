using System;
using BuildScreen.ContinousIntegration.Client;
using BuildScreen.ContinousIntegration.Persistance;

namespace BuildScreen.ContinousIntegration
{
    public enum ClientType
    {
        TeamCity = 1,
        Hudson = 2,
        Bamboo = 3
    }

    public class ClientFactory : IClientFactory
    {
        public IClient CreateClient(ClientConfiguration clientConfiguration)
        {
            if (clientConfiguration == null)
                throw new ArgumentNullException("clientConfiguration");

            switch (clientConfiguration.ClientType)
            {
                case ClientType.TeamCity:
                    return new TeamCityClient(clientConfiguration);
                case ClientType.Hudson:
                    return new HudsonClient(clientConfiguration);
                case ClientType.Bamboo:
                    return new BambooClient(clientConfiguration);
            }
            //TODO
            return null;
        }
    }
}
