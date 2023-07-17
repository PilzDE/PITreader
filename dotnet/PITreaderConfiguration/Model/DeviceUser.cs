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
using System.Security.Cryptography;

namespace Pilz.PITreader.Configuration.Model
{
    public class DeviceUser
    {
        public string Name { get; set; }

        public DeviceUserState State { get; set; }

        public DeviceUserRole Role { get; set; }

        public DeviceUserAuthType AuthenticationType { get; set; }

        public byte[] PasswordHash { get; set; }

        public string ApiToken { get; set; }

        public string RemoteIp { get; set; }

        public static DeviceUser CreateNew(string name)
        {
            var user = new DeviceUser
            {
                Name = name,
                State = DeviceUserState.Active,
                Role = DeviceUserRole.Readonly,
                AuthenticationType = DeviceUserAuthType.ApiToken,
                PasswordHash = new byte[48],
                RemoteIp = string.Empty,
            };

            var apiToken = new byte[16];

            var rnd = new RNGCryptoServiceProvider();
            rnd.GetBytes(user.PasswordHash);
            rnd.GetBytes(apiToken);

            user.ApiToken = Convert.ToBase64String(apiToken);
            return user;
        }

        public void RegenerateApiToken()
        {
            var apiToken = new byte[16];

            var rnd = new RNGCryptoServiceProvider();
            rnd.GetBytes(apiToken);

            this.ApiToken = Convert.ToBase64String(apiToken);
        }

        public void SetPassword(string password)
        {
            var salt = new byte[16];

            var rnd = new RNGCryptoServiceProvider();
            rnd.GetBytes(salt);

            var pkbf2 = new Rfc2898DeriveBytes(password, salt, 1024, HashAlgorithmName.SHA256);
            var hash = pkbf2.GetBytes(32);

            salt.CopyTo(this.PasswordHash, 0);
            hash.CopyTo(this.PasswordHash, 16);
        }
    }
}
