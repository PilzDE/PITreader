// Copyright (c) 2026 Pilz GmbH & Co. KG
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

using System.Security.Cryptography;

namespace Pilz.PITreader.PitTransponderManager.Import
{
    /// <summary>
    /// Computes device identifiers from transponder parameters.
    /// </summary>
    public static class TransponderId
    {
        /// <summary>
        /// Minimum transponder UID size in bytes (4-byte UID per ISO 14443-A).
        /// </summary>
        public const int TagUidSizeMin = 4;

        /// <summary>
        /// Maximum transponder UID size in bytes (10-byte UID per ISO 14443-A / ISO 15693).
        /// </summary>
        public const int TagUidSizeMax = 10;

        private static readonly byte[] _ftable = new byte[]
        {
            0x00, 0xEA, 0x3E, 0xD4, 0x7C, 0x96, 0x42, 0xA8, 0xF8, 0x12, 0xC6, 0x2C, 0x84, 0x6E, 0xBA, 0x50,
            0x1A, 0xF0, 0x24, 0xCE, 0x66, 0x8C, 0x58, 0xB2, 0xE2, 0x08, 0xDC, 0x36, 0x9E, 0x74, 0xA0, 0x4A,
            0x34, 0xDE, 0x0A, 0xE0, 0x48, 0xA2, 0x76, 0x9C, 0xCC, 0x26, 0xF2, 0x18, 0xB0, 0x5A, 0x8E, 0x64,
            0x2E, 0xC4, 0x10, 0xFA, 0x52, 0xB8, 0x6C, 0x86, 0xD6, 0x3C, 0xE8, 0x02, 0xAA, 0x40, 0x94, 0x7E,
            0x68, 0x82, 0x56, 0xBC, 0x14, 0xFE, 0x2A, 0xC0, 0x90, 0x7A, 0xAE, 0x44, 0xEC, 0x06, 0xD2, 0x38,
            0x72, 0x98, 0x4C, 0xA6, 0x0E, 0xE4, 0x30, 0xDA, 0x8A, 0x60, 0xB4, 0x5E, 0xF6, 0x1C, 0xC8, 0x22,
            0x5C, 0xB6, 0x62, 0x88, 0x20, 0xCA, 0x1E, 0xF4, 0xA4, 0x4E, 0x9A, 0x70, 0xD8, 0x32, 0xE6, 0x0C,
            0x46, 0xAC, 0x78, 0x92, 0x3A, 0xD0, 0x04, 0xEE, 0xBE, 0x54, 0x80, 0x6A, 0xC2, 0x28, 0xFC, 0x16,
            0xD0, 0x3A, 0xEE, 0x04, 0xAC, 0x46, 0x92, 0x78, 0x28, 0xC2, 0x16, 0xFC, 0x54, 0xBE, 0x6A, 0x80,
            0xCA, 0x20, 0xF4, 0x1E, 0xB6, 0x5C, 0x88, 0x62, 0x32, 0xD8, 0x0C, 0xE6, 0x4E, 0xA4, 0x70, 0x9A,
            0xE4, 0x0E, 0xDA, 0x30, 0x98, 0x72, 0xA6, 0x4C, 0x1C, 0xF6, 0x22, 0xC8, 0x60, 0x8A, 0x5E, 0xB4,
            0xFE, 0x14, 0xC0, 0x2A, 0x82, 0x68, 0xBC, 0x56, 0x06, 0xEC, 0x38, 0xD2, 0x7A, 0x90, 0x44, 0xAE,
            0xB8, 0x52, 0x86, 0x6C, 0xC4, 0x2E, 0xFA, 0x10, 0x40, 0xAA, 0x7E, 0x94, 0x3C, 0xD6, 0x02, 0xE8,
            0xA2, 0x48, 0x9C, 0x76, 0xDE, 0x34, 0xE0, 0x0A, 0x5A, 0xB0, 0x64, 0x8E, 0x26, 0xCC, 0x18, 0xF2,
            0x8C, 0x66, 0xB2, 0x58, 0xF0, 0x1A, 0xCE, 0x24, 0x74, 0x9E, 0x4A, 0xA0, 0x08, 0xE2, 0x36, 0xDC,
            0x96, 0x7C, 0xA8, 0x42, 0xEA, 0x00, 0xD4, 0x3E, 0x6E, 0x84, 0x50, 0xBA, 0x12, 0xF8, 0x2C, 0xC6
        };

        /// <summary>
        /// Computes a ID from a transponder UID.
        /// </summary>
        /// <param name="tagUid">Transponder UID bytes (4-10 bytes).</param>
        /// <returns>ID as a 16-character uppercase hexadecimal string.</returns>
        /// <exception cref="ArgumentException">UID size is outside the valid range (4-10 bytes).</exception>
        public static string Generate(ReadOnlySpan<byte> tagUid)
        {
            if (tagUid.Length < TagUidSizeMin || tagUid.Length > TagUidSizeMax)
            {
                throw new ArgumentException(
                    $"Tag UID must be {TagUidSizeMin}-{TagUidSizeMax} bytes, got {tagUid.Length}.",
                    nameof(tagUid));
            }

            Span<byte> input = stackalloc byte[tagUid.Length + 8];
            tagUid.CopyTo(input);
            input.Slice(tagUid.Length, 8).Clear();

            Span<byte> result = stackalloc byte[8];
            result.Clear();
            DeriveBytes(input, result.Slice(0, 7));
            result[7] = Finalize(result.Slice(0, 7));

            return Convert.ToHexString(result);
        }

        /// <summary>
        /// Computes a ID from a transponder UID hex string.
        /// </summary>
        /// <param name="tagUidHex">Transponder UID as hex string (8-20 characters).</param>
        /// <returns>ID as a 16-character uppercase hexadecimal string.</returns>
        public static string Generate(string tagUidHex)
        {
            byte[] tagUid = Convert.FromHexString(tagUidHex);
            return Generate(tagUid);
        }

        private static void DeriveBytes(ReadOnlySpan<byte> input, Span<byte> output)
        {
            Shake128.HashData(input, output);
        }

        private static byte Finalize(ReadOnlySpan<byte> data)
        {
            byte v = 0;
            foreach (byte b in data)
            {
                v = _ftable[v ^ b];
            }
            return v;
        }
    }
}
