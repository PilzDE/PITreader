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
using System.Threading;
using System.Threading.Tasks;

namespace Pilz.PITreader.Network.Configuration
{
    internal class RequestContext : IDisposable
    {
        private readonly ManualResetEvent locking = new ManualResetEvent(false);

        private ResponsePacket response;
        private bool disposed;

        public RequestContext(RequestPacket request)
        {
            this.Request = request ?? throw new ArgumentNullException(nameof(request));
        }

        public RequestPacket Request { get; }

        public ResponsePacket Response 
        { 
            get => this.response;
            set
            {
                if (value != null)
                {
                    this.response = value;
                    if (!this.disposed) this.locking.Set();
                }
            }
        }

        public Task<bool> WaitAsync(TimeSpan timeout, CancellationToken cancellationToken)
        {
            return this.locking.WaitOneAsync(timeout, cancellationToken);
        }

        public bool IsMatch(ResponsePacket response)
        {
            if (response == null || this.Request == null)
                return false;

            return this.Request.OrderNumber == response.OrderNumber
                && this.Request.SerialNumber == response.SerialNumber
                && (this.Request.RequestId == response.RequestId || response.RequestId == 0)
                && (this.Request.Command == response.Command || response.Command == CommandType.None);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.locking.Dispose();
                }

                this.disposed = true;
            }
        }
        
        public void Dispose()
        {
            this.Dispose(true);

            GC.SuppressFinalize(this);
        }
    }
}
