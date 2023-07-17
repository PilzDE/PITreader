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
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using Makaretu.Dns;

namespace Pilz.PITreader.Network
{
    /// <summary>
    /// Handler to perform network scan for PITreader devices.
    /// </summary>
    public class PITreaderNetworkScan
    {
        private const string DeviceServiceName = "pitreader.pilz.local";

        private readonly MulticastService mdns;

        private Timer timer;

        /// <summary>
        /// Creates a new network scan instance.
        /// </summary>
        /// <param name="networkInterfaceFilter">Filter for correct network interface.</param>
        public PITreaderNetworkScan(Func<IEnumerable<NetworkInterface>, IEnumerable<NetworkInterface>> networkInterfaceFilter)
        {
            this.mdns = new MulticastService(networkInterfaceFilter)
            {
                UseIpv4 = true,
                UseIpv6 = false
            };

            this.mdns.NetworkInterfaceDiscovered += this.OnNetworkInterfaceDiscovered;
            this.mdns.AnswerReceived += this.OnMdnsAnswerReceived;

            this.timer = null;
        }

        /// <summary>
        /// Event is triggered when a PITreader device is discovered.
        /// Filtering for duplicate devices needs to be done by the caller.
        /// </summary>
        public EventHandler<PITreaderScanResultEventArgs> DeviceDiscovered;

        /// <summary>
        /// Event is triggered when a network interface is discovered.
        /// Filtering for duplicate interfaces needs to be done by the caller.
        /// </summary>
        public EventHandler<NetworkInterfaceEventArgs> NetworkInterfaceDiscovered;

        /// <summary>
        /// Start scanning for devices on the selected network.
        /// The interval is 15 seconds.
        /// </summary>
        public void Start()
        {
            this.mdns.Start();

            if (this.timer == null)
            {
                this.timer = new Timer((object o) => this.Query(), null, 15000, 15000);
            }
        }

        /// <summary>
        /// Stops the mdns service
        /// </summary>
        public void Stop()
        {
            // Stop removes all event listeners
            this.mdns.Stop();

            if (this.timer != null)
            {
                this.timer.Dispose();
                this.timer = null;
            }
        }

        /// <summary>
        /// Restart scanning for devices (i.e. when the selected network interface changed).
        /// </summary>
        public void Restart()
        {
            // Stop removes all event listeners
            this.mdns.Stop();

            this.mdns.NetworkInterfaceDiscovered += this.OnNetworkInterfaceDiscovered;
            this.mdns.AnswerReceived += this.OnMdnsAnswerReceived;
            this.mdns.Start();

            this.Query();
        }

        /// <summary>
        /// Send a query to detect devices on the selected network.
        /// </summary>
        public void Query()
        {
            this.mdns.SendQuery(DeviceServiceName, DnsClass.IN, DnsType.ANY);
        }

        private void OnMdnsAnswerReceived(object sender, MessageEventArgs e)
        {
            if (e == null)
                return;

            if (!e.Message.Answers.Any(q => string.Equals(q.CanonicalName, DeviceServiceName, StringComparison.InvariantCultureIgnoreCase)))
                return;

            if (this.DeviceDiscovered == null)
                return;

            var model = new PITreaderScanResult
            {
                IpAddress = e.RemoteEndPoint.Address.ToString(),
                HttpsPort = 443
            };

            var srv = e.Message.AdditionalRecords.OfType<SRVRecord>().FirstOrDefault();
            if (srv != null && srv.Name == "_https._tcp.local") model.HttpsPort = srv.Port;

            foreach (var ar in e.Message.AdditionalRecords.OfType<TXTRecord>())
            {
                foreach (var str in ar.Strings)
                {
                    var parts = str.Split(new char[] { '=' }, 2);
                    if (parts.Length == 2)
                    {
                        switch (parts[0])
                        {
                            case "on": model.OrderNumber = parts[1]; break;
                            case "sn": model.SerialNumber = parts[1]; break;
                            case "mac": model.MacAddress = parts[1]; break;
                        }
                    }
                }
            }

            this.DeviceDiscovered(this, new PITreaderScanResultEventArgs(model));
        }

        private void OnNetworkInterfaceDiscovered(object sender, Makaretu.Dns.NetworkInterfaceEventArgs e)
        {
            if (e == null)
                return;

            if (this.NetworkInterfaceDiscovered != null)
            {
                var interfaceList = e.NetworkInterfaces.Where(item => item.OperationalStatus == OperationalStatus.Up
                    && item.SupportsMulticast).ToList();

                this.NetworkInterfaceDiscovered(this, new NetworkInterfaceEventArgs(interfaceList));
            }

            this.Query();
        }
    }
}
