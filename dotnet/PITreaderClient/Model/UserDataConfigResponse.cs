using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Pilz.PITreader.Client.Model
{
    /// <summary>
    /// Contains complete user data configuration of a device.
    /// </summary>
    public class UserDataConfigResponse
    {
        /// <summary>
        /// Version number stored with the configuration.
        /// </summary>
        [JsonPropertyName("version")]
        public ushort Version { get; set; }

        /// <summary>
        /// Comment stored with the configuration.
        /// </summary>
        [JsonPropertyName("comment")]
        public string Comment { get; set; }

        /// <summary>
        /// List of configured user data parameters.
        /// </summary>
        [JsonPropertyName("parameters")]
        public List<UserDataParameter> Parameters { get; set; }
    }
}
