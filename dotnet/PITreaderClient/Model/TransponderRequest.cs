using System;
using System.Text.Json.Serialization;
using Pilz.PITreader.Client.Serialization;

namespace Pilz.PITreader.Client.Model
{
    /// <summary>
    /// Data to be written to a transponder.
    /// </summary>
    public class TransponderRequest
    {
        /// <summary>
        /// Array of permissions (32 values, one for each device group).
        /// </summary>
        [JsonPropertyName("permissions")]
        public Permission[] Permissions { get; set; }

        /// <summary>
        /// Start date for evaluation of start/end dates of the transponder.
        /// </summary>
        [JsonPropertyName("timeLimitationStart"), JsonConverter(typeof(JsonNullableDateTimeConverter))]
        public DateTime? TimeLimitationStart { get; set; }

        /// <summary>
        /// End date for evaluation of start/end dates of the transponder.
        /// </summary>
        [JsonPropertyName("timeLimitationEnd"), JsonConverter(typeof(JsonNullableDateTimeConverter))]
        public DateTime? TimeLimitationEnd { get; set; }

        /// <summary>
        /// If <c>true</c>, data on that transponder is only readable on PITreader devices with a matching coding.
        /// </summary>
        [JsonPropertyName("codingLock")]
        public bool CodingLock { get; set; }
    }
}
