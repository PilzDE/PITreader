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
    /// User data value
    /// </summary>
    public class UserDataValue
    {
        /// <summary>
        /// Perameter-ID
        /// </summary>
        [JsonPropertyName("id")]
        public ushort Id { get; set; }

        /// <summary>
        /// Numeric value of the parameter (optional)
        /// Note: The data field is only available when a numeric value is concerned(type ID 10 … 15 or 30).
        /// </summary>
        [JsonPropertyName("numericValue")]
        public int? NumericValue { get; set; }

        /// <summary>
        /// String values of the parameter (optional)
        /// Note: The data field is only available when a parameter with type-ID 1 (STRING) or type-ID 20 (DATETIME) is concerned.
        /// </summary>
        [JsonPropertyName("stringValue")]
        public string StringValue { get; set; }
    }
}