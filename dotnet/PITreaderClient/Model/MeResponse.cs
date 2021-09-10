using System.Text.Json.Serialization;

namespace Pilz.PITreader.Client.Model
{
    /// <summary>
    /// Information about the user who is currently logged in
    /// </summary>
    public class MeResponse
    {
        /// <summary>
        /// ID of the user who is currently logged in
        /// </summary>
        [JsonPropertyName("id")]
        public uint Id { get; set; }

        /// <summary>
        /// Name of the user who is currently logged in
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Access permission (role) of the user who is currently logged in
        /// </summary>
        [JsonPropertyName("level")]
        public uint Level { get; set; }

        /// <summary>
        /// API user type
        /// </summary>
        [JsonPropertyName("type")]
        public UserType Type { get; set; }
    }
}
