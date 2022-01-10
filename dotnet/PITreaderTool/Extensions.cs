using System;
using System.CommandLine;
using Pilz.PITreader.Client;

namespace Pilz.PITreader.Tool
{
    internal static class Extensions
    {
        public static PITreaderClient CreateClient(this ConnectionProperties properties)
        {
            var client = new PITreaderClient(properties.Host, properties.Port, properties.AcceptAll ? CertificateValidators.AcceptAll : CertificateValidators.AcceptThumbprintSha2(properties.Thumbprint))
            {
                ApiToken = properties.ApiToken
            };

            return client;
        }

        public static void WriteError(this IConsole console, string message)
        {
            if (!Console.IsOutputRedirected) Console.ForegroundColor = ConsoleColor.Red;
            console.Error.Write("ERROR: " + message + Environment.NewLine);
            if (!Console.IsOutputRedirected) Console.ResetColor();
        }
    }
}
