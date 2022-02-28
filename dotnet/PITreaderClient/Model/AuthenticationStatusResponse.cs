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

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Pilz.PITreader.Client.Model
{
    /// <summary>
    /// Current information on a transponder key’s authentication status
    /// </summary>
    public class AuthenticationStatusResponse
    {
        /// <summary>
        /// Authentication status of the transponder key
        /// </summary>
        [JsonPropertyName("authenticated")]
        public bool Authenticated { get; set; }

        /// <summary>
        /// Authenticated permission
        /// </summary>
        [JsonPropertyName("permission")]
        public Permission Permission { get; set; }

        /// <summary>
        /// Status of authentication process
        /// </summary>
        [JsonPropertyName("authenticationStatus")]
        public AuthenticationStatus AuthenticationStatus { get; set; }

        /// <summary>
        /// Reason for failed authentication
        /// </summary>
        [JsonPropertyName("failureReason")]
        public AuthenticationFailureReason FailureReason { get; set; }

        /// <summary>
        /// Security ID
        /// </summary>
        [JsonPropertyName("securityId")]
        public SecurityId SecurityId { get; set; }

        /// <summary>
        /// Order number of the transponder key
        /// </summary>
        [JsonPropertyName("orderNo")]
        public uint OrderNumber { get; set; }

        /// <summary>
        /// Serial number of the transponder key
        /// </summary>
        [JsonPropertyName("serialNo")]
        public string SerialNumber { get; set; }

        /// <summary>
        /// User data
        /// </summary>
        [JsonPropertyName("userData")]
        public List<UserDataValue> UserData { get; set; }
    }
}
