using System.Text.Json.Serialization;

namespace Pilz.PITreader.Client.Model
{
    /// <summary>
    /// Data stored on a transponder key.
    /// </summary>
    public class TransponderResponse : TransponderRequest
    {
        /// <summary>
        /// UID for teach-in of the transponder.
        /// </summary>
        [JsonPropertyName("teachInId")]
        public string TeachInId { get; set; }

        /// <summary>
        /// Security ID of the transponder.
        /// </summary>
        [JsonPropertyName("securityId")]
        public SecurityId SecurityId { get; set; }

        /// <summary>
        /// <c>true</c>, if permissions are locked and can not be edited, anymore.
        /// </summary>
        [JsonPropertyName("lockedPermissions")]
        public bool LockedPermissions { get; set; }
    }
}
