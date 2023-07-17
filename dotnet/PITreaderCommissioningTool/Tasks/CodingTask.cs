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
using Pilz.PITreader.Client;
using Pilz.PITreader.Client.Model;

namespace Pilz.PITreader.CommissioningTool.Tasks
{
    internal class CodingTask : ICommissioningTask
    {
        private Option<FileInfo> codingOption = new Option<FileInfo>(new[] { "--coding" }, "Coding identifier (first line) and optional comment (second line)")
        {
            ArgumentHelpName = "path to file",
            IsRequired = false
        };
        
        private Option<FileInfo> codingOemOption = new Option<FileInfo>(new[] { "--coding-oem" }, "OEM coding identifier (first line) and optional comment (second line)")
        {
            ArgumentHelpName = "path to file",
            IsRequired = false
        };

        public CodingTask()
        {
            this.codingOption.ExistingOnly();
            this.codingOemOption.ExistingOnly();
        }

        public Option[] Options => new Option[] { codingOption, codingOemOption };

        public bool CheckOptions(TaskContext context)
        {
            return true;
        }

        public async Task RunAsync(TaskContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            FileInfo? coding = context.GetValue(codingOption);
            if (coding != null)
            {
                context.LogInfo("Reading coding file...");

                string? codingComment = null;
                string? codingId = this.ReadCodingFile(coding, ref codingComment);

                if (codingId != null)
                {
                    var model = new BasicCodingRequest
                    {
                        Identifier = codingId
                    };

                    if (codingComment != null)
                    {
                        model.Comment = codingComment;
                    }

                    context.LogInfo("Setting basic coding...");
                    var response = await (await context.GetClientAsync()).SetBasicCoding(model);
                    if (response == null || !response.Success)
                    {
                        context.LogError("Unable to set basic coding.");
                        context.Invocation.ExitCode = 2;
                    }
                }
            }

            FileInfo? codingOem = context.GetValue(codingOemOption);
            if (codingOem != null)
            {
                context.LogInfo("Reading oem coding file...");

                string? codingComment = null;
                string? codingId = this.ReadCodingFile(codingOem, ref codingComment);

                if (codingId != null)
                {
                    var model = new OemCodingRequest
                    {
                        OldIdentifier = string.Empty,
                        NewIdentifier = codingId
                    };

                    if (codingComment != null)
                    {
                        model.Comment = codingComment;
                    }

                    context.LogInfo("Setting oem coding...");
                    var response = await (await context.GetClientAsync()).SetOemCoding(model);
                    if (response == null || !response.Success)
                    {
                        context.LogError("Unable to set oem coding.");
                        context.Invocation.ExitCode = 2;
                    }
                }
            }
        }

        private string? ReadCodingFile(FileInfo file, ref string? comment)
        {
            using (var fs = file.OpenRead())
            {
                using (var reader = new StreamReader(fs))
                {
                    string? id = reader.ReadLine();

                    if (!reader.EndOfStream)
                    {
                        comment = reader.ReadLine();
                    }

                    return id;
                }
            }
        }
    }
}
