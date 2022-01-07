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
        public BlocklistCommand(IValueDescriptor<PITreaderClient> clientBinder)
            : base ("bl")
        {
            var csvPathArgument = new Argument<string>("path to csv");    

            var importCommand = new Command("import");            
            importCommand.AddArgument(csvPathArgument);

            System.CommandLine.Handler.SetHandler(importCommand, (PITreaderClient c, string s) => this.HandleImport(c, s), clientBinder, csvPathArgument);
            this.AddCommand(importCommand);
        }

        private async Task HandleImport(PITreaderClient client, string pathToCsv)
        {
            var manager = new BlocklistManager(client);
            await manager.SyncFromCsvAsync(pathToCsv);
        }
    }
}
