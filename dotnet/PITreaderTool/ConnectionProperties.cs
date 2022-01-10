namespace Pilz.PITreader.Tool
{
    internal class ConnectionProperties
    {
        public string Host { get; set; }

        public ushort Port { get; set; }

        public bool AcceptAll { get; set; }

        public string Thumbprint { get; set; }

        public string ApiToken { get; set; }
    }
}
