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
    /// A per-device-group validity date range override.
    /// Groups 0–31 store dates directly on the transponder;
    /// groups above 31 use user data parameters 2 and 3.
    /// </summary>
    public class GroupDate
    {
        /// <summary>
        /// Device group number (0–9999).
        /// </summary>
        [JsonPropertyName("groupNumber")]
        public int GroupNumber { get; set; }

        /// <summary>
        /// Group-specific validity start (format: YYYY-M-D, non-zero-padded), or empty string.
        /// </summary>
        [JsonPropertyName("startDate")]
        public string StartDate { get; set; } = string.Empty;

        /// <summary>
        /// Group-specific validity end (format: YYYY-M-D, non-zero-padded), or empty string.
        /// </summary>
        [JsonPropertyName("endDate")]
        public string EndDate { get; set; } = string.Empty;
    }
}
