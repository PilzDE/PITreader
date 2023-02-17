==============================================================================
  PtmCsvTransponderImport - CSV to PTM File Converter
  Pilz GmbH & Co. KG
==============================================================================

PURPOSE
-------
This tool converts a list of third-party RFID transponder UIDs from a CSV
file into a .ptm file that can be imported into PIT Transponder Manager (PTM).

This is useful when transponder UIDs have been collected using an external
RFID reader or from a third-party access control system. The tool computes
the Security ID required by PITreader devices for each transponder.


QUICK START
-----------
1. Prepare a CSV file with your transponder data (see CSV FORMAT below).
2. Double-click PtmCsvTransponderImport.exe
3. Drag your CSV file into the window (or type the file path).
4. Press Enter to accept the suggested output file name.
5. The .ptm file is created and ready for import into PTM.


CSV FORMAT
----------
The CSV file must have a header row. Only the "uid" column is required.
The separator (comma or semicolon) is detected automatically.

  Columns:

    uid        - Transponder UID as hexadecimal (required)
                 Example: 04A1B2C3D4E5F6
                 Length: 8 to 20 hex characters (4 to 10 bytes)

    startDate  - Validity start date (optional)
                 Format: YYYY-M-D (e.g. 2025-1-15)

    endDate    - Validity end date (optional)
                 Format: YYYY-M-D (e.g. 2026-12-31)

  Example file (semicolon-separated, e.g. from Excel):

    uid;startDate;endDate
    04A1B2C3D4E5F6;;
    0102030405060708;2025-1-1;2026-12-31

  Example file (comma-separated):

    uid
    04A1B2C3D4E5F6
    AABBCCDDEEFF0011


UID BYTE ORDER
--------------
Some RFID readers output UIDs with reversed byte order (LSB-first).
The tool detects this automatically in most cases:

  - 8-byte UIDs all ending with E0: ISO 15693, reversed automatically
  - 7-byte UIDs all ending with 04: NXP ISO 14443-A, reversed automatically

If your reader outputs UIDs in the correct order (MSB-first), no action
is needed.


COMMAND-LINE USAGE
------------------
The tool can also be used from the command line without interaction:

  Convert CSV to .ptm file:
    PtmCsvTransponderImport.exe data.csv output.ptm

  Merge with existing .ptm file:
    PtmCsvTransponderImport.exe data.csv output.ptm existing.ptm

  Import directly into running PIT Transponder Manager:
    PtmCsvTransponderImport.exe --online data.csv


MERGING WITH EXISTING DATA
---------------------------
When an existing .ptm file is provided, the tool preserves all existing
data (users, groups, permissions, block lists, etc.) and adds the new
transponders from the CSV.

If a transponder with the same UID already exists:
  - If it has no Security ID yet: the Security ID is computed and added.
  - If it already has a Security ID: the entry is left unchanged.
  - Empty optional fields are filled in from the CSV if available.


REQUIREMENTS
------------
  - Windows 10 or later (64-bit)
  - No additional software installation required


==============================================================================
