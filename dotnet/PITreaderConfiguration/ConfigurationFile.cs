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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SharpCompress.Readers;
using SharpCompress.Writers;

namespace Pilz.PITreader.Configuration
{
    public class ConfigurationFile : IEnumerable
    {
        private List<IConfigurationComponent> components = new List<IConfigurationComponent>();

        public ConfigurationFile()
        {
        }

        public void Read(Stream stream)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            IConfigurationComponent comp;

            // Check for old file type with just settings
            if (stream.CanSeek)
            {
                long position = stream.Position;
                byte[] header = new byte[SettingsFile.Header.Length];
                stream.Read(header, 0, header.Length);
                stream.Position = position;

                if (header.SequenceEqual(Encoding.ASCII.GetBytes(SettingsFile.Header)))
                {
                    comp = new SettingsFile();
                    comp.Read(stream);
                    this.components.Add(comp);
                    return;
                }
            }

            // Read tar
            var reader = ReaderFactory.Open(stream);
            while (reader.MoveToNextEntry())
            {
                if (!reader.Entry.IsDirectory)
                {
                    using (var ms = new MemoryStream())
                    {
                        reader.WriteEntryTo(ms);

                        string data = Encoding.UTF8.GetString(ms.ToArray());
                        ms.Position = 0;

                        switch (reader.Entry.Key)
                        {
                            case SettingsFile.FileName:
                                comp = new SettingsFile();
                                break;
                            case DeviceUserFile.FileName:
                                comp = new DeviceUserFile();
                                break;
                            case OpcUaClientCertificateFile.FileName:
                                comp = new OpcUaClientCertificateFile();
                                break;
                            default:
                                comp = new GenericFile(reader.Entry.Key);
                                break;
                        }

                        comp.Read(ms);
                        this.components.Add(comp);
                    }
                }
            }
        }

        public void Add(IConfigurationComponent component)
        {
            if (component is null)
            {
                throw new ArgumentNullException(nameof(component));
            }

            this.components.Add(component);
        }

        public IConfigurationComponent Get(string key)
        {
            return this.components.SingleOrDefault(c => c.Key == key);
        }

        public TComponent Get<TComponent>(string key = null) where TComponent : IConfigurationComponent
        {
            return this.components.OfType<TComponent>()
                .Where(c => key == null || c.Key == key)
                .FirstOrDefault();
        }

        public void Write(Stream stream)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            using (var writer = WriterFactory.Open(
                stream, 
                SharpCompress.Common.ArchiveType.Tar, 
                new WriterOptions(SharpCompress.Common.CompressionType.None)))
            {
                foreach (var component in this.components)
                {
                    using (var ms = new MemoryStream())
                    {
                        component.Write(ms);
                        ms.Position = 0;

                        writer.Write(component.Key, ms);
                    }
                }
            }
        }

        /// <summary>
        /// Parameters for API 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetRequstParameters()
        {
            foreach (var component in this.components)
            {
                switch (component.Key)
                {
                    case SettingsFile.FileName: yield return "deviceConfig"; break;
                    case DeviceUserFile.FileName: yield return "deviceUser"; break;
                    case "udc.csv": yield return "userDataConfig"; break;
                    case "bl.csv": yield return "transponderBlocklist"; break;
                    case "dgn.csv": yield return "deviceGroupNames"; break;
                    case "xpndrdb.csv": yield return "permissionList"; break;
                    case OpcUaClientCertificateFile.FileName: yield return "opcuaClients"; break;
                }
            }
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.components.GetEnumerator();
        }
    }
}
