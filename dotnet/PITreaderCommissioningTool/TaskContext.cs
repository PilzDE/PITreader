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
using System.CommandLine.Invocation;
using System.Diagnostics.CodeAnalysis;
using Pilz.PITreader.Client;
using Pilz.PITreader.Configuration;

namespace Pilz.PITreader.CommissioningTool
{
    /// <summary>
    /// Task execution context.
    /// </summary>
    internal class TaskContext
    {
        private PITreaderClient? client = null;

        private string? clientThumbprint;

        public TaskContext(InvocationContext context)
        {
            this.Invocation = context ?? throw new ArgumentNullException(nameof(context));
        }

        public string DeviceIp { get; set; } = PITreaderClient.DefaultIp;

        public ushort HttpsPort { get; set; } = PITreaderClient.DefaultPortNumber;

        public ConfigurationFile ConfigurationFile { get; } = new ConfigurationFile();

        public CommissioningUser? ApiUser { get; set; }

        public InvocationContext Invocation { get; }

        [return: MaybeNull]
        public T GetValue<T>(Option<T> option)
        {
            return this.Invocation.ParseResult.GetValueForOption(option);
        }

        public async Task<PITreaderClient> GetClientAsync()
        {
            if (this.client == null)
            {
                if (this.clientThumbprint is null)
                {
                    await this.RefetchCertificateThumbprintAsync();
                }

                if (ApiUser?.InteractiveLogin == true)
                {
                    var iaClient = new PITreaderInteractiveClient(DeviceIp, HttpsPort, CertificateValidators.AcceptThumbprintSha2(this.clientThumbprint));
                    var response = await iaClient.LoginAsync(this.ApiUser.Name, this.ApiUser.PasswordOrApiToken ?? string.Empty);
                    if (!response.Success)
                    {
                        this.LogError("Login at PITreader failed.");
                        return iaClient;
                    }

                    this.client = iaClient;
                }
                else
                {
                    this.client = new PITreaderClient(this.DeviceIp, this.HttpsPort, CertificateValidators.AcceptThumbprintSha2(this.clientThumbprint));
                    this.client.ApiToken = ApiUser?.PasswordOrApiToken ?? string.Empty;
                }
            }

            return this.client;
        }

        public async Task RefetchCertificateThumbprintAsync()
        {
            this.clientThumbprint = await Extensions.GetCurrentThumbprintAsync(this.DeviceIp, this.HttpsPort);
            this.LogInfo("Pin connection to certificate fingerprint: " + this.clientThumbprint);
        }

        public Task WaitForReboot()
        {
            this.client?.Dispose();
            this.client = null;

            // wait for reboot
            this.LogInfo("Awaiting restart of device...");
            return Task.Delay(15000);
        }

        public void LogInfo(string message)
        {
            this.Invocation.Console.WriteLine(FormatLogMessage(message));
        }

        public void LogError(string message)
        {
            if (!this.Invocation.Console.IsErrorRedirected) Console.ForegroundColor = ConsoleColor.Red;
            this.Invocation.Console.Error.Write(FormatLogMessage("ERROR: " + message) + Environment.NewLine);
            if (!this.Invocation.Console.IsErrorRedirected) Console.ResetColor();
        }

        private static string FormatLogMessage(string input)
        {
            return $"[{DateTime.Now.ToLongTimeString()}] {input}";
        }
    }
}
