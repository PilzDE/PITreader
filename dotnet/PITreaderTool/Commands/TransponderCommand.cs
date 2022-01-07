using Pilz.PITreader.Client;
using Pilz.PITreader.Client.Model;
using System;
using System.CommandLine;
using System.CommandLine.Binding;
using System.Threading.Tasks;

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
        public TransponderCommand(IValueDescriptor<PITreaderClient> clientBinder)
            : base ("xpndr")
        {
            var jsonPathArg = new Argument<string>("path to json");
            
            var xpndrExportCommand = new Command("export");
            xpndrExportCommand.AddArgument(jsonPathArg);
            System.CommandLine.Handler.SetHandler(xpndrExportCommand, (PITreaderClient c, string s) => this.HandleExport(c, s), clientBinder, jsonPathArg);
            this.AddCommand(xpndrExportCommand);
                       

            var xpndrWriteCommand = new Command("write");
            var updateUdcOption = new Option<bool>("--update-udc", () => false, "if set the user data configuration in the device is updated, if necessary.");
            xpndrWriteCommand.AddOption(updateUdcOption);

            var loopOption = new Option<bool>("--loop", () => false, "if set the operation runs forever and updates transponder when inserted.");
            xpndrWriteCommand.AddOption(loopOption);

            xpndrWriteCommand.AddArgument(jsonPathArg);

            System.CommandLine.Handler.SetHandler(xpndrWriteCommand, (PITreaderClient c, bool b1, bool b2, string s) => this.HandleWrite(c, b1, b2, s), clientBinder, updateUdcOption, loopOption, jsonPathArg);
            this.AddCommand(xpndrWriteCommand);
        }

        private async Task HandleExport(PITreaderClient client, string pathToJson)
        {
            var manager = new TransponderManager(client);
            var data = await manager.GetTransponderDataAsync();
            manager.ExportToFile(data, pathToJson);
        }

        private async Task HandleWrite(PITreaderClient client, bool updateUdc, bool loop, string pathToJson)
        {
            var manager = new TransponderManager(client);
            var data = manager.ImportFromFile(pathToJson);

            if (updateUdc && data.UserData != null && data.UserData.ParameterDefintion != null)
            {
                var udcManager = new UserDataConfigManager(client);
                var udc = await udcManager.GetConfigurationAsync();
                if (!udc.Success)
                {
                    Console.WriteLine("ERROR: Reading user data configuration from device failed.");
                }
                else
                {
                    if (!UserDataConfigManager.AreConfigurationsMatching(udc.Data.Parameters, data.UserData.ParameterDefintion)
                        || udc.Data.Version != data.UserData.ParameterDefintion.Version)
                    {
                        if ((await udcManager.ApplyConfigurationAsync(data.UserData.ParameterDefintion)).Success)
                        {
                            Console.WriteLine("Updated user data on device.");
                        }
                        else
                        {
                            Console.WriteLine("ERROR: Updating user data configuration in device failed.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("User data configuration in device up to date.");
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
                        return t.ContinueWith(z => Console.WriteLine(z.Result ? ("Updated data on tranponder, Security ID: " + r.SecurityId) : ("ERROR: failed to update data on tranponder, Security ID: " + r.SecurityId)));
                    },
                    1000);
                Console.WriteLine("Waiting for transponders...");
                Console.WriteLine("[Enter] to abort.");
                Console.ReadLine();
            }
            else
            {
                var result = await manager.WriteDataToTransponderAsync(data);
                if (result) Console.WriteLine("Data successfully written to transponder.");
                else Console.WriteLine("ERROR: Updating data on transponder.");
            }
        }
    }
}
