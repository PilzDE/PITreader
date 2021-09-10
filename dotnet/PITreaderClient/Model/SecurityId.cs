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
        /// Create a security id from specified string of hexadecimal values.
        /// </summary>
        /// <param name="hexString">String of hexadecimal values.</param>
        /// <exception cref="System.ArgumentNullException">Empty of null value provided as hex string</exception>
        /// <exception cref="System.ArgumentException">Invalid hexadecimal string</exception>
        public SecurityId(string hexString)
        {
            if (string.IsNullOrEmpty(hexString)) throw new ArgumentNullException(nameof(hexString));
            if (hexString.Length != 16 || Regex.IsMatch(hexString, "[^A-Fa-f0-9]")) throw new ArgumentException("Invalid format of hexString", nameof(hexString));

            this.HexString = hexString.ToUpperInvariant();
            this.Value = ulong.Parse(this.HexString, NumberStyles.HexNumber);
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
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">Empty of null value provided as hex string</exception>
        /// <exception cref="System.ArgumentException">Invalid hexadecimal string</exception>
        public static SecurityId Parse(string value)
        {
            if (string.IsNullOrEmpty(value) || value == new string('0', 16)) return null;
            return new SecurityId(value);
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
