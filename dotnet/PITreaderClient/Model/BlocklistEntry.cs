using System.Text.Json.Serialization;
using Pilz.PITreader.Client.Serialization;

namespace Pilz.PITreader.Client.Model
{
    /// <summary>
    /// An entry in the blocklist.
    /// </summary>
    public class BlocklistEntry
    {
        /// <summary>
        /// Security ID
        /// </summary>
        [JsonPropertyName("id"), JsonConverter(typeof(JsonSecurityIdConverter))]
        public SecurityId Id { get; set; }

        /// <summary>
        /// Comment stored with the security id.
        /// </summary>
        [JsonPropertyName("comment")]
        public string Comment { get; set; }
    }
}