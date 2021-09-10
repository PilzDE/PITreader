using System.Text.Json.Serialization;

namespace Pilz.PITreader.Client.Model
{
    /// <summary>
    /// Request to change LED settings.
    /// </summary>
    public class LedRequest
    {
        /// <summary>
        /// Sets the colour of the LED.
        /// </summary>
        [JsonPropertyName("colour")]
        public LedColour? Colour { get; set; }

        /// <summary>
        /// Sets the flash mode (static or blinking) of the LED.
        /// </summary>
        [JsonPropertyName("flashMode")]
        public LedFlashMode? FlashMode { get; set; }

        /// <summary>
        /// If true, these settings override the internal logic for the LED colour of the device.
        /// </summary>
        [JsonPropertyName("activated")]
        public bool? Activated { get; set; }
    }
}
