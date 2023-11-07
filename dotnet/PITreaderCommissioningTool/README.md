# PITreader Commissioning Tool

The `PITreaderCommissioningTool` is a small helper to commission/setup PITreader devices from initial unboxing to fully configured state for operation.

It is designed for usage in automated in environment and only a command line tool.

The tool has two commands:
- `list` can be used to scan the network for PITreader devices and just outputs a list of all found devices
- `run` is the main command to setup a device

## list command

Output of `PITreaderCommissioningTool list -h`

```
Description:
  Lists all devices on connected networks

Usage:
  PITreaderCommissioningTool list [options]

Options:
  -t, --scan-timeout <scan-timeout>  Timemout for network scan operation in seconds [default: 5]
  -?, -h, --help                     Show help and usage information
```

### Example output

```
---------------------------------------------------------------------------------
| Order number | Serial number | MAC address       | IP address    | HTTPS port |
---------------------------------------------------------------------------------
| 402321       | 100487526     | 9C:69:B4:50:0E:39 | 192.168.0.16  | 443        |
---------------------------------------------------------------------------------
```

## run command

Output of `PITreaderCommissioningTool run -h`

```
Description:
  Performs commissioning of a PITreader device

  Exit codes:
    0 - all tasks successfully executed
    1 - execution failed and was aborted
    2 - one or more tasks failed, but execution of other tasks continued


Usage:
  PITreaderCommissioningTool run [options]

Options:
  -t, --scan-timeout <scan-timeout>                       Timemout for network scan operation in seconds [default: 5]
  --set-ip <ip address> (REQUIRED)                        Target ip address
  --set-netmask <netmask>                                 Target netmask [default: 255.255.255.0]
  --set-gateway <ip address>                              Target default gateway [default: 0.0.0.0]
  --firmware <path to file>                               Path to latest firmware update
  --force-firmware-update                                 If set the update is always performed. Otherwise only on upgrades.
  --config <path to file>                                 Configuration file to be restored
  --config-strip-network                                  If set, the network settings are stripped from the configuration file before restore 
                                                          [default: False]
  --commissioning-user <name>                             User created during commissioning process to access the system
  --generate-password <name1 [name2 [name3]]>             Names of user for which passwords should be generated
  --generate-api-token <name1 [name2 [name3]]>            Names of user for which API tokens should be generated
  --opc-ua-client-certificate <path to file>              Path to a OPC UA client certificate that should be uploaded to the device
  --coding <path to file>                                 Coding identifier (first line) and optional comment (second line)
  --coding-oem <path to file>                             OEM coding identifier (first line) and optional comment (second line)
  --upload-certificate <path to file>                     Path to a TLS certificate file (PEM incl. key) to be uploaded to the device
  --regenerate-certificate                                If set, the certificate is regenerated after changing the IP address [default: False]
  --opc-ua-server-certificate-destination <path to file>  Path to which the OPC UA server certificate should be exported
  --config-backup-destination <path to file>              Path to which the backup of the device configuration should be exported
  -?, -h, --help                                          Show help and usage information
```

### Example

Example to
- set the ip
- update the firmware
- upload a config file
- generate a new password for the admin user
- set an oem coding
- and download the final configuration of the device for backup

**Windows Batch:**

```batch
PITreaderCommissioningTool run ^
    --set-ip 192.168.0.23 ^
    --firmware PITreader_update_2-2-0.zip ^
    --config template_default.config --config-strip-network ^
    --generate-password admin ^
    --coding-oem coding_oem.txt ^
    --config-backup-destination pitreader_backup.config
```