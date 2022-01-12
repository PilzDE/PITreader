using System;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Linq;
using Pilz.PITreader.Tool.Commands;

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

            var hostOption = new Option<string>(new[] { "--host", "-h" }, getDefaultValue: () => "192.168.0.12", "IP address or hostname of PITreader device");
            var portOption = new Option<ushort>(new[] { "--port", "-p" }, getDefaultValue: () => 443, "HTTPS port number of PITreader device");
            var acceptAllOption = new Option<bool>("--accept-all", () => false, "If set, certificates of the PITreader device are not validated");
            var thumbprintOption = new Option<string>("--thumbprint", "Sha2 hex thumbprint of certificate of PITreader device");

            var apiTokenArgument = new Argument<string>("api token", "API token (see Configuration -> API Clients) for REST API of PITreader device");

            var clientBinder = new ConnectionPropertiesBinder(hostOption, portOption, acceptAllOption, thumbprintOption, apiTokenArgument);

            var rootCommand = new RootCommand("Tool to automate tasks using the REST API of PITreader devices");
            rootCommand.AddGlobalOption(hostOption);
            rootCommand.AddGlobalOption(portOption);
            rootCommand.AddGlobalOption(acceptAllOption);
            rootCommand.AddGlobalOption(thumbprintOption);
            rootCommand.AddArgument(apiTokenArgument);

            rootCommand.AddCommand(new TransponderCommand(clientBinder));
            rootCommand.AddCommand(new BlocklistCommand(clientBinder));
            rootCommand.AddCommand(new UserDataConfigurationCommand(clientBinder));

            rootCommand.AddValidator(commandResult =>
            {
                if (!commandResult.Children.Any(sr => sr.Symbol is IdentifierSymbol id && id.HasAlias(acceptAllOption.Name))
                    && !commandResult.Children.Any(sr => sr.Symbol is IdentifierSymbol id && id.HasAlias(thumbprintOption.Name)))
                {
                    return $"Either option '{acceptAllOption.Name}' or '{thumbprintOption.Name}' needs to be used.";
                }

                return default(string);
            });

            return new CommandLineBuilder(rootCommand)
                .UseDefaults()
                .UseHelp()
                .UseExceptionHandler(HandleException)
                .Build()
                .Invoke(args);
        }

        internal static void HandleException(Exception exception, InvocationContext context)
        {
            try
            {
                if (!(exception is OperationCanceledException))
                {
                    context.Console.WriteError(exception.Message);
                }
                context.ExitCode = 1;
            }
            catch
            {
                // pass
            }
        }
    }
}