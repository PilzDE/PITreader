using Pilz.PITreader.Client;
using System.CommandLine;
using System.CommandLine.Binding;
using System.Threading.Tasks;

namespace Pilz.PITreader.Tool.Commands
{
    internal class BlocklistCommand : Command
    {
        /*
            //  bl export <path to csv>
            //  bl import <path to csv>
         */
        public BlocklistCommand(IValueDescriptor<ConnectionProperties> connectionPropertiesBinder)
            : base ("bl", "Import of blocklists")
        {
            var csvPathArgument = new Argument<string>("path to csv");    

            var importCommand = new Command("import", "Import blocklist from a csv file into a device");
            importCommand.AddArgument(csvPathArgument);

            System.CommandLine.Handler.SetHandler(importCommand, (ConnectionProperties c, string s) => this.HandleImport(c, s), connectionPropertiesBinder, csvPathArgument);
            this.AddCommand(importCommand);
        }

        private async Task HandleImport(ConnectionProperties properties, string pathToCsv)
        {
            using (var client = properties.CreateClient())
            {
                var manager = new BlocklistManager(client);
                await manager.SyncFromCsvAsync(pathToCsv);
            }
        }
    }
}
