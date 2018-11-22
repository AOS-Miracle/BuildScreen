using System;
using BuildScreen.ContinousIntegration;
using BuildScreen.ContinousIntegration.Client;
using BuildScreen.Tests.Helpers.Extensions;
using NUnit.Framework;

namespace BuildScreen.Tests.ContinousIntegration
{
    [TestFixture]
    public class ClientFactoryTest
    {
        [Test]
        public void CreateClient_ConfigurationIsNull_ShouldThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new ClientFactory().CreateClient(null));
        }

        [Test]
        public void CreateClient_ClientTypeIsTeamCity_ShouldBeTeamCityClient()
        {
            new ClientFactory().CreateClient(
                new ClientConfiguration { ClientType = ClientType.TeamCity }
                ).ShouldBe<TeamCityClient>();
        }

        [Test]
        public void CreateClient_ClientTypeIsHudson_ShouldBeHudsonClient()
        {
            new ClientFactory().CreateClient(
                new ClientConfiguration { ClientType = ClientType.Hudson }
                ).ShouldBe<HudsonClient>();
        }

        [Test]
        public void CreateClient_ClientTypeIsBamboo_ShouldBeBambooClient()
        {
            new ClientFactory().CreateClient(
                new ClientConfiguration { ClientType = ClientType.Bamboo }
                ).ShouldBe<BambooClient>();
        }
    }
}
