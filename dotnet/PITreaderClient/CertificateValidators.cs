using System;
using System.Linq;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Pilz.PITreader.Client
{
    /// <summary>
    /// Delegate for certificate validation.
    /// </summary>
    /// <param name="message">HTTP request message.</param>
    /// <param name="certificate">Certificate recieved from the device.</param>
    /// <param name="chain">Chain of intermediate certificates received from the device</param>
    /// <param name="pollicyErrors">SSL policy errors.</param>
    /// <returns></returns>
    public delegate bool CertificateValidationDelegate(HttpRequestMessage message, X509Certificate2 certificate, X509Chain chain, SslPolicyErrors pollicyErrors);

    /// <summary>
    /// Container for predefined certificate validation strategies.
    /// </summary>
    public static class CertificateValidators
    {
        /// <summary>
        /// Accept all certificates (insecure).
        /// </summary>
        public static CertificateValidationDelegate AcceptAll = (msg, cert, chain, policyErrors) => true;

        /// <summary>
        /// Accept only the certificates with the specified SHA2 thumbprint.
        /// </summary>
        public static Func<string, CertificateValidationDelegate> AcceptThumbprintSha2 = (string thumbprint) => {
            byte[] thumbprintRaw = thumbprint.HexStringToByteArray();
            return (HttpRequestMessage message, X509Certificate2 certificate, X509Chain chain, SslPolicyErrors pollicyErrors) =>
            {
                if (certificate == null)
                    return false;

                using (var sha2 = new System.Security.Cryptography.SHA256CryptoServiceProvider())
                {
                    byte[] sha2Thumbprint = sha2.ComputeHash(certificate.GetRawCertData());
                    return sha2Thumbprint.SequenceEqual(thumbprintRaw);
                }
            };
        };
    }
}
