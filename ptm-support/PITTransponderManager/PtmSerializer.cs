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

using System;
using System.IO;
using System.Text;
using System.Text.Json;
using Pilz.PITreader.PitTransponderManager.Model;

namespace Pilz.PITreader.PitTransponderManager
{
    /// <summary>
    /// Serializer for loading and saving PTM export files.
    /// </summary>
    public static class PtmSerializer
    {
        /// <summary>
        /// Loads PTM export file to data model.
        /// </summary>
        /// <param name="path">Full name of PTM export file to load.</param>
        /// <returns>The data model of a PTM export file.</returns>
        /// <exception cref="ArgumentNullException">path parameter is null</exception>
        /// <exception cref="ArgumentException">path parameter is invalid (e.g. file does not exist)</exception>
        /// <exception cref="InvalidOperationException">Unable to read data from file</exception>
        public static PtmExportContainer Load(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            if (!File.Exists(path))
                throw new ArgumentException("Path does not exist.", nameof(path));

            try
            {
                var content = File.ReadAllText(path, Encoding.UTF8);
                return JsonSerializer.Deserialize(content, PtmJsonContext.Default.PtmExportContainer)
                    ?? throw new InvalidOperationException("Deserialization returned null.");
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException("Unable to read data from file.", exception);
            }
        }

        /// <summary>
        /// Saves PTM model to file.
        /// </summary>
        /// <param name="container">The PTM model.</param>
        /// <param name="path">Full name of file.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">Unable to save data to file.</exception>
        public static void Save(PtmExportContainer container, string path)
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));

            if (path == null)
                throw new ArgumentNullException(nameof(path));

            try
            {
                var outputString = JsonSerializer.Serialize(container, PtmJsonContext.Default.PtmExportContainer);
                using (var writer = File.Open(path, FileMode.Create))
                {
                    var data = Encoding.UTF8.GetBytes(outputString);
                    writer.Write(data, 0, data.Length);
                    writer.Flush();
                }
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException("Unable to save data to file.", exception);
            }
        }
    }
}
