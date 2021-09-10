using System.Text.Json.Serialization;

namespace Pilz.PITreader.Client.Model
{
    /// <summary>
    /// Container for all data stored on a transponder.
    /// </summary>
    public class TransponderDataContainer
    {
        /// <summary>
        /// Data in standard layout stored on the transponder.
        /// </summary>
        [JsonPropertyName("staticData")]
        public TransponderResponse StaticData { get; set; }

        /// <summary>
        /// Data in user data area stored on the transponder.
        /// </summary>
        [JsonPropertyName("userData")]
        public UserDataResponse UserData { get; set; }
    }
}
