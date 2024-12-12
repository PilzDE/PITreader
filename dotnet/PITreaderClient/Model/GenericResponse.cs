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

using System.Diagnostics;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Pilz.PITreader.Client.Model
{
    /// <summary>
    /// Response to POST requests or on entry.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class GenericResponse : AbstractGenericResponse
    {
        /// <summary>
        /// Data returned by the device.
        /// </summary>
        [JsonPropertyName("data")]
        public JsonNode Data { get; set; }

        private string DebuggerDisplay { get => string.Format("Succes: {0}, Message: {1}, Data: {{ {2} }}", this.Success, this.Message, (object)this.Data ?? "null"); }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() => this.DebuggerDisplay;
    }

    /// <summary>
    /// Response to POST requests or on entry.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class GenericResponse<T> : AbstractGenericResponse where T : class
    {
        /// <summary>
        /// Data returned by the device.
        /// </summary>
        [JsonPropertyName("data")]
        public T Data { get; set; }

        private string DebuggerDisplay { get => string.Format("Succes: {0}, Message: {1}, Data: {{ {2} }}", this.Success, this.Message, (object)this.Data ?? "null"); }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() => this.DebuggerDisplay;
    }

    /// <summary>
    /// Abstract base class for generic responses.
    /// </summary>
    public abstract class AbstractGenericResponse
    {
        /// <summary>
        /// Status of request execution.
        /// </summary>
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        /// <summary>
        /// Message key returned by the device.
        /// </summary>
        [JsonPropertyName("msg")]
        public string Message { get; set; }
    }
}
