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
    /// Response packet of Multicast Configuration Protocol.
    /// </summary>
    public class ResponsePacket
    {
        /// <summary>
        /// Order number of the device.
        /// </summary>
        public string OrderNumber { get; set; }

        /// <summary>
        /// Serial number of the device.
        /// </summary>
        public string SerialNumber { get; set; }

        /// <summary>
        /// Request ID (identical to RequestId in request packet).
        /// </summary>
        public UInt32 RequestId { get; set; }

        /// <summary>
        /// Command type (identical to Command in request packet).
        /// </summary>
        public CommandType Command { get; set; }

        /// <summary>
        /// Status code.
        /// </summary>
        public StatusCode Status { get; set; }

        /// <summary>
        /// Message code with additional status information.
        /// </summary>
        public UInt32 MessageCode { get; set; }

        /// <summary>
        /// Reads response packet from stream and returns parsed packet or NULL, if no packet was present in the stream.
        /// </summary>
        /// <param name="stream">The io stream for reading data.</param>
        /// <returns>The parsed response packet or NULL</returns>
        public static ResponsePacket Read(Stream stream)
        {
            var response = new ResponsePacket();

            if (!PacketReader.VerifyMagic(stream)) return null;

            // Version
            if (PacketReader.ReadUInt16(stream) != Constants.PacketVersion) return null;
            if (PacketReader.ReadUInt16(stream) != (UInt16)PacketType.Response) return null;

            response.RequestId = PacketReader.ReadUInt32(stream);

            response.OrderNumber = PacketReader.ReadStringFixed(stream, 12);
            response.SerialNumber = PacketReader.ReadStringFixed(stream, 12);

            response.Command = (CommandType)PacketReader.ReadUInt32(stream);

            // Extension
            if (PacketReader.ReadUInt16(stream) != 0) return null;

            response.Status = (StatusCode)PacketReader.ReadUInt16(stream);
            response.MessageCode = PacketReader.ReadUInt32(stream);
            return response;
        }
    }
}
