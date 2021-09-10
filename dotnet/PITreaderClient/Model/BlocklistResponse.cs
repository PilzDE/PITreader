using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Pilz.PITreader.Client.Model
{
    /// <summary>
    /// Response with the list of blocklist entries stored on the device.
    /// </summary>
    public class BlocklistResponse
    {
        /// <summary>
        /// List of blocklist entries stored on the device.
        /// </summary>
        [JsonPropertyName("items")]
        public List<BlocklistEntry> Items { get; set; }
    }
}
