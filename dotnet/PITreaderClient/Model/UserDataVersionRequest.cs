using System.Text.Json.Serialization;

namespace Pilz.PITreader.Client.Model
{
    /// <summary>
    /// Request to update version and comment of user data stored on a device.
    /// </summary>
    public class UserDataVersionRequest
    {
        /// <summary>
        /// Version number.
        /// </summary>
        [JsonPropertyName("version")]
        public ushort Version { get; set; }

        /// <summary>
        /// Comment.
        /// </summary>
        [JsonPropertyName("comment")]
        public string Comment { get; set; }
    }
}
