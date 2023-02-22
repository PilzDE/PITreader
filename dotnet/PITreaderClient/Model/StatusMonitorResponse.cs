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
using System.Text.Json.Serialization;

namespace Pilz.PITreader.Client.Model
{
    /// <summary>
    /// Summarized status information of device
    /// </summary>
    public class StatusMonitorResponse : IEquatable<StatusMonitorResponse>
    {
        /// <summary>
        /// General device status information
        /// </summary>
        [JsonPropertyName("status")]
        public StatusMonitorStatus Status { get; set; }

        /// <summary>
        /// LED status information
        /// </summary>
        [JsonPropertyName("led")]
        public StatusMonitorLed Led { get; set; }

        /// <summary>
        /// Configuration settings
        /// </summary>
        [JsonPropertyName("config")]
        public StatusMonitorConfig Config { get; set; }

        /// <summary>
        /// Transponder authentication status information
        /// </summary>
        [JsonPropertyName("authentication")]
        public StatusMonitorAuthentication Authentication { get; set; }

        /// <summary>
        /// Diagnostic log status
        /// </summary>
        [JsonPropertyName("log")]
        public StatusMonitorLog Log { get; set; }

        /// <summary>
        /// Change notification tags for settings and list data
        /// </summary>
        [JsonPropertyName("tags")]
        public StatusMonitorTags Tags { get; set; }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the other parameter; otherwise, false.</returns>
        public bool Equals(StatusMonitorResponse other)
        {
            if (other == null)
                return false;

            return this.Status.SafeEquals(other.Status)
                && this.Led.SafeEquals(other.Led)
                && this.Config.SafeEquals(other.Config)
                && this.Authentication.SafeEquals(other.Authentication)
                && this.Log.SafeEquals(other.Log)
                && this.Tags.SafeEquals(other.Tags);
        }
    }
}