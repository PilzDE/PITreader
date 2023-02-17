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

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Pilz.PITreader.PitTransponderManager.Model
{
    /// <summary>
    /// Root container for a PTM (.ptm) export file. Produced and consumed by
    /// PIT Transponder Manager (PTM), Myzel Lifecycle Platform (MLP), and PIT User Authentication Service (UAS).
    /// </summary>
    public class PtmExportContainer
    {
        /// <summary>
        /// RFID transponder configurations (keys, cards, stickers).
        /// </summary>
        [JsonPropertyName("transponders")]
        public List<Transponder> Transponders { get; set; } = new List<Transponder>();

        /// <summary>
        /// User data configuration: defines custom parameters stored on transponders.
        /// Called "user data configuration" in the PITreader REST API.
        /// </summary>
        [JsonPropertyName("userDefinedDataFields")]
        public List<UserDefinedDataField> UserDefinedDataFields { get; set; } = new List<UserDefinedDataField>();

        /// <summary>
        /// Permission level definitions (0–64). Determines authorization levels for transponders.
        /// </summary>
        [JsonPropertyName("permissions")]
        public List<Permission> Permissions { get; set; } = new List<Permission>();

        /// <summary>
        /// Reusable transponder configuration templates with default permissions, dates, and user data values.
        /// </summary>
        [JsonPropertyName("templates")]
        public List<Template> Templates { get; set; } = new List<Template>();

        /// <summary>
        /// Lists of transponders denied access regardless of permission level.
        /// </summary>
        [JsonPropertyName("blocklists")]
        public List<Blocklist> Blocklists { get; set; } = new List<Blocklist>();

        /// <summary>
        /// Device group definitions (0–9999). Permissions for groups 0–31 are stored directly on
        /// transponders; groups above 31 use user data parameters.
        /// </summary>
        [JsonPropertyName("groups")]
        public List<Group> Groups { get; set; } = new List<Group>();

        /// <summary>
        /// PTM file format version, corresponding to the PIT Transponder Manager version.
        /// Known values: "1.1" (PTM 1.1), "1.2" (PTM 1.2).
        /// </summary>
        [JsonPropertyName("version")]
        public List<string> Version { get; set; } = new List<string> { "1.2" };

        /// <summary>
        /// Transponder holder profiles (people assigned transponders).
        /// Not to be confused with PITreader device users.
        /// </summary>
        [JsonPropertyName("users")]
        public List<User> Users { get; set; } = new List<User>();
    }
}
