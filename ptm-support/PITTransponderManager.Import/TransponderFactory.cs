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
    /// Creates <see cref="Transponder"/> objects from CSV row data, computing security IDs.
    /// </summary>
    internal static class TransponderFactory
    {
        /// <summary>
        /// Creates a transponder from CSV row data with computed security ID.
        /// </summary>
        /// <param name="uid">Transponder UID bytes (already in correct byte order).</param>
        /// <param name="row">CSV row data as column-name to value dictionary.</param>
        /// <returns>A fully populated <see cref="Transponder"/> object.</returns>
        public static Transponder Create(byte[] uid, Dictionary<string, string> row)
        {
            return new Transponder
            {
                SecureId = TransponderId.Generate(uid),
                TransponderId = Convert.ToHexString(uid),
                StartDate = row.GetValueOrDefault("startDate", ""),
                EndDate = row.GetValueOrDefault("endDate", ""),
                User = string.Empty,
                Type = Transponder.TypeThirdParty,
            };
        }
    }
}
