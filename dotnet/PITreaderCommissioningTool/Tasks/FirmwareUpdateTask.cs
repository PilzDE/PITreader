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
    internal class FirmwareUpdateTask : ICommissioningTask
    {
        private Option<FileInfo> firmwareOption = new Option<FileInfo>(new[] { "--firmware" }, "Path to latest firmware update")
        {
            ArgumentHelpName = "path to file",
            IsRequired = false
        };

        private Option<bool> forceFirmwareUpdateOption = new Option<bool>(new[] { "--force-firmware-update" }, "If set the update is always performed. Otherwise only on upgrades.");

        public FirmwareUpdateTask()
        {
            this.firmwareOption.ExistingOnly();
        }

        public Option[] Options => new Option[] { this.firmwareOption, this.forceFirmwareUpdateOption };

        public bool CheckOptions(TaskContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            FileInfo? firmwareUpdate = context.GetValue(this.firmwareOption);
            bool forceUpdate = context.GetValue(this.forceFirmwareUpdateOption);
            if (firmwareUpdate == null && forceUpdate)
            {
                context.LogError("No firmware update file specified, but option to force the update.");
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

            FileInfo? firmwareUpdate = context.GetValue(this.firmwareOption);
            bool forceUpdate = context.GetValue(this.forceFirmwareUpdateOption);
            if (firmwareUpdate != null)
            {
                context.LogInfo("Reading firmware update package...");
                var fwu = new PITreaderFirmwarePackage(firmwareUpdate.FullName);
                var fwManager = new PITreaderFirmwareManager(await context.GetClientAsync());

                context.LogInfo("Applying firmware update...");
                var fwResponse = await fwManager.PerformUpdateAsync(fwu, forceUpdate);
                if (fwResponse != PITreaderFirmwareUpdateResult.Success && fwResponse != PITreaderFirmwareUpdateResult.AlreadyInstalledOrNewer)
                {
                    context.LogError("Applying firmware update failed.");
                    context.Invocation.ExitCode = 2;
                }

                if (fwResponse == PITreaderFirmwareUpdateResult.Success)
                {
                    await context.WaitForReboot();
                }
            }
        }
    }
}
