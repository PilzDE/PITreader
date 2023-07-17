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
    /// Identify Request (flash led of device)
    /// </summary>
    public class IdentifyRequest : RequestPacket
    {
        public IdentifyRequest()
        {
            this.Command = CommandType.Identify;
        }

        /// <summary>
        /// Enable flashing of LED
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Timeout for fallback to default led behaviour.
        /// </summary>
        public UInt32 Timeout { get; set; }

        /// <summary>
        /// Write request packet to stream.
        /// </summary>
        /// <param name="stream">Target io stream.</param>
        public override void Write(Stream stream)
        {
            base.Write(stream);
            PacketWriter.WriteUInt8(stream, this.Enabled);
            PacketWriter.WriteUInt32(stream, this.Timeout);
        }
    }
}
