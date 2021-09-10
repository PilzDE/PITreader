using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Pilz.PITreader.Client.Model
{
    /// <summary>
    /// Current information on a transponder key’s authentication status
    /// </summary>
    public class AuthenticationStatusResponse
    {
        /// <summary>
        /// Authentication status of the transponder key
        /// </summary>
        [JsonPropertyName("authenticated")]
        public bool Authenticated { get; set; }

        /// <summary>
        /// Authenticated permission
        /// </summary>
        [JsonPropertyName("permission")]
        public Permission Permission { get; set; }

        /// <summary>
        /// Status of authentication process
        /// </summary>
        [JsonPropertyName("authenticationStatus")]
        public AuthenticationStatus AuthenticationStatus { get; set; }

        /// <summary>
        /// Reason for failed authentication
        /// </summary>
        [JsonPropertyName("failureReason")]
        public AuthenticationFailureReason FailureReason { get; set; }

        /// <summary>
        /// Security ID
        /// </summary>
        [JsonPropertyName("securityId")]
        public SecurityId SecurityId { get; set; }

        /// <summary>
        /// Order number of the transponder key
        /// </summary>
        [JsonPropertyName("orderNo")]
        public uint OrderNumber { get; set; }

        /// <summary>
        /// Serial number of the transponder key
        /// </summary>
        [JsonPropertyName("serialNo")]
        public string SerialNumber { get; set; }

        /// <summary>
        /// User data
        /// </summary>
        [JsonPropertyName("userData")]
        public List<UserDataValue> UserData { get; set; }
    }
}
