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
