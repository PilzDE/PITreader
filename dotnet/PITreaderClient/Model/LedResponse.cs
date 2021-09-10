using System.Text.Json.Serialization;

namespace Pilz.PITreader.Client.Model
{
    /// <summary>
    /// Current LED state and overwrite settings
    /// </summary>
    public class LedResponse
    {
        /// <summary>
        /// Current LED colour
        /// </summary>
        [JsonPropertyName("colour")]
        public LedColour Colour { get; set; }

        /// <summary>
        /// Current LED flash mode
        /// </summary>
        [JsonPropertyName("flashMode")]
        public LedFlashMode FlashMode { get; set; }

        /// <summary>
        /// Current LED overwrite settings
        /// </summary>
        [JsonPropertyName("overwrite")]
        public LedOverwriteSettings Overwrite { get; set; }
    }
}
