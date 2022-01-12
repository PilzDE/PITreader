using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Pilz.PITreader.Client.Model
{
    /// <summary>
    /// User data stored for a distinct device group or default values.
    /// </summary>
    public class UserDataGroupResponse
    {
        /// <summary>
        /// Device group or <c>null</c> for default values.
        /// </summary>
        [JsonPropertyName("deviceGroup")]
        public ushort? DeviceGroup { get; set; }

        /// <summary>
        /// List of values stored for the device group.
        /// </summary>
        [JsonPropertyName("values")]
        public List<UserDataValue> Values { get; set; }
    }
}
