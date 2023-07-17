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
using System.Text;

namespace Pilz.PITreader.Network.Configuration
{
    internal class PacketWriter
    {
        public static void WriteMagic(Stream stream)
        {
            stream.Write(Constants.PacketMagic, 0, Constants.PacketMagic.Length);
        }

        public static void WriteUInt8(Stream stream, bool value)
        {
            stream.WriteByte(value ? (byte)1 : (byte)0);
        }
        public static void WriteUInt8(Stream stream, byte value)
        {
            stream.WriteByte(value);
        }

        public static void WriteUInt16(Stream stream, UInt16 value)
        {
            stream.WriteByte((byte)(value >> 8));
            stream.WriteByte((byte)value);
        }

        public static void WriteUInt32(Stream stream, UInt32 value)
        {
            stream.WriteByte((byte)(value >> 24));
            stream.WriteByte((byte)(value >> 16));
            stream.WriteByte((byte)(value >> 8));
            stream.WriteByte((byte)value);
        }

        public static void WriteStringFixed(Stream stream, string value, uint size)
        {
            var result = new byte[size];
            Array.Clear(result, 0, result.Length);

            var arr = Encoding.ASCII.GetBytes(value ?? string.Empty);
            if (arr.Length <= size)
            {
                arr.CopyTo(result, 0);
            }
            else
            {
                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = arr[i];
                }
            }

            stream.Write(result, 0, result.Length);
        }
    }
}
