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
        public UserDataConfigurationCommand(IValueDescriptor<ConnectionProperties> connectionPropertiesBinder)
            : base("udc", "Export and import of user data configurations")
        {
            var jsonPathArg = new Argument<string>("path to json");

            var exportCommand = new Command("export", "Export user data configuration from a device to a json file");
            exportCommand.AddArgument(jsonPathArg);
            System.CommandLine.Handler.SetHandler(exportCommand, (ConnectionProperties c, string s) => this.HandleExport(c, s), connectionPropertiesBinder, jsonPathArg);
            this.AddCommand(exportCommand);

            var importCommand = new Command("import", "Import user data confguration from a json file to a device");
            importCommand.AddArgument(jsonPathArg);
            System.CommandLine.Handler.SetHandler(importCommand, (ConnectionProperties c, string s) => this.HandleImport(c, s), connectionPropertiesBinder, jsonPathArg);
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

