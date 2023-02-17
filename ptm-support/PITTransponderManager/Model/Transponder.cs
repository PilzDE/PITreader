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
    /// An RFID transponder configuration (key, card, sticker, or third-party).
    /// </summary>
    public class Transponder
    {
        /// <summary>Pilz transponder key (key fob form factor).</summary>
        public const string TypePilzKey = "PILZ_KEY";

        /// <summary>Pilz transponder card.</summary>
        public const string TypePilzCard = "PILZ_CARD";

        /// <summary>Pilz transponder sticker.</summary>
        public const string TypePilzSticker = "PILZ_STICKER";

        /// <summary>Non-Pilz RFID transponder (ISO 14443-A, ISO 15693, FeliCa, or MIFARE DESFire).</summary>
        public const string TypeThirdParty = "THIRD_PARTY_TRANSPONDER";

        /// <summary>
        /// Security ID — unique transponder identifier (16-char hex string).
        /// Called <c>securityId</c> in the PITreader REST API. Empty string if not yet assigned.
        /// </summary>
        [JsonPropertyName("secureId")]
        public string SecureId { get; set; } = string.Empty;

        /// <summary>
        /// Transponder serial number (9-digit numeric string).
        /// Internal structure: 2-digit prefix (top byte) + 7-digit consecutive number (lower 3 bytes).
        /// </summary>
        [JsonPropertyName("serialNo")]
        public string SerialNo { get; set; } = string.Empty;

        /// <summary>
        /// Alternative transponder identifier, empty string, or <c>null</c>.
        /// </summary>
        [JsonPropertyName("transponderId")]
        public string? TransponderId { get; set; }

        /// <summary>
        /// Article/order number of the transponder product (e.g., "402260"), or "0", if not present.
        /// </summary>
        [JsonPropertyName("orderNo")]
        public string OrderNo { get; set; } = "0";

        /// <summary>
        /// Validity period start date (format: YYYY-M-D, non-zero-padded).
        /// Called <c>timeLimitationStart</c> in the PITreader REST API. Empty string if unset.
        /// </summary>
        [JsonPropertyName("startDate")]
        public string StartDate { get; set; } = string.Empty;

        /// <summary>
        /// Validity period end date (format: YYYY-M-D, non-zero-padded).
        /// Called <c>timeLimitationEnd</c> in the PITreader REST API. Empty string means no expiry.
        /// </summary>
        [JsonPropertyName("endDate")]
        public string EndDate { get; set; } = string.Empty;

        /// <summary>
        /// Assigned transponder holder identifier.
        /// In PTM exports this is the user's email; in MLP exports this is the user's <see cref="Model.User.UserID"/>.
        /// Consumers resolve to <see cref="PtmExportContainer.Users"/> by matching email first, then falling back to UserID.
        /// </summary>
        [JsonPropertyName("user")]
        public string User { get; set; } = string.Empty;

        /// <summary>
        /// Per-device-group permission assignments for this transponder.
        /// </summary>
        [JsonPropertyName("groupPermissions")]
        public List<GroupPermission> GroupPermissions { get; set; } = new List<GroupPermission>();

        /// <summary>
        /// Per-device-group validity date overrides for this transponder.
        /// </summary>
        [JsonPropertyName("groupDates")]
        public List<GroupDate> GroupDates { get; set; } = new List<GroupDate>();

        /// <summary>
        /// User data parameter values stored on this transponder.
        /// Called "user data" in the PITreader REST API.
        /// </summary>
        [JsonPropertyName("userDefinedDataKeyValues")]
        public List<UserDefinedDataKeyValue> UserDefinedDataKeyValues { get; set; } = new List<UserDefinedDataKeyValue>();

        /// <summary>
        /// Transponder hardware type: <c>PILZ_KEY</c>, <c>PILZ_CARD</c>,
        /// <c>PILZ_STICKER</c>, or <c>THIRD_PARTY_TRANSPONDER</c>.
        /// Present in v1.2 files; may be absent in older exports.
        /// </summary>
        [JsonPropertyName("type")]
        public string? Type { get; set; }
    }
}
