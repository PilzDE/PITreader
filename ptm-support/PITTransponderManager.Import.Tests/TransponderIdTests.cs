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

using Xunit;

namespace Pilz.PITreader.PitTransponderManager.Import.Tests
{
    public class TransponderIdTests
    {
        /// <summary>
        /// Known-good UID -> expected ID tuples.
        /// Add new [InlineData] lines to extend test coverage with additional test vectors.
        /// </summary>
        [Theory]
        [InlineData("04A1B2C3D4E5F6",   "ED15644BB4A7E5B2")]  // 7-byte UID (ISO 14443-A)
        [InlineData("0102030405060708",  "FAE5EB8EA97B4792")]  // 8-byte UID (ISO 15693)
        [InlineData("01020304",          "7B19A81D1C14C748")]  // 4-byte UID
        public void Generate_ReturnsExpectedId(string uidHex, string expectedid)
        {
            var result = TransponderId.Generate(uidHex);

            Assert.Equal(expectedid, result);
        }

        [Fact]
        public void Generate_IsDeterministic()
        {
            var id1 = TransponderId.Generate("04A1B2C3D4E5F6");
            var id2 = TransponderId.Generate("04A1B2C3D4E5F6");

            Assert.Equal(id1, id2);
        }

        [Fact]
        public void Generate_DifferentInputsProduceDifferentIds()
        {
            var id1 = TransponderId.Generate("04A1B2C3D4E5F6");
            var id2 = TransponderId.Generate("04A1B2C3D4E5F7");

            Assert.NotEqual(id1, id2);
        }

        [Fact]
        public void Generate_ReturnsUppercaseHex16Chars()
        {
            var result = TransponderId.Generate("04A1B2C3D4E5F6");

            Assert.Equal(16, result.Length);
            Assert.Equal(result, result.ToUpperInvariant());
        }

        [Fact]
        public void Generate_ThrowsOnUidTooShort()
        {
            Assert.Throws<ArgumentException>(() => TransponderId.Generate("010203"));
        }

        [Fact]
        public void Generate_ThrowsOnUidTooLong()
        {
            Assert.Throws<ArgumentException>(() => TransponderId.Generate("0102030405060708090A0B"));
        }
    }
}
