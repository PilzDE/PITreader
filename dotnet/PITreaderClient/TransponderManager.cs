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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pilz.PITreader.Client.Model;

namespace Pilz.PITreader.Client
{
    /// <summary>
    /// Manager for transponder data
    /// </summary>
    public class TransponderManager
    {
        private readonly PITreaderClient client;

        /// <summary>
        /// Creates a new TransponderManager object instance.
        /// </summary>
        /// <param name="client">Reference to a PITreaderClient object.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public TransponderManager(PITreaderClient client)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
        }

        /// <summary>
        /// Queries complete data set from a transponder.
        /// </summary>
        /// <returns></returns>
        public async Task<TransponderDataContainer> GetTransponderDataAsync()
        {
            var dataResponse = await this.client.GetTransponderData();
            if (!dataResponse.Success) throw new InvalidOperationException(dataResponse.ErrorData?.Message);

            if (dataResponse.Data.SecurityId == null) return null;

            var genericResponse = await this.client.PostAsync<GenericResponse>(ApiEndpoints.TransponderUserDataRead);
            if (!genericResponse.Success) throw new InvalidOperationException(genericResponse.ErrorData?.Message);

            var userDataResponse = await this.client.GetAsync<UserDataResponse>(ApiEndpoints.TransponderUserData);
            if (!userDataResponse.Success && userDataResponse.ResponseCode != ResponseCode.DeviceError) throw new InvalidOperationException(userDataResponse.ErrorData?.Message);

            return new TransponderDataContainer
            {
                StaticData = dataResponse.Data,
                UserData = userDataResponse.Data
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<GenericResponse> WriteDataToTransponderAsync(TransponderDataContainer data)
        {
            if (data is null || data.StaticData is null)
                throw new ArgumentNullException(nameof(data));

            var staticData = new TransponderRequest
            {
                Permissions = data.StaticData.Permissions,
                TimeLimitationStart = data.StaticData.TimeLimitationStart ?? DateTime.MinValue,
                TimeLimitationEnd = data.StaticData.TimeLimitationEnd ?? DateTime.MaxValue,
                CodingLock = data.StaticData.CodingLock
            };

            var dataResponse = await this.client.PostAsync<GenericResponse, TransponderRequest>(ApiEndpoints.Transponder, staticData);
            if (!dataResponse.Success) return dataResponse.ErrorData;

            if (data.UserData != null)
            {
                if (data.UserData.ParameterDefintion == null)
                    throw new ArgumentException("No ParameterDefinition for user data.", nameof(data));

                var genericResponse = await this.client.PostAsync<GenericResponse>(ApiEndpoints.TransponderUserDataClear);
                if (!genericResponse.Success) return genericResponse.ErrorData;

                foreach (var group in data.UserData.Groups)
                {
                    var dataRequest = this.GetRequest(group, data.UserData.ParameterDefintion.Parameters);
                    if (dataRequest.Values.Count > 0)
                    {
                        genericResponse = await this.client.PostAsync<GenericResponse, UserDataGroupRequest>(ApiEndpoints.TransponderUserDataAddGroupValues, dataRequest);
                        if (!genericResponse.Success) return genericResponse.ErrorData;
                    }
                }

                genericResponse = await this.client.PostAsync<GenericResponse>(ApiEndpoints.TransponderUserDataWrite);
                if (!genericResponse.Success) return genericResponse.ErrorData;
            }

            return null;
        }

        /// <summary>
        /// Exports the current user data configuration to a file in JSON format.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="path">Path to the file.</param>
        /// <returns></returns>
        public bool ExportToFile(TransponderDataContainer data, string path)
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data));

            string json = PITreaderJsonSerializer.Serialize(data);
            using (var file = File.Open(path, FileMode.Create, FileAccess.Write))
            {
                byte[] bytes = Encoding.UTF8.GetBytes(json);
                file.Write(bytes, 0, bytes.Length);
                file.Flush();
                file.Close();
            }

            return true;
        }

        /// <summary>
        /// Exports the current user data configuration to a file in JSON format.
        /// </summary>
        /// <param name="path">Path to the file.</param>
        /// <returns></returns>
        public TransponderDataContainer ImportFromFile(string path)
        {
            using (var file = File.OpenText(path))
            {
                return PITreaderJsonSerializer.Deserialize<TransponderDataContainer>(file.ReadToEnd());
            }
        }

        private UserDataGroupRequest GetRequest(UserDataGroupResponse entry, IList<UserDataParameter> parameters)
        {
            if (entry is null) throw new ArgumentNullException(nameof(entry));
            if (parameters is null) throw new ArgumentNullException(nameof(parameters));

            var data = new UserDataGroupRequest { DeviceGroup = entry.DeviceGroup, Values = new List<UserDataValueRequest>(entry.Values.Count) };

            foreach (var value in entry.Values)
            {
                var parameter = parameters.Single(p => p.Id == value.Id);
                data.Values.Add(new UserDataValueRequest
                {
                    Id = value.Id,
                    Type = parameter.Type,
                    Size = parameter.Size,
                    NumericValue = value.NumericValue,
                    StringValue = value.StringValue
                });
            }

            return data;
        }
    }
}
