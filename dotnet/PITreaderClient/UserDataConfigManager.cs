using Pilz.PITreader.Client.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Pilz.PITreader.Client
{
    /// <summary>
    /// Manager for user data configuration.
    /// </summary>
    public class UserDataConfigManager
    {
        private readonly PITreaderClient client;

        /// <summary>
        /// Creates a new UserDataConfigManager object instance.
        /// </summary>
        /// <param name="client">Reference to a PITreaderClient object.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public UserDataConfigManager(PITreaderClient client)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
        }

        /// <summary>
        /// Applies the user data configuration to the device.
        /// </summary>
        /// <param name="version">Version number, must be larger than 0 if a comment is provided</param>
        /// <param name="comment">Comment for the version</param>
        /// <param name="parameters">List of parameters</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">version == 0, but a comment is provided</exception>
        public async Task<ApiResponse<GenericResponse>> ApplyConfigurationAsync(ushort version, string comment, IEnumerable<UserDataParameter> parameters)
        {
            if (version < 1 && !string.IsNullOrWhiteSpace(comment)) throw new ArgumentException("Version must be >= 1", nameof(version));

            ApiResponse<GenericResponse> response = await this.client.PostAsync<GenericResponse>(ApiEndpoints.ConfigUserDataReset);
            if (!response.Success) return response;

            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    response = await this.client.PostAsync<GenericResponse, UserDataParameter>(ApiEndpoints.ConfigUserDataParameter, parameter);
                    if (!response.Success) return response;
                }
            }

            if (version > 0)
            {
                response = await this.client.PostAsync<GenericResponse, UserDataVersionRequest>(
                    ApiEndpoints.ConfigUserDataVersion, 
                    new UserDataVersionRequest { Version = version, Comment = comment ?? string.Empty });
            }

            return response;
        }

        /// <summary>
        /// Migrates user data configuration on the device with keeping the order.
        /// </summary>
        /// <param name="version">Version number, must be larger than 0 if a comment is provided</param>
        /// <param name="comment">Comment for the version</param>
        /// <param name="parameters">List of parameters</param>
        /// <param name="dontDeleteParamaters">if <c>true</c>, no parameters are deleted on the device.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">version == 0, but a comment is provided</exception>
        public async Task<ApiResponse<GenericResponse>> MigrateConfigurationAsync(ushort? version, string comment, IEnumerable<UserDataParameter> parameters, bool dontDeleteParamaters = false)
        {
            var config = await this.GetConfigurationAsync();
            if (version < 1 && !string.IsNullOrWhiteSpace(comment)) throw new ArgumentException("Version must be >= 1", nameof(version));

            ApiResponse<GenericResponse> response = await this.client.PostAsync<GenericResponse>(ApiEndpoints.ConfigUserDataReset);
            if (!response.Success) return response;

            if (parameters != null)
            {
                // TODO
                ////foreach (var parameter in parameters)
                ////{
                ////    response = await this.client.PostAsync<GenericResponse, UserDataParameter>("/api/config/userData/parameter", parameter);
                ////    if (!response.Success) return response;
                ////}
            }

            if (!version.HasValue || version == 0)
            {
                version = config.Data.Version;
            }

            if (string.IsNullOrEmpty(comment))
            {
                comment = config.Data.Comment;
            }

            if (version.HasValue && version > 0)
            {
                response = await this.client.PostAsync<GenericResponse, UserDataVersionRequest>(
                    ApiEndpoints.ConfigUserDataVersion,
                    new UserDataVersionRequest { Version = version.Value, Comment = comment ?? string.Empty });
            }

            return response;
        }

        /// <summary>
        /// Returns the user data configuration from the device.
        /// </summary>
        /// <returns>The user data configuration from the device.</returns>
        public Task<ApiResponse<UserDataConfigResponse>> GetConfigurationAsync()
        {
            return this.client.GetUserDataConfig();
        }

        /// <summary>
        /// Applies the user data configuration to the device.
        /// </summary>
        /// <param name="userDataConfig"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public Task<ApiResponse<GenericResponse>> ApplyConfigurationAsync(UserDataParamaterDefintionResponse userDataConfig)
        {
            if (userDataConfig is null) throw new ArgumentNullException(nameof(userDataConfig));

            var parameters = userDataConfig.Parameters.Select((p, i) => new UserDataParameter { Id = p.Id, Name = $"Parameter {i + 1}", Type = p.Type, Size = p.Size }).ToList();
            return this.ApplyConfigurationAsync(userDataConfig.Version, string.Empty, parameters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceParameters"></param>
        /// <param name="transponderParameters"></param>
        /// <returns></returns>
        public static bool AreConfigurationsMatching(IList<UserDataParameter> deviceParameters, UserDataParamaterDefintionResponse transponderParameters)
        {
            if (deviceParameters is null) throw new ArgumentNullException(nameof(deviceParameters));
            if (transponderParameters is null) throw new ArgumentNullException(nameof(transponderParameters));

            if (deviceParameters.Count != (transponderParameters?.Parameters.Count ?? 0)) return false;

            foreach (var parameter in deviceParameters)
            {
                var xpndrParameter = transponderParameters.Parameters.SingleOrDefault(p => p.Id == parameter.Id);
                if (xpndrParameter == null) return false;

                if (parameter.Type != xpndrParameter.Type || parameter.Size != xpndrParameter.Size)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Exports the current user data configuration to a file in JSON format.
        /// </summary>
        /// <param name="path">Path to the file.</param>
        /// <returns></returns>
        public async Task<bool> ExportToFile(string path)
        {
            var response = await this.client.GetUserDataConfig();
            if (!response.Success) return false;

            string json = PITreaderJsonSerializer.Serialize(response.Data);
            using (var file = File.OpenWrite(path))
            {
                byte[] data = Encoding.UTF8.GetBytes(json);
                file.Write(data, 0, data.Length);
                file.Flush();
                file.Close();
            }

            return true;
        }
    }
}
