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
using Pilz.PITreader.Client.Serialization;

namespace Pilz.PITreader.Client.Model
{
    /// <summary>
    /// Data to be written to a transponder.
    /// </summary>
    public class TransponderRequest
    {
        /// <summary>
        /// Array of permissions (32 values, one for each device group).
        /// </summary>
        [JsonPropertyName("permissions")]
        public Permission[] Permissions { get; set; }

        /// <summary>
        /// Start date for evaluation of start/end dates of the transponder.
        /// </summary>
        [JsonPropertyName("timeLimitationStart"), JsonConverter(typeof(JsonNullableDateTimeConverter))]
        public DateTime? TimeLimitationStart { get; set; }

        /// <summary>
        /// End date for evaluation of start/end dates of the transponder.
        /// </summary>
        [JsonPropertyName("timeLimitationEnd"), JsonConverter(typeof(JsonNullableDateTimeConverter))]
        public DateTime? TimeLimitationEnd { get; set; }

        /// <summary>
        /// If <c>true</c>, data on that transponder is only readable on PITreader devices with a matching coding.
        /// </summary>
        [JsonPropertyName("codingLock")]
        public bool CodingLock { get; set; }
    }
}
