// Copyright (c) 2022 Pilz GmbH & Co. KG
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

namespace Pilz.PITreader.Tool
{
    internal class ConnectionPropertiesBinder
    {
        private readonly Option<string> host;
        private readonly Option<ushort> port;
        private readonly Option<bool> acceptAll;
        private readonly Option<string> thumbprint;
        private readonly Argument<string> apiToken;

        public ConnectionPropertiesBinder(Option<string> host, Option<ushort> port, Option<bool> acceptAll, Option<string> thumbprint, Argument<string> apiToken)
        {
            this.host = host;
            this.port = port;
            this.acceptAll = acceptAll;
            this.thumbprint = thumbprint;
            this.apiToken = apiToken;
        }

        public ConnectionProperties GetValue(InvocationContext context)
        {
            var properties = new ConnectionProperties
            {
                Host = context.ParseResult.GetValueForOption(this.host),
                Port = context.ParseResult.GetValueForOption(this.port),
                AcceptAll = context.ParseResult.GetValueForOption(this.acceptAll),
                Thumbprint = context.ParseResult.GetValueForOption(this.thumbprint),
                ApiToken = context.ParseResult.GetValueForArgument(this.apiToken)
            };

            return properties;
        }
    }
}