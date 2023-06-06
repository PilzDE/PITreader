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
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Pilz.PITreader.Client.Model;

namespace Pilz.PITreader.Client
{
    /// <summary>
    /// REST API Client for Pilz PITreader device.
    /// </summary>
    public class PITreaderClient : IDisposable
    {
        /// <summary>
        /// Default IP address of PITreader devices.
        /// </summary>
        public const string DefaultIp = "192.168.0.12";

        /// <summary>
        /// Default port number for HTTPS connections to PITreader devices.
        /// </summary>
        public const ushort DefaultPortNumber = 443;

        private readonly HttpClient client;

        private readonly HttpClientHandler handler;

        private bool disposed;

        private string apiToken;

        /// <summary>
        /// Create a new client instance.
        /// </summary>
        /// <param name="hostnameOrIp">Hostname or IP address of device.</param>
        /// <param name="portNumber">Portnumber for webserver (https)</param>
        /// <param name="certificateValidation">Validator for server certificate, see <see cref="CertificateValidators"/>.</param>
        public PITreaderClient(string hostnameOrIp = DefaultIp, ushort portNumber = DefaultPortNumber, CertificateValidationDelegate certificateValidation = null)
        {
            this.handler = new HttpClientHandler();

            if (certificateValidation != null)
            {
                handler.ServerCertificateCustomValidationCallback = (msg, cert, chain, policyErrors) => certificateValidation(msg, cert, chain, policyErrors);
            }

            this.client = this.CreateClient(hostnameOrIp, portNumber, true);
            this.client.Timeout = TimeSpan.FromSeconds(5);
        }

        /// <summary>
        /// Gets or sets the API token for connections to the PITreader device.
        /// </summary>
        public string ApiToken
        {
            get { return this.apiToken; }
            set
            {
                if (value != this.apiToken)
                {
                    this.apiToken = value;
                    if (!string.IsNullOrEmpty(this.apiToken))
                    {
                        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.apiToken);
                    }
                    else
                    {
                        this.client.DefaultRequestHeaders.Remove("Authorization");
                    }
                }
            }
        }

        /// <summary>
        /// Returns the certificate of a PITreader device.
        /// </summary>
        /// <param name="hostnameOrIp">Hotname or ip address of the PITreader device.</param>
        /// <param name="portNumber">Port number for HTTPS connections to the PITreader device.</param>
        /// <returns></returns>
        public static async Task<X509Certificate2> GetDeviceCertificateAsync(string hostnameOrIp = DefaultIp, ushort portNumber = DefaultPortNumber)
        {
            X509Certificate2 certificate = null;
            using (var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (msg, cert, chain, policyErrors) => { certificate = new X509Certificate2(cert.GetRawCertData()); return true; }
            })
            {
                using (var client = new HttpClient(handler))
                {
                    client.BaseAddress = new Uri($"https://{hostnameOrIp}:{portNumber}");
                    await client.GetAsync("/");
                }
            }

            return certificate;
        }

        /// <summary>
        /// Executes an HTTP GET request to the given endpoint.
        /// </summary>
        /// <typeparam name="TResponse">Type of the response data object.</typeparam>
        /// <param name="url">Endpoint</param>
        /// <returns></returns>
        public async Task<ApiResponse<TResponse>> GetAsync<TResponse>(string url) where TResponse : class
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            return await this.ExecuteRequest<TResponse>(this.client, request);
        }

        /// <summary>
        /// Executes an HTTP POST request to the given endpoint.
        /// </summary>
        /// <typeparam name="TResponse">Type of the response data object.</typeparam>
        /// <typeparam name="TRequest">Type of the request data object.</typeparam>
        /// <param name="url">Endpoint</param>
        /// <param name="data">Request data</param>
        /// <returns></returns>
        public async Task<ApiResponse<TResponse>> PostAsync<TResponse, TRequest>(string url, TRequest data) where TResponse : class
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            if (data != null)
            {
                string serialized = PITreaderJsonSerializer.Serialize(data);
                request.Content = new StringContent(serialized, Encoding.UTF8, "application/json");
            }

            return await this.ExecuteRequest<TResponse>(this.client, request);
        }

        /// <summary>
        /// Executes an HTTP POST request to the given endpoint.
        /// </summary>
        /// <typeparam name="TResponse">Type of the response data object.</typeparam>
        /// <param name="url">Endpoint</param>
        /// <returns></returns>
        public async Task<ApiResponse<TResponse>> PostAsync<TResponse>(string url) where TResponse : class
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            return await this.ExecuteRequest<TResponse>(this.client, request);
        }

        /// <summary>
        /// Executes an HTTP POST request to the given endpoint uploading a file.
        /// </summary>
        /// <typeparam name="TResponse">Type of the response data object.</typeparam>
        /// <param name="url">Endpoint</param>
        /// <param name="file">File contents</param>
        /// <param name="fileName">File name</param>
        /// <param name="fieldName">Field name</param>
        /// <param name="timeout">Request timeout</param>
        /// <returns></returns>
        public async Task<ApiResponse<TResponse>> PostFileAsync<TResponse>(string url, Stream file, string fileName, string fieldName, TimeSpan timeout) where TResponse : class
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url);

            string boundary = Guid.NewGuid().ToString();
            using (var multipartFormDataContent = new MultipartFormDataContent(boundary))
            {
                var fileData = new StreamContent(file);

                // Generate custom Content-Disposition header, to prevent additional parameter "filename*="
                var cdHeader = new ContentDispositionHeaderValue("form-data")
                {
                    Name = $"\"{fieldName}\"",
                    FileName = $"\"{fileName}\""
                };

                fileData.Headers.ContentDisposition = cdHeader;
                fileData.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                multipartFormDataContent.Add(fileData, fieldName, fileName);

                // Cleanup quotes from bondary parameter
                var boundaryParam = multipartFormDataContent.Headers.ContentType?.Parameters.SingleOrDefault(p => p.Name == "boundary");
                if (boundaryParam != null)
                {
                    boundaryParam.Value = boundary;
                }

                request.Content = multipartFormDataContent;

                using (var client = this.CreateClient(
                    this.client.BaseAddress.Host, 
                    (ushort)this.client.BaseAddress.Port,
                    false))
                {
                    client.Timeout = timeout;
                    return await this.ExecuteRequest<TResponse>(client, request);
                }
            }
        }

        private async Task<ApiResponse<T>> ExecuteRequest<T>(HttpClient client, HttpRequestMessage message) where T : class
        {
            try
            {
                using (var response = await client.SendAsync(message))
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        if (response.Content == null)
                        {
                            return ApiResponse<T>.Error("Response content was null", true, ResponseCode.Ok);
                        }

                        T data = await response.Content.ReadFromJsonAsync<T>();
                        return ApiResponse<T>.Ok(data);
                    }

                    if (response.StatusCode != HttpStatusCode.NotFound)
                    {
                        // HTTP 404 is returned with a default HTML error page -> no JSON
                        return ApiResponse<T>.Error(response.StatusCode, ((int)response.StatusCode) % 100 != 4, await response.Content.ReadFromJsonAsync<GenericResponse>());
                    }

                    return ApiResponse<T>.Error("Endpoint not found", false);
                }
            }
            catch (HttpRequestException exception)
            {
                return ApiResponse<T>.Error("HttpRequestException calling the API: " + exception.Message, false);
            }
            catch (JsonException exception)
            {
                return ApiResponse<T>.Error("Error in JSON deserialization: " + exception.Message, true);
            }
            catch (NotImplementedException exception)
            {
                return ApiResponse<T>.Error("Error in JSON deserialization: " + exception.Message, true);
            }
            catch (TimeoutException exception)
            {
                return ApiResponse<T>.Error("TimeoutException during call to API: " + exception.Message, true);
            }
            catch (OperationCanceledException exception)
            {
                return ApiResponse<T>.Error("Task was canceled during call to API: " + exception.Message, true);
            }
            catch (Exception exception)
            {
                return ApiResponse<T>.Error("Unhandled exception when calling the API: " + exception.Message, false);
            }
        }

        private HttpClient CreateClient(string hostnameOrIp, ushort portNumber, bool disposeHandler)
        {
            var client = new HttpClient(this.handler, disposeHandler)
            {
                BaseAddress = new Uri($"https://{hostnameOrIp}:{portNumber}"),
            };

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json", 1));

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.apiToken);

            return client;
        }

        /// <summary>
        /// Implementation of the dispose functionality.
        /// The disposing parameter should be false when called from a finalizer, and true when called from the IDisposable.Dispose method. In other words, it is true when deterministically called and false when non-deterministically called.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    this.client.Dispose();
                    this.handler.Dispose();
                }

                disposed = true;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
