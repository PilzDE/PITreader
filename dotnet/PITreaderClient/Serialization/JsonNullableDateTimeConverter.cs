using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Pilz.PITreader.Client.Serialization
{
    /// <summary>
    /// Converts between <see cref="Nullable{DateTime}"/> values and their JSON representation.
    /// </summary>
    public class JsonNullableDateTimeConverter : JsonConverter<DateTime?>
    {
        /// <summary>
        /// Date-/time-format defined in RFC 3339 as used by the PITreader API.
        /// </summary>
        public const string Rfc3339Format = "yyyy'-'MM'-'dd'T'HH':'mm':'ssK";

        /// <summary>
        /// Reads and converts the JSON to type CrudAction.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="typeToConvert">The type to convert.</param>
        /// <param name="options">An object that specifies serialization options to use.</param>
        /// <returns>The converted value.</returns>
        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string value = reader.GetString();
            if (string.IsNullOrWhiteSpace(value)) return null;
            return DateTime.Parse(value);
        }

        /// <summary>
        /// Writes a specified value as JSON.
        /// </summary>
        /// <param name="writer">The writer to write to.</param>
        /// <param name="value">The value to convert to JSON.</param>
        /// <param name="options">An object that specifies serialization options to use.</param>
        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                string json = string.Empty;
                if (value.Value != DateTime.MinValue && value.Value != DateTime.MaxValue)
                {
                    json = value.Value.ToUniversalTime().ToString(Rfc3339Format);
                }

                writer.WriteStringValue(json);
            }
        }
    }
}
