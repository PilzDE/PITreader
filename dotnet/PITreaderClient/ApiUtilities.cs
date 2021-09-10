﻿using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;

namespace Pilz.PITreader.Client
{
    /// <summary>
    /// Utility functions for PITreader API requests and responses.
    /// </summary>
    public static class ApiUtilities
    {
        /// <summary>
        /// Converts a string of hexadecimal numbers (with even length) to a byte array.
        /// </summary>
        /// <param name="hexString">A string of hexadecimal numbers.</param>
        /// <returns></returns>
        public static byte[] HexStringToByteArray(this string hexString)
        {
            if (string.IsNullOrEmpty(hexString))
                return new byte[0];

            hexString = hexString.Replace(":", string.Empty)
                .Replace(" ", string.Empty)
                .Replace("-", string.Empty);

            if (hexString.Length % 2 != 0 || Regex.IsMatch(hexString, "[^A-Fa-f0-9]"))
                throw new ArgumentException("Invalid hex string", nameof(hexString));

            return Enumerable.Range(0, hexString.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hexString.Substring(x, 2), 16))
                .ToArray();
        }

        /// <summary>
        /// Returns the SHA2 thumbprint for the certificate.
        /// </summary>
        /// <param name="certificate"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static string GetThumbprintSha2(this X509Certificate2 certificate)
        {
            if (certificate == null)
                throw new ArgumentNullException(nameof(certificate));

            byte[] sha2Thumbprint = null;
            using (var sha2 = new System.Security.Cryptography.SHA256CryptoServiceProvider())
            {
                sha2Thumbprint = sha2.ComputeHash(certificate.GetRawCertData());
            }

            var result = new StringBuilder();
            for (int i = 0; i < sha2Thumbprint.Length; i += 1)
            {
                result.AppendFormat("{0:X2}", sha2Thumbprint[i]);
            }
            return result.ToString();
        }


        /// <summary>
        /// Returns the SHA1 thumbprint for the certificate (<seealso cref="X509Certificate2.Thumbprint"/>).
        /// </summary>
        /// <param name="certificate"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static string GetThumbprintSha1(this X509Certificate2 certificate)
        {
            if (certificate == null)
                throw new ArgumentNullException(nameof(certificate));

            return certificate.Thumbprint;
        }
    }
}
