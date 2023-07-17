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

using System.CommandLine;
using Pilz.PITreader.Configuration.Model;
using Pilz.PITreader.Configuration;

namespace Pilz.PITreader.CommissioningTool.Tasks
{
    internal class CommissioningUserTask : ICommissioningTask
    {
        public Option<string?> CommissioningUserOption = new Option<string?>(new[] { "--commissioning-user" }, "User created during commissioning process to access the system")
        {
            ArgumentHelpName = "name"
        };

        public Option[] Options => new Option[] { this.CommissioningUserOption };

        public bool CheckOptions(TaskContext context)
        {
            return true;
        }

        public Task RunAsync(TaskContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            string? commissioningUser = context.GetValue(this.CommissioningUserOption);
            if (!string.IsNullOrEmpty(commissioningUser))
            {
                var userFile = context.ConfigurationFile.Get<DeviceUserFile>();
                if (userFile == null)
                {
                    userFile = new DeviceUserFile();
                    context.ConfigurationFile.Add(userFile);
                }

                context.ApiUser = new CommissioningUser(commissioningUser)
                {
                    InteractiveLogin = false
                };

                var apiUser = userFile.Users.FirstOrDefault(u => u.Name == commissioningUser);
                if (apiUser == null)
                {
                    apiUser = DeviceUser.CreateNew(commissioningUser);
                    apiUser.Role = DeviceUserRole.Administrator;
                    apiUser.AuthenticationType = DeviceUserAuthType.ApiToken;

                    userFile.Users.Add(apiUser);
                    context.ApiUser.DeleteOnCleanup = true;
                }

                context.ApiUser.PasswordOrApiToken = apiUser.ApiToken;

                context.LogInfo($"Added new user for commissioning: \"{commissioningUser}\"");
            }

            return Task.CompletedTask;
        }
    }
}
