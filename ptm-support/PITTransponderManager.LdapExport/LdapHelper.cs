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

﻿using System.DirectoryServices.Protocols;
using System.Text.RegularExpressions;

namespace Pilz.PITreader.PitTransponderManager.LdapExport
{
    internal static class LdapHelper
    {
        public static string? GetStringValue(this SearchResultAttributeCollection? attributes, string name)
        {
            if (attributes == null || !attributes.Contains(name))
                return null;

            return attributes[name][0].ToString();
        }

        public static string? GetDateStringValue(this SearchResultAttributeCollection? attributes, string name)
        {
            string? value = attributes.GetStringValue(name);

            if (value != null && Regex.IsMatch(value, @"^\d{14}\.\dZ$"))
            {
                return value.Substring(0, 4) + "-" + value.Substring(4, 2) + "-" + value.Substring(6, 2);
            }

            return value;
        }
    }
}
