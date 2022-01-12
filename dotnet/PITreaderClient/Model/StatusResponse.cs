using System;
using System.Text.Json.Serialization;

namespace Pilz.PITreader.Client.Model
{
    /// <summary>
    /// Current status information on the status and properties of the device.
    /// </summary>
    public class StatusResponse
    {
        /// <summary>
        /// Hostname/device name
        /// </summary>
        [JsonPropertyName("hostName")]
        public string HostName { get; set; }

        /// <summary>
        /// Device's IP address
        /// </summary>
        [JsonPropertyName("ipAddress")]
        public string IpAddress { get; set; }

        /// <summary>
        /// Device's order number
        /// </summary>
        [JsonPropertyName("orderNo")]
        public string OrderNumber { get; set; }

        /// <summary>
        /// Device's product name
        /// </summary>
        [JsonPropertyName("productType")]
        public string ProductType { get; set; }

        /// <summary>
        /// Device’s serial number
        /// </summary>
        [JsonPropertyName("serialNo")]
        public uint? SerialNumber { get; set; }

        /// <summary>
        /// Device's MAC address
        /// Format: 00:00:00:00:00:00
        /// </summary>
        [JsonPropertyName("macAddress")]
        public string MacAddress { get; set; }

        /// <summary>
        /// Status of the connection between PIT m4SEU and PITreader
        /// </summary>
        /// <value><c>true</c>: There is an active connection between a PIT m4SEU and the PITreader, otherwise: <c>false</c>.</value>
        [JsonPropertyName("seuStatus")]
        public bool SeuStatus { get; set; }

        /// <summary>
        /// Status of the basic coding on the PITreader
        /// </summary>
        [JsonPropertyName("coded")]
        public bool Coded { get; set; }

        /// <summary>
        /// Status of the OEM coding on the PITreader
        /// </summary>
        [JsonPropertyName("codedOem")]
        public bool CodedOem { get; set; }

        /// <summary>
        /// Authentication status of the transponder key
        /// </summary>
        [JsonPropertyName("transponderAuthenticated")]
        public bool TransponderAuthenticated { get; set; }

        /// <summary>
        /// Device's firmware version
        /// </summary>
        [JsonPropertyName("fwVersion")]
        public Version FirmwareVersion { get; set; }

        /// <summary>
        /// PITreader's hardware version
        /// </summary>
        [JsonPropertyName("hwVersion")]
        public Version HardwareVersion { get; set; }

        /// <summary>
        /// PITreader's hardware type
        /// </summary>
        [JsonPropertyName("hwVariant")]
        public byte HarwareVariant { get; set; }

        /// <summary>
        /// Device’s current date and time
        /// </summary>
        [JsonPropertyName("realTimeClock")]
        public DateTime? RealTimeClock { get; set; }

        /// <summary>
        /// True: it is officially released firmware.
        /// </summary>
        [JsonPropertyName("released")]
        public bool Released { get; set; }

        /// <summary>
        /// Configuration of 24 V I/O port
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter)), JsonPropertyName("ioPortMode")]
        public IoPortMode IoPortMode { get; set; }

        /// <summary>
        /// Signal present at the 24 V I/O port
        /// </summary>
        [JsonPropertyName("ioPortValue")]
        public IoPortValue IoPortValue { get; set; }
    }
}
