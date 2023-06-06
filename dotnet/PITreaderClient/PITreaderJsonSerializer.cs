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

using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Pilz.PITreader.Client.Serialization;

namespace Pilz.PITreader.Client
{
    /// <summary>
    /// JSON serializer for PITreaderClient model entities.
    /// </summary>
    public static class PITreaderJsonSerializer
    {
        private static readonly JsonSerializerOptions options;

        static PITreaderJsonSerializer()
        {
            options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            options.Converters.Add(new JsonVersionConverter());
            options.Converters.Add(new JsonCrudActionConverter());
            options.Converters.Add(new JsonFirmwareUpdateActionConverter());
            options.Converters.Add(new JsonSecurityIdConverter());
            options.Converters.Add(new JsonNullableDateTimeConverter());
        }

        /// <summary>
        /// Converts the value of a type specified by a generic type parameter into a JSON string.
        /// </summary>
        /// <typeparam name="TValue">The type of the value to serialize.</typeparam>
        /// <param name="value">The value to convert.</param>
        /// <param name="pretty">Pretty print output.</param>
        /// <returns> A JSON string representation of the value.</returns>
        /// <exception cref="System.NotSupportedException">There is no compatible System.Text.Json.Serialization.JsonConverter for TValue or its serializable members.</exception>
        public static string Serialize<TValue>(TValue value, bool pretty = false)
        {
            var localOptions = new JsonSerializerOptions(options)
            {
                WriteIndented = pretty
            };

            return JsonSerializer.Serialize(value, localOptions);
        }

        /// <summary>
        /// Parses the text representing a single JSON value into an instance of the type specified by a generic type parameter.
        /// </summary>
        /// <typeparam name="TValue">The target type of the JSON value.</typeparam>
        /// <param name="json">The JSON text to parse.</param>
        /// <returns>A TValue representation of the JSON value.</returns>
        /// <exception cref="System.ArgumentNullException">json is null.</exception>
        /// <exception cref="System.Text.Json.JsonException">The JSON is invalid. -or- TValue is not compatible with the JSON. -or- There is remaining data in the string beyond a single JSON value.</exception>
        /// <exception cref="System.NotSupportedException">There is no compatible System.Text.Json.Serialization.JsonConverter for TValue or its serializable members.</exception>
        public static TValue Deserialize<TValue>(string json)
        {
            return JsonSerializer.Deserialize<TValue>(json, options);
        }

        /// <summary>
        /// Read the HTTP content and return the value resulting from deserialize the content as JSON in an asynchronous operation.
        /// </summary>
        /// <typeparam name="T">The target type to deserialize to.</typeparam>
        /// <param name="content">The content to read from.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public static Task<T> ReadFromJsonAsync<T>(this HttpContent content, CancellationToken cancellationToken = default)
        {
            return HttpContentJsonExtensions.ReadFromJsonAsync<T>(content, options, cancellationToken);
        }
    }
}
