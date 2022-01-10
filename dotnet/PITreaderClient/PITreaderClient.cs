using Pilz.PITreader.Client.Model;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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

            this.client = new HttpClient(this.handler)
            {
                BaseAddress = new Uri($"https://{hostnameOrIp}:{portNumber}"),
            };

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json", 1));

            client.Timeout = TimeSpan.FromSeconds(5);
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
        public async Task<ApiResponse<TResponse>> GetAsync<TResponse>(string url) where TResponse: class
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            return await this.ExecuteRequest<TResponse>(request);
        }

        /// <summary>
        /// Executes an HTTP POST request to the given endpoint.
        /// </summary>
        /// <typeparam name="TResponse">Type of the response data object.</typeparam>
        /// <typeparam name="TRequest">Type of the request data object.</typeparam>
        /// <param name="url">Endpoint</param>
        /// <param name="data">Request data</param>
        /// <returns></returns>
        public async Task<ApiResponse<TResponse>> PostAsync<TResponse, TRequest>(string url, TRequest data) where TResponse: class
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            if (data != null)
            {
                string serialized = PITreaderJsonSerializer.Serialize(data);
                request.Content = new StringContent(serialized, Encoding.UTF8, "application/json");
            }

            return await this.ExecuteRequest<TResponse>(request);
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
            return await this.ExecuteRequest<TResponse>(request);
        }

        private async Task<ApiResponse<T>> ExecuteRequest<T>(HttpRequestMessage message) where T : class
        {
            try
            {
                using (var response = await this.client.SendAsync(message))
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
