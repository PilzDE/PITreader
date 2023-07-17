// Copyright (c) 2023 Pilz GmbH & Co. KG
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

using System.CommandLine;
using Pilz.PITreader.Client;
using Pilz.PITreader.Client.Model;
using Pilz.PITreader.Configuration;

namespace Pilz.PITreader.CommissioningTool
{
    internal static class Extensions
    {
        private const string RestoreEndpoint = "/api/files/restore";

        public static void WriteError(this IConsole console, string message)
        {
            if (!Console.IsOutputRedirected) Console.ForegroundColor = ConsoleColor.Red;
            console.Error.Write("ERROR: " + message + Environment.NewLine);
            if (!Console.IsOutputRedirected) Console.ResetColor();
        }

        public static async Task<string> GetCurrentThumbprintAsync(string hostnameOrIp, ushort httpsPort)
        {
            var certificate = await PITreaderClient.GetDeviceCertificateAsync(hostnameOrIp, httpsPort);

            using (var sha2 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] sha2Thumbprint = sha2.ComputeHash(certificate.GetRawCertData());
                return string.Join(":", sha2Thumbprint.Select(d => d.ToString("X2")));
            }
        }

        public async static Task<ApiResponse<GenericResponse>> RestoreConfigurationAsync(this PITreaderClient client, ConfigurationFile config)
        {
            using (var ms = new MemoryStream())
            {
                config.Write(ms);

                ms.Position = 0;

                return await client.PostFileAsync<GenericResponse>(
                    RestoreEndpoint,
                    ms,
                    "restore.config",
                    "configFile",
                    config.GetRequstParameters().Select(p => new KeyValuePair<string, string>(p, "true")),
                    TimeSpan.FromSeconds(30));
            }
        }
    }
}
