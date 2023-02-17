// Copyright (c) 2026 Pilz GmbH & Co. KG
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

namespace Pilz.PITreader.PitTransponderManager.Model
{
    /// <summary>
    /// A user data parameter value. Associates a parameter (by <see cref="DeviceId"/>)
    /// with a value, optionally scoped to a specific device group.
    /// </summary>
    public class UserDefinedDataKeyValue
    {
        /// <summary>
        /// Device group number (0–9999) this value applies to,
        /// or <c>null</c> for the group-independent default value.
        /// </summary>
        [JsonPropertyName("groupNo")]
        public int? GroupNo { get; set; }

        /// <summary>
        /// Parameter ID (1–65535) referencing a <see cref="UserDefinedDataField"/>
        /// by its <see cref="UserDefinedDataField.DeviceId"/>. Called <c>parameterId</c> in the PITreader REST API.
        /// </summary>
        [JsonPropertyName("deviceId")]
        public int DeviceId { get; set; }

        /// <summary>
        /// The parameter value, encoded as a string.
        /// </summary>
        [JsonPropertyName("defaultValue")]
        public string DefaultValue { get; set; } = string.Empty;
    }
}
