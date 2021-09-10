using System.Text.Json.Serialization;

namespace Pilz.PITreader.Client.Model
{
    /// <summary>
    /// User data value
    /// </summary>
    public class UserDataValueRequest : UserDataValue
    {
        /// <summary>
        /// Data type of the parameter.
        /// </summary>
        [JsonPropertyName("type")]
        public UserDataType Type { get; set; }

        /// <summary>
        /// Max. size for the paramater (only for STRING types).
        /// </summary>
        [JsonPropertyName("size")]
        public byte? Size { get; set; }
    }
}