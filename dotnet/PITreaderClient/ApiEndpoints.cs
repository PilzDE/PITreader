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
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Pilz.PITreader.Client.Model;

namespace Pilz.PITreader.Client
{
    /// <summary>
    /// REST API endpoints of PITreader devices.
    /// </summary>
    public static class ApiEndpoints
    {
        /// <summary>
        /// Status endpoint
        /// </summary>
        public const string Status = "/api/status";

        /// <summary>
        /// Authentication status endpoint
        /// </summary>
        public const string StatusAuthentication = "/api/status/authentication";

        /// <summary>
        /// Status monitoring endpoint
        /// </summary>
        public const string StatusMonitor = "/api/status/monitor";

        /// <summary>
        /// Configuration endpoint.
        /// </summary>
        public const string Config = "/api/config";

        /// <summary>
        /// Blocklist endpoint.
        /// </summary>
        public const string ConfigBlocklist = "/api/config/blocklist";

        /// <summary>
        /// Regenerate certificate endpoint.
        /// </summary>
        public const string FilesCertificate = "/api/files/certificate";

        /// <summary>
        /// Regenerate certificate endpoint.
        /// </summary>
        public const string ConfigCertificateRegenerate = "/api/config/certificate/regenerate";

        /// <summary>
        /// Coding endpoint.
        /// </summary>
        public const string ConfigCoding = "/api/config/coding";

        /// <summary>
        /// OEM Coding endpoint.
        /// </summary>
        public const string ConfigCodingOem = "/api/config/coding/oem";

        /// <summary>
        /// Permission list endpoint.
        /// </summary>
        public const string ConfigPermissionList = "/api/config/permissionList";

        /// <summary>
        /// User data configuration endpoint.
        /// </summary>
        public const string ConfigUserData = "/api/config/userData";

        /// <summary>
        /// User data configuration reset endpoint.
        /// </summary>
        public const string ConfigUserDataReset = "/api/config/userData/reset";

        /// <summary>
        /// User data configuration parameter endpoint.
        /// </summary>
        public const string ConfigUserDataParameter = "/api/config/userData/parameter";

        /// <summary>
        /// User data configuration version endpoint.
        /// </summary>
        public const string ConfigUserDataVersion = "/api/config/userData/version";

        /// <summary>
        /// Device user list endpoint.
        /// </summary>
        public const string ConfigDeviceUsers = "/api/config/deviceUsers";

        /// <summary>
        /// Device user endpoint.
        /// </summary>
        public const string ConfigDeviceUser = "/api/config/deviceUser";

        /// <summary>
        /// Device user API token endpoint.
        /// </summary>
        public const string ConfigDeviceUserApiToken = "/api/config/deviceUser/apiToken";

        /// <summary>
        /// Led endpoint.
        /// </summary>
        public const string Led = "/api/led";

        /// <summary>
        /// Transponder data endpoint.
        /// </summary>
        public const string Transponder = "/api/transponder";

        /// <summary>
        /// Transponder user data endpoint.
        /// </summary>
        public const string TransponderUserData = "/api/transponder/userData";

        /// <summary>
        /// Transponder user data clear endpoint.
        /// </summary>
        public const string TransponderUserDataClear = "/api/transponder/userData/clear";

        /// <summary>
        /// Transponder user data read endpoint.
        /// </summary>
        public const string TransponderUserDataRead = "/api/transponder/userData/read";

        /// <summary>
        /// Transponder user data write endpoint.
        /// </summary>
        public const string TransponderUserDataWrite = "/api/transponder/userData/write";

        /// <summary>
        /// Transponder user data clear group data endpoint.
        /// </summary>
        public const string TransponderUserDataClearGroup = "/api/transponder/userData/clearGroup";

        /// <summary>
        /// Transponder user data add group values endpoint.
        /// </summary>
        public const string TransponderUserDataAddGroupValues = "/api/transponder/userData/addGroupValues";

        /// <summary>
        /// Transponder teach-in endpoint.
        /// </summary>
        public const string TransponderTeachIn = "/api/transponder/teachIn";

        /// <summary>
        /// Transponder crypto endpoint.
        /// </summary>
        public const string TransponderCrypto = "/api/transponder/crypto";

        /// <summary>
        /// Firmware upload endpoint.
        /// </summary>
        public const string FilesFirmware = "/api/files/firmware";

        /// <summary>
        /// Firmware update endpoint.
        /// </summary>
        public const string FirmwareUpdate = "/api/firmware/update";

        /// <summary>
        /// OPC UA server certificate endpoint.
        /// </summary>
        public const string FilesOpcuaServer = "/api/files/opcua/server";

        /// <summary>
        /// Configuration backup endpoint.
        /// </summary>
        public const string FilesBackup = "/api/files/backup";

        /// <summary>
        /// Read general device information (e.g. device name, order number, serial number, firmware version, hardware version, software version)
        /// </summary>
        /// <param name="client">PITreader client instance.</param>
        /// <returns></returns>
        public static Task<ApiResponse<StatusResponse>> GetDeviceStatus(this PITreaderClient client)
        {
            return client.GetAsync<StatusResponse>(Status);
        }

        /// <summary>
        /// Returns the device settings of a PITreader.
        /// </summary>
        /// <param name="client">PITreader client instance.</param>
        /// <returns></returns>
        public static Task<ApiResponse<ConfigResponse>> GetDeviceConfiguration(this PITreaderClient client)
        {
            return client.GetAsync<ConfigResponse>(Config);
        }

        /// <summary>
        /// Changes device settings of a PITreader.
        /// </summary>
        /// <param name="client">PITreader client instance.</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Task<ApiResponse<GenericResponse>> SetDeviceConfiguration(this PITreaderClient client, ConfigRequest config)
        {
            return client.PostAsync<GenericResponse, ConfigRequest>(Config, config);
        }

        /// <summary>
        /// Returns information about the basic coding settings on a device.
        /// </summary>
        /// <param name="client">PITreader client instance.</param>
        /// <returns></returns>
        public static Task<ApiResponse<CodingResponse>> GetBasicCoding(this PITreaderClient client)
        {
            return client.GetAsync<CodingResponse>(ConfigCoding);
        }

        /// <summary>
        /// Returns information about the oem coding settings on a device.
        /// </summary>
        /// <param name="client">PITreader client instance.</param>
        /// <returns></returns>
        public static Task<ApiResponse<CodingResponse>> GetOemCoding(this PITreaderClient client)
        {
            return client.GetAsync<CodingResponse>(ConfigCodingOem);
        }

        /// <summary>
        /// Sets or updates the basic coding on a device.
        /// </summary>
        /// <param name="client">PITreader client instance.</param>
        /// <param name="coding"></param>
        /// <returns></returns>
        public static Task<ApiResponse<GenericResponse>> SetBasicCoding(this PITreaderClient client, BasicCodingRequest coding)
        {
            return client.PostAsync<GenericResponse, BasicCodingRequest>(ConfigCoding, coding);
        }

        /// <summary>
        /// Sets or updates the oem coding on a device.
        /// </summary>
        /// <param name="client">PITreader client instance.</param>
        /// <param name="coding"></param>
        /// <returns></returns>
        public static Task<ApiResponse<GenericResponse>> SetOemCoding(this PITreaderClient client, OemCodingRequest coding)
        {
            return client.PostAsync<GenericResponse, OemCodingRequest>(ConfigCodingOem, coding);
        }

        /// <summary>
        /// Returns the authentication status of the current transponder.
        /// </summary>
        /// <param name="client">PITreader client instance.</param>
        /// <returns></returns>
        public static Task<ApiResponse<AuthenticationStatusResponse>> GetAuthenticationStatus(this PITreaderClient client)
        {
            return client.GetAsync<AuthenticationStatusResponse>(StatusAuthentication);
        }

        /// <summary>
        /// Returns the status monitoring data.
        /// </summary>
        /// <param name="client">PITreader client instance.</param>
        /// <returns></returns>
        public static Task<ApiResponse<StatusMonitorResponse>> GetStatusMonitor(this PITreaderClient client)
        {
            return client.GetAsync<StatusMonitorResponse>(StatusMonitor);
        }

        /// <summary>
        /// Returns general data of a transpodner currently authenticated.
        /// </summary>
        /// <param name="client">PITreader client instance.</param>
        /// <returns></returns>
        public static Task<ApiResponse<TransponderResponse>> GetTransponderData(this PITreaderClient client)
        {
            return client.GetAsync<TransponderResponse>(Transponder);
        }

        /// <summary>
        /// Performs a teach-in of the current transponder to the basic coding of the device.
        /// </summary>
        /// <param name="client">PITreader client instance.</param>
        /// <param name="teachInId">The teach-in id of the transponder as returned by <see cref="GetTransponderData"/></param>
        /// <returns></returns>
        public static Task<ApiResponse<GenericResponse>> TeachInTransponder(this PITreaderClient client, string teachInId)
        {
            return client.PostAsync<GenericResponse, object>(TransponderTeachIn, new { teachInId });
        }

        /// <summary>
        /// Sets the cryptographic key for the transponder on the device.
        /// </summary>
        /// <param name="client">PITreader client instance.</param>
        /// <param name="type">Transponder crypto key type.</param>
        /// <param name="keyData">The cryptographic key data as a 16-character string.</param>
        /// <returns>An ApiResponse of type GenericResponse.</returns>
        /// <exception cref="ArgumentException">Thrown when keyData is not exactly 16 characters long.</exception>
        public static Task<ApiResponse<GenericResponse<TransponderCryptoResponseData>>> SetTransponderCryptoKey(this PITreaderClient client, TransponderCryptoKeyType type, byte[] keyData)
        {
            // Send the request to the /api/transponder/crypto endpoint
            return client.PostAsync<GenericResponse<TransponderCryptoResponseData>, TransponderCryptoRequest>(
                TransponderCrypto, 
                new TransponderCryptoRequest { Key = new TransponderCryptoKeyRequestData { Type = type, Data = keyData } });
        }

        /// <summary>
        /// Sets the cryptographic key for the transponder on the device.
        /// </summary>
        /// <param name="client">PITreader client instance.</param>
        /// <param name="type">Transponder crypto key type.</param>
        /// <param name="passphrase">Passphrase for the logon key stored in the device configuration.</param>
        /// <returns>An ApiResponse of type GenericResponse.</returns>
        /// <exception cref="ArgumentException">Thrown when keyData is not exactly 16 characters long.</exception>
        public static Task<ApiResponse<GenericResponse<TransponderCryptoResponseData>>> SetTransponderCryptoKey(this PITreaderClient client, TransponderCryptoKeyType type, string passphrase)
        {
            // Send the request to the /api/transponder/crypto endpoint
            return client.PostAsync<GenericResponse<TransponderCryptoResponseData>, TransponderCryptoRequest>(
                TransponderCrypto,
                new TransponderCryptoRequest { Key = new TransponderCryptoKeyRequestData { Type = type, Passphrase = passphrase } });
        }

        /// <summary>
        /// Performs a cryptographic operation (encryption or decryption) on the device.
        /// </summary>
        /// <param name="client">PITreader client instance.</param>
        /// <param name="request">The cryptographic request data.</param>
        /// <returns>An ApiResponse of type CryptoResponse.</returns>
        public static Task<ApiResponse<GenericResponse<TransponderCryptoResponseData>>> TransponderCrypt(this PITreaderClient client, TransponderCryptoRequest request)
        {
            // Send the request to the CryptoEndpoint
            return client.PostAsync<GenericResponse<TransponderCryptoResponseData>, TransponderCryptoRequest>(TransponderCrypto, request);
        }

        /// <summary>
        /// Returns the current LED status (GET request to /api/led)
        /// </summary>
        /// <param name="client">PITreader client instance.</param>
        /// <returns>The current LED status.</returns>
        public static Task<ApiResponse<LedResponse>> GetLed(this PITreaderClient client)
        {
            return client.GetAsync<LedResponse>(Led);
        }

        /// <summary>
        /// Changes LED settings.
        /// </summary>
        /// <param name="client">PITreader client instance.</param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static Task<ApiResponse<GenericResponse>> SetLed(this PITreaderClient client, LedRequest settings)
        {
            return client.PostAsync<GenericResponse, LedRequest>(Led, settings);
        }

        /// <summary>
        /// Sets the permission for the current transponder key in external mode.
        /// </summary>
        /// <param name="client">PITreader client instance.</param>
        /// <param name="securityId">The security id for which the permission should be set.</param>
        /// <param name="permission">The permission to be set for the specified security id.</param>
        /// <returns></returns>
        public static Task<ApiResponse<GenericResponse>> SetExternalAuth(this PITreaderClient client, SecurityId securityId, Permission permission)
        {
            return client.PostAsync<GenericResponse, ExternalAuthenticationRequest>(StatusAuthentication, new ExternalAuthenticationRequest { SecurityId = securityId, Permission = permission });
        }

        /// <summary>
        /// Sets the permission for the current transponder key in external mode.
        /// </summary>
        /// <param name="client">PITreader client instance.</param>
        /// <param name="securityId">The security id for which the permission should be set.</param>
        /// <param name="permission">The permission to be set for the specified security id.</param>
        /// <returns></returns>
        public static Task<ApiResponse<GenericResponse>> SetExternalAuth(this PITreaderClient client, string securityId, Permission permission)
        {
            if (!SecurityId.TryParse(securityId, out SecurityId securityIdObject))
            {
                throw new ArgumentException($"Invalid security id \"{securityId}\".", nameof(securityId));
            }

            return client.SetExternalAuth(securityIdObject, permission);
        }

        /// <summary>
        /// Returns the current user data configuration of the device.
        /// </summary>
        /// <param name="client">PITreader client instance.</param>
        /// <returns></returns>
        public static Task<ApiResponse<UserDataConfigResponse>> GetUserDataConfig(this PITreaderClient client)
        {
            return client.GetAsync<UserDataConfigResponse>(ConfigUserData);
        }

        /// <summary>
        /// Returns all blocklist entries currently configured in the device.
        /// </summary>
        /// <param name="client">PITreader client instance.</param>
        /// <returns></returns>
        public static Task<ApiResponse<BlocklistResponse>> GetBlocklist(this PITreaderClient client)
        {
            return client.GetAsync<BlocklistResponse>(ConfigBlocklist);
        }

        /// <summary>
        /// Adds, deletes or updates a blocklist entry on the device.
        /// </summary>
        /// <param name="client">PITreader client instance.</param>
        /// <param name="action">Action to be performed.</param>
        /// <param name="entry">Data of the blocklist entry.</param>
        /// <returns></returns>
        public static Task<ApiResponse<GenericResponse>> UpdateBlocklist(this PITreaderClient client, CrudAction action, BlocklistEntry entry)
        {
            return client.PostAsync<GenericResponse, BlocklistCrudRequest>(ConfigBlocklist, new BlocklistCrudRequest { Id = entry.Id, Action = action, Data = entry });
        }

        /// <summary>
        /// Returns all permission list entries currently configured in the device.
        /// </summary>
        /// <param name="client">PITreader client instance.</param>
        /// <returns></returns>
        public static Task<ApiResponse<PermissionListResponse>> GetPermissionList(this PITreaderClient client)
        {
            return client.GetAsync<PermissionListResponse>(ConfigPermissionList);
        }

        /// <summary>
        /// Adds, deletes or updates a permission list entry on the device.
        /// </summary>
        /// <param name="client">PITreader client instance.</param>
        /// <param name="action">Action to be performed.</param>
        /// <param name="entry">Data of the permission list entry.</param>
        /// <returns></returns>
        public static Task<ApiResponse<GenericResponse>> UpdatePermissionList(this PITreaderClient client, CrudAction action, PermissionListEntry entry)
        {
            return client.PostAsync<GenericResponse, PermissionListCrudRequest>(ConfigBlocklist, new PermissionListCrudRequest { Id = entry.Id, Action = action, Data = entry });
        }

        /// <summary>
        /// Uploads a TLS certificate to the PITreader device.
        /// </summary>
        /// <param name="client">PITreader client instance.</param>
        /// <param name="certificate">Stream with certificate data.</param>
        /// <returns></returns>
        public static Task<ApiResponse<GenericResponse>> UploadCertificate(this PITreaderClient client, Stream certificate)
        {
            return client.PostFileAsync<GenericResponse>(FilesCertificate, certificate, "cert.pem", "certFile", null, System.TimeSpan.FromSeconds(30));
        }

        /// <summary>
        /// Request to renew certificate on PITreader device.
        /// </summary>
        /// <param name="client">PITreader client instance.</param>
        /// <param name="scope">Scope of certificate renew request</param>
        /// <returns></returns>
        public static Task<ApiResponse<GenericResponse>> RegenerateCertificate(this PITreaderClient client, CertificateRegenerateScope scope)
        {
            return client.PostAsync<GenericResponse, CertificateRegenerateRequest>(ConfigCertificateRegenerate, new CertificateRegenerateRequest { Scope = scope });
        }

        /// <summary>
        /// Downloads the current OPC UA Server certificate to provided io stream.
        /// </summary>
        /// <param name="client">PITreader client instance.</param>
        /// <param name="stream">IO stream to receive the certificate data.</param>
        /// <returns></returns>
        public static Task<ApiResponse<Stream>> GetOpcUaServerCertificate(this PITreaderClient client, Stream stream)
        {
            return client.GetFileAsync(FilesOpcuaServer, stream);
        }

        /// <summary>
        /// Downloads the current configuration backup to provided io stream.
        /// </summary>
        /// <param name="client">PITreader client instance.</param>
        /// <param name="stream">IO stream to receive the configuration backup.</param>
        /// <returns></returns>
        public static Task<ApiResponse<Stream>> GetConfigurationBackup(this PITreaderClient client, Stream stream)
        {
            return client.GetFileAsync(FilesBackup, stream);
        }

        /// <summary>
        /// Gets a list of all device users.
        /// </summary>
        /// <param name="client">PITreader client instance.</param>
        /// <returns></returns>
        public static Task<ApiResponse<DeviceUsersResponse>> GetDeviceUsers(this PITreaderClient client)
        {
            return client.GetAsync<DeviceUsersResponse>(ConfigDeviceUsers);
        }

        /// <summary>
        /// Gets details of a device user.
        /// </summary>
        /// <param name="client">PITreader client instance.</param>
        /// <param name="name">Name of the device user.</param>
        /// <returns></returns>
        public static Task<ApiResponse<DeviceUserResponse>> GetDeviceUser(this PITreaderClient client, string name)
        {
            return client.GetAsync<DeviceUserResponse>(ConfigDeviceUser, new Dictionary<string, string> { { "name", name } });
        }

        /// <summary>
        /// Creates a new device user with given name.
        /// </summary>
        /// <param name="client">PITreader client instance.</param>
        /// <param name="name">Name of the device user.</param>
        /// <param name="data">Data of the new device user.</param>
        /// <returns></returns>
        public static async Task<ApiResponse<GenericResponse>> CreateDeviceUser(this PITreaderClient client, string name, DeviceUserRequestData data)
        {
            var response = await client.PostAsync<GenericResponse, DeviceUserRequest>(
                ConfigDeviceUser,
                new DeviceUserRequest
                {
                    Action = CrudAction.Create,
                    Id = name
                });

            if (!response.Success)
            {
                return response;
            }

            return await client.PostAsync<GenericResponse, DeviceUserRequest>(
                ConfigDeviceUser,
                new DeviceUserRequest
                {
                    Action = CrudAction.Edit,
                    Id = name,
                    Data = data
                });
        }

        /// <summary>
        /// Update a device user.
        /// </summary>
        /// <param name="client">PITreader client instance.</param>
        /// <param name="name">Name of the device user.</param>
        /// <param name="data">Data of the device user.</param>
        /// <returns></returns>
        public static Task<ApiResponse<GenericResponse>> UpdateDeviceUser(this PITreaderClient client, string name, DeviceUserRequestData data)
        {
            return client.PostAsync<GenericResponse, DeviceUserRequest>(
                ConfigDeviceUser,
                new DeviceUserRequest
                {
                    Action = CrudAction.Edit,
                    Id = name,
                    Data = data
                });
        }

        /// <summary>
        /// Deletes a device user.
        /// </summary>
        /// <param name="client">PITreader client instance.</param>
        /// <param name="name">Name of the device user.</param>
        /// <returns></returns>
        public static Task<ApiResponse<GenericResponse>> DeleteDeviceUser(this PITreaderClient client, string name)
        {
            return client.PostAsync<GenericResponse, DeviceUserRequest>(
                ConfigDeviceUser,
                new DeviceUserRequest { Action = CrudAction.Delete, Id = name });
        }

        /// <summary>
        /// Renews the API token of a device user.
        /// </summary>
        /// <param name="client">PITreader client instance.</param>
        /// <param name="name">Name of the device user.</param>
        /// <returns></returns>
        public static Task<ApiResponse<GenericResponse<ApiTokenResponseData>>> RenewApiToken(this PITreaderClient client, string name)
        {
            return client.PostAsync<GenericResponse<ApiTokenResponseData>, ApiTokenRenewRequest>(ConfigDeviceUserApiToken, new ApiTokenRenewRequest { Name = name });
        }
    }
}
