using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Net;
using System.Xml.Linq;
using BuildScreen.ContinousIntegration.Exceptions;
using BuildScreen.ContinousIntegration.Persistance;

namespace BuildScreen.ContinousIntegration.Client
{
    public abstract class BaseClient : IBaseClient
    {
        private bool _disposed;
        private readonly ClientConfiguration _clientConfiguration;

        #region Implementation of IContinousIntegrationClient

        public bool IsConnected { get; set; }

        #endregion

        internal BaseClient(ClientConfiguration clientConfiguration)
        {
            if (clientConfiguration == null || string.IsNullOrEmpty(clientConfiguration.Domain))
            {
                IsConnected = false;
                return;
            }

            _clientConfiguration = clientConfiguration;

            TryConnect();
        }

        internal HttpWebRequest CreateWebRequest(Uri requestUri)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUri);
            httpWebRequest.Credentials = new NetworkCredential(_clientConfiguration.UserName, _clientConfiguration.Password);
            httpWebRequest.Method = WebRequestMethods.Http.Get;

            if (_clientConfiguration.IgnoreInvalidCertificate)
                IgnoreInvalidCertificate();

            return httpWebRequest;
        }

        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        internal XDocument LoadXmlDocument(Uri xmlUri)
        {
            XDocument xDocument = null;

            try
            {
                using (WebResponse webResponse = CreateWebRequest(xmlUri).GetResponse())
                {
                    using (Stream stream = webResponse.GetResponseStream())
                    {
                        if (stream != null)
                        {
                            using (StreamReader streamReader = new StreamReader(stream))
                            {
                                xDocument = XDocument.Load(streamReader);

                                streamReader.Close();
                            }

                            stream.Close();
                        }
                    }

                    webResponse.Close();
                }
            }
            catch (WebException ex)
            {
                throw new ClientLoadDocumentException(ex.Message, ex);
            }

            return xDocument;
        }

        /// <summary>
        /// Ignores an invalid certificate (e.g. a self signed one) and bypasses the <see cref="System.Net.WebExceptionStatus" />
        /// (The underlying connection was closed: Could not establish trust relationship for the SSL/TLS secure channel.).
        /// </summary>
        private static void IgnoreInvalidCertificate()
        {
            // delegate(object sender1, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { ... }
            ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
        }

        internal Uri BaseUri()
        {
            string protocol = "http";

            if (_clientConfiguration.UseSsl)
                protocol = "https";

            return new Uri(string.Format(CultureInfo.InvariantCulture, _clientConfiguration.BaseUrl, protocol, _clientConfiguration.Domain, _clientConfiguration.Port));
        }

        internal void TryConnect()
        {
            WebResponse webResponse = null;
            try
            {
                webResponse = CreateWebRequest(BaseUri()).GetResponse();
                IsConnected = true;
            }
            catch (WebException ex)
            {
                IsConnected = false;
                throw new ClientConnectionException(ex.Message, ex);
            }
            finally
            {
                if (webResponse != null)
                    webResponse.Close();
            }
        }

        #region Implementation of IDisposable

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        internal void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                }

                _disposed = true;
            }
        }

        #endregion
    }
}
