namespace BuildScreen.ContinousIntegration
{
    public class ClientConfiguration
    {
        public string Domain { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool UseSsl { get; set; }
        public bool IgnoreInvalidCertificate { get; set; }
        public ClientType ClientType { get; set; }
        public string BaseUrl { get; set; }

        public bool AllDataExists
        {
            get
            {
                return ((!string.IsNullOrEmpty(Domain) && !Port.Equals(0)) && !string.IsNullOrEmpty(UserName)) && !string.IsNullOrEmpty(Password);
            }
        }
    }
}
