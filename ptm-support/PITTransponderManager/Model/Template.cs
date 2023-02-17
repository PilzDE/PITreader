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

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Pilz.PITreader.PitTransponderManager.Model
{
    /// <summary>
    /// A reusable transponder configuration template with default permissions, dates, and user data values.
    /// </summary>
    public class Template
    {
        /// <summary>
        /// Template name.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Template description.
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Default per-group permission assignments.
        /// </summary>
        [JsonPropertyName("groupPermissions")]
        public List<GroupPermission> GroupPermissions { get; set; } = new List<GroupPermission>();

        /// <summary>
        /// Default per-group validity dates.
        /// </summary>
        [JsonPropertyName("groupDates")]
        public List<GroupDate> GroupDates { get; set; } = new List<GroupDate>();

        /// <summary>
        /// Default user data parameter values.
        /// </summary>
        [JsonPropertyName("userDefinedDataKeyValues")]
        public List<UserDefinedDataKeyValue> UserDefinedDataKeyValues { get; set; } = new List<UserDefinedDataKeyValue>();
    }
}
