using System;
using BuildScreen.ContinousIntegration.Exceptions;
using BuildScreen.Tests.Helpers.Extensions;
using NUnit.Framework;

namespace BuildScreen.Tests.ContinousIntegration.Exceptions
{
    [TestFixture]
    public class ClientLoadDocumentExceptionTest
    {
        [Test]
        public void ClientLoadDocumentException_CunstructWithMessage_ShouldHaveMessage()
        {
            ClientLoadDocumentException clientLoadDocumentException = new ClientLoadDocumentException("Error");
            clientLoadDocumentException.Message.ShouldBeEqual("Error");

            ClientLoadDocumentException clientLoadDocumentExceptionOverload = new ClientLoadDocumentException("Error", null);
            clientLoadDocumentExceptionOverload.Message.ShouldBeEqual("Error");
        }

        [Test]
        public void ClientLoadDocumentException_ConstructWithInnerException_ShouldHaveInnerException()
        {
            Exception innerException = new Exception("InnerError");

            ClientLoadDocumentException clientLoadDocumentException = new ClientLoadDocumentException(null, innerException);
            clientLoadDocumentException.InnerException.Message.ShouldBeEqual("InnerError");
        }
    }
}
