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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Pilz.PITreader.Network.Configuration
{
    /// <summary>
    /// Service for sending Multicast Configuration Protocol requests.
    /// </summary>
    public class ConfigurationService
    {
        private readonly TimeSpan timeout;

        private MulticastClient client;

        private List<RequestContext> pending = new List<RequestContext>();

        /// <summary>
        /// Creates a new instance of the service.
        /// </summary>
        /// <param name="nics">List of network interfaces.</param>
        /// <param name="timeout">Timeout for requests (default: 2 seconds)</param>
        public ConfigurationService(IEnumerable<NetworkInterface> nics, TimeSpan? timeout = null)
        {
            this.client = new MulticastClient(nics);
            this.client.MessageReceived += this.OnUdpMessageReceived;
            this.timeout = timeout ?? TimeSpan.FromMilliseconds(2000);
        }

        /// <summary>
        /// Sends a request and returns the response.
        /// </summary>
        /// <param name="request">Configuration request.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the request before the configured timeout is reached.</param>
        /// <returns>The response to the request or a timeout.</returns>
        public async Task<Response> SendAsnyc(RequestPacket request, CancellationToken? cancellationToken = null)
        {
            byte[] data;
            using (var ms = new MemoryStream())
            {
                request.Write(ms);
                data = ms.ToArray();
            }

            Response response = null;
            using (var context = new RequestContext(request))
            {
                lock (this.pending)
                {
                    this.pending.Add(context);
                }

                await this.client.SendAsync(data)
                    .ConfigureAwait(false);

                bool completted = await context.WaitAsync(this.timeout, cancellationToken ?? CancellationToken.None)
                    .ConfigureAwait(false);

                response = new Response
                {
                    Timeout = !completted,
                    Status = context.Response?.Status ?? StatusCode.None,
                    MessageCode = context.Response?.MessageCode ?? 0,
                };
            }

            return response;
        }

        private void OnUdpMessageReceived(object sender, UdpReceiveResult e)
        {
            if (e == null || e.Buffer == null) return;

            ResponsePacket response = null;
            using (var ms = new MemoryStream(e.Buffer))
            {
                response = ResponsePacket.Read(ms);
            }

            if (response != null)
            {
                lock (this.pending)
                {
                    var context = this.pending.FirstOrDefault(x => x.IsMatch(response));
                    if (context != null)
                    {
                        this.pending.Remove(context);

                        context.Response = response;
                    }
                }
            }
        }
    }
}
