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

using System.Net;
using Pilz.PITreader.Client;
using Pilz.PITreader.Client.Model;

namespace Pilz.PITreader.CommissioningTool
{
    internal class PITreaderInteractiveClient : PITreaderClient
    {
        public const string DefaultUsername = "admin";

        private const string InteractiveLoginEndpoint = "/api/login";

        private CookieContainer cookies = new CookieContainer();

        /// <inheritdoc/>
        public PITreaderInteractiveClient(string hostnameOrIp = DefaultIp, ushort portNumber = DefaultPortNumber, CertificateValidationDelegate? certificateValidation = null)
            : base(hostnameOrIp, portNumber, certificateValidation)
        {
            this.handler.CookieContainer = this.cookies;
            this.handler.UseCookies = true;
        }

        public async Task<ApiResponse<GenericResponse>> LoginAsync(string username = DefaultUsername, string password = "")
        {
            var request = new HttpRequestMessage(HttpMethod.Post, InteractiveLoginEndpoint);
            var dict = new Dictionary<string, string>
            {
                { "user", username },
                { "password", password }
            };

            request.Content = new FormUrlEncodedContent(dict);
            return await this.ExecuteRequest<GenericResponse>(this.client, request);
        }

        protected override Task<ApiResponse<T>> ExecuteRequest<T>(HttpClient client, HttpRequestMessage message)
        {
            if (message != null && message.Method == HttpMethod.Post)
            {
                var xsrfCookie = (client.BaseAddress == null
                    ? this.cookies.GetAllCookies()
                    : this.cookies.GetCookies(client.BaseAddress)).FirstOrDefault(c => c.Name == "XSRF-TOKEN");

                if (xsrfCookie != null)
                {
                    message.Headers.Add("X-Xsrf-Token", xsrfCookie.Value);
                }
            }

            return base.ExecuteRequest<T>(client, message);
        }
    }
}
