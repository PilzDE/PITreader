using System.Text.Json.Serialization;

namespace Pilz.PITreader.Client.Model
{
    /// <summary>
    /// Settings for overwriting the internal LED control logic.
    /// </summary>
    public class LedOverwriteSettings
    {
        /// <summary>
        /// Colour to be applied to the device's LED.
        /// </summary>
        [JsonPropertyName("colour")]
        public LedColour Colour { get; set; }

        /// <summary>
        /// Flash mode to be applied to the device's LED.
        /// </summary>
        [JsonPropertyName("flashMode")]
        public LedFlashMode FlashMode { get; set; }

        /// <summary>
        /// Setting to enable overwrite of internal LED logic.
        /// </summary>
        [JsonPropertyName("activated")]
        public bool Acticated { get; set; }
    }
}
