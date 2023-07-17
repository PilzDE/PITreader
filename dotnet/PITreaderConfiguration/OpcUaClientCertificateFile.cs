namespace Pilz.PITreader.Configuration
{
    public class OpcUaClientCertificateFile : GenericFile
    {
        public const string FileName = "opcuacli.der";

        public OpcUaClientCertificateFile() 
            : base(FileName)
        {
            this.Component = ConfigurationType.OpcUaClientCertificate;
        }
    }
}
