using Pilz.PITreader.Client;
using Pilz.PITreader.Client.Model;
using Pilz.PITreader.Tool.Commands;
using System;
using System.CommandLine;
using System.Threading.Tasks;

namespace Pilz.PITreader.Tool
{
    internal class Program
    {
        private static int Main(string[] args)
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

            var hostOption = new Option<string>(new[] { "--host", "-h" }, getDefaultValue: () => "192.168.0.12");
            var portOption = new Option<ushort>(new[] { "--port", "-p" }, getDefaultValue: () => 443);
            var acceptAllOption = new Option<bool>("--accept-all", () => false);
            var thumbprintOption = new Option<string>("--thumbprint", "sha2 hex thumbprint of certificate");
            var apiTokenArgument = new Argument<string>("api token");

            var clientBinder = new PITreaderClientBinder(hostOption, portOption, acceptAllOption, thumbprintOption, apiTokenArgument);

            var rootCommand = new RootCommand();
            rootCommand.AddOption(hostOption);
            rootCommand.AddOption(portOption);
            rootCommand.AddOption(acceptAllOption);
            rootCommand.AddOption(thumbprintOption);
            rootCommand.AddArgument(apiTokenArgument);

            rootCommand.AddCommand(new TransponderCommand(clientBinder));
            rootCommand.AddCommand(new BlocklistCommand(clientBinder));
            rootCommand.AddCommand(new UserDataConfigurationCommand(clientBinder));

            return rootCommand.Invoke(args);
        }
    }
}