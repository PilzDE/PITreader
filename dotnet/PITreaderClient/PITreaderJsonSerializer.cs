using Pilz.PITreader.Client.Serialization;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

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
            options.Converters.Add(new JsonSecurityIdConverter());
            options.Converters.Add(new JsonNullableDateTimeConverter());
        }

        /// <summary>
        /// Converts the value of a type specified by a generic type parameter into a JSON string.
        /// </summary>
        /// <typeparam name="TValue">The type of the value to serialize.</typeparam>
        /// <param name="value">The value to convert.</param>
        /// <returns> A JSON string representation of the value.</returns>
        /// <exception cref="System.NotSupportedException">There is no compatible System.Text.Json.Serialization.JsonConverter for TValue or its serializable members.</exception>
        public static string Serialize<TValue>(TValue value)
        {
            return JsonSerializer.Serialize(value, options);
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
