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
using System.Linq;
using System.Text;

namespace Pilz.PITreader.Network.Configuration
{
    internal class PacketReader
    {
        public static bool VerifyMagic(Stream stream)
        {
            byte[] input = new byte[Constants.PacketMagic.Length];
            stream.Read(input, 0, input.Length);
            return input.SequenceEqual(Constants.PacketMagic);
        }

        public static bool ReadBool(Stream stream)
        {
            return stream.ReadByte() == 1;
        }

        public static UInt16 ReadUInt16(Stream stream)
        {
            int value = stream.ReadByte();
            value = value << 8 | stream.ReadByte();
            return (UInt16)value;
        }

        public static UInt32 ReadUInt32(Stream stream)
        {
            UInt32 value = (UInt32)stream.ReadByte();
            value = value << 8 | (UInt32)stream.ReadByte();
            value = value << 8 | (UInt32)stream.ReadByte();
            value = value << 8 | (UInt32)stream.ReadByte();
            return value;
        }

        public static string ReadStringFixed(Stream stream, int size)
        {
            byte[] data = new byte[size];
            stream.Read(data, 0, size);

            int count0s = 0;
            for (int i = size - 1; i >= 0; i--)
            {
                if (data[i] == 0) count0s++;
                else break;
            }

            return Encoding.ASCII.GetString(data, 0, size - count0s);
        }
    }
}
