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

namespace Pilz.PITreader.Client
{
    /// <summary>
    /// Event handler for notification of status changes by monitoring /api/status/monitor
    /// </summary>
    /// <param name="sender">The sender of the event</param>
    /// <param name="e">Event arguments</param>
    public delegate void PITreaderStatusMonitorChange(object sender, PITreaderStatusMonitorChangeEventArgs e);

    /// <summary>
    /// Argument for <see cref="PITreaderStatusMonitorChange"/> events.
    /// </summary>
    public class PITreaderStatusMonitorChangeEventArgs : EventArgs
    {
        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        /// <param name="change">Flags of detected changes.</param>
        /// <param name="data">API response data</param>
        public PITreaderStatusMonitorChangeEventArgs(StatusMonitorFlags change, StatusMonitorResponse data)
        {
            Change = change;
            Data = data;
        }

        /// <summary>
        /// Flags of detected changes.
        /// </summary>
        public StatusMonitorFlags Change { get; }

        /// <summary>
        /// API response data
        /// </summary>
        public StatusMonitorResponse Data { get; }
    }
}
