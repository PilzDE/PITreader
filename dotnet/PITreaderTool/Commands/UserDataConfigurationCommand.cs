// Copyright (c) 2022 Pilz GmbH & Co. KG
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
using Pilz.PITreader.Client.Model;

namespace Pilz.PITreader.Tool.Commands
{
    internal class UserDataConfigurationCommand : Command
    {
        /*
            //  udc export <path to json>                   Exports user data configuration of device to JSON file.
            //  udc import <path to json>                   Imports user data configuration from JSON file into device.
         */
        public UserDataConfigurationCommand(ConnectionPropertiesBinder connectionPropertiesBinder)
            : base("udc", "Export and import of user data configurations")
        {
            var jsonPathArg = new Argument<string>("path to json");

            var exportCommand = new Command("export", "Export user data configuration from a device to a json file");
            exportCommand.AddArgument(jsonPathArg);

            exportCommand.SetHandler(ctx =>
            {
                var conn = connectionPropertiesBinder.GetValue(ctx);
                string jsonPath = ctx.ParseResult.GetValueForArgument(jsonPathArg);

                return this.HandleExport(conn, jsonPath);
            });

            this.AddCommand(exportCommand);

            var importCommand = new Command("import", "Import user data confguration from a json file to a device");
            importCommand.AddArgument(jsonPathArg);

            importCommand.SetHandler(ctx =>
            {
                var conn = connectionPropertiesBinder.GetValue(ctx);
                string jsonPath = ctx.ParseResult.GetValueForArgument(jsonPathArg);

                return this.HandleImport(conn, jsonPath);
            });

            this.AddCommand(importCommand);
        }

        private async Task<int> HandleExport(ConnectionProperties properties, string pathToJson)
        {
            using (var client = properties.CreateClient())
            {
                var manager = new UserDataConfigManager(client);
                return await manager.ExportToFile(pathToJson) ? 0 : 1;
            }
        }

        private async Task<int> HandleImport(ConnectionProperties properties, string pathToJson)
        {
            UserDataConfigResponse data;
            using (var file = File.OpenText(pathToJson))
            {
                data = PITreaderJsonSerializer.Deserialize<UserDataConfigResponse>(file.ReadToEnd());
            }

            using (var client = properties.CreateClient())
            {
                var manager = new UserDataConfigManager(client);
                var response = await manager.ApplyConfigurationAsync(data.Version, data.Comment, data.Parameters);
                return response.Success ? 0 : 1;
            }
        }
    }
}

