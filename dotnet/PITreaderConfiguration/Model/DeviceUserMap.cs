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
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;

namespace Pilz.PITreader.Configuration.Model
{
    internal class DeviceUserMap : ClassMap<DeviceUser>
    {
        public DeviceUserMap()
        {
            Map(m => m.Name).Name("Name");

            Map(m => m.State).Name("State").TypeConverter(new EnumNumberConverter(typeof(DeviceUserState)));

            Map(m => m.Role).Name("Role").TypeConverter(new EnumNumberConverter(typeof(DeviceUserRole)));

            Map(m => m.AuthenticationType).Name("Authentication Type").TypeConverter(new EnumNumberConverter(typeof(DeviceUserAuthType)));

            ConvertFromString<byte[]> passwordHashFromString = s =>
            {
                string dataString = s.Row.GetField("Password Hash");

                byte[] data = new byte[dataString.Length / 2];
                for (int index = 0; index < dataString.Length; index += 2)
                {
                    data[index / 2] = Convert.ToByte(dataString.Substring(index, 2), 16);
                }

                return data;
            };

            ConvertToString<DeviceUser> passwordHashToString = a =>
            {
                return string.Join(string.Empty, a.Value.PasswordHash.Select(d => d.ToString("X2")));
            };

            Map(m => m.PasswordHash).Name("Password Hash")
                .Convert(passwordHashFromString)
                .Convert(passwordHashToString);

            ConvertFromString<string> apiTokenFromString = s =>
            {
                string dataString = s.Row.GetField("API Token");
                
                byte[] data = new byte[dataString.Length / 2];
                for (int index = 0; index < dataString.Length; index += 2)
                {
                    data[index / 2] = Convert.ToByte(dataString.Substring(index, 2), 16);
                }

                return Convert.ToBase64String(data);
            };

            ConvertToString<DeviceUser> apiTokenToString = a =>
            {
                byte[] data = Convert.FromBase64String(a.Value.ApiToken);
                return string.Join(string.Empty, data.Select(d => d.ToString("X2")));
            };

            Map(m => m.ApiToken).Name("API Token")
                .Convert(apiTokenFromString)
                .Convert(apiTokenToString);

            Map(m => m.RemoteIp).Name("Remote IP");
        }
    }
}
