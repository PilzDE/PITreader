using CommandLine;
using Pilz.PITreader.Client;
using Pilz.PITreader.Client.Model;
using System;
using System.Threading.Tasks;

namespace PITreaderTool
{
    class Program
    {
        class BaseOptions
        {
            [Option('h', Default = "192.168.0.12", HelpText = "Hostname or ip address of PITreader")]
            public string Host { get; set; }

            [Option('p', Default = (ushort)443, HelpText = "HTTPS port number")]
            public ushort Port { get; set; }

            [Option("accept-all", HelpText = "Dangerous! Accepts all certificates.")]
            public bool AcceptAllCerts { get; set; }

            [Option("thumbprint", HelpText = "Sha256 fingerprint of trusted PITreader certificate")]
            public string FingerprintSha256 { get; set; }

            [Option("api-token", Required = true, HelpText = "API token to access PITreader REST API.")]
            public string ApiToken { get; set; }
        }

        [Verb("udc", HelpText = "User data configuration tasks")]
        class UdcOptions : BaseOptions
        {
            [Option(SetName = "import", HelpText = "Import user data configuration from JSON file.")]
            public string ImportPath { get; set; }


            [Option(SetName = "export", HelpText = "Export user data configuration to JSON file.")]
            public string ExportPath { get; set; }
        }

        [Verb("xpndr", HelpText = "Transponder data tasks")]
        class XpndrOptions : BaseOptions
        {
            [Option("export", SetName = "export", HelpText = "Export transponder data to JSON file.")]
            public bool Export { get; set; }


            [Option("write", SetName = "write", HelpText = "Import data from JSON file to a transponder key.")]
            public bool Write { get; set; }


            [Option("update-udc", SetName = "write", Default = false, HelpText = "If set, user data configuration is updated from the given transponder template.")]
            public bool UpdateUdc { get; set; }


            [Option("loop", SetName = "write", Default = false, HelpText = "If set, app will loop and write data to every inserted transponder.")]
            public bool Loop { get; set; }

            [Value(0, Required = true, MetaName = "path", HelpText = "Path to JSON file.")]
            public string Path { get; set; }

        }

        static int Main(string[] args)
        {
            // PITreaderTool.exe [--host=192.168.12.0|-h=192.168.12.0] [--port=443|-p=443] --accept-all|--thumbprint=<sha2 hex> [api token] COMMAND OPTIONS
            // 
            // Commands
            //  udc export <path to json>                   Exports user data configuration of device to JSON file.
            //  udc import <path to json>                   Imports user data configuration from JSON file into device.
            //
            //  xpndr export <path to json>                 Export all data stored on a transponder into a JSON file
            //  xpndr write --update-udc --loop <path to json>     Imports all data from a JSON file and writes it to a transponder
            //                                                          if --update-udc is set the user data configuration in the device is updated, if necessary.
            //                                                          if --loop is set the operation runs forever and updates transponder when inserted.
            //
            //  bl export <path to csv>
            //  bl import <path to csv>
            //
            //  auth --csv <path to csv>|--json <path to json>
            //Console.WriteLine("Hello World!");

            return Parser.Default.ParseArguments<UdcOptions, XpndrOptions>(args)
                .MapResult(
                  (UdcOptions opts) => { return 0; },
                  (XpndrOptions opts) => CommandXpndrAsync(opts).Result,
                  errs => 1);
        }

        private static PITreaderClient CreateClient(BaseOptions options)
        {
            var client = new PITreaderClient(
                options.Host,
                options.Port,
                options.AcceptAllCerts ? CertificateValidators.AcceptAll : CertificateValidators.AcceptThumbprintSha2(options.FingerprintSha256))
            {
                ApiToken = options.ApiToken
            };

            return client;
        }

        private static async Task<int> CommandXpndrAsync(XpndrOptions options)
        {
            Console.WriteLine("Initializing...");
            var client = CreateClient(options);

            if (options.Export)
            {
                var manager = new TransponderManager(client);
                var data = await manager.GetTransponderDataAsync();
                manager.ExportToFile(data, options.Path);
            }
            else if (options.Write)
            {
                var manager = new TransponderManager(client);
                var data = manager.ImportFromFile(options.Path);

                if (options.UpdateUdc && data.UserData != null && data.UserData.ParameterDefintion != null)
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

                if (options.Loop)
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

            return 0;
        }
    }
}
