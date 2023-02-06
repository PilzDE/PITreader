// Copyright (c) 2022 Pilz GmbH & Co. KG
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice (including the next paragraph) shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
// SPDX-License-Identifier: MIT

using System;
using System.Text.Json.Serialization;
using Pilz.PITreader.Client.Serialization;

namespace Pilz.PITreader.Client.Model
{
    /// <summary>
    /// Request to update the device configuration.
    /// </summary>
    public class ConfigRequest
    {
        /// <summary>
        /// Hostname/device name
        /// </summary>
        [JsonPropertyName("hostName")]
        public string HostName { get; set; }

        /// <summary>
        /// Location information of the device.
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
        public ushort? HttpPort { get; set; }

        /// <summary>
        /// HTTP protocol enabled
        /// </summary>
        [JsonPropertyName("httpEnabled")]
        public bool? HttpEnabled { get; set; }

        /// <summary>
        /// HTTPS port number
        /// </summary>
        [JsonPropertyName("httpsPort")]
        public ushort? HttpsPort { get; set; }

        /// <summary>
        /// Network discovery using MDNS enabled
        /// </summary>
        [JsonPropertyName("networkDiscoveryEnabled")]
        public bool? NetworkDiscoveryEnabled { get; set; }

        /// <summary>
        /// Multicast configuration protocol for initial setup of device enabled
        /// </summary>
        [JsonPropertyName("multicastConfEnabled")]
        public bool? MulticastConfigurationEnabled { get; set; }

        /// <summary>
        /// Status of SNTP client on the device
        /// </summary>
        [JsonPropertyName("sntpEnabled")]
        public bool? SntpEnabled { get; set; }

        /// <summary>
        /// IP address of configured SNTP server.
        /// </summary>
        [JsonPropertyName("sntpServer")]
        public string SntpServer { get; set; }

        /// <summary>
        /// Port number of configured SNTP server.
        /// </summary>
        [JsonPropertyName("sntpPort")]
        public ushort? SntpPort { get; set; }

        /// <summary>
        /// SNTP refresh interval in minutes.
        /// </summary>
        [JsonPropertyName("sntpRefreshRate")]
        public uint? SntpRefreshRate { get; set; }

        /// <summary>
        /// Activation state of the Modbus/TCP slave (server) in the device.
        /// </summary>
        [JsonPropertyName("modbusTcpEnabled")]
        public bool? ModbusTcpEnabled { get; set; }

        /// <summary>
        /// Port number of the Modbus/TCP slave (server) in the device.
        /// </summary>
        [JsonPropertyName("modbusTcpPort")]
        public ushort? ModbusTcpPort { get; set; }

        /// <summary>
        /// Device's authentication mode
        /// </summary>
        [JsonPropertyName("authenticationMode")]
        public AuthenticationMode? AuthenticationMode { get; set; }

        /// <summary>
        /// Allow external override
        /// </summary>
        [JsonPropertyName("allowExternalOverride")]
        public bool? AllowExternalOverride { get; set; }

        /// <summary>
        /// Device group for authentication mode "TransponderData"
        /// </summary>
        [JsonPropertyName("deviceGroup")]
        public ushort? DeviceGroup { get; set; }

        /// <summary>
        /// Mode of 24 V I/O port
        /// </summary>
        [JsonPropertyName("ioPortFunction")]
        public IoPortFunction? IoPortFunction { get; set; }

        /// <summary>
        /// Permission for the 24 V I/O port when configured as an output.
        /// </summary>
        [JsonPropertyName("ioPortPermission")]
        public Permission? IoPortPermission { get; set; }

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
        /// Recording of personal indentifiable information (PII) in diagnostic list and diagnostic log
        /// </summary>
        [JsonPropertyName("logPersonalData")]
        public bool? LogPersonalData { get; set; }

        /// <summary>
        /// Active authentication type
        /// </summary>
        [JsonPropertyName("authenticationType")]
        public AuthenticationType? AuthenticationType { get; set; }

        /// <summary>
        /// Updates the real time clock on the device.
        /// </summary>
        [JsonPropertyName("realTimeClock"), JsonConverter(typeof(JsonNullableDateTimeConverter))]
        public DateTime? RealTimeClock { get; set; }
    }
}
