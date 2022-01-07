using Pilz.PITreader.Client;
using Pilz.PITreader.Client.Model;
using System.CommandLine;
using System.CommandLine.Binding;
using System.IO;
using System.Threading.Tasks;

namespace Pilz.PITreader.Tool.Commands
{
    internal class UserDataConfigurationCommand : Command
    {
        /*
         
            //  udc export <path to json>                   Exports user data configuration of device to JSON file.
            //  udc import <path to json>                   Imports user data configuration from JSON file into device.
         
         */
        public UserDataConfigurationCommand(IValueDescriptor<PITreaderClient> clientBinder)
            : base("udc")
        {
            var jsonPathArg = new Argument<string>("path to json");

            var exportCommand = new Command("export");
            exportCommand.AddArgument(jsonPathArg);
            System.CommandLine.Handler.SetHandler(exportCommand, (PITreaderClient c, string s) => this.HandleExport(c, s), clientBinder, jsonPathArg);
            this.AddCommand(exportCommand);

            var importCommand = new Command("import");
            importCommand.AddArgument(jsonPathArg);
            System.CommandLine.Handler.SetHandler(importCommand, (PITreaderClient c, string s) => this.HandleImport(c, s), clientBinder, jsonPathArg);
            this.AddCommand(importCommand);
        }

        private async Task HandleExport(PITreaderClient client, string pathToJson)
        {
            var manager = new UserDataConfigManager(client);
            await manager.ExportToFile(pathToJson);
        }

        private async Task HandleImport(PITreaderClient client, string pathToJson)
        {
            UserDataConfigResponse data;
            using (var file = File.OpenText(pathToJson))
            {
                data = PITreaderJsonSerializer.Deserialize<UserDataConfigResponse>(file.ReadToEnd());
            }

            var manager = new UserDataConfigManager(client);
            await manager.ApplyConfigurationAsync(data.Version, data.Comment, data.Parameters);
        }
    }
}

