using System.Text.Json.Serialization;

namespace Pilz.PITreader.Client.Model
{
    /// <summary>
    /// Request to set or update the basic coding of a device.
    /// </summary>
    public class BasicCodingRequest
    {
        /// <summary>
        /// Coding indentifier.
        /// </summary>
        [JsonPropertyName("identifier")]
        public string Identifier { get; set; }

        /// <summary>
        /// Comment
        /// </summary>
        [JsonPropertyName("comment")]
        public string Comment { get; set; }
    }
}
