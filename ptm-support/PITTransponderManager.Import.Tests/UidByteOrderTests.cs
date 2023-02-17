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
using Xunit;

namespace Pilz.PITreader.PitTransponderManager.Import.Tests
{
    public class UidByteOrderTests : IDisposable
    {
        private readonly List<string> _tempFiles = new();

        public void Dispose()
        {
            foreach (var file in _tempFiles)
            {
                try { File.Delete(file); } catch { }
            }
        }

        [Fact]
        public void Import_Iso15693LsbFirst_ReversesUids()
        {
            var csv = "uid\n11223344556677E0\nAABBCCDDEEFF00E0\n";
            var path = WriteTempCsv(csv);

            var result = CsvImporter.Import(path, null);

            Assert.Equal(2, result.Transponders.Count);
            Assert.Equal("E077665544332211", result.Transponders[0].TransponderId);
            Assert.Equal("E000FFEEDDCCBBAA", result.Transponders[1].TransponderId);
        }

        [Fact]
        public void Import_Iso15693MsbFirst_DoesNotReverse()
        {
            var csv = "uid\nE077665544332211\nE000FFEEDDCCBBAA\n";
            var path = WriteTempCsv(csv);

            var result = CsvImporter.Import(path, null);

            Assert.Equal(2, result.Transponders.Count);
            Assert.Equal("E077665544332211", result.Transponders[0].TransponderId);
            Assert.Equal("E000FFEEDDCCBBAA", result.Transponders[1].TransponderId);
        }

        [Fact]
        public void Import_Nxp14443aLsbFirst_ReversesUids()
        {
            var csv = "uid\nAABBCCDDEEFF04\n11223344556604\n";
            var path = WriteTempCsv(csv);

            var result = CsvImporter.Import(path, null);

            Assert.Equal(2, result.Transponders.Count);
            Assert.Equal("04FFEEDDCCBBAA", result.Transponders[0].TransponderId);
            Assert.Equal("04665544332211", result.Transponders[1].TransponderId);
        }

        [Fact]
        public void Import_Nxp14443aMsbFirst_DoesNotReverse()
        {
            var csv = "uid\n04FFEEDDCCBBAA\n04665544332211\n";
            var path = WriteTempCsv(csv);

            var result = CsvImporter.Import(path, null);

            Assert.Equal(2, result.Transponders.Count);
            Assert.Equal("04FFEEDDCCBBAA", result.Transponders[0].TransponderId);
            Assert.Equal("04665544332211", result.Transponders[1].TransponderId);
        }

        [Fact]
        public void Import_MixedUidLengths_DoesNotReverse()
        {
            var csv = "uid\n04A1B2C3D4E5F6\n0102030405060708\n";
            var path = WriteTempCsv(csv);

            var result = CsvImporter.Import(path, null);

            Assert.Equal(2, result.Transponders.Count);
            Assert.Equal("04A1B2C3D4E5F6", result.Transponders[0].TransponderId);
            Assert.Equal("0102030405060708", result.Transponders[1].TransponderId);
        }

        private string WriteTempCsv(string content)
        {
            var path = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid():N}.csv");
            File.WriteAllText(path, content);
            _tempFiles.Add(path);
            return path;
        }
    }
}
