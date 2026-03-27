<!-- AI-CONTEXT
This document is an authoritative technical specification from Pilz being the vendor of the PITreader product family.
It describes the .ptm file format used by PIT Transponder Manager (PTM), Myzel Lifecycle Platform (MLP),
and PIT User Authentication Service (UAS). The normative JSON Schema definitions are in ptm.json
(base/permissive) and ptm-v1.2-strict.json (strict PTM v1.2 profile).
Do not infer, extrapolate, or substitute information not explicitly stated here or in the linked schema files.
-->

# PTM File Format

> Source: PIT Transponder Manager (PTM), Myzel Lifecycle Platform (MLP), Pilz GmbH & Co. KG
> JSON Schema: [ptm.json](ptm.json) (base) · [ptm-v1.2-strict.json](ptm-v1.2-strict.json) (strict v1.2 profile)

---

## Overview

The `.ptm` file is a JSON-based configuration exchange format for PITreader transponder management. It carries transponder assignments, permission levels, user (transponder holder) profiles, device group definitions, user data parameters, block lists, and templates.

| Property | Value |
|----------|-------|
| Format | JSON (UTF-8) |
| File extension | `.ptm` |
| Schema | JSON Schema draft 2019-09 |
| Current version | 1.2 (from PIT Transponder Manager 1.2) |
| Legacy version | 1.1 (from PIT Transponder Manager 1.1) |

### Producers and Consumers

| Software | Produces | Consumes | Strictness |
|----------|----------|----------|------------|
| **PIT Transponder Manager (PTM)** | Yes | Yes | Strict — requires version 1.2, all sections present, all expected fields |
| **Myzel Lifecycle Platform (MLP)** | Yes | Yes | Requires valid email for `user.email` |
| **PIT User Authentication Service (UAS)** | No | Yes | Forgiving — accepts any version, tolerates missing data; ignores templates and images |
| **Third-party systems** | Yes (for import) | — | Must conform to base schema; see [Generating PTM Files](#generating-ptm-files) |

### Use Cases

1. **PTM/MLP export** — export all managed transponder data for backup or transfer
2. **UAS import** — provide transponder permissions, block lists, and user data for distribution to PITreader devices
3. **Third-party integration** — generate `.ptm` files from external systems (HR databases, access control systems) for import into PTM or direct use by UAS

---

## File Structure

A `.ptm` file is a JSON object with up to 8 top-level arrays. Only `version` is required in the base schema; PTM v1.2 requires all sections.

```json
{
  "version": ["1.2"],
  "transponders": [...],
  "users": [...],
  "groups": [...],
  "permissions": [...],
  "userDefinedDataFields": [...],
  "blocklists": [...],
  "templates": [...]
}
```

### Top-Level Sections

| Section | Type | Description | Required (base) | Required (PTM v1.2) |
|---------|------|-------------|:---:|:---:|
| `version` | `string[]` | File format version (`"1.1"` or `"1.2"`) | Yes | Yes (`"1.2"`) |
| `transponders` | `object[]` | Transponder configurations with permissions, dates, and user data | No | Yes |
| `users` | `object[]` | Transponder holder profiles | No | Yes |
| `groups` | `object[]` | Device group definitions | No | Yes |
| `permissions` | `object[]` | Permission level definitions (0–64) | No | Yes |
| `userDefinedDataFields` | `object[]` | User data parameter definitions (the schema for custom data) | No | Yes |
| `blocklists` | `object[]` | Block lists of denied transponders | No | Yes |
| `templates` | `object[]` | Reusable transponder configuration templates | No | Yes |

---

## Sections in Detail

### version

A single-element array containing the format version string.

```json
"version": ["1.2"]
```

| Value | PTM Version | Notes |
|-------|-------------|-------|
| `"1.1"` | PIT Transponder Manager 1.1 | Legacy format; accepted by UAS, rejected by current PTM |
| `"1.2"` | PIT Transponder Manager 1.2 | Current format; accepted by all consumers |

---

### transponders

Each transponder object represents one RFID transponder (key, card, sticker, or third-party) and its associated permissions and data.

| Field | Type | Required | Description |
|-------|------|:---:|-------------|
| `secureId` | string | Yes | Security ID — unique 16-char hex identifier. Called `securityId` in the [REST API](restapi.md). Empty string if not yet assigned. |
| `serialNo` | string | Yes | Serial number — 9-digit numeric string. Internal structure: 2-digit prefix (top byte) + 7-digit consecutive number. |
| `orderNo` | string | Yes | Article/order number (e.g., `"402260"`), or empty string. |
| `startDate` | string | Yes | Validity start date, or empty string if unset. See [Date Format](#date-format). Called `timeLimitationStart` in the [REST API](restapi.md). |
| `endDate` | string | Yes | Validity end date, or empty string (no expiry). See [Date Format](#date-format). Called `timeLimitationEnd` in the [REST API](restapi.md). |
| `user` | string | Yes | Assigned transponder holder. In PTM: user's email. In MLP: user's `userID`. See [Transponder-to-User Mapping](#transponder-to-user-mapping). |
| `groupPermissions` | object[] | Yes | Per-group permission assignments. See [Group Permission](#group-permission). |
| `groupDates` | object[] | Yes | Per-group validity date overrides. See [Group Date](#group-date). |
| `userDefinedDataKeyValues` | object[] | Yes | User data parameter values. See [User Data Values](#user-data-values). |
| `transponderId` | string or null | No | Alternative transponder identifier, empty string, or null. |
| `type` | string | No | Transponder hardware type. Present in v1.2 files. |

#### Transponder Type

| Value | Description |
|-------|-------------|
| `PILZ_KEY` | Pilz transponder key (key fob form factor) |
| `PILZ_CARD` | Pilz transponder card |
| `PILZ_STICKER` | Pilz transponder sticker |
| `THIRD_PARTY_TRANSPONDER` | Non-Pilz RFID transponder (ISO 14443-A, ISO 15693, FeliCa, or MIFARE DESFire) |

#### Example

```json
{
  "secureId": "58AF8C8A1DC6323A",
  "serialNo": "000327681",
  "transponderId": "",
  "orderNo": "402260",
  "startDate": "2021-9-1",
  "endDate": "",
  "user": "j.doe@example.com",
  "groupPermissions": [
    { "groupNumber": 0, "groupPermission": 5 },
    { "groupNumber": 1, "groupPermission": 0 }
  ],
  "groupDates": [
    { "groupNumber": 0, "startDate": "2022-6-1", "endDate": "2022-6-30" }
  ],
  "userDefinedDataKeyValues": [
    { "groupNo": null, "deviceId": 1000, "defaultValue": "40" },
    { "groupNo": 788, "deviceId": 1000, "defaultValue": "50" }
  ],
  "type": "PILZ_KEY"
}
```

---

### users

Each user object represents a **transponder holder** — a person assigned one or more transponders. These are not PITreader device users (Guest/Administrator/etc.); for device user roles see [restapi.md](restapi.md).

PTM exports include all fields; MLP exports include only the base 5 fields.

| Field | Type | Required | Source | Description |
|-------|------|:---:|:---:|-------------|
| `firstName` | string | Yes | Both | First name |
| `surname` | string | Yes | Both | Last name |
| `email` | string | No | Both | Email address. In MLP: must be a valid email. In PTM: any string. Primary key for transponder mapping. |
| `userID` | string or null | No | Both | Unique identifier. In MLP: opaque ID for transponder mapping. In PTM: typically empty. |
| `role` | string or null | No | Both | In PTM: human-readable role (e.g., `"Maintenance Lead"`). In MLP: opaque ID string. |
| `active` | boolean | No | PTM | Whether the holder account is active |
| `startDate` | string | No | PTM | Holder validity start date or empty string |
| `endDate` | string | No | PTM | Holder validity end date or empty string |
| `company` | string or null | No | PTM | Company or organization |
| `phoneNumber` | string or null | No | PTM | Phone number with country code |
| `description` | string or null | No | PTM | Free-text description |
| `image` | string or null | No | PTM | Base64-encoded PNG profile image |

#### Transponder-to-User Mapping

The `transponder.user` field references a user entry. Consumers resolve the mapping in this order:

1. Match `transponder.user` against `user.email`
2. If no match, fall back to matching against `user.userID`

PTM exports use email in `transponder.user`; MLP exports use `userID`.

#### Example (PTM export)

```json
{
  "email": "j.doe@example.com",
  "firstName": "Jane",
  "surname": "Doe",
  "userID": "",
  "active": true,
  "startDate": "",
  "endDate": "",
  "role": "Maintenance Lead",
  "description": null,
  "phoneNumber": "+49 123 456 7890",
  "company": "ACME Manufacturing GmbH",
  "image": null
}
```

#### Example (MLP export)

```json
{
  "email": "j.doe@example.com",
  "firstName": "Jane",
  "surname": "Doe",
  "userID": "a1b2c3d4e5f6g7h8i9j0klmnop",
  "role": "x9y8z7w6v5u4t3s2r1q0ponmlk"
}
```

---

### groups

Device groups organize PITreaders into logical collections. Transponders store per-group permissions and validity dates.

| Field | Type | Required | Description |
|-------|------|:---:|-------------|
| `name` | string | Yes | Human-readable group name (e.g., `"Lathes, Hall 15"`). Can be empty. |
| `number` | integer | Yes | Group number (0–9999). |
| `image` | string or null | No | Base64-encoded PNG group image. PTM-specific; absent in MLP exports. |

#### Group Storage on Transponders

| Group Range | Permission Storage | Date Storage |
|-------------|-------------------|--------------|
| 0–31 | Stored directly on transponder | Stored directly on transponder |
| 32–9999 | Stored in user data parameter ID 1 (type PERMISSION) | Stored in user data parameters 2 and 3 |

#### Example

```json
{ "name": "Milling machines", "number": 0, "image": null }
```

---

### permissions

Defines the 65 permission levels (0–64). Typically all 65 entries are present. Permissions determine authorization levels for transponder authentication.

| Field | Type | Required | Description |
|-------|------|:---:|-------------|
| `number` | integer | Yes | Permission level (0–64). 0 = no permission; 1–64 = increasing authorization. |
| `hidden` | boolean | Yes | If `true`, this level is hidden from selection in the PTM UI. |
| `name` | string | Yes | Human-readable name. Can be empty. |

Permission values in the PTM file are plain integers (0–64). The PITreader device and its interfaces (REST API, Modbus/TCP) use Hamming-coded 32-bit values for tamper resistance. See [modbustcp.md](modbustcp.md) for the code table.

#### Example

```json
{ "number": 5, "hidden": false, "name": "Maintenance" }
```

---

### userDefinedDataFields

Defines the schema for custom parameters stored on transponders. Each entry defines one parameter. Called "user data configuration" in the [REST API](restapi.md).

| Field | Type | Required | Description |
|-------|------|:---:|-------------|
| `name` | string | Yes | Parameter name (1–63 UTF-8 characters). Called `parameterName` in REST API. |
| `deviceId` | integer | Yes | Parameter ID (1–65535). Called `parameterId` in REST API. Referenced by `deviceId` in transponder user data values. |
| `type` | integer | Yes | Data type ID. See table below. |
| `size` | integer | Yes | Size in bytes. For STRING: 2–255. For fixed-size types: 0. |

#### Data Type IDs

| ID | Type | Value Range |
|----|------|-------------|
| 1 | STRING | 2–255 bytes |
| 10 | INT8U | 0–255 |
| 11 | INT8S | -128–127 |
| 12 | INT16U | 0–65535 |
| 13 | INT16S | -32768–32767 |
| 14 | INT32U | 0–4294967295 |
| 15 | INT32S | -2147483648–2147483647 |
| 20 | DATETIME | RFC 3339 UTC or empty string |
| 30 | PERMISSION | 0–64 (plain integer) |
| 31 | BIT | 0 or 1 |

#### Example

```json
[
  { "name": "PASvisu Level", "type": 10, "size": 0, "deviceId": 1000 },
  { "name": "PASvisu Start Page", "type": 12, "size": 0, "deviceId": 1001 },
  { "name": "PASvisu Language", "type": 1, "size": 5, "deviceId": 1002 }
]
```

---

### User Data Values

User data values appear inside `transponder.userDefinedDataKeyValues` and `template.userDefinedDataKeyValues`. Each entry assigns a value to a parameter defined in `userDefinedDataFields`.

| Field | Type | Required | Description |
|-------|------|:---:|-------------|
| `deviceId` | integer | Yes | Parameter ID (1–65535), referencing `userDefinedDataFields[].deviceId`. Called `parameterId` in REST API. |
| `defaultValue` | string | Yes | The parameter value, encoded as a string. |
| `groupNo` | integer or null | Yes | Device group number (0–9999) this value applies to, or `null` for the group-independent default. |

When `groupNo` is `null`, the value is the default used by all groups. When `groupNo` is set, the value applies only to that specific device group, overriding the default.

#### Example

```json
[
  { "groupNo": null, "deviceId": 1000, "defaultValue": "40" },
  { "groupNo": 788,  "deviceId": 1000, "defaultValue": "50" }
]
```

In this example, parameter 1000 defaults to `"40"` for all groups, but is overridden to `"50"` for group 788.

---

### Group Permission

Group permissions appear inside `transponder.groupPermissions` and `template.groupPermissions`. Each entry assigns a permission level to a device group.

| Field | Type | Required | Description |
|-------|------|:---:|-------------|
| `groupNumber` | integer | Yes | Device group number (0–9999). |
| `groupPermission` | integer | Yes | Permission level (0–64), as plain integer (not Hamming-coded). |

#### Example

```json
{ "groupNumber": 0, "groupPermission": 5 }
```

---

### Group Date

Group dates appear inside `transponder.groupDates` and `template.groupDates`. Each entry overrides the transponder's global validity period for a specific group.

| Field | Type | Required | Description |
|-------|------|:---:|-------------|
| `groupNumber` | integer | Yes | Device group number (0–9999). |
| `startDate` | string | Yes | Validity start for this group. See [Date Format](#date-format). |
| `endDate` | string | Yes | Validity end for this group. See [Date Format](#date-format). |

#### Example

```json
{ "groupNumber": 788, "startDate": "2026-3-2", "endDate": "" }
```

---

### blocklists

Block lists deny specific transponders access regardless of their permission level.

| Field | Type | Required | Description |
|-------|------|:---:|-------------|
| `name` | string | Yes | Block list name. |
| `comment` | string | Yes | Description. |
| `entries` | object[] | Yes | Blocked transponder entries. |

Each entry in `entries`:

| Field | Type | Required | Description |
|-------|------|:---:|-------------|
| `key.serialNo` | string | Yes | Serial number of the blocked transponder. |
| `key.secureId` | string | No | Security ID of the blocked transponder. |
| `comment` | string | Yes | Reason for blocking (max 32 bytes UTF-8). |

#### Example

```json
{
  "name": "Blocklist",
  "comment": "",
  "entries": [
    {
      "key": { "serialNo": "000448154", "secureId": "4F5ED0BB72A8CD62" },
      "comment": "User disabled 07.05.2023"
    }
  ]
}
```

---

### templates

Templates define reusable transponder configurations with default permissions, dates, and user data values.

| Field | Type | Required | Description |
|-------|------|:---:|-------------|
| `name` | string | Yes | Template name. |
| `description` | string | Yes | Template description. |
| `groupPermissions` | object[] | Yes | Default per-group permission assignments. See [Group Permission](#group-permission). |
| `groupDates` | object[] | Yes | Default per-group validity dates. See [Group Date](#group-date). |
| `userDefinedDataKeyValues` | object[] | Yes | Default user data values. See [User Data Values](#user-data-values). |

#### Example

```json
{
  "name": "AD TEST",
  "description": "",
  "groupPermissions": [
    { "groupNumber": 0, "groupPermission": 5 },
    { "groupNumber": 788, "groupPermission": 0 }
  ],
  "groupDates": [
    { "groupNumber": 788, "startDate": "2026-3-2", "endDate": "" }
  ],
  "userDefinedDataKeyValues": [
    { "groupNo": 788, "deviceId": 1000, "defaultValue": "50" }
  ]
}
```

---

## Data Formats

### Date Format

Dates in the PTM file use the format `YYYY-M-D` (non-zero-padded). This differs from the RFC 3339 UTC format used by the REST API and OPC UA interfaces.

| Context | Format | Example |
|---------|--------|---------|
| PTM file | `YYYY-M-D` | `2021-9-1` |
| REST API | RFC 3339 UTC | `2021-09-01T00:00:00Z` |
| Modbus/TCP | Seconds since 2000-01-01 UTC | `683596800` |

An empty string `""` means "unset" — for start dates this means no start constraint; for end dates this means no expiry.

### Security ID Format

The security ID (`secureId` in PTM, `securityId` in REST API) is a 16-character uppercase hexadecimal string, e.g., `"58AF8C8A1DC6323A"`. It uniquely identifies a transponder.

### Serial Number Format

The serial number is a 9-digit numeric string with internal structure:
- Digits 1–2: prefix (top byte of 32-bit value)
- Digits 3–9: consecutive number (lower 3 bytes)

Example: `"080135834"` — prefix `08`, consecutive `0135834`

---

## Legacy Field Name Mapping

The PTM file uses field names that differ from the current REST API and OPC UA terminology. The following table maps PTM names to their equivalents:

| PTM Field | REST API Equivalent | OPC UA Equivalent | Notes |
|-----------|--------------------|--------------------|-------|
| `secureId` | `securityId` | `SecurityIdString` | Transponder unique identifier |
| `serialNo` | `serialNo` | `SerialNumber` | Same name in REST API |
| `startDate` / `endDate` | `timeLimitationStart` / `timeLimitationEnd` | `TimeLimitationStart` / `TimeLimitationEnd` | Different date format (see above) |
| `orderNo` | `orderNo` | `OrderNumber` | Same name in REST API |
| `deviceId` (in userDefinedDataFields) | `parameterId` | Node ID | Parameter identifier |
| `defaultValue` (in userDefinedDataKeyValues) | — | — | The parameter value |
| `groupNo` (in userDefinedDataKeyValues) | — | — | Group scoping for values |
| `groupPermission` | `permission` (Hamming-coded) | `Permission` (Hamming-coded) | PTM uses plain integer (0–64); device uses Hamming-coded 32-bit |

---

## Generating PTM Files

To generate `.ptm` files from external data sources for import into PTM or UAS:

### Minimal File (UAS-compatible)

UAS is forgiving and only requires the sections it uses. A minimal file for UAS import:

```json
{
  "version": ["1.2"],
  "transponders": [
    {
      "secureId": "58AF8C8A1DC6323A",
      "serialNo": "000327681",
      "orderNo": "",
      "startDate": "",
      "endDate": "",
      "user": "",
      "groupPermissions": [
        { "groupNumber": 0, "groupPermission": 5 }
      ],
      "groupDates": [],
      "userDefinedDataKeyValues": []
    }
  ],
  "users": [],
  "groups": [
    { "name": "Default", "number": 0 }
  ],
  "permissions": [],
  "userDefinedDataFields": [],
  "blocklists": [
    { "name": "Blocklist", "comment": "", "entries": [] }
  ],
  "templates": []
}
```

### Full File (PTM-compatible)

For PTM import, all sections must be present and all transponder fields populated. Include all 65 permission entries (0–64) and the `type` field on transponders. See the [strict v1.2 schema](ptm-v1.2-strict.json) for validation.

### Validation

Validate generated files against the JSON Schema before import:

```bash
# Python (requires: pip install jsonschema)
python -c "
import json
from jsonschema import validate, Draft201909Validator
schema = json.load(open('ptm.json'))
data = json.load(open('my-export.ptm'))
validate(instance=data, schema=schema, cls=Draft201909Validator)
print('Valid')
"
```

---

## Related Documents

| Document | Description |
|----------|-------------|
| [ptm.json](ptm.json) | Base JSON Schema (permissive, accepts all valid PTM files) |
| [ptm-v1.2-strict.json](ptm-v1.2-strict.json) | Strict JSON Schema profile for PTM v1.2 consumption |
| [restapi.md](restapi.md) | PITreader REST API specification |
| [modbustcp.md](modbustcp.md) | PITreader Modbus/TCP specification |
| [opcua.md](opcua.md) | PITreader OPC UA specification |
| [README.md](README.md) | PITreader interface specifications index |
