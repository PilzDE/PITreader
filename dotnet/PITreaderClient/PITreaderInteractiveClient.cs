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
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Pilz.PITreader.Client.Model;

namespace Pilz.PITreader.Client
{
    /// <summary>
    /// PITreader API client with interactive logon (i.e. username and password).
    /// </summary>
    public class PITreaderInteractiveClient : PITreaderClient
    {
        /// <summary>
        /// Default user name.
        /// </summary>
        public const string DefaultUsername = "admin";

        private const string InteractiveLoginEndpoint = "/api/login";

        private CookieContainer cookies = new CookieContainer();

        /// <inheritdoc/>
        public PITreaderInteractiveClient(string hostnameOrIp = DefaultIp, ushort portNumber = DefaultPortNumber, CertificateValidationDelegate certificateValidation = null)
            : base(hostnameOrIp, portNumber, certificateValidation)
        {
            this.httpHandler.CookieContainer = this.cookies;
            this.httpHandler.UseCookies = true;
        }

        /// <inheritdoc/>
        public PITreaderInteractiveClient(Uri uri, CertificateValidationDelegate certificateValidation = null)
            : base(uri, certificateValidation)
        {
            this.httpHandler.CookieContainer = this.cookies;
            this.httpHandler.UseCookies = true;
        }

        /// <summary>
        /// Perform interactive login with username and password.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public async Task<ApiResponse<GenericResponse>> LoginAsync(string username = DefaultUsername, string password = "")
        {
            var dict = new Dictionary<string, string>
            {
                { "user", username },
                { "password", password }
            };

            using (var request = new HttpRequestMessage(HttpMethod.Post, InteractiveLoginEndpoint))
            {
                using (var content = new FormUrlEncodedContent(dict))
                {
                    request.Content = content;
                    return await this.ExecuteRequest<GenericResponse>(this.httpClient, request);
                }
            }
        }

        /// <summary>
        /// Overwrite for ExecuteRequest with implementation for XSRF-TOKEN header.
        /// </summary>
        /// <inheritdoc/>
        protected override Task<ApiResponse<T>> ExecuteRequest<T>(HttpClient client, HttpRequestMessage message)
        {
            if (message != null && message.Method == HttpMethod.Post)
            {
                Cookie xsrfCookie = this.cookies.GetCookies(client.BaseAddress)
                    .OfType<Cookie>().FirstOrDefault(c => c.Name == "XSRF-TOKEN");

                if (xsrfCookie != null)
                {
                    message.Headers.Add("X-Xsrf-Token", xsrfCookie.Value);
                }
            }

            return base.ExecuteRequest<T>(client, message);
        }
    }
}
