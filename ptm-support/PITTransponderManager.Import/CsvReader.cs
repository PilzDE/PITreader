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

namespace Pilz.PITreader.PitTransponderManager.Import
{
    /// <summary>
    /// Reads CSV files with auto-detected separator and Excel-compatible quoting.
    /// </summary>
    internal static class CsvReader
    {
        /// <summary>
        /// Reads a CSV file and returns rows as column-name to value dictionaries.
        /// </summary>
        /// <param name="path">Path to the CSV file.</param>
        /// <returns>List of rows, each a dictionary mapping column name to value.</returns>
        public static List<Dictionary<string, string>> Read(string path)
        {
            var lines = File.ReadAllLines(path, System.Text.Encoding.UTF8);
            if (lines.Length == 0)
                return [];

            // Strip UTF-8 BOM if present
            if (lines[0].Length > 0 && lines[0][0] == '\uFEFF')
                lines[0] = lines[0][1..];

            var separator = DetectSeparator(lines[0]);
            var headers = ParseLine(lines[0], separator)
                .Select(h => h.Trim())
                .ToArray();

            var rows = new List<Dictionary<string, string>>();
            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                if (string.IsNullOrEmpty(line))
                    continue;

                var values = ParseLine(line, separator);
                var row = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                for (int j = 0; j < headers.Length; j++)
                {
                    row[headers[j]] = j < values.Count ? values[j].Trim() : string.Empty;
                }

                rows.Add(row);
            }

            return rows;
        }

        /// <summary>
        /// Detects whether the separator is ',' or ';' by counting occurrences in the header row.
        /// </summary>
        private static char DetectSeparator(string headerLine)
        {
            int commas = CountUnquoted(headerLine, ',');
            int semicolons = CountUnquoted(headerLine, ';');
            return semicolons >= commas ? ';' : ',';
        }

        /// <summary>
        /// Counts occurrences of a character outside of quoted regions.
        /// </summary>
        private static int CountUnquoted(string line, char target)
        {
            int count = 0;
            bool inQuotes = false;
            foreach (char c in line)
            {
                if (c == '"')
                    inQuotes = !inQuotes;
                else if (c == target && !inQuotes)
                    count++;
            }
            return count;
        }

        /// <summary>
        /// Parses a CSV line respecting quoted fields (RFC 4180).
        /// </summary>
        private static List<string> ParseLine(string line, char separator)
        {
            var fields = new List<string>();
            int i = 0;

            while (i <= line.Length)
            {
                if (i == line.Length)
                {
                    fields.Add(string.Empty);
                    break;
                }

                if (line[i] == '"')
                {
                    // Quoted field
                    var sb = new System.Text.StringBuilder();
                    i++; // skip opening quote
                    while (i < line.Length)
                    {
                        if (line[i] == '"')
                        {
                            if (i + 1 < line.Length && line[i + 1] == '"')
                            {
                                sb.Append('"');
                                i += 2;
                            }
                            else
                            {
                                i++; // skip closing quote
                                break;
                            }
                        }
                        else
                        {
                            sb.Append(line[i]);
                            i++;
                        }
                    }

                    fields.Add(sb.ToString());

                    // Skip separator after closing quote
                    if (i < line.Length && line[i] == separator)
                        i++;
                }
                else
                {
                    // Unquoted field
                    int start = i;
                    while (i < line.Length && line[i] != separator)
                        i++;

                    fields.Add(line[start..i]);

                    if (i < line.Length)
                        i++; // skip separator
                }
            }

            return fields;
        }
    }
}
