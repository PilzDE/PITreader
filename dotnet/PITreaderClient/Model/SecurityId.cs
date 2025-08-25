// Copyright (c) 2022 Pilz GmbH & Co. KG
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
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Pilz.PITreader.Client.Model
{
    /// <summary>
    /// Security ID of a PITreader transponder.
    /// </summary>
    [DebuggerDisplay("{HexString}")]
    public class SecurityId : IEquatable<SecurityId>
    {
        /// <summary>
        /// Alternative representation of an "empty" security id (all zeros).
        /// </summary>
        public static readonly string EmptySecurityIdString = new string('0', 16);

        /// <summary>
        /// Create a security id from specified string of hexadecimal values.
        /// </summary>
        /// <param name="hexString">String of hexadecimal values.</param>
        /// <exception cref="System.ArgumentNullException">Empty of null value provided as hex string</exception>
        /// <exception cref="System.ArgumentException">Invalid hexadecimal string</exception>
        public SecurityId(string hexString)
        {
            if (string.IsNullOrEmpty(hexString))
            {
                throw new ArgumentNullException(nameof(hexString));
            }

            this.HexString = hexString.ToUpperInvariant();

            if (TryParseInternal(hexString, out ulong value))
            {
                this.Value = value;
            }
            else
            {
                throw new ArgumentException("Invalid format of hexString", nameof(hexString));
            }
        }

        private SecurityId(string hexString, ulong value)
        {
            if (string.IsNullOrEmpty(hexString))
            {
                throw new ArgumentNullException(nameof(hexString));
            }

            this.HexString = hexString.ToUpperInvariant();
            this.Value = value;
        }

        /// <summary>
        /// String of 8 hexadecimal numbers (16 characters) representing the Security ID.
        /// </summary>
        public string HexString { get; }

        /// <summary>
        /// Numeric value (64 bit unsigned integer) of Security ID
        /// </summary>
        public ulong Value { get; }

        /// <summary>
        /// Creates a Security ID based on the provided value.
        /// </summary>
        /// <param name="value">String of hexadecimal values.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">Empty of null value provided as hex string</exception>
        /// <exception cref="System.ArgumentException">Invalid hexadecimal string</exception>
        public static SecurityId Parse(string value)
        {
            if (string.IsNullOrEmpty(value) || value == EmptySecurityIdString) return null;
            return new SecurityId(value);
        }

        /// <summary>
        /// Tries to parse the provided hex string into a Security ID.
        /// An empty string of a string with all zeros results in a null value.
        /// </summary>
        /// <param name="value">String of hexadecimal values.</param>
        /// <param name="securityId">Set to the security id or null.</param>
        /// <returns>True, if input represents a valid security id (empty or zeros is parsed to null with result true).</returns>
        public static bool TryParse(string value, out SecurityId securityId)
        {
            if (string.IsNullOrEmpty(value) || value == EmptySecurityIdString)
            {
                securityId = null;
                return true;
            }

            if (TryParseInternal(value, out ulong parsed))
            {
                securityId = new SecurityId(value, parsed);
                return true;
            }

            securityId = null;
            return false;
        }

        private static bool TryParseInternal(string input, out ulong value)
        {
            if (string.IsNullOrEmpty(input))
            {
                value = 0;
                return false;
            }

            if (input.Length != 16 || Regex.IsMatch(input, "[^A-Fa-f0-9]"))
            {
                value = 0;
                return false;
            }

            return ulong.TryParse(input, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out value);
        }

        /// <summary>
        /// Converts a Security ID to a string.
        /// </summary>
        /// <param name="id"></param>
        public static implicit operator string(SecurityId id) => id?.HexString;

        /// <summary>
        /// Converts a string to a Security ID.
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator SecurityId(string value) => Parse(value);

        /// <summary>
        /// Compares a Security ID with a string representing a Security ID and checks for equivalent values.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool operator ==(SecurityId id, string s) => (Object.Equals(null, id) ? string.IsNullOrEmpty(s) : string.Equals(id.HexString, s, StringComparison.InvariantCultureIgnoreCase));

        /// <summary>
        /// Compares a Security ID with a string representing a Security ID and checks for non-equivalent values.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool operator !=(SecurityId id, string s) => (Object.Equals(null, id) ? !string.IsNullOrEmpty(s) : !string.Equals(id.HexString, s, StringComparison.InvariantCultureIgnoreCase));

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return this.HexString;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as SecurityId);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public bool Equals(SecurityId other)
        {
            return other != null && this.Value == other.Value;
        }

        /// <summary>
        ///  Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return -1937169414 + Value.GetHashCode();
        }
    }
}
