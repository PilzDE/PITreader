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