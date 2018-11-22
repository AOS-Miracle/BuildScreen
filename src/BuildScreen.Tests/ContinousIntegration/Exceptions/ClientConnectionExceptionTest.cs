using System;
using BuildScreen.ContinousIntegration.Exceptions;
using BuildScreen.Tests.Helpers.Extensions;
using NUnit.Framework;

namespace BuildScreen.Tests.ContinousIntegration.Exceptions
{
    [TestFixture]
    public class ClientConnectionExceptionTest
    {
        [Test]
        public void ClientConnectionException_CunstructWithMessage_ShouldHaveMessage()
        {
            ClientConnectionException clientConnectionException = new ClientConnectionException("Error");
            clientConnectionException.Message.ShouldBeEqual("Error");

            ClientConnectionException clientConnectionExceptionOverload = new ClientConnectionException("Error", null);
            clientConnectionExceptionOverload.Message.ShouldBeEqual("Error");
        }

        [Test]
        public void ClientConnectionException_ConstructWithInnerException_ShouldHaveInnerException()
        {
            Exception innerException = new Exception("InnerError");

            ClientConnectionException clientConnectionException = new ClientConnectionException(null, innerException);
            clientConnectionException.InnerException.Message.ShouldBeEqual("InnerError");
        }
    }
}
