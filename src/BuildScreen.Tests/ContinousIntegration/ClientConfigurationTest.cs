using BuildScreen.ContinousIntegration;
using BuildScreen.Tests.Helpers.Extensions;
using NUnit.Framework;

namespace BuildScreen.Tests.ContinousIntegration
{
    [TestFixture]
    public class ClientConfigurationTest
    {
        private ClientConfiguration _clientConfiguration;

        [SetUp]
        public void Setup()
        {
            _clientConfiguration = new ClientConfiguration
                {
                    Domain = "domain",
                    Password = "password",
                    Port = 1234,
                    UserName = "username"
                };
        }

        [Test]
        public void AllDataExists_DomainIsNull_ShouldBeFalse()
        {
            _clientConfiguration.Domain = null;
            _clientConfiguration.AllDataExists.ShouldBeFalse();
        }

        [Test]
        public void AllDataExists_PasswordIsNull_ShouldBeFalse()
        {
            _clientConfiguration.Password = null;
            _clientConfiguration.AllDataExists.ShouldBeFalse();
        }

        [Test]
        public void AllDataExists_UserNameIsNull_ShouldBeFalse()
        {
            _clientConfiguration.UserName = null;
            _clientConfiguration.AllDataExists.ShouldBeFalse();
        }

        [Test]
        public void AllDataExists_PortIsZero_ShouldBeFalse()
        {
            _clientConfiguration.Port = 0;
            _clientConfiguration.AllDataExists.ShouldBeFalse();
        }

        [Test]
        public void AllDataExists_RequiredPropertiesAreSet_ShouldBeTrue()
        {
            _clientConfiguration.AllDataExists.ShouldBeTrue();
        }
    }
}
