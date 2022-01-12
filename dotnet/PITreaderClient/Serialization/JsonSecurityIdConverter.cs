﻿using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Pilz.PITreader.Client.Model;

namespace Pilz.PITreader.Client.Serialization
{
    /// <summary>
    /// Converts between <see cref="SecurityId"/> values and their JSON representation.
    /// </summary>
    public class JsonSecurityIdConverter : JsonConverter<SecurityId>
    {
        /// <summary>
        /// Reads and converts the JSON to type CrudAction.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="typeToConvert">The type to convert.</param>
        /// <param name="options">An object that specifies serialization options to use.</param>
        /// <returns>The converted value.</returns>
        public override SecurityId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return SecurityId.Parse(reader.GetString());
        }

        /// <summary>
        /// Writes a specified value as JSON.
        /// </summary>
        /// <param name="writer">The writer to write to.</param>
        /// <param name="value">The value to convert to JSON.</param>
        /// <param name="options">An object that specifies serialization options to use.</param>
        public override void Write(Utf8JsonWriter writer, SecurityId value, JsonSerializerOptions options)
        {
            if (value == null) writer.WriteStringValue(string.Empty);
            writer.WriteStringValue(value.HexString);
        }
    }
}
