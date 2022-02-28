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
    /// Request to update the device configuration.
    /// </summary>
    public class ConfigRequest
    {
        /// <summary>
        /// Location information of the device.
        /// </summary>
        [JsonPropertyName("location")]
        public string Location { get; set; }

        /// <summary>
        /// Device group for authentication mode "TransponderData"
        /// </summary>
        [JsonPropertyName("deviceGroup")]
        public ushort? DeviceGroup { get; set; }

        /// <summary>
        /// Setting to enable/disable evaluation of start-/end dates on transponder keys.
        /// </summary>
        [JsonPropertyName("evaluateTimeLimitation")]
        public bool? EvaluateTimeLimitation { get; set; }

        /// <summary>
        /// Timezone for the evaluation of start-/end dates.
        /// </summary>
        [JsonPropertyName("timeZone")]
        public string TimeZone { get; set; }

        /// <summary>
        /// Updates the real time clock on the device.
        /// </summary>
        [JsonPropertyName("realTimeClock"), JsonConverter(typeof(JsonNullableDateTimeConverter))]
        public DateTime? RealTimeClock { get; set; }
    }
}
