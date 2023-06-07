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
using System.CommandLine.Binding;
using System.IO;
using System.Threading.Tasks;
using Pilz.PITreader.Client;

namespace Pilz.PITreader.Tool.Commands
{
    internal class FirmwareCommand : Command
    {
        /*
            //  firmware version
            //  firmware update <path to fwu> [--force]
         */
        public FirmwareCommand(IValueDescriptor<ConnectionProperties> connectionPropertiesBinder)
            : base("firmware", "Firmware update")
        {
            var versionCommand = new Command("version", "Get firmware version");

            System.CommandLine.Handler.SetHandler(versionCommand, (ConnectionProperties c, IConsole console) => this.HandleVersion(c, console), connectionPropertiesBinder);
            this.AddCommand(versionCommand);

            var fwuPathArgument = new Argument<string>("path to *.fwu");
            var forceOption = new Option<bool>("--force", () => false, "if set the update is started also the same or newerr firmware is already running on the device");

            var updateCommand = new Command("update", "Update firmware of device");
            updateCommand.AddArgument(fwuPathArgument);
            updateCommand.AddOption(forceOption);

            System.CommandLine.Handler.SetHandler(updateCommand, (ConnectionProperties c, IConsole console, string s, bool b) => this.HandleUpdate(c, console, s, b), connectionPropertiesBinder, fwuPathArgument, forceOption);
            this.AddCommand(updateCommand);
        }

        private async Task HandleVersion(ConnectionProperties properties, IConsole console)
        {
            using (var client = properties.CreateClient())
            {
                var manager = new PITreaderFirmwareManager(client);
                var version = await manager.GetPITreaderFirmwareVersionAsync();

                if (version == null)
                {
                    console.WriteError("Error reading version");
                }
                else
                {
                    console.WriteLine($"Version {version}");
                }
            }
        }

        private async Task HandleUpdate(ConnectionProperties properties, IConsole console, string pathToFwu, bool force)
        {
            if (!File.Exists(pathToFwu))
            {
                console.WriteError("Specified update file does not exist.");
                return;
            }

            var fwu = new PITreaderFirmwarePackage(pathToFwu);

            using (var client = properties.CreateClient())
            {
                var manager = new PITreaderFirmwareManager(client);
                var result = await manager.PerformUpdateAsync(fwu, force);

                switch (result)
                {
                    case PITreaderFirmwareUpdateResult.Success:
                        console.WriteLine($"Updating to {fwu.PackageVersion}. Now restarting...");
                        break;
                    case PITreaderFirmwareUpdateResult.AlreadyInstalledOrNewer:
                        console.WriteLine("The firmware or a newer version is already running on the device.");
                        break;
                    case PITreaderFirmwareUpdateResult.UpdateError:
                        console.WriteError("Error performing update.");
                        break;
                    case PITreaderFirmwareUpdateResult.UploadError:
                        console.WriteError("Error uploading firmware to device.");
                        break;
                }
            }
        }
    }
}
