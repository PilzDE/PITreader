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
using System.Linq;
using System.IO;

namespace Pilz.PITreader.Client
{
    /// <summary>
    /// Wrapper for firmware update packages
    /// </summary>
    public class PITreaderFirmwarePackage
    {
        /// <summary>
        /// The file name of the update file
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// The package version (major, minor and patch)
        /// </summary>
        public FirmwareVersion PackageVersion { get; private set; }

        /// <summary>
        /// The build number of the update file
        /// </summary>
        public UInt32 PackageBuild { get; private set; }

        /// <summary>
        /// The firmware update data.
        /// </summary>
        public Stream Data { get; private set; } = new MemoryStream();

        /// <summary>
        /// Creates a wrapper for a firmware update package.
        /// </summary>
        /// <param name="filePath">Path to the firmware update package (*.fwu or *.zip)</param>
        public PITreaderFirmwarePackage(string filePath)
        {
            string fileName = Path.GetFileName(filePath);
            this.Load(filePath, ref fileName);
            this.FileName = fileName;
        }

        private void Load(string filePath, ref string fileName)
        {
            using (var fs = File.OpenRead(filePath))
            {
                if (!CheckAndUnpack(fs, this.Data, ref fileName))
                {
                    throw new InvalidDataException();
                }
            }

            this.Data.Position = 16;

            // Parse/skip certificate
            byte[] tempBuffer = new byte[8];
            this.Data.Read(tempBuffer, 0, 8);
            uint certLength = BitConverter.ToUInt32(tempBuffer, 4);
            this.Data.Position += certLength;

            // Parse/skip signature
            this.Data.Read(tempBuffer, 0, 8);
            uint signLength = BitConverter.ToUInt32(tempBuffer, 4);
            this.Data.Position += signLength;

            byte[] version = new byte[4 * 4];
            this.Data.Read(version, 0, version.Length);

            int major, minor, patch;
            major = BitConverter.ToInt32(version, 0);
            minor = BitConverter.ToInt32(version, 4);
            patch = BitConverter.ToInt32(version, 8);
            uint build = BitConverter.ToUInt32(version, 12);
            this.PackageVersion = new FirmwareVersion(major, minor, patch, build);

            // Rewind
            this.Data.Position = 0;
        }

        private static bool CheckAndUnpack(Stream source, Stream destination, ref string fileName)
        {
            if (source.Length < 48)
            {
                throw new InvalidDataException();
            }

            // PILZ_FW_402
            byte[] expectedMagic = { 0x50, 0x49, 0x4C, 0x5A, 0x5F, 0x46, 0x57, 0x5F, 0x34, 0x30, 0x32, 0x00 };
            Predicate<Stream> checkPackage = stream =>
            {
                stream.Position = 0;
                byte[] actualMagic = new byte[12];
                stream.Read(actualMagic, 0, actualMagic.Length);
                stream.Position = 0;

                return expectedMagic.SequenceEqual(actualMagic);
            };

            if (checkPackage(source))
            {
                source.CopyTo(destination);
                return true;
            }

            try
            {
                using (var zip = new System.IO.Compression.ZipArchive(source))
                {
                    foreach (var entry in zip.Entries.Where(f => f.Name.EndsWith(".fwu")))
                    {
                        using (var zipStream = entry.Open())
                        {
                            using (var tempMemoryStream = new MemoryStream())
                            {
                                zipStream.CopyTo(tempMemoryStream);

                                if (checkPackage(tempMemoryStream))
                                {
                                    tempMemoryStream.CopyTo(destination);
                                    fileName = entry.Name;
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            catch (InvalidDataException)
            {
                // pass
            }

            return false;
        }
    }
}
