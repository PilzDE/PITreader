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
