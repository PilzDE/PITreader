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
using Pilz.PITreader.Configuration;

namespace Pilz.PITreader.CommissioningTool.Tasks
{
    internal class ConfigurationBackupTask : ICommissioningTask
    {
        private Option<FileInfo> configBackupDestinationOption = new Option<FileInfo>(new[] { "--config-backup-destination" }, "Path to which the backup of the device configuration should be exported")
        {
            ArgumentHelpName = "path to file"
        };

        public Option[] Options => new Option[] { configBackupDestinationOption };

        public bool CheckOptions(TaskContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            FileInfo? configBackupDestination = context.GetValue(this.configBackupDestinationOption);
            if (configBackupDestination != null
                && (configBackupDestination.Directory?.Exists != true))
            {
                context.LogError("Directory to configuration backup destination does not exist.");
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

            FileInfo? configBackupDestination = context.GetValue(this.configBackupDestinationOption);
            if (configBackupDestination != null)
            {
                context.LogInfo("Getting configuration backup...");
                var configBackup = new MemoryStream();

                var response = await (await context.GetClientAsync()).GetConfigurationBackup(configBackup);
                if (!response.Success)
                {
                    context.LogError("Unable to download configuration backup.");
                    context.Invocation.ExitCode = 2;
                    return;
                }

                configBackup.Position = 0;
                context.ConfigurationFile.Read(configBackup);

                if (context.ApiUser?.DeleteOnCleanup == true)
                {
                    var userFile = context.ConfigurationFile.Get<DeviceUserFile>();
                    if (userFile != null)
                    {
                        var selectedUser = userFile.Users.FirstOrDefault(u => u.Name == context.ApiUser?.Name);
                        if (selectedUser != null)
                        {
                            userFile.Users.Remove(selectedUser);
                        }
                    }

                    configBackup.Dispose();
                    configBackup = new MemoryStream();
                    context.ConfigurationFile.Write(configBackup);
                }

                context.LogInfo("Copying configuration backup to file system...");
                using (var fs = configBackupDestination.Open(FileMode.Create, FileAccess.Write))
                {
                    configBackup.Position = 0;
                    configBackup.CopyTo(fs);
                }
            }
        }
    }
}
