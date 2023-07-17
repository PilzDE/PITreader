/**
 * The following code is based on https://github.com/richardschneider/net-mdns/blob/master/src/MulticastClient.cs
 * 
 * MIT License
 * 
 * Copyright (c) 2018 Richard Schneider
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Pilz.PITreader.Network.Configuration
{
    internal class MulticastClient : IDisposable
    {
        /// <summary>
        ///   The port number used by the Multicast Configuration Protocol.
        /// </summary>
        /// <value>
        ///   Port number 7075.
        /// </value>
        public static readonly int MulticastPort = 7075;

        static readonly IPAddress MulticastAddressIp4 = IPAddress.Parse("239.255.0.12");
        static readonly IPEndPoint MulticastEndpointIp4 = new IPEndPoint(MulticastAddressIp4, MulticastPort);

        readonly List<UdpClient> receivers;
        readonly ConcurrentDictionary<IPAddress, UdpClient> senders = new ConcurrentDictionary<IPAddress, UdpClient>();

        public event EventHandler<UdpReceiveResult> MessageReceived;

        public MulticastClient(IEnumerable<NetworkInterface> nics)
        {
            // Setup the receivers.
            receivers = new List<UdpClient>();

            UdpClient receiver4 = null;
            receiver4 = new UdpClient(AddressFamily.InterNetwork);
            receiver4.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
#if NETSTANDARD2_0
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                LinuxHelper.ReuseAddresss(receiver4.Client);
            }
#endif
            receiver4.Client.Bind(new IPEndPoint(IPAddress.Any, MulticastPort));
            receivers.Add(receiver4);

            // Get the IP addresses that we should send to.
            var addreses = nics
                .SelectMany(GetNetworkInterfaceLocalAddresses)
                .Where(a => a.AddressFamily == AddressFamily.InterNetwork);
            foreach (var address in addreses)
            {
                if (senders.Keys.Contains(address))
                {
                    continue;
                }

                var localEndpoint = new IPEndPoint(address, MulticastPort);
                var sender = new UdpClient(address.AddressFamily);
                try
                {
                    switch (address.AddressFamily)
                    {
                        case AddressFamily.InterNetwork:
                            receiver4.Client.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(MulticastAddressIp4, address));
                            sender.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
#if NETSTANDARD2_0
                            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                            {
                                LinuxHelper.ReuseAddresss(sender.Client);
                            }
#endif
                            sender.Client.Bind(localEndpoint);
                            sender.Client.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(MulticastAddressIp4));
                            sender.Client.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastLoopback, true);
                            break;
                        default:
                            throw new NotSupportedException($"Address family {address.AddressFamily}.");
                    }

                    if (!senders.TryAdd(address, sender)) // Should not fail
                    {
                        sender.Dispose();
                    }
                }
                catch (SocketException ex) when (ex.SocketErrorCode == SocketError.AddressNotAvailable)
                {
                    // VPN NetworkInterfaces
                    sender.Dispose();
                }
                catch (Exception)
                {
                    sender.Dispose();
                }
            }

            // Start listening for messages.
            foreach (var r in receivers)
            {
                Listen(r);
            }
        }

        public async Task SendAsync(byte[] message)
        {
            foreach (var sender in senders)
            {
                try
                {
                    await sender.Value.SendAsync(
                        message, message.Length,
                        MulticastEndpointIp4)
                    .ConfigureAwait(false);
                }
                catch
                {
                    // eat it.
                }
            }
        }

        void Listen(UdpClient receiver)
        {
            // ReceiveAsync does not support cancellation.  So the receiver is disposed
            // to stop it. See https://github.com/dotnet/corefx/issues/9848
            Task.Run(async () =>
            {
                try
                {
                    var task = receiver.ReceiveAsync();

                    _ = task.ContinueWith(x => Listen(receiver), TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.RunContinuationsAsynchronously);

                    _ = task.ContinueWith(x => MessageReceived?.Invoke(this, x.Result), TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.RunContinuationsAsynchronously);

                    await task.ConfigureAwait(false);
                }
                catch
                {
                    return;
                }
            });
        }

        IEnumerable<IPAddress> GetNetworkInterfaceLocalAddresses(NetworkInterface nic)
        {
            return nic
                .GetIPProperties()
                .UnicastAddresses
                .Select(x => x.Address)
                .Where(x => x.AddressFamily != AddressFamily.InterNetworkV6 || x.IsIPv6LinkLocal)
                ;
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    MessageReceived = null;

                    foreach (var receiver in receivers)
                    {
                        try
                        {
                            receiver.Dispose();
                        }
                        catch
                        {
                            // eat it.
                        }
                    }
                    receivers.Clear();

                    foreach (var address in senders.Keys)
                    {
                        if (senders.TryRemove(address, out var sender))
                        {
                            try
                            {
                                sender.Dispose();
                            }
                            catch
                            {
                                // eat it.
                            }
                        }
                    }
                    senders.Clear();
                }

                disposedValue = true;
            }
        }

        ~MulticastClient()
        {
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }

    static class LinuxHelper
    {
#if NETSTANDARD2_0
        // see https://github.com/richardschneider/net-mdns/issues/22
        public static unsafe void ReuseAddresss(Socket socket)
        {
            int setval = 1;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                int rv = Tmds.Linux.LibC.setsockopt(
                    socket.Handle.ToInt32(),
                    Tmds.Linux.LibC.SOL_SOCKET,
                    Tmds.Linux.LibC.SO_REUSEADDR,
                    &setval, sizeof(int));
                if (rv != 0)
                {
                    throw new Exception("Socet reuse addr failed.");
                    //todo: throw new PlatformException();
                }
            }
        }
#endif

    }
}