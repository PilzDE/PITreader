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
using System.Net;
using System.Net.NetworkInformation;
using Pilz.PITreader.Client;
using Pilz.PITreader.Network;
using Pilz.PITreader.Network.Configuration;

namespace Pilz.PITreader.CommissioningTool.Tasks
{
    internal class ScanAndSetupTask : ICommissioningTask
    {
        private Option<ushort> scanTimeoutOption = new Option<ushort>(new[] { "--scan-timeout", "-t" }, getDefaultValue: () => 5, "Timemout for network scan operation in seconds");

        private Option<IPAddress> setIpOption = new Option<IPAddress>(new[] { "--set-ip" }, "Target ip address")
        {
            ArgumentHelpName = "ip address",
            Arity = ArgumentArity.ExactlyOne,
            IsRequired = true
        };
        
        private Option<IPAddress> setNetmaskOption = new Option<IPAddress>(new[] { "--set-netmask" }, getDefaultValue: () => IPAddress.Parse("255.255.255.0"), "Target netmask")
        {
            ArgumentHelpName = "netmask",
            Arity = ArgumentArity.ZeroOrOne
        };

        private Option<IPAddress> setGatewayOption = new Option<IPAddress>(new[] { "--set-gateway" }, getDefaultValue: () => IPAddress.Any, "Target default gateway")
        {
            ArgumentHelpName = "ip address",
            Arity = ArgumentArity.ZeroOrOne
        };

        public Option[] Options => new Option[] { this.scanTimeoutOption, this.setIpOption, this.setNetmaskOption, this.setGatewayOption };

        public bool CheckOptions(TaskContext context)
        {
            return true;
        }

        public async Task RunAsync(TaskContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            ushort timeout = context.GetValue(scanTimeoutOption);

            IPAddress setIp = context.GetValue(this.setIpOption)!;
            IPAddress setNetmask = context.GetValue(this.setNetmaskOption)!;
            IPAddress setGateway = context.GetValue(this.setGatewayOption)!;

            var matchingNics = NetworkInterfaceService.GetMatchingInterfaces(setIp).ToList();

            context.LogInfo("Scanning for PITreader device...");
            var device = await this.ScanNetworkAsync(timeout, matchingNics);
            if (device == null)
            {
                context.LogError("Unable to find single device with default ip.");
                context.Invocation.ExitCode = 1;
                return;
            }

            context.LogInfo("Setting network configuration...");
            var mcp = new ConfigurationService(matchingNics, TimeSpan.FromSeconds(5));
            var mcpResponse = await mcp.SendAsnyc(new SetIPv4Request
            {
                OrderNumber = device.OrderNumber,
                SerialNumber = device.SerialNumber,
                DeviceIp = setIp.ToString(),
                Netmask = setNetmask.ToString(),
                GatewayIp = setGateway.ToString()
            });

            if (mcpResponse == null || !mcpResponse.Success)
            {
                context.LogError("Unable to set network configuration.");
                context.Invocation.ExitCode = 1;
                return;
            }

            // Update IP address for upcomming tasks
            context.DeviceIp = setIp.ToString();
            context.HttpsPort = device.HttpsPort;

            context.ApiUser = new CommissioningUser(PITreaderInteractiveClient.DefaultUsername)
            {
                InteractiveLogin = true,
                PasswordOrApiToken = device.SerialNumber,
                DeleteOnCleanup = false
            };

            // wait for reboot
            await context.WaitForReboot();

            // Dummy call to init session
            await context.GetClientAsync();
        }

        private async Task<PITreaderScanResult?> ScanNetworkAsync(ushort timeout, IEnumerable<NetworkInterface> matchingNics)
        {
            var devices = new List<PITreaderScanResult>();
            var scan = new PITreaderNetworkScan(nics => nics.Where(n => matchingNics.Any(m => m.Id == n.Id)));
            scan.DeviceDiscovered += (_, e) => {
                var existing = devices.FirstOrDefault(d => d.Equals(e.Device));
                if (existing == null)
                {
                    devices.Add(e.Device);
                }
                else
                {
                    existing.Update(e.Device);
                }
            };

            scan.Start();
            do
            {
                scan.Query();
                await Task.Delay(1000);

                if (timeout > 0)
                {
                    timeout -= 1;
                }
            }
            while (timeout > 0);

            return devices.SingleOrDefault(d => d.IpAddress == PITreaderClient.DefaultIp);
        }
    }
}
