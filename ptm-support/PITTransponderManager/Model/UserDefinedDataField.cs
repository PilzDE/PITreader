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
    /// A user data parameter definition. Defines a custom parameter that can be stored on transponders.
    /// Called "user data configuration" in the PITreader REST API.
    /// </summary>
    public class UserDefinedDataField
    {
        /// <summary>
        /// Parameter name (1–63 UTF-8 characters). Called "parameter name" in the PITreader REST API.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Data type ID:
        /// 1 = STRING (2–255 bytes), 10 = INT8U, 11 = INT8S, 12 = INT16U, 13 = INT16S,
        /// 14 = INT32U, 15 = INT32S, 20 = DATETIME, 30 = PERMISSION, 31 = BIT.
        /// </summary>
        [JsonPropertyName("type")]
        public int Type { get; set; }

        /// <summary>
        /// Field size in bytes. For STRING type: 2–255. For fixed-size types: 0.
        /// </summary>
        [JsonPropertyName("size")]
        public int Size { get; set; }

        /// <summary>
        /// Parameter ID (1–65535). Called <c>parameterId</c> in the PITreader REST API.
        /// Referenced by <see cref="UserDefinedDataKeyValue.DeviceId"/>.
        /// </summary>
        [JsonPropertyName("deviceId")]
        public int DeviceId { get; set; }
    }
}
