﻿using System.Text.Json.Serialization;

namespace Pilz.PITreader.Client.Model
{
    /// <summary>
    /// Current device configuration.
    /// </summary>
    public class ConfigResponse
    {
        /// <summary>
        /// Hostname/device name
        /// </summary>
        [JsonPropertyName("hostName")]
        public string HostName { get; set; }

        /// <summary>
        /// Devices’s location description
        /// </summary>
        [JsonPropertyName("location")]
        public string Location { get; set; }

        /// <summary>
        /// Domain name for HTTPS certificates
        /// </summary>
        [JsonPropertyName("domain")]
        public string Domain { get; set; }

        /// <summary>
        /// Device's IP address
        /// </summary>
        [JsonPropertyName("ipAddress")]
        public string IpAddress { get; set; }

        /// <summary>
        /// Device’s subnet mask
        /// </summary>
        [JsonPropertyName("subnetMask")]
        public string SubnetMask { get; set; }

        /// <summary>
        /// Standard Gateway's IP address
        /// </summary>
        [JsonPropertyName("defaultGateway")]
        public string DefaultGateway { get; set; }

        /// <summary>
        /// HTTP port number
        /// </summary>
        [JsonPropertyName("httpPort")]
        public ushort HttpPort { get; set; }

        /// <summary>
        /// HTTP protocol enabled
        /// </summary>
        [JsonPropertyName("httpEnabled")]
        public bool HttpEnabled { get; set; }

        /// <summary>
        /// HTTPS port number
        /// </summary>
        [JsonPropertyName("httpsPort")]
        public ushort HttpsPort { get; set; }

        /// <summary>
        /// Status of SNTP client on the device
        /// </summary>
        [JsonPropertyName("sntpEnabled")]
        public bool SntpEnabled { get; set; }

        /// <summary>
        /// IP address of configured SNTP server.
        /// </summary>
        [JsonPropertyName("sntpServer")]
        public string SntpServer { get; set; }

        /// <summary>
        /// Port number of configured SNTP server.
        /// </summary>
        [JsonPropertyName("sntpPort")]
        public ushort SntpPort { get; set; }

        /// <summary>
        /// SNTP refresh interval in minutes.
        /// </summary>
        [JsonPropertyName("sntpRefreshRate")]
        public uint SntpRefreshRate { get; set; }

        /// <summary>
        /// Activation state of the Modbus/TCP slave (server) in the device.
        /// </summary>
        [JsonPropertyName("modbusTcpEnabled")]
        public bool ModbusTcpEnabled { get; set; }

        /// <summary>
        /// Port number of the Modbus/TCP slave (server) in the device.
        /// </summary>
        [JsonPropertyName("modbusTcpPort")]
        public ushort ModbusTcpPort { get; set; }

        /// <summary>
        /// Device's authentication mode
        /// </summary>
        [JsonPropertyName("authenticationMode")]
        public AuthenticationMode AuthenticationMode { get; set; }

        /// <summary>
        /// Device group for authentication mode "TransponderData"
        /// </summary>
        [JsonPropertyName("deviceGroup")]
        public ushort DeviceGroup { get; set; }

        /// <summary>
        /// Mode of 24 V I/O port
        /// </summary>
        [JsonPropertyName("ioPortFunction")]
        public IoPortFunction IoPortFunction { get; set; }

        /// <summary>
        /// Permission for the 24 V I/O port when configured as an output.
        /// </summary>
        [JsonPropertyName("ioPortPermission")]
        public Permission IoPortPermission { get; set; }

        /// <summary>
        /// Indicates whether start date ("Valid from") and end date ("Valid until") of transponder keys are evaluated.
        /// </summary>
        [JsonPropertyName("evaluateTimeLimitation")]
        public bool EvaluateTimeLimitation { get; set; }

        /// <summary>
        /// Name of time zone for evaluation of start and end dates.
        /// </summary>
        [JsonPropertyName("timeZone")]
        public string TimeZone { get; set; }

        /// <summary>
        /// Recording of personal indentifiable information (PII) in diagnostic list and diagnostic log
        /// </summary>
        [JsonPropertyName("logPersonalData")]
        public bool LogPersonalData { get; set; }

        /// <summary>
        /// Active authentication type
        /// </summary>
        [JsonPropertyName("authenticationType")]
        public AuthenticationType AuthenticationType { get; set; }
    }
}
