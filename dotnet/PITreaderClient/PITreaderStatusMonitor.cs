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

using Pilz.PITreader.Client.Model;
using System;
using System.Threading.Tasks;

namespace Pilz.PITreader.Client
{
    /// <summary>
    /// Monitors PITreader status monitoring endpoint
    /// </summary>
    public class PITreaderStatusMonitor
    {
        private readonly ApiEndpointMonitor<StatusMonitorResponse> monitor;

        private bool disposed;

        private StatusMonitorFlags changeNotificationFlags;

        private StatusMonitorResponse previousStatus;

        /// <summary>
        /// Event triggered when a change in the selected area was detected
        /// </summary>
        public event PITreaderStatusMonitorChange OnChange;

        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        /// <param name="client">Reference to PITreaderClient object.</param>
        /// <param name="changeNotificationFlags">Flags of changes for notifications see event <see cref="OnChange"/></param>
        /// <param name="queryInterval">Interval in milliseconds to check for transponder waiting for authentication.</param>
        public PITreaderStatusMonitor(PITreaderClient client, StatusMonitorFlags changeNotificationFlags = StatusMonitorFlags.All, int queryInterval = 500)
        {
            this.changeNotificationFlags = changeNotificationFlags;

            this.monitor = new ApiEndpointMonitor<StatusMonitorResponse>(
                client,
                ApiEndpoints.StatusMonitor,
                r => r != null && !r.Equals(this.previousStatus),
                this.CheckChanges,
                queryInterval);
        }

        private Task CheckChanges(StatusMonitorResponse data)
        {
            if (data == null)
                return Task.CompletedTask;

            StatusMonitorFlags changes = StatusMonitorFlags.None;

            if (previousStatus == null || !data.Status.Equals(previousStatus.Status))
            {
                changes |= StatusMonitorFlags.StatusChange;
            }

            if (previousStatus == null || !data.Led.Equals(previousStatus.Led))
            {
                changes |= StatusMonitorFlags.LedChange;
            }

            if (previousStatus == null
                || !data.Config.Equals(previousStatus.Config)
                 || data.Tags?.Settings != previousStatus?.Tags?.Settings)
            {
                changes |= StatusMonitorFlags.ConfigurationChange;
            }

            if (previousStatus == null || !data.Authentication.Equals(previousStatus.Authentication))
            {
                changes |= StatusMonitorFlags.TransponderChange;
            }

            if (previousStatus == null || !data.Log.Equals(previousStatus.Log))
            {
                changes |= StatusMonitorFlags.DiagnosticChange;
            }

            if (previousStatus?.Tags == null || data.Tags.BlockList != previousStatus.Tags.BlockList)
            {
                changes |= StatusMonitorFlags.BlocklistChange;
            }

            if (previousStatus?.Tags == null || data.Tags.PermissionList != previousStatus.Tags.PermissionList)
            {
                changes |= StatusMonitorFlags.PermissionListChange;
            }

            if (previousStatus?.Tags == null || data.Tags.UserDataConfig != previousStatus.Tags.UserDataConfig)
            {
                changes |= StatusMonitorFlags.UserDataConfigChange;
            }

            this.previousStatus = data;

            if ((changes & this.changeNotificationFlags) != StatusMonitorFlags.None)
            {
                this.OnChange?.Invoke(this, new PITreaderStatusMonitorChangeEventArgs(changes, data));
            }

            return Task.CompletedTask;
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
