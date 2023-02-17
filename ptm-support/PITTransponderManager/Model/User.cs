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

using System.Text.Json.Serialization;

namespace Pilz.PITreader.PitTransponderManager.Model
{
    /// <summary>
    /// A transponder holder profile (a person assigned one or more transponders).
    /// Not to be confused with PITreader device users (Guest / Transponder manager / Device manager / Administrator).
    /// MLP exports include only the base fields (Email, FirstName, Surname, UserID, Role);
    /// PTM exports include all fields.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Email address of the transponder holder. Primary key for transponder-to-user mapping.
        /// In MLP, must be a valid email; in PTM, can be any string. Can be empty.
        /// </summary>
        [JsonPropertyName("email")]
        public string? Email { get; set; }

        /// <summary>
        /// First name.
        /// </summary>
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Last name.
        /// </summary>
        [JsonPropertyName("surname")]
        public string Surname { get; set; } = string.Empty;

        /// <summary>
        /// Unique user identifier. In MLP: opaque ID used as transponder-to-user mapping key.
        /// In PTM: typically empty or <c>null</c>.
        /// </summary>
        [JsonPropertyName("userID")]
        public string? UserID { get; set; }

        /// <summary>
        /// Whether this transponder holder account is active. PTM-specific field.
        /// </summary>
        [JsonPropertyName("active")]
        public bool Active { get; set; }

        /// <summary>
        /// Holder validity start date (format: YYYY-M-D, non-zero-padded), or empty string if unset.
        /// PTM-specific field.
        /// </summary>
        [JsonPropertyName("startDate")]
        public string? StartDate { get; set; }

        /// <summary>
        /// Holder validity end date (format: YYYY-M-D, non-zero-padded), or empty string if unset.
        /// PTM-specific field.
        /// </summary>
        [JsonPropertyName("endDate")]
        public string? EndDate { get; set; }

        /// <summary>
        /// Role identifier, or <c>null</c>.
        /// In PTM: human-readable role string. In MLP: opaque ID string.
        /// Not related to PITreader device user roles.
        /// </summary>
        [JsonPropertyName("role")]
        public string? Role { get; set; }

        /// <summary>
        /// Free-text description, or <c>null</c>. PTM-specific field.
        /// </summary>
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        /// <summary>
        /// Phone number with country code (e.g., "+49 123 456 7890"), or <c>null</c>. PTM-specific field.
        /// </summary>
        [JsonPropertyName("phoneNumber")]
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Company or organization name, or <c>null</c>. PTM-specific field.
        /// </summary>
        [JsonPropertyName("company")]
        public string? Company { get; set; }

        /// <summary>
        /// Profile image (PNG), or <c>null</c>. Serialized as Base64 in JSON. PTM-specific field.
        /// </summary>
        [JsonPropertyName("image")]
        public byte[]? Image { get; set; }
    }
}
