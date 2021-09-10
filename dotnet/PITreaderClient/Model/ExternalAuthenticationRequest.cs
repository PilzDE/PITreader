using System.Text.Json.Serialization;

namespace Pilz.PITreader.Client.Model
{
    /// <summary>
    /// Sets the permission for a transponder key in authentication mode "External"
    /// </summary>
    public class ExternalAuthenticationRequest
    {
        /// <summary>
        /// Security ID, for which external authentication is to be defined
        /// </summary>
        [JsonPropertyName("securityId")]
        public SecurityId SecurityId { get; set; }

        /// <summary>
        /// Permission that is to be set for the stated transponder key
        /// </summary>
        [JsonPropertyName("permission")]
        public Permission Permission { get; set; }
    }
}
