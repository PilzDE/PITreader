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
using Pilz.PITreader.Network;

namespace Pilz.PITreader.CommissioningTool.Commands
{
    internal class ListCommand : Command
    {
        public ListCommand() 
            : base("list", "Lists all devices on connected networks")
        {
            var scanTimeoutOption = new Option<ushort>(new[] { "--scan-timeout", "-t" }, getDefaultValue: () => 5, "Timemout for network scan operation in seconds");
            this.AddOption(scanTimeoutOption);

            this.SetHandler(context =>
            {
                ushort timeout = context.ParseResult.GetValueForOption(scanTimeoutOption);
                return this.HandleCommand(context.Console, timeout);
            });
        }

        private async Task HandleCommand(IConsole console, ushort timeout)
        {
            var devices = new List<PITreaderScanResult>();
            var scan = new PITreaderNetworkScan(null);

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
            scan.Stop();

            if (devices.Count > 0)
            {
                console.WriteLine("---------------------------------------------------------------------------------");
                console.WriteLine("| Order number | Serial number | MAC address       | IP address    | HTTPS port |");
                console.WriteLine("---------------------------------------------------------------------------------");
                foreach (var device in devices)
                {
                    Console.WriteLine($"| {device.OrderNumber,-12} | {device.SerialNumber,-13} | {device.MacAddress,-17} | {device.IpAddress,-13} | {device.HttpsPort,-10} |");
                }
                console.WriteLine("---------------------------------------------------------------------------------");
            }
        }
    }
}
