// Copyright (c) 2025 Pilz GmbH & Co. KG
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

using System.Text.Json.Serialization;

namespace Pilz.PITreader.Client.Model
{
    /// <summary>
    /// Device user properties.
    /// </summary>
    public class DeviceUserRequestData
    {
        /// <summary>
        /// State
        /// </summary>
        [JsonPropertyName("state")]
        public DeviceUserState? State { get; set; }

        /// <summary>
        /// Role
        /// </summary>
        [JsonPropertyName("role")]
        public DeviceUserRole? Role { get; set; }

        /// <summary>
        /// Authentication type
        /// </summary>
        [JsonPropertyName("authType")]
        public DeviceUserAuthType? AuthType { get; set; }

        /// <summary>
        /// Restricted to a remote ip
        /// </summary>
        [JsonPropertyName("remoteIp")]
        public string RemoteIp { get; set; }

        /// <summary>
        /// Set password
        /// </summary>
        [JsonPropertyName("password")]
        public string Password { get; set; }
    }
}
