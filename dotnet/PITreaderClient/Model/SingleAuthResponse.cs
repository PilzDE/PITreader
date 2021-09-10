using System.Text.Json.Serialization;

namespace Pilz.PITreader.Client.Model
{
    /// <summary>
    /// Response showing whether an authentication lock is set via the "Single authentication" authentication type and what the security ID of the locking transponder key is.
    /// </summary>
    public class SingleAuthResponse
    {
        /// <summary>
        /// Security ID of the locking transponder key
        /// </summary>
        [JsonPropertyName("securityId")]
        public string SeurityId { get; set; }

        /// <summary>
        /// <c>true</c>, if the authentication locked by a transponder key
        /// </summary>
        [JsonIgnore]
        public bool SingleAuthLocked => !string.IsNullOrEmpty(this.SeurityId);
    }
}
