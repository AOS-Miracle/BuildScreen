using System;
using System.Net;
using System.Runtime.Serialization;

namespace BuildScreen.ContinousIntegration.Exceptions
{
    [Serializable]
    public class ClientConnectionException : WebException
    {
        public ClientConnectionException()
        {
        }

        public ClientConnectionException(string message)
            : base(message)
        {
        }

        public ClientConnectionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ClientConnectionException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
