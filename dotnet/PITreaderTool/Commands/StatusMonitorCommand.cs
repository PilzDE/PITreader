using System;
using System.CommandLine;
using System.CommandLine.Binding;
using System.Threading.Tasks;
using Pilz.PITreader.Client;

namespace Pilz.PITreader.Tool.Commands
{
    internal class StatusMonitorCommand : Command
        {
            public StatusMonitorCommand(IValueDescriptor<ConnectionProperties> connectionPropertiesBinder)
                : base("monitor", "Monitor the status of a PITreader")
            {
                System.CommandLine.Handler.SetHandler(this, (ConnectionProperties c, IConsole console) => this.Handle(c, console), connectionPropertiesBinder);
            }

            private Task Handle(ConnectionProperties properties, IConsole console)
            {
                using (var client = properties.CreateClient())
                {   
                    var monitor = new PITreaderStatusMonitor(client);
                    monitor.OnChange += (_, e) =>
                    {
                        if (e != null)
                        {
                            console.WriteLine($"Detected change: {e.Change}");
                        }
                    };

                    console.WriteLine("Waiting for transponders...");
                    console.WriteLine("[Enter] to abort.");
                    Console.ReadLine();
                }

                return Task.CompletedTask;
            }
        }
    }
