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
        private readonly PITreaderClient client;
        private readonly string endpoint;
        private readonly Func<TResponse, bool> changeTrigger;
        private readonly Func<TResponse, Task> changeHandler;
        private readonly Timer timer;
        private bool disposed;
        private int activeJobCount = 0;

        /// <summary>
        /// Create a new instance of the class.
        /// </summary>
        /// <param name="client">API client</param>
        /// <param name="endpoint">Endpoint to be monitored.</param>
        /// <param name="changeTrigger">Callback for change detection (returns true if change occured).</param>
        /// <param name="changeHandler">Callback executed when a change was detected.</param>
        /// <param name="queryInterval">Query interval.</param>
        public ApiEndpointMonitor(PITreaderClient client, string endpoint, Func<TResponse, bool> changeTrigger, Func<TResponse, Task> changeHandler, int queryInterval = 500)
        {
            if (string.IsNullOrEmpty(endpoint))
            {
                throw new ArgumentException("message", nameof(endpoint));
            }

            this.client = client ?? throw new ArgumentNullException(nameof(client));
            this.endpoint = endpoint;
            this.changeTrigger = changeTrigger ?? throw new ArgumentNullException(nameof(changeTrigger));
            this.changeHandler = changeHandler ?? throw new ArgumentNullException(nameof(changeHandler));

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
                        if (result.Success && this.changeTrigger(result.Data))
                        {
                            this.changeHandler(result.Data).Wait();
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
