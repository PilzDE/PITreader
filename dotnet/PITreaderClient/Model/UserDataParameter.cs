using System.Text.Json.Serialization;

namespace Pilz.PITreader.Client.Model
{
    /// <summary>
    /// User data parameter configuration.
    /// </summary>
    public class UserDataParameter
    {
        /// <summary>
        /// Paramater-ID
        /// </summary>
        [JsonPropertyName("id")]
        public ushort Id { get; set; } 

        /// <summary>
        /// Name of the paramater (only for configuration stored on a device)
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

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
