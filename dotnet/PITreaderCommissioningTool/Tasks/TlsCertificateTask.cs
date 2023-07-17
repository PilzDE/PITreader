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

namespace Pilz.PITreader.CommissioningTool.Tasks
{
    internal class TlsCertificateTask : ICommissioningTask
    {
        private Option<FileInfo> uploadCertificateOption = new Option<FileInfo>(new[] { "--upload-certificate" }, "Path to a TLS certificate file (PEM incl. key) to be uploaded to the device")
        {
            ArgumentHelpName = "path to file"
        };
        
        private Option<bool> regenerateCertificateOption = new Option<bool>(new[] { "--regenerate-certificate" }, getDefaultValue: () => false, "If set, the certificate is regenerated after changing the IP address");

        public TlsCertificateTask()
        {
            this.uploadCertificateOption.ExistingOnly();
        }

        public Option[] Options => new Option[] { uploadCertificateOption, regenerateCertificateOption };

        public bool CheckOptions(TaskContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            FileInfo? uploadCertificatePath = context.GetValue(this.uploadCertificateOption);
            bool regenerateCertificate = context.GetValue(this.regenerateCertificateOption);
            if (regenerateCertificate && uploadCertificatePath != null)
            {
                context.LogError("Cannot specify a certificate upload path as well as the option to regenerate the certificate.");
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

            bool rebootAndCheck = false;

            FileInfo? uploadCertificate = context.GetValue(this.uploadCertificateOption);
            bool regenerateCertificate = context.GetValue(this.regenerateCertificateOption);

            if (uploadCertificate != null)
            {
                context.LogInfo("Uploading TLS certificate to device...");
                ApiResponse<GenericResponse> response;
                using (var fs = uploadCertificate.OpenRead())
                {
                    response = await (await context.GetClientAsync()).UploadCertificate(fs);
                }

                if (!response.Success)
                {
                    context.LogError("Unable to upload TLS certificate: " + response.ResponseCode);
                    context.Invocation.ExitCode = 2;
                    return;
                }

                rebootAndCheck = true;
            }
            else if (regenerateCertificate)
            {
                context.LogInfo("Renewing TLS certificate of device...");
                var response = await (await context.GetClientAsync()).RegenerateCertificate(CertificateRegenerateScope.CertOnly);
                if (!response.Success)
                {
                    context.LogError("Unable to regenerate TLS certificate.");
                    context.Invocation.ExitCode = 2;
                    return;
                }

                rebootAndCheck = true;
            }

            if (rebootAndCheck)
            {
                // wait for reboot
                await context.WaitForReboot();

                context.LogInfo("Retrieving new TLS certificate thumbprint...");
                await context.RefetchCertificateThumbprintAsync();
            }
        }
    }
}
