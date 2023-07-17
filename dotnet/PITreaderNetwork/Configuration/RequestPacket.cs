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
using System.IO;

namespace Pilz.PITreader.Network.Configuration
{
    /// <summary>
    /// Generic Multicast Configuration Protocol request packet.
    /// </summary>
    public abstract class RequestPacket
    {
        public RequestPacket()
        {
            var rnd = new Random();
            this.RequestId = (UInt32)rnd.Next();
        }

        /// <summary>
        /// ID of the request to match response packets.
        /// </summary>
        public UInt32 RequestId { get; set; }

        /// <summary>
        /// Order number of the targeted device.
        /// </summary>
        public string OrderNumber { get; set; }

        /// <summary>
        /// Serial number of the targeted device.
        /// </summary>
        public string SerialNumber { get; set; }

        /// <summary>
        /// Command type of the request.
        /// </summary>
        public CommandType Command { get; set; }

        /// <summary>
        /// Write request packet to stream.
        /// </summary>
        /// <param name="stream">Target io stream.</param>
        public virtual void Write(Stream stream)
        {
            PacketWriter.WriteMagic(stream);
            PacketWriter.WriteUInt16(stream, Constants.PacketVersion);
            PacketWriter.WriteUInt16(stream, (UInt16)PacketType.Request);
            PacketWriter.WriteUInt32(stream, this.RequestId);
            PacketWriter.WriteStringFixed(stream, this.OrderNumber, 12);
            PacketWriter.WriteStringFixed(stream, this.SerialNumber, 12);
            PacketWriter.WriteUInt32(stream, (UInt32)this.Command);
            PacketWriter.WriteUInt16(stream, 0);
        }
    }
}
