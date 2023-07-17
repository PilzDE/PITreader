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

namespace Pilz.PITreader.CommissioningTool.Tasks
{
    internal class OpcUaServerCertificateTask : ICommissioningTask
    {
        private Option<FileInfo> opcUaServerCertificateDestinationOption = new Option<FileInfo>(new[] { "--opc-ua-server-certificate-destination" }, "Path to which the OPC UA server certificate should be exported")
        {
            ArgumentHelpName = "path to file"
        };

        public Option[] Options => new Option[] { opcUaServerCertificateDestinationOption };

        public bool CheckOptions(TaskContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            FileInfo? opcUaServerCertificateDestination = context.GetValue(this.opcUaServerCertificateDestinationOption);
            if (opcUaServerCertificateDestination != null
                && (opcUaServerCertificateDestination.Directory?.Exists != true))
            {
                context.LogError("Directory to OPC UA server certificate destination does not exist.");
                return false;
            }

            return true;
        }

        public async Task RunAsync(TaskContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            FileInfo? opcUaServerCertificateDestination = context.GetValue(opcUaServerCertificateDestinationOption);
            if (opcUaServerCertificateDestination != null)
            {
                context.LogInfo("Copying OPC UA server certificate to file system...");
                using (var fs = opcUaServerCertificateDestination.Open(FileMode.Create, FileAccess.Write))
                {
                    var response = await (await context.GetClientAsync()).GetOpcUaServerCertificate(fs);
                    if (!response.Success)
                    {
                        context.LogError("Unable to download OPC UA server certificate: " + response.ResponseCode);
                        context.Invocation.ExitCode = 2;
                    }
                }
            }
        }
    }
}
