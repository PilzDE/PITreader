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

using System.IO;

namespace Pilz.PITreader.Configuration
{
    /// <summary>
    /// Component of backup/restore file
    /// </summary>
    public interface IConfigurationComponent
    {
        /// <summary>
        /// Component key (i.e. file name)
        /// </summary>
        string Key { get; }

        /// <summary>
        /// Component type
        /// </summary>
        ConfigurationType Component { get; }

        /// <summary>
        /// Read component from binary stream.
        /// </summary>
        /// <param name="stream">Binary stream from .config file.</param>
        void Read(Stream stream);

        /// <summary>
        /// Write component to binary stream.
        /// </summary>
        /// <param name="stream">Destination stream (i.e. .config file)</param>
        void Write(Stream stream);
    }
}