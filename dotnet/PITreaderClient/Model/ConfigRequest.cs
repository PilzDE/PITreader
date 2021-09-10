using Pilz.PITreader.Client.Serialization;
using System;
using System.Text.Json.Serialization;

namespace Pilz.PITreader.Client.Model
{
    /// <summary>
    /// Request to update the device configuration.
    /// </summary>
    public class ConfigRequest
    {
        /// <summary>
        /// Location information of the device.
        /// </summary>
        [JsonPropertyName("location")]
        public string Location { get; set; }

        /// <summary>
        /// Device group for authentication mode "TransponderData"
        /// </summary>
        [JsonPropertyName("deviceGroup")]
        public ushort? DeviceGroup { get; set; }

        /// <summary>
        /// Setting to enable/disable evaluation of start-/end dates on transponder keys.
        /// </summary>
        [JsonPropertyName("evaluateTimeLimitation")]
        public bool? EvaluateTimeLimitation { get; set; }

        /// <summary>
        /// Timezone for the evaluation of start-/end dates.
        /// </summary>
        [JsonPropertyName("timeZone")]
        public string TimeZone { get; set; }

        /// <summary>
        /// Updates the real time clock on the device.
        /// </summary>
        [JsonPropertyName("realTimeClock"), JsonConverter(typeof(JsonNullableDateTimeConverter))]
        public DateTime? RealTimeClock { get; set; }
    }
}
