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
        /// Verification code for the TLS fingerprint
        /// Format: 000000 (6 digits)
        /// </summary>
        [JsonPropertyName("tlsFingerprintVerification")]
        public string TlsFingerprintVerification { get; set; }

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
