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

using Pilz.PITreader.PitTransponderManager.Import;
using Pilz.PITreader.PitTransponderManager.Model;
using Xunit;

namespace Pilz.PITreader.PitTransponderManager.Import.Tests
{
    public class TransponderFactoryTests
    {
        [Fact]
        public void Create_SetsTransponderIdToUidHex()
        {
            var uid = Convert.FromHexString("04A1B2C3D4E5F6");
            var row = new Dictionary<string, string> { { "uid", "04A1B2C3D4E5F6" } };

            var transponder = TransponderFactory.Create(uid, row);

            Assert.Equal("04A1B2C3D4E5F6", transponder.TransponderId);
        }

        [Fact]
        public void Create_ComputesSecurityId()
        {
            var uid = Convert.FromHexString("04A1B2C3D4E5F6");
            var row = new Dictionary<string, string> { { "uid", "04A1B2C3D4E5F6" } };

            var transponder = TransponderFactory.Create(uid, row);

            Assert.Equal("ED15644BB4A7E5B2", transponder.SecureId);
        }

        [Fact]
        public void Create_SetsTypeToThirdParty()
        {
            var uid = Convert.FromHexString("04A1B2C3D4E5F6");
            var row = new Dictionary<string, string> { { "uid", "04A1B2C3D4E5F6" } };

            var transponder = TransponderFactory.Create(uid, row);

            Assert.Equal(Transponder.TypeThirdParty, transponder.Type);
        }

        [Fact]
        public void Create_ReadsOptionalDates()
        {
            var uid = Convert.FromHexString("04A1B2C3D4E5F6");
            var row = new Dictionary<string, string>
            {
                { "uid", "04A1B2C3D4E5F6" },
                { "startDate", "2025-1-1" },
                { "endDate", "2026-12-31" }
            };

            var transponder = TransponderFactory.Create(uid, row);

            Assert.Equal("2025-1-1", transponder.StartDate);
            Assert.Equal("2026-12-31", transponder.EndDate);
        }

        [Fact]
        public void Create_DefaultsEmptyDates()
        {
            var uid = Convert.FromHexString("04A1B2C3D4E5F6");
            var row = new Dictionary<string, string> { { "uid", "04A1B2C3D4E5F6" } };

            var transponder = TransponderFactory.Create(uid, row);

            Assert.Equal(string.Empty, transponder.StartDate);
            Assert.Equal(string.Empty, transponder.EndDate);
        }
    }
}
