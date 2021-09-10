using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace Pilz.PITreader.Client.Model
{
    /// <summary>
    /// User data stored on a transponder.
    /// </summary>
    public class UserDataResponse
    {
        /// <summary>
        /// Configuration (parameter definitions) stored on the transponder.
        /// </summary>
        [JsonPropertyName("parameterDefinition")]
        public UserDataParamaterDefintionResponse ParameterDefintion { get; set; }

        /// <summary>
        /// User data stored on the transponder.
        /// </summary>
        [JsonPropertyName("groups")]
        public List<UserDataGroupResponse> Groups { get; set; }
    }
}
