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
    /// Additional key object in response of API /config/customKeys
    /// </summary>
    public class CustomKeysAdditionalKey
    {
        /// <summary>
        /// The key type (e.g., 16 for logon key)
        /// </summary>
        [JsonPropertyName("type")]
        public CustomKeyType Type { get; set; }

        /// <summary>
        /// The data type (1 = AES128, 2 = AES192, 3 = AES256)
        /// </summary>
        [JsonPropertyName("dataType")]
        public CustomKeyDataType? DataType { get; set; }

        /// <summary>
        /// The checksum
        /// </summary>
        [JsonPropertyName("checksum")]
        public string Checksum { get; set; }

        /// <summary>
        /// The cryptographic key data
        /// </summary>
        [JsonPropertyName("data")]
        public byte[] Data { get; set; }

        /// <summary>
        /// The passphrase for key type <see cref="CustomKeyType.TransponderKey"/>.
        /// </summary>
        [JsonPropertyName("passphrase")]
        public string Passphrase { get; set; }
    }
}
