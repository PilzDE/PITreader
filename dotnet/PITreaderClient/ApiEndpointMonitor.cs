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
using System.Threading;
using System.Threading.Tasks;

namespace Pilz.PITreader.Client
{
    /// <summary>
    /// Wrapper to monitor an API endpoint periodically for changes.
    /// </summary>
    public class ApiEndpointMonitor<TResponse> : IDisposable where TResponse : class
    {
        /// <summary>
        /// Default value for tolerated, consecutive failed requests.
        /// </summary>
        public const int DEFAULT_REQUEST_RETRY_COUNT = 3;

        private readonly PITreaderClient client;
        private readonly string endpoint;
        private readonly Func<TResponse, bool> changeTrigger;
        private readonly Func<TResponse, Task> changeHandler;
        private readonly Func<TResponse, Task> errorHandler;
        private readonly int failedRequestLimit;
        private readonly Timer timer;
        private bool disposed;
        private int activeJobCount = 0;
        private int failedRequestCount = 0;        

        /// <summary>
        /// Create a new instance of the class.
        /// </summary>
        /// <param name="client">API client</param>
        /// <param name="endpoint">Endpoint to be monitored.</param>
        /// <param name="changeTrigger">Callback for change detection (returns true if change occured).</param>
        /// <param name="changeHandler">Callback executed when a change was detected.</param>
        /// <param name="errorHandler">Callback executed when an error was detected.</param>
        /// <param name="queryInterval">Query interval.</param>
        /// <param name="failedRequestLimit">Count of tolerated, consecutive failed reuests.</param>
        public ApiEndpointMonitor(
            PITreaderClient client, 
            string endpoint, 
            Func<TResponse, bool> changeTrigger, 
            Func<TResponse, Task> changeHandler, 
            Func<TResponse, Task> errorHandler, 
            int queryInterval = 500,
            int failedRequestLimit = DEFAULT_REQUEST_RETRY_COUNT)
        {
            if (string.IsNullOrEmpty(endpoint))
            {
                throw new ArgumentException("message", nameof(endpoint));
            }

            this.client = client ?? throw new ArgumentNullException(nameof(client));
            this.endpoint = endpoint;
            this.changeTrigger = changeTrigger ?? throw new ArgumentNullException(nameof(changeTrigger));
            this.changeHandler = changeHandler ?? throw new ArgumentNullException(nameof(changeHandler));
            this.errorHandler = errorHandler ?? throw new ArgumentNullException(nameof(errorHandler));
            this.failedRequestLimit = failedRequestLimit;
            this.timer = new Timer(this.TimerCallback, null, queryInterval, queryInterval);
        }

        private void TimerCallback(object state)
        {
            if (Interlocked.CompareExchange(ref this.activeJobCount, 1, 0) == 1)
            {
                return;
            }
            try
            {
                this.client.GetAsync<TResponse>(endpoint)
                    .ContinueWith(t =>
                    {
                        var result = t.Result;
                        if (result.Success)
                        {
                            // after successful endpoint call "failed request" counter is reset
                            this.failedRequestCount = 0;

                            if (this.changeTrigger(result.Data))
                            {
                                this.changeHandler(result.Data).Wait();
                            }
                        }
                        else
                        {
                            // call error handler only after a certain number of consecutive failed endpoint calls
                            if (this.failedRequestCount == this.failedRequestLimit)
                            {
                                this.errorHandler(result.Data).Wait();
                            }

                            // after failed endpoint call "failed request" counter is incremented
                            this.failedRequestCount += 1;
                        }
                    }).Wait();
            }
            finally
            {
                Interlocked.Decrement(ref this.activeJobCount);
            }
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
                    this.timer.Dispose();
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
