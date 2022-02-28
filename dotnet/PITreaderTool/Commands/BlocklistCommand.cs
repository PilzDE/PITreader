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

using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Binding;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Pilz.PITreader.Client;
using Pilz.PITreader.Client.Model;

namespace Pilz.PITreader.Tool.Commands
{
    internal class BlocklistCommand : Command
    {
        /*
            //  bl export <path to csv>
            //  bl import <path to csv>
         */
        public BlocklistCommand(IValueDescriptor<ConnectionProperties> connectionPropertiesBinder)
            : base("bl", "Import of blocklists")
        {
            var csvPathArgument = new Argument<string>("path to csv");

            var importCommand = new Command("import", "Import blocklist from a csv file into a device");
            importCommand.AddArgument(csvPathArgument);

            System.CommandLine.Handler.SetHandler(importCommand, (ConnectionProperties c, string s) => this.HandleImport(c, s), connectionPropertiesBinder, csvPathArgument);
            this.AddCommand(importCommand);
        }

        private async Task HandleImport(ConnectionProperties properties, string pathToCsv)
        {
            IList<BlocklistEntry> imported = new List<BlocklistEntry>();
            using (var fileReader = File.OpenText(pathToCsv))
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    DetectDelimiter = true,
                    HasHeaderRecord = true
                };

                var csvReader = new CsvReader(fileReader, config);

                csvReader.Read();
                csvReader.ReadHeader();

                while (csvReader.Read())
                {
                    imported.Add(new BlocklistEntry
                    {
                        Id = csvReader.GetField<string>(0),
                        Comment = csvReader.GetField<string>(1)
                    });
                }
            }

            using (var client = properties.CreateClient())
            {
                var manager = new BlocklistManager(client);
                await manager.SyncAsync(imported);
            }
        }
    }
}
