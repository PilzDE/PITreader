using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace Pilz.PITreader.Client.Model
{
    /// <summary>
    /// User data configuration stored on a transponder.
    /// </summary>
    public class UserDataParamaterDefintionResponse
    {
        /// <summary>
        /// Version number stored for the user data configuration.
        /// </summary>
        [JsonPropertyName("version")]
        public ushort Version { get; set; }

        /// <summary>
        /// User data parameters stored on the transponder (name field is always empty).
        /// </summary>
        [JsonPropertyName("parameters")]
        public List<UserDataParameter> Parameters { get; set; }
    }
}
