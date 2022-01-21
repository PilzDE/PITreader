# PITreaderTool

*PITreaderTool* is an example application for the REST API of *PITreader* devices.

## Getting Started

Access to the REST API is secured by *API tokens* (see Configuration -> API-Clients in the web interface of the device and the [API user manual](https://www.pilz.com/download/open/PITreader_REST_API_Operat_Man_1005365-DE-04.pdf)).

The client (*PITreaderTool*) can ensure that communication is established with the correct PITreader device by specifying the certificate thumbprint for the connection (see Configuration -> Certificate in the web interface of the device).

## Parameters

The tool accepts the following general parameters for the connection to a PITreader device:

- `-h, --host <host>` IP address or hostname of PITreader device [optional, default: 192.168.0.12]
- `-p, --port <port>` HTTPS port number of PITreader device [optional, default: 443]
- `--accept-all` If set, certificates of the PITreader device are not validated [default: false]
- `--thumbprint <thumbprint>` Sha2 thumbprint in hexadecimal format of the certificate of the PITreader device
- `<api token>` API token for the connection to the device

## Commands

Following the general parameters one of the commands can be passed to the tool for execution:

- `xpndr` Transponder
- `udc` User Data Configuration
- `bl` Blocklist

### Transponder

The transponder command (`xpndr`) has two sub-commands:

- `export <path to json>`  Export content of a transponder to file
- `write <path to json>`   Write data from file (see export) to transponders

`<path to json>` is a realtive of absolute path to a file to store or read the contents of the transponder.

The `write` command has two additional options:

- `--update-udc` To update the user data configuration of the device before writing data onto the transponder
- `--loop` To run forever (exit with `Ctrl + C`) and update all transponders detected by the PITreader device

The combination of both commands can be used to clone transponders.

#### Example

Example to write the data of a transponder to the file `my_transponder.json`\
The PITreader has a certificate with SHA2 thumbprint `C44E954C64B50AA2AF7CAC9F1108CEDF59FFF7D520DE27223AF92A5976F7E5FC` and API token `hJgwmy/5gyl84lKSynGIVQ==`.

    PITreaderTool.exe --thumbprint C44E954C64B50AA2AF7CAC9F1108CEDF59FFF7D520DE27223AF92A5976F7E5FC hJgwmy/5gyl84lKSynGIVQ== xpndr export my_transponder.json


### User Data Configuration

The user data configuration command (`udc`) has two sub-commands:

- `export <path to json>`  Export user data configuration from a device to a json file
- `import <path to json>`  Import user data confguration from a json file to a device

`<path to json>` is a realtive of absolute path to a file to store or read the user data configuration.

#### Example

Example to write the data of a transponder to the file `my_transponder.json`\
The PITreader has a certificate with SHA2 thumbprint `C44E954C64B50AA2AF7CAC9F1108CEDF59FFF7D520DE27223AF92A5976F7E5FC` and API token `hJgwmy/5gyl84lKSynGIVQ==`.

    PITreaderTool.exe --thumbprint C44E954C64B50AA2AF7CAC9F1108CEDF59FFF7D520DE27223AF92A5976F7E5FC hJgwmy/5gyl84lKSynGIVQ== udc export my_udc.json


### Blocklist

The blocklist command (`bl`) has one sub-command:

- `import <path to csv>` Import blocklist from a csv file into a device

`<path to csv>` is a realtive of absolute path to an existing csv file with two fields/columns: Security ID and comment.\
The first line of the CSV file is ignored.