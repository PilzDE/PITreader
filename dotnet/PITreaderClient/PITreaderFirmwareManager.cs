// Copyright (c) 2023 Pilz GmbH & Co. KG
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
using System.Threading.Tasks;
using Pilz.PITreader.Client.Model;

namespace Pilz.PITreader.Client
{
    /// <summary>
    /// Checks and manages firmware versions and updates
    /// </summary>
    public class PITreaderFirmwareManager
    {
        private readonly PITreaderClient client;

        /// <summary>
        /// Creates a new firmware manager instance.
        /// </summary>
        /// <param name="client">A PITreader API client instance.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public PITreaderFirmwareManager(PITreaderClient client)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
        }

        /// <summary>
        /// Reads the current firmware version from the PITreader device.
        /// </summary>
        /// <returns>The current firmware version running on the device.</returns>
        public async Task<FirmwareVersion> GetPITreaderFirmwareVersionAsync()
        {
            var statusResponse = await this.client.GetDeviceStatus();

            if (statusResponse?.Data?.FirmwareVersion == null)
            {
                return null;
            }

            return new FirmwareVersion(statusResponse.Data.FirmwareVersion)
            {
                Released = statusResponse.Data.Released
            };
        }

        /// <summary>
        /// Uploads a firmware package to the device and applies the update.
        /// </summary>
        /// <param name="package">The firmware update package</param>
        /// <param name="force">If true, the update is started also the device reports it already has the same or a newer firmware.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<PITreaderFirmwareUpdateResult> PerformUpdateAsync(PITreaderFirmwarePackage package, bool force = false)
        {
            if (package is null)
            {
                throw new ArgumentNullException(nameof(package));
            }
 
            var currentVersion = await this.GetPITreaderFirmwareVersionAsync();

            if (!force && currentVersion != null && currentVersion >= package.PackageVersion)
            {
                return PITreaderFirmwareUpdateResult.AlreadyInstalledOrNewer;
            }

            var fileResult = await this.client.PostFileAsync<GenericResponse>(
                ApiEndpoints.FilesFirmware, 
                package.Data, 
                package.FileName, 
                "fwFile",
                TimeSpan.FromMinutes(5));

            string hash = fileResult?.Data?.Data?.AsValue()?.GetValue<string>();
            if (!fileResult.Success || hash == null)
            {
                if (fileResult.ErrorData?.Message == "API.errorFwFileIsOutdated")
                {
                    return PITreaderFirmwareUpdateResult.AlreadyInstalledOrNewer;
                }

                return PITreaderFirmwareUpdateResult.UploadError;
            }

            var updateResponse = await this.client.PostAsync<GenericResponse, FirmwareUpdateRequest>(
                ApiEndpoints.FirmwareUpdate, 
                new FirmwareUpdateRequest { Action = FirmwareUpdateAction.Confirm, Hash = hash });

            return updateResponse.Success 
                ? PITreaderFirmwareUpdateResult.Success 
                : PITreaderFirmwareUpdateResult.UpdateError;
        }
    }
}
