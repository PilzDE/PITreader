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

using System;
using System.Threading.Tasks;
using Pilz.PITreader.Client.Model;

namespace Pilz.PITreader.Client
{
    /// <summary>
    /// Service for externally authenticating transponder keys inserted into a PITreader device.
    /// </summary>
    public class ExternalAuthService : IDisposable
    {
        private readonly PITreaderClient client;

        private readonly ApiEndpointMonitor<AuthenticationStatusResponse> monitor;

        private readonly Func<SecurityId, Task<Permission>> evaluator;

        private bool disposed;

        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        /// <param name="client">Reference to PITreaderClient object.</param>
        /// <param name="evaluator">Function to resolve a security id (input parameter) to a permission.</param>
        /// <param name="queryInterval">Interval in milliseconds to check for transponder waiting for authentication.</param>
        public ExternalAuthService(PITreaderClient client, Func<SecurityId, Permission> evaluator, int queryInterval = 500)
            : this(client, id => Task.FromResult(evaluator(id)), queryInterval)
        {
        }

        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        /// <param name="client">Reference to PITreaderClient object.</param>
        /// <param name="asyncEvaluator">Function to resolve a security id (input parameter) to a permission.</param>
        /// <param name="queryInterval">Interval in milliseconds to check for transponder waiting for authentication.</param>
        public ExternalAuthService(PITreaderClient client, Func<SecurityId, Task<Permission>> asyncEvaluator, int queryInterval = 500)
        {
            this.client = client;
            this.evaluator = asyncEvaluator;
            this.monitor = new ApiEndpointMonitor<AuthenticationStatusResponse>(
                client,
                ApiEndpoints.StatusAuthentication,
                r => r != null && r.AuthenticationStatus == AuthenticationStatus.Waiting,
                this.SetSecurityId,
                queryInterval);
        }

        private Task SetSecurityId(AuthenticationStatusResponse data)
        {
            return this.evaluator(data.SecurityId).ContinueWith(t2 =>
            {
                this.client.SetExternalAuth(data.SecurityId, t2.Result);
            });
        }

        /// <summary>
        /// Implementation of the dispose functionality.
        /// The disposing parameter should be false when called from a finalizer, and true when called from the IDisposable.Dispose method. In other words, it is true when deterministically called and false when non-deterministically called.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.monitor.Dispose();
                }

                this.disposed = true;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
