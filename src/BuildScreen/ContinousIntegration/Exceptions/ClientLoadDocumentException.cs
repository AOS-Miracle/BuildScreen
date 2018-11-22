using System;
using System.Net;
using System.Runtime.Serialization;

namespace BuildScreen.ContinousIntegration.Exceptions
{
    [Serializable]
    public class ClientLoadDocumentException : WebException
    {
        public ClientLoadDocumentException()
        {
        }

        public ClientLoadDocumentException(string message)
            : base(message)
        {
        }

        public ClientLoadDocumentException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ClientLoadDocumentException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
