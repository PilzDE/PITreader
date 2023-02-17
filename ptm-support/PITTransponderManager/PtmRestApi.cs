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

using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Pilz.PITreader.PitTransponderManager.ApiModel;
using Pilz.PITreader.PitTransponderManager.Model;

namespace Pilz.PITreader.PitTransponderManager
{
    /// <summary>
    /// Wrapper for PIT Transponder Manager server REST API.
    /// </summary>
    public class PtmRestApi
    {
        private readonly HttpClient client;

        /// <summary>
        /// Base URL for default installation
        /// </summary>
        public const string BaseUrl = "http://127.0.0.1:8090/";

        public PtmRestApi()
        {
            this.client = new HttpClient
            {
                BaseAddress = new Uri(BaseUrl)
            };
        }

        /// <summary>
        /// Get the export file from PIT Transponder Manager
        /// </summary>
        /// <returns>The export file or null</returns>
        public async Task<PtmExportContainer?> GetExportAsync()
        {
            var response = await this.client.GetAsync("export");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsByteArrayAsync();
                var json = Encoding.UTF8.GetString(data);
                return JsonSerializer.Deserialize(json, PtmJsonContext.Default.PtmExportContainer);
            }

            return null;
        }

        /// <summary>
        /// Performs an import to the PIT Transponder Manager tool.
        /// </summary>
        /// <param name="data">Combined data</param>
        /// <param name="overrideDatabase">Set to true, to override existing data in the database</param>
        /// <returns>The API result.</returns>
        public async Task<ApiResult?> DoImportAsync(PtmExportContainer data, bool overrideDatabase)
        {
            HttpResponseMessage response;
            using (var message = new MultipartFormDataContent())
            {
                message.Add(new ByteArrayContent(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data, PtmJsonContext.Default.PtmExportContainer))), "importFile", "ldapSyncImport.ptm");
                response = await this.client.PostAsync("import?overrideDatabase=" + overrideDatabase, message);

                return await response.Content.ReadFromJsonAsync(PtmJsonContext.Default.ApiResult);
            }
        }
    }
}
