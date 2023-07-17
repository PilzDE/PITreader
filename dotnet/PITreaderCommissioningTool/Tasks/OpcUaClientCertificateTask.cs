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
using Pilz.PITreader.Configuration;

namespace Pilz.PITreader.CommissioningTool.Tasks
{
    internal class OpcUaClientCertificateTask : ICommissioningTask
    {
        private Option<FileInfo> opcUaClientCertificateOption = new Option<FileInfo>(new[] { "--opc-ua-client-certificate" }, "Path to a OPC UA client certificate that should be uploaded to the device")
        {
            ArgumentHelpName = "path to file",
            IsRequired = false
        };

        public OpcUaClientCertificateTask()
        {
            this.opcUaClientCertificateOption.ExistingOnly();
        }

        public Option[] Options => new Option[] { opcUaClientCertificateOption };

        public bool CheckOptions(TaskContext context)
        {
            return true;
        }

        public Task RunAsync(TaskContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            FileInfo? opcUaClientCertificate = context.GetValue(opcUaClientCertificateOption);
            if (opcUaClientCertificate != null)
            {
                context.LogInfo("Adding OPC UA client certificate to configuration...");

                var certFile = context.ConfigurationFile.Get<OpcUaClientCertificateFile>();
                if (certFile == null)
                {
                    certFile = new OpcUaClientCertificateFile();
                    context.ConfigurationFile.Add(certFile);
                }

                using (var fs = opcUaClientCertificate.OpenRead())
                {
                    certFile.Read(fs);
                }
            }

            return Task.CompletedTask;
        }
    }
}
