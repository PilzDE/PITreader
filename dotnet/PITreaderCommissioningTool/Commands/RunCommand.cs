// Copyright (c) 2023 Pilz GmbH & Co. KG
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice (including the next paragraph) shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
// SPDX-License-Identifier: MIT

using System.CommandLine;
using System.CommandLine.Invocation;
using Pilz.PITreader.CommissioningTool.Tasks;

namespace Pilz.PITreader.CommissioningTool.Commands
{
    internal class RunCommand : Command
    {
        private const string helpDescription = @"Performs commissioning of a PITreader device

Exit codes:
  0 - all tasks successfully executed
  1 - execution failed and was aborted
  2 - one or more tasks failed, but execution of other tasks continued
";

        private IList<ICommissioningTask> tasks = new List<ICommissioningTask>
        {
            new ScanAndSetupTask(),

            new FirmwareUpdateTask(),

            new ConfigurationFileTask(),

            // all tasks manpulating the configuration file go here
            new CommissioningUserTask(),
            new GenerateUserSecretsTask(),
            new OpcUaClientCertificateTask(),

            new RestoreConfigurationTask(),

            new CodingTask(),
            new TlsCertificateTask(),
            new OpcUaServerCertificateTask(),
            new ConfigurationBackupTask(),

            new CommissioningUserCleanupTask()
        };

        public RunCommand()
            : base("run", helpDescription)
        {
            foreach (var task in this.tasks)
            {
                foreach (var symbol in task.Options)
                {
                    this.AddOption(symbol);
                }
            }

            this.SetHandler((InvocationContext context) => this.HandleCommand(context));
        }

        private async Task HandleCommand(
            InvocationContext context)
        {
            var taskContext = new TaskContext(context);

            // Check command line options
            foreach (var task in this.tasks)
            {
                if (!task.CheckOptions(taskContext))
                {
                    context.ExitCode = 1;
                    return;
                }
            }

            // Run tasks
            foreach (var task in this.tasks)
            {
                await task.RunAsync(taskContext);

                if (context.ExitCode == 1)
                {
                    // Skip further tasks, if fatal error occured
                    return;
                }
            }
        }
    }
}
