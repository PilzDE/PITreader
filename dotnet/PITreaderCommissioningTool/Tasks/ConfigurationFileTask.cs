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
using Pilz.PITreader.Configuration.Model;
using Pilz.PITreader.Configuration;

namespace Pilz.PITreader.CommissioningTool.Tasks
{
    internal class ConfigurationFileTask : ICommissioningTask
    {
        private Option<FileInfo> configOption = new Option<FileInfo>(new[] { "--config" }, "Configuration file to be restored")
        {
            ArgumentHelpName = "path to file"
        };

        private Option<bool> configStripNetworkOption = new Option<bool>(new[] { "--config-strip-network" }, getDefaultValue: () => false, "If set, the network settings are stripped from the configuration file before restore");

        public ConfigurationFileTask()
        {
            this.configOption.ExistingOnly();
        }

        public Option[] Options => new Option[] { this.configOption, this.configStripNetworkOption };

        public bool CheckOptions(TaskContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            FileInfo? config = context.GetValue(this.configOption);
            bool configStripNetwork = context.GetValue(this.configStripNetworkOption);
            if (config == null && configStripNetwork)
            {
                context.LogError("No configuration file specified, but option to strip network settings.");
                return false;
            }

            return true;
        }

        public Task RunAsync(TaskContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            FileInfo? configFile = context.GetValue(this.configOption);
            bool configStripNetwork = context.GetValue(this.configStripNetworkOption);

            if (configFile != null)
            {
                context.LogInfo("Reading configuration file...");
                using (var stream = configFile.OpenRead())
                {
                    context.ConfigurationFile.Read(stream);
                }

                if (configStripNetwork)
                {
                    var settings = context.ConfigurationFile.Get<SettingsFile>();
                    if (settings != null)
                    {
                        var networkSettingIds = new[]
                        {
                            SettingsParameter.TCPIP_IP_ADDRESS,
                            SettingsParameter.TCPIP_IP_SUBNETMASK,
                            SettingsParameter.TCPIP_IP_GATEWAY
                        };

                        settings.Parameter.RemoveAll(p => networkSettingIds.Contains(p.Id));

                        context.LogInfo("Stripped network settings from configuration file...");
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}
