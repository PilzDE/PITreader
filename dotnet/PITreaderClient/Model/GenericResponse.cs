using System.Text.Json.Serialization;
using System.Diagnostics;

namespace Pilz.PITreader.Client.Model
{
    /// <summary>
    /// Response to POST requests or on entry.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class GenericResponse
    {
        /// <summary>
        /// Status of request execution.
        /// </summary>
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        /// <summary>
        /// Message key returned by the device.
        /// </summary>
        [JsonPropertyName("msg")]
        public string Message { get; set; }

        /// <summary>
        /// Data returned by the device.
        /// </summary>
        [JsonPropertyName("data")]
        public dynamic Data { get; set; }

        private string DebuggerDisplay { get => string.Format("Succes: {0}, Message: {1}, Data: {{ {2} }}", this.Success, this.Message, (object)this.Data ?? "null"); }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() => this.DebuggerDisplay;
    }
}
