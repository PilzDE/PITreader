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

using System.Text.Json.Serialization;

namespace Pilz.PITreader.Client.Model
{
    /// <summary>
    /// Tags indicating changes in settings and list data of device
    /// </summary>
    public class StatusMonitorTags
    {
        /// <summary>
        /// Indicates change in device settings
        /// </summary>
        [JsonPropertyName("settings")]
        public uint Settings { get; set; }

        /// <summary>
        /// Indicates change in blocklist
        /// </summary>
        [JsonPropertyName("blockList")]
        public uint BlockList { get; set; }

        /// <summary>
        /// Indicates change in permission list
        /// </summary>
        [JsonPropertyName("permissionList")]
        public uint PermissionList { get; set; }

        /// <summary>
        /// Indicates change in user data configuration
        /// </summary>
        [JsonPropertyName("userDataConfig")]
        public uint UserDataConfig { get; set; }
    }
}