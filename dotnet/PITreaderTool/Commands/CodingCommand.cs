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
    internal class CodingCommand : Command
    {
        /*
            //  coding set <coding> [<comment>]
            //  coding delete
         */
        public CodingCommand(IValueDescriptor<ConnectionProperties> connectionPropertiesBinder)
            : base("coding", "Set basic coding of device")
        {
            var codingArgument = new Argument<string>("coding", "coding identifier");
            var commentArgument = new Argument<string>("comment", () => null, "coding comment");

            var setCommand = new Command("set", "Set basic coding of device");
            setCommand.AddArgument(codingArgument);
            setCommand.AddArgument(commentArgument);

            System.CommandLine.Handler.SetHandler(setCommand, (ConnectionProperties c, string id, string comment, IConsole console) => this.HandleSet(c, id, comment, console), connectionPropertiesBinder, codingArgument, commentArgument);
            this.AddCommand(setCommand);

            var deleteCommand = new Command("delete", "Delete basic coding of device");
            System.CommandLine.Handler.SetHandler(deleteCommand, (ConnectionProperties c, IConsole console) => this.HandleDelete(c, console), connectionPropertiesBinder);
            this.AddCommand(deleteCommand);
        }

        private async Task HandleSet(ConnectionProperties properties, string identifier, string comment, IConsole console)
        {
            using (var client = properties.CreateClient())
            {
                var result = await client.SetBasicCoding(new BasicCodingRequest
                {
                    Identifier = identifier,
                    Comment = string.IsNullOrEmpty(comment) ? null : comment
                });

                if (result.Success)
                {
                    console.WriteLine("Coding set.");
                    var getResult = await client.GetBasicCoding();
                    if (getResult.Success)
                    {
                        console.WriteLine("Coding checksum: " + getResult.Data.Checksum);
                    }
                }
                else
                {
                    console.WriteError("Set coding failed: " + result.ErrorData?.Message);
                }
            }
        }

        private async Task HandleDelete(ConnectionProperties properties, IConsole console)
        {
            using (var client = properties.CreateClient())
            {
                var result = await client.SetBasicCoding(new BasicCodingRequest
                {
                    Identifier = string.Empty
                });

                if (result.Success) console.WriteLine("Coding deleted.");
                else console.WriteError("Deleting coding failed: " + result.ErrorData?.Message);
            }
        }
    }
}
