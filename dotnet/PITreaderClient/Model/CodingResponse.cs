using System.Text.Json.Serialization;

namespace Pilz.PITreader.Client.Model
{
    /// <summary>
    /// Status of a coding on a device.
    /// </summary>
    public class CodingResponse
    {
        /// <summary>
        /// Device is coded.
        /// </summary>
        [JsonPropertyName("activated")]
        public bool Activated { get; set; }

        /// <summary>
        /// Checksum of the coding.
        /// </summary>
        [JsonPropertyName("checksum")]
        public string Checksum { get; set; }

        /// <summary>
        /// Comment stored with the coding.
        /// </summary>
        [JsonPropertyName("comment")]
        public string Comment { get; set; }
    }
}
