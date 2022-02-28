// Copyright (c) 2022 Pilz GmbH & Co. KG
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice (including the next paragraph) shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
// SPDX-License-Identifier: MIT

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
        public static Func<string, CertificateValidationDelegate> AcceptThumbprintSha2 = (string thumbprint) =>
        {
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
