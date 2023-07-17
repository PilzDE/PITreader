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
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using Pilz.PITreader.Configuration.Model;

namespace Pilz.PITreader.Configuration
{
    public class DeviceUserFile : IConfigurationComponent
    {
        public const string FileName = "user.csv";

        public string Key => FileName;

        public ConfigurationType Component { get; } = ConfigurationType.DeviceUsers;

        public List<DeviceUser> Users { get; } = new List<DeviceUser>();

        public DeviceUserFile()
        {
        }

        public DeviceUserFile(IEnumerable<DeviceUser> users)
        {
            this.Users.AddRange(users);
        }

        public void Read(Stream stream)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            try
            {
                using (var reader = new StreamReader(stream))
                {
                    using (var csv = new CsvReader(reader, PITreaderCsvConfiguration.Configuration))
                    {
                        csv.Context.RegisterClassMap<DeviceUserMap>();

                        csv.Read();
                        try
                        {
                            csv.ReadHeader();
                        }
                        catch
                        {
                            //
                        }

                        var users = csv.GetRecords<DeviceUser>().ToList();

                        this.Users.Clear();
                        this.Users.AddRange(users);
                    }
                }
            }
            catch (Exception exception)
            {
                throw new InvalidDataException("Invalid data", exception);
            }
        }

        public void Write(Stream stream)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            try
            {
                using (var writer = new StreamWriter(stream, Encoding.UTF8, 1024, true))
                {
                    using (var csv = new CsvWriter(writer, PITreaderCsvConfiguration.Configuration))
                    {
                        csv.Context.RegisterClassMap<DeviceUserMap>();
                        csv.WriteRecords(this.Users);
                    }
                }
            }
            catch (Exception exception)
            {
                throw new InvalidDataException("Invalid data", exception);
            }
        }
    }
}
