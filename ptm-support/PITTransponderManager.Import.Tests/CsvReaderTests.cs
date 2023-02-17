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
    public class CsvReaderTests : IDisposable
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
        public void Read_CommaSeparated_ParsesCorrectly()
        {
            var csv = "uid,colA,colB\nAABB,12345,hello\n";
            var path = WriteTempCsv(csv);

            var rows = CsvReader.Read(path);

            Assert.Single(rows);
            Assert.Equal("AABB", rows[0]["uid"]);
            Assert.Equal("12345", rows[0]["colA"]);
            Assert.Equal("hello", rows[0]["colB"]);
        }

        [Fact]
        public void Read_SemicolonSeparated_ParsesCorrectly()
        {
            var csv = "uid;colA;colB\nAABB;12345;hello\n";
            var path = WriteTempCsv(csv);

            var rows = CsvReader.Read(path);

            Assert.Single(rows);
            Assert.Equal("AABB", rows[0]["uid"]);
            Assert.Equal("12345", rows[0]["colA"]);
        }

        [Fact]
        public void Read_QuotedFieldsWithSeparator_ParsesCorrectly()
        {
            var csv = "name;value\n\"hello;world\";42\n";
            var path = WriteTempCsv(csv);

            var rows = CsvReader.Read(path);

            Assert.Single(rows);
            Assert.Equal("hello;world", rows[0]["name"]);
            Assert.Equal("42", rows[0]["value"]);
        }

        [Fact]
        public void Read_QuotedFieldsWithEscapedQuotes_ParsesCorrectly()
        {
            var csv = "name;value\n\"say \"\"hello\"\"\";42\n";
            var path = WriteTempCsv(csv);

            var rows = CsvReader.Read(path);

            Assert.Single(rows);
            Assert.Equal("say \"hello\"", rows[0]["name"]);
        }

        [Fact]
        public void Read_Utf8Bom_StrippedFromHeader()
        {
            var csv = "\uFEFFuid;value\nAABB;42\n";
            var path = WriteTempCsv(csv);

            var rows = CsvReader.Read(path);

            Assert.Single(rows);
            Assert.True(rows[0].ContainsKey("uid"));
            Assert.Equal("AABB", rows[0]["uid"]);
        }

        [Fact]
        public void Read_EmptyLinesSkipped()
        {
            var csv = "uid;value\nAABB;1\n\n\nCCDD;2\n";
            var path = WriteTempCsv(csv);

            var rows = CsvReader.Read(path);

            Assert.Equal(2, rows.Count);
            Assert.Equal("AABB", rows[0]["uid"]);
            Assert.Equal("CCDD", rows[1]["uid"]);
        }

        [Fact]
        public void Read_ColumnLookupIsCaseInsensitive()
        {
            var csv = "UID;SerialNo\nAABB;12345\n";
            var path = WriteTempCsv(csv);

            var rows = CsvReader.Read(path);

            Assert.Equal("AABB", rows[0]["uid"]);
            Assert.Equal("12345", rows[0]["serialno"]);
        }

        [Fact]
        public void Read_WhitespaceTrimmed()
        {
            var csv = "uid ; value \n  AABB  ;  42  \n";
            var path = WriteTempCsv(csv);

            var rows = CsvReader.Read(path);

            Assert.Single(rows);
            Assert.Equal("AABB", rows[0]["uid"]);
            Assert.Equal("42", rows[0]["value"]);
        }

        [Fact]
        public void Read_EmptyFile_ReturnsEmptyList()
        {
            var path = WriteTempCsv("");

            var rows = CsvReader.Read(path);

            Assert.Empty(rows);
        }

        private string WriteTempCsv(string content)
        {
            var path = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid():N}.csv");
            File.WriteAllText(path, content, System.Text.Encoding.UTF8);
            _tempFiles.Add(path);
            return path;
        }
    }
}
