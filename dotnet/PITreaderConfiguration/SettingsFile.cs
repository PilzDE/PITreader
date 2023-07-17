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

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Pilz.PITreader.Configuration.Model;

namespace Pilz.PITreader.Configuration
{
    public class SettingsFile : IConfigurationComponent
    {
        public const string FileName = "config.txt";

        internal const string Header = "PITR_CFG";

        private const string Version = "V=01";

        public string Key { get; } = FileName;

        public ConfigurationType Component { get; } = ConfigurationType.Settings;

        public List<SettingsParameter> Parameter { get; } = new List<SettingsParameter>();

        public void Read(Stream stream)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            var list = new List<SettingsParameter>();

            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (line == Header || line == Version)
                    {
                        continue;
                    }

                    string[] parts = line.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray();
                    if (parts.Length != 3)
                    {
                        throw new InvalidDataException("Invalid settings line (invalid part count): " + line);
                    }

                    int id;
                    int size;
                    byte[] data;

                    if (!int.TryParse(parts[0].Substring(2).TrimStart('0'), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out id)
                        || !int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out size))
                    {
                        throw new InvalidDataException("Invalid settings line (format error): " + line);
                    }

                    string dataString = parts[2];
                    if (!dataString.StartsWith("0x") || (dataString.Length % 2) != 0)
                    {
                        throw new InvalidDataException("Invalid settings line (format error): " + line);
                    }

                    data = new byte[(dataString.Length - 2) / 2];
                    for (int index = 2; index < dataString.Length; index += 2)
                    {
                        data[(index - 2) / 2] = Convert.ToByte(dataString.Substring(index, 2), 16);
                    }

                    list.Add(new SettingsParameter
                    {
                        Id = id,
                        Size = size,
                        Data = data
                    });
                }
            }

            this.Parameter.Clear();
            this.Parameter.AddRange(list);
        }

        public void Write(Stream stream)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            using (var writer = new StreamWriter(stream, Encoding.ASCII, 1024, true))
            {
                writer.WriteLine(Header);
                writer.WriteLine(Version);
                foreach (var item in this.Parameter)
                {
                    writer.WriteLine(item.Format());
                }
            }
        }
    }
}
