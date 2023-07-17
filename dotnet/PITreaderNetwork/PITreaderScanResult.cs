// Copyright (c) 2023 Pilz GmbH & Co. KG
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

namespace Pilz.PITreader.Network
{
    /// <summary>
    /// Model to represent a PITreader device.
    /// </summary>
    public class PITreaderScanResult : IEquatable<PITreaderScanResult>
    {
        private string ipAddress;
        private ushort httpsPort;
        private string orderNumber;
        private string serialNumber;
        private string macAddress;

        /// <summary>
        /// IP address of the device.
        /// </summary>
        public string IpAddress 
        { 
            get => this.ipAddress;
            set 
            {
                if (this.ipAddress != value)
                {
                    this.ipAddress = value;
                }
            }
        }

        /// <summary>
        /// HTTPS port number configured in the device (default: 443)
        /// </summary>
        public ushort HttpsPort 
        { 
            get => this.httpsPort;
            set
            {
                if (this.httpsPort != value)
                {
                    this.httpsPort = value;
                }
            }
        }

        /// <summary>
        /// Order number of the device.
        /// </summary>
        public string OrderNumber 
        { 
            get => this.orderNumber;
            set
            {
                if (this.orderNumber != value)
                {
                    this.orderNumber = value;
                }
            }
        }

        /// <summary>
        /// Serial number of the device.
        /// </summary>
        public string SerialNumber 
        { 
            get => this.serialNumber;
            set
            {
                if (this.serialNumber != value)
                {
                    this.serialNumber = value;
                }
            }
        }

        /// <summary>
        /// MAC address of the device.
        /// </summary>
        public string MacAddress 
        { 
            get => this.macAddress;
            set
            {
                if (this.macAddress != value)
                {
                    this.macAddress = value;
                }
            }
        }

        /// <summary>
        /// Updates data of object with data from other object
        /// </summary>
        /// <param name="other">Other PITreader device</param>
        public void Update(PITreaderScanResult other)
        {
            if (other is null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            this.IpAddress = other.IpAddress;
            this.HttpsPort = other.HttpsPort;
            this.OrderNumber = other.OrderNumber;
            this.SerialNumber = other.SerialNumber;
            this.MacAddress = other.MacAddress;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// The comparison is based on order and serial number.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the other parameter; otherwise, false.</returns>
        public bool Equals(PITreaderScanResult other)
        {
            if (other is null)
            {
                return false;
            }

            return string.Equals(this.OrderNumber, other.OrderNumber, StringComparison.InvariantCultureIgnoreCase)
                && string.Equals(this.SerialNumber, other.SerialNumber, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
