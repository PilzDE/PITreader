# PITTransponderManager Support Tools

.NET tools and libraries for working with PIT Transponder Manager (PTM) and PITreader devices.

## Projects

### PITTransponderManager (Library)

Shared library (`netstandard2.0`) providing the PTM data model and utilities.

| Component | Description |
|-----------|-------------|
| **Model/** | C# classes for the `.ptm` file format: `PtmExportContainer`, `Transponder`, `User`, `Group`, `Permission`, `Blocklist`, `Template`, `UserDefinedDataField`, etc. |
| **PtmSerializer** | Load and save `.ptm` files (JSON serialization) |
| **PtmRestApi** | REST API client for the server exposed by PIT Transponder Manager (`http://127.0.0.1:8090/`) — export and import operations |
| **PtmApplication** | Start, stop, and monitor a local PTM installation |
| **PtmInstallation** | Detect installed PTM versions on the local system |

Referenced by all other projects.

---

### PITTransponderManager.Import (CLI Tool)

Command-line tool (`net8.0`) that converts a CSV file of RFID transponder UIDs into a `.ptm` file.

**Use cases:**
- Import transponder UIDs collected from an external RFID reader into PTM
- Compute Security IDs required by PITreader devices from raw transponder UIDs
- Merge new transponders into an existing `.ptm` dataset
- Direct import into a running PTM instance via REST API

**Key features:**
- Auto-detects CSV separator (`,` or `;`) and handles Excel-style quoting
- Auto-detects UID byte order (ISO 15693 / NXP ISO 14443-A LSB-first)
- Computes Security IDs required by PITreader devices
- Interactive mode when double-clicked (prompts for file paths)

```
PtmCsvTransponderImport data.csv output.ptm              # offline
PtmCsvTransponderImport data.csv output.ptm existing.ptm  # merge
PtmCsvTransponderImport --online data.csv                  # direct PTM import
```

Publish as self-contained single-file exe:
```
dotnet publish -p:PublishProfile=win-x64
```

See `PITTransponderManager.Import/README.txt` and `LIESMICH.txt` for end-user documentation.

---

### PITTransponderManager.LdapExport (CLI Tool)

Command-line tool (`net8.0`) that synchronizes user data from an LDAP directory (e.g., Active Directory) into PTM.

**Use cases:**
- Automatically populate PTM with employee data from Active Directory
- Deactivate users removed from LDAP and optionally blocklist their transponders
- Scheduled synchronization via Windows Task Scheduler or as part of a CI pipeline

**Modes:**
- **Offline:** reads LDAP, writes a `.ptm` file for manual import
- **Online:** reads LDAP, imports directly into running PTM via REST API

Configuration via `config.ini` (LDAP connection, attribute mappings, options).

```
PtmLdapSync output.ptm [input.ptm]   # offline
PtmLdapSync --online                  # online
```

---

### PITTransponderManager.Service (Windows Service)

Windows background service (`net8.0-windows`) that wraps the PTM Java server process.

**Use case:**
- Run PIT Transponder Manager as a Windows service for unattended operation (e.g., on a server that hosts PTM for scheduled LDAP synchronization)

```
install_service.bat   # install as Windows service
```

---

## Building

Prerequisites: .NET 8.0 SDK (or later)

```bash
dotnet build                              # build all projects
dotnet build PITTransponderManager.Import  # build single project
```
