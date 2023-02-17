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

using Pilz.PITreader.PitTransponderManager.Model;

namespace Pilz.PITreader.PitTransponderManager.Import
{
    /// <summary>
    /// Imports transponders from a CSV file into a <see cref="PtmExportContainer"/>,
    /// computing security IDs and optionally merging with existing data.
    /// </summary>
    internal static class CsvImporter
    {
        /// <summary>
        /// Imports transponders from a CSV file.
        /// </summary>
        /// <param name="csvPath">Path to the CSV file.</param>
        /// <param name="inputData">Optional existing PTM data to merge with.</param>
        /// <returns>A populated <see cref="PtmExportContainer"/>.</returns>
        public static PtmExportContainer Import(string csvPath, PtmExportContainer? inputData)
        {
            var rows = CsvReader.Read(csvPath);

            if (rows.Count == 0)
            {
                Console.WriteLine("Warning: CSV file is empty.");
                return inputData ?? new PtmExportContainer();
            }

            // Validate that uid column exists
            if (!rows[0].ContainsKey("uid"))
            {
                Console.Error.WriteLine("Error: CSV must have a 'uid' column.");
                Environment.Exit(1);
            }

            // Parse all UIDs first for byte-order detection
            var uidStrings = rows
                .Select(r => r.GetValueOrDefault("uid", ""))
                .Where(u => !string.IsNullOrWhiteSpace(u))
                .ToList();

            bool reverseUids = DetectAndReportByteOrder(uidStrings);

            // Build output container from input data or fresh
            var container = new PtmExportContainer();
            if (inputData != null)
            {
                container.Version.Clear();
                container.Version.AddRange(inputData.Version);
                container.Groups.AddRange(inputData.Groups);
                container.Permissions.AddRange(inputData.Permissions);
                container.Users.AddRange(inputData.Users);
                container.Blocklists.AddRange(inputData.Blocklists);
                container.Templates.AddRange(inputData.Templates);
                container.UserDefinedDataFields.AddRange(inputData.UserDefinedDataFields);
                container.Transponders.AddRange(inputData.Transponders);
            }

            // Build lookup of existing transponders by transponderId (UID)
            var existingByUid = container.Transponders
                .Where(t => !string.IsNullOrEmpty(t.TransponderId))
                .ToDictionary(t => t.TransponderId!.ToUpperInvariant(), t => t);

            int added = 0, updated = 0, skipped = 0;

            foreach (var row in rows)
            {
                var uidHex = row.GetValueOrDefault("uid", "");
                if (string.IsNullOrWhiteSpace(uidHex))
                {
                    skipped++;
                    continue;
                }

                byte[] uid;
                try
                {
                    uid = Convert.FromHexString(uidHex.Trim());
                }
                catch (FormatException)
                {
                    Console.Error.WriteLine($"Warning: Invalid UID hex '{uidHex}', skipping row.");
                    skipped++;
                    continue;
                }

                if (uid.Length < TransponderId.TagUidSizeMin || uid.Length > TransponderId.TagUidSizeMax)
                {
                    Console.Error.WriteLine($"Warning: UID '{uidHex}' has invalid length ({uid.Length} bytes), skipping row.");
                    skipped++;
                    continue;
                }

                if (reverseUids)
                    Array.Reverse(uid);

                var uidKey = Convert.ToHexString(uid);

                if (existingByUid.TryGetValue(uidKey, out var existing))
                {
                    if (!string.IsNullOrEmpty(existing.SecureId))
                    {
                        skipped++;
                        continue;
                    }

                    // Update empty secureId and fill in missing optional fields
                    var newTransponder = TransponderFactory.Create(uid, row);
                    existing.SecureId = newTransponder.SecureId;

                    if (string.IsNullOrEmpty(existing.SerialNo))
                        existing.SerialNo = newTransponder.SerialNo;
                    if (string.IsNullOrEmpty(existing.OrderNo))
                        existing.OrderNo = newTransponder.OrderNo;
                    if (string.IsNullOrEmpty(existing.StartDate))
                        existing.StartDate = newTransponder.StartDate;
                    if (string.IsNullOrEmpty(existing.EndDate))
                        existing.EndDate = newTransponder.EndDate;
                    if (string.IsNullOrEmpty(existing.Type))
                        existing.Type = newTransponder.Type;

                    updated++;
                }
                else
                {
                    var transponder = TransponderFactory.Create(uid, row);
                    container.Transponders.Add(transponder);
                    existingByUid[uidKey] = transponder;
                    added++;
                }
            }

            Console.WriteLine($"Result: {added} added, {updated} updated, {skipped} skipped.");
            return container;
        }

        /// <summary>
        /// Detects whether UIDs are in LSB-first byte order and need reversal.
        /// </summary>
        /// <returns><c>true</c> if UIDs should be reversed.</returns>
        private static bool DetectAndReportByteOrder(List<string> uidHexStrings)
        {
            if (uidHexStrings.Count == 0)
                return false;

            var uids = new List<byte[]>();
            foreach (var hex in uidHexStrings)
            {
                try
                {
                    uids.Add(Convert.FromHexString(hex.Trim()));
                }
                catch (FormatException)
                {
                    return false; // Can't determine, don't reverse
                }
            }

            // ISO 15693 LSB-first: all 8 bytes, all end with 0xE0 (but not already starting with 0xE0)
            if (uids.All(u => u.Length == 8) && uids.All(u => u[^1] == 0xE0) && !uids.All(u => u[0] == 0xE0))
            {
                Console.WriteLine("Detected ISO 15693 LSB-first byte order, reversing UIDs.");
                return true;
            }

            // NXP ISO 14443-A LSB-first: all 7 bytes, all end with 0x04 (but not already starting with 0x04)
            if (uids.All(u => u.Length == 7) && uids.All(u => u[^1] == 0x04) && !uids.All(u => u[0] == 0x04))
            {
                Console.WriteLine("Detected NXP ISO 14443-A LSB-first byte order, reversing UIDs.");
                return true;
            }

            return false;
        }
    }
}
