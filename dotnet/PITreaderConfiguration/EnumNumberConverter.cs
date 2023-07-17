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
using System.Reflection;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using CsvHelper;

namespace Pilz.PITreader.Configuration
{
    /// <summary>
	/// Converts an <see cref="Enum"/> to and from a <see cref="string"/> with representation as a number.
	/// </summary>
    internal class EnumNumberConverter : DefaultTypeConverter
    {
        private readonly Type type;

        /// <summary>
        /// Creates a new <see cref="EnumNumberConverter"/> for the given <see cref="Enum"/> <see cref="System.Type"/>.
        /// </summary>
        /// <param name="type">The type of the Enum.</param>
        public EnumNumberConverter(Type type)
        {
            if (!typeof(Enum).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
            {
                throw new ArgumentException($"'{type.FullName}' is not an Enum.");
            }

            this.type = type;
        }

        /// <inheritdoc/>
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            return Enum.Parse(type, text);
        }

        /// <inheritdoc/>
        public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            return base.ConvertToString((int)value, row, memberMapData);
        }
    }
}
