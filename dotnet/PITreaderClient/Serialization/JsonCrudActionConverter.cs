using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Pilz.PITreader.Client.Model;

namespace Pilz.PITreader.Client.Serialization
{
    /// <summary>
    /// Converts between <see cref="CrudAction"/> values and their JSON representation.
    /// </summary>
    public class JsonCrudActionConverter : JsonConverter<CrudAction>
    {
        /// <inheritdoc cref="JsonConverter{T}.Read(ref Utf8JsonReader, Type, JsonSerializerOptions)"/>
        public override CrudAction Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var names = Enum.GetNames(typeof(CrudAction)).Select(x => x.ToLowerInvariant()).ToList();
            var value = reader.GetString().ToLowerInvariant();
            var index = names.IndexOf(value);
            if (index >= 0) return (CrudAction)(Enum.GetValues(typeof(CrudAction)).GetValue(index));
            return default;
        }

        /// <summary>
        /// Writes a specified value as JSON.
        /// </summary>
        /// <param name="writer">The writer to write to.</param>
        /// <param name="value">The value to convert to JSON.</param>
        /// <param name="options">An object that specifies serialization options to use.</param>
        public override void Write(Utf8JsonWriter writer, CrudAction value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString().ToLowerInvariant());
        }
    }
}
