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

using Pilz.PITreader.Client;

namespace PITreaderClient.Tests
{
    public class FirmwareVersionTests
    {
        [Fact]
        public void ComparisonTests()
        {
            Func<int, int, int, uint?, FirmwareVersion> gv = (a, b, c, d) => d.HasValue 
                ? new FirmwareVersion(a, b, c, d.Value)
                : new FirmwareVersion(a, b, c);

            Func<int, int, int, uint?, bool, FirmwareVersion> gvr = (a, b, c, d, r) =>
            {
                var v = d.HasValue
                    ? new FirmwareVersion(a, b, c, d.Value)
                    : new FirmwareVersion(a, b, c);
                v.Released = r;
                return v;
            };

            Assert.True(gv(2, 1, 0, null) == gv(2, 1, 0, 0));
            Assert.True(gvr(2, 1, 0, null, true) > gv(2, 1, 0, 100));
            Assert.True(gvr(2, 1, 0, null, false) < gv(2, 1, 0, 100));
        }
    }
}
