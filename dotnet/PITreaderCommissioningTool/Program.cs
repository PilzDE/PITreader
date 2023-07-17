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
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using Pilz.PITreader.CommissioningTool.Commands;

namespace Pilz.PITreader.CommissioningTool
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            var rootCommand = new RootCommand("Tool to automate commissioning of PITreader devices");

            rootCommand.AddCommand(new ListCommand());
            rootCommand.AddCommand(new RunCommand());

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