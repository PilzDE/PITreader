﻿// Copyright (c) 2022 Pilz GmbH & Co. KG
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

using System;
using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Invocation;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Pilz.PITreader.Client;
using Pilz.PITreader.Client.Model;

namespace Pilz.PITreader.Tool.Commands
{
    internal class TransponderCommand : Command
    {
        /*
            //  xpndr export <path to json>                 Export all data stored on a transponder into a JSON file
            //  xpndr write --update-udc --loop <path to json>     Imports all data from a JSON file and writes it to a transponder
            //                                                          if --update-udc is set the user data configuration in the device is updated, if necessary.
            //                                                          if --loop is set the operation runs forever and updates transponder when inserted.
         */
        public TransponderCommand(ConnectionPropertiesBinder connectionPropertiesBinder)
            : base("xpndr", "Export and writing of transponder content")
        {
            var jsonPathArg = new Argument<string>("path to json");

            var xpndrExportCommand = new Command("export", "Export content of a transponder to file");
            xpndrExportCommand.AddArgument(jsonPathArg);

            xpndrExportCommand.SetHandler(ctx =>
            {
                var conn = connectionPropertiesBinder.GetValue(ctx);
                string pathToJson = ctx.ParseResult.GetValueForArgument(jsonPathArg);

                return this.HandleExport(conn, pathToJson);
            });

            this.AddCommand(xpndrExportCommand);

            var xpndrWriteCommand = new Command("write", "Write data from file (see export) to transponders");
            var updateUdcOption = new Option<bool>("--update-udc", () => false, "if set the user data configuration in the device is updated, if necessary.");
            xpndrWriteCommand.AddOption(updateUdcOption);

            var loopOption = new Option<bool>("--loop", () => false, "if set the operation runs forever and updates transponder when inserted.");
            xpndrWriteCommand.AddOption(loopOption);

            xpndrWriteCommand.AddArgument(jsonPathArg);

            xpndrWriteCommand.SetHandler((InvocationContext c) =>
            {
                var conn = connectionPropertiesBinder.GetValue(c);
                bool updateUdc = c.ParseResult.GetValueForOption(updateUdcOption);
                bool loop = c.ParseResult.GetValueForOption(loopOption);
                string pathToJson = c.ParseResult.GetValueForArgument(jsonPathArg);

                return this.HandleWrite(conn, updateUdc, loop, pathToJson, c.Console);
            });

            this.AddCommand(xpndrWriteCommand);

            var xpndrLogCommand = new Command("log", "Log ids of transponder to file");

            var csvPathArg = new Argument<string>("path to csv");
            xpndrLogCommand.AddArgument(csvPathArg);

            xpndrLogCommand.SetHandler((InvocationContext c) =>
            {
                var conn = connectionPropertiesBinder.GetValue(c);
                string pathToCsv = c.ParseResult.GetValueForArgument(csvPathArg);

                return this.HandleLog(conn, pathToCsv, c.Console);
            });

            this.AddCommand(xpndrLogCommand);
        }

        private async Task<int> HandleExport(ConnectionProperties properties, string pathToJson)
        {
            using (var client = properties.CreateClient())
            {
                var manager = new TransponderManager(client);
                var data = await manager.GetTransponderDataAsync();
                return manager.ExportToFile(data, pathToJson) ? 0 : 1;
            }
        }

        private async Task HandleWrite(ConnectionProperties properties, bool updateUdc, bool loop, string pathToJson, IConsole console)
        {
            using (var client = properties.CreateClient())
            {
                var manager = new TransponderManager(client);
                var data = manager.ImportFromFile(pathToJson);

                if (updateUdc && data.UserData != null && data.UserData.ParameterDefintion != null)
                {
                    var udcManager = new UserDataConfigManager(client);
                    var udc = await udcManager.GetConfigurationAsync();
                    if (!udc.Success)
                    {
                        console.WriteError("Reading user data configuration from device failed.");
                    }
                    else
                    {
                        if (!UserDataConfigManager.AreConfigurationsMatching(udc.Data.Parameters, data.UserData.ParameterDefintion)
                            || udc.Data.Version != data.UserData.ParameterDefintion.Version)
                        {
                            if ((await udcManager.ApplyConfigurationAsync(data.UserData.ParameterDefintion)).Success)
                            {
                                console.WriteLine("Updated user data on device.");
                            }
                            else
                            {
                                console.WriteError("Updating user data configuration in device failed.");
                            }
                        }
                        else
                        {
                            console.WriteLine("User data configuration in device up to date.");
                        }
                    }
                }

                if (loop)
                {
                    string teachInId = string.Empty;
                    var monitor = new ApiEndpointMonitor<TransponderResponse>(
                        client,
                        ApiEndpoints.Transponder,
                        r => r != null && r.TeachInId != teachInId,
                        r =>
                        {
                            teachInId = r.TeachInId;
                            if (string.IsNullOrWhiteSpace(teachInId) || string.IsNullOrWhiteSpace(teachInId.Replace("0", string.Empty))) return Task.CompletedTask;

                            var t = manager.WriteDataToTransponderAsync(data);
                            return t.ContinueWith(z =>
                            {
                                if (z.Result == null) console.WriteLine("Updated data on tranponder, Security ID: " + r.SecurityId);
                                else console.WriteError("failed to update data on tranponder, Security ID: " + r.SecurityId 
                                    + (string.IsNullOrWhiteSpace(z.Result.Message) ? string.Empty : (" (" + z.Result.Message + ")")));
                            });
                        },
                        1000);
                    console.WriteLine("Waiting for transponders...");
                    console.WriteLine("[Enter] to abort.");
                    Console.ReadLine();
                }
                else
                {
                    var result = await manager.WriteDataToTransponderAsync(data);
                    if (result == null) console.WriteLine("Data successfully written to transponder.");
                    else console.WriteError("Updating data on transponder" + (string.IsNullOrWhiteSpace(result.Message) ? "." : (": " + result.Message)));
                }
            }
        }

        private async Task HandleLog(ConnectionProperties properties, string pathToCsv, IConsole console)
        {
            using (var client = properties.CreateClient())
            {
                using (var writer = File.Open(pathToCsv, FileMode.Append, FileAccess.Write, FileShare.Read))
                {
                    console.WriteLine("Collecting order number, serial number and security id of transponders...");

                    if (writer.Position == 0)
                    {
                        byte[] data = Encoding.UTF8.GetBytes("Order Number;Serial Number;Security ID" + Environment.NewLine);
                        await writer.WriteAsync(data, 0, data.Length);
                        await writer.FlushAsync();
                    }

                    SecurityId securityId = null;
                    var monitor = new ApiEndpointMonitor<AuthenticationStatusResponse>(
                        client,
                        ApiEndpoints.StatusAuthentication,
                        r => r != null && r.SecurityId != securityId,
                        async r => 
                        {
                            securityId = r.SecurityId;
                            if (securityId == null) return;

                            string line = $"{r.OrderNumber};{r.SerialNumber};{r.SecurityId}";
                            console.WriteLine(line);

                            byte[] data = Encoding.UTF8.GetBytes(line + Environment.NewLine);
                            await writer.WriteAsync(data, 0, data.Length);
                            await writer.FlushAsync();
                        },
                        1000);

                    console.WriteLine("Waiting for transponders...");
                    console.WriteLine("[Enter] to abort.");
                    Console.ReadLine();
                }
            }
        }
    }
}
