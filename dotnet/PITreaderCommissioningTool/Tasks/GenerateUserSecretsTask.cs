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
    internal class GenerateUserSecretsTask : ICommissioningTask
    {
        private Option<IEnumerable<string>> generatePasswordOption = new Option<IEnumerable<string>>(new[] { "--generate-password" }, "Names of user for which passwords should be generated")
        {
            ArgumentHelpName = "name1 [name2 [name3]]",
            Arity = ArgumentArity.ZeroOrMore,
            AllowMultipleArgumentsPerToken = true
        };

        private Option<IEnumerable<string>> generateApiTokenOption = new Option<IEnumerable<string>>(new[] { "--generate-api-token" }, "Names of user for which API tokens should be generated")
        {
            ArgumentHelpName = "name1 [name2 [name3]]",
            Arity = ArgumentArity.ZeroOrMore,
            AllowMultipleArgumentsPerToken = true
        };

        public Option[] Options => new Option[] { this.generatePasswordOption, this.generateApiTokenOption };

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

            IEnumerable<string> generatePassword = context.GetValue(this.generatePasswordOption) ?? Enumerable.Empty<string>();
            IEnumerable<string> generateApiToken = context.GetValue(this.generateApiTokenOption) ?? Enumerable.Empty<string>();

            if (generatePassword != null && generatePassword.Any(x => !string.IsNullOrWhiteSpace(x)))
            {
                var userFile = context.ConfigurationFile.Get<DeviceUserFile>();
                if (userFile == null)
                {
                    context.LogError("No user file in config restore for generating passwords for user.");
                    return Task.CompletedTask;
                }

                foreach (string userName in generatePassword.Where(x => !string.IsNullOrWhiteSpace(x)))
                {
                    DeviceUser? user = userFile.Users.FirstOrDefault(u => u.Name == userName);
                    if (user == null)
                    {
                        context.LogError($"No user named \"{userName}\" in user file in config restore for generating password.");
                    }
                    else
                    {
                        string password = PasswordGenerator.GenerateSecurePassword(12);
                        user.SetPassword(password);
                        context.LogInfo($"Generated new password for user {userName}: {password}");
                    }
                }
            }

            if (generateApiToken != null && generateApiToken.Any(x => !string.IsNullOrWhiteSpace(x)))
            {
                var userFile = context.ConfigurationFile.Get<DeviceUserFile>();
                if (userFile == null)
                {
                    context.LogError("No user file in config restore for generating passwords for user.");
                    return Task.CompletedTask;
                }

                foreach (string userName in generateApiToken.Where(x => !string.IsNullOrWhiteSpace(x)))
                {
                    DeviceUser? user = userFile.Users.FirstOrDefault(u => u.Name == userName);
                    if (user == null)
                    {
                        context.LogError($"No user named \"{userName}\" in user file in config restore for generating API token.");
                    }
                    else
                    {
                        user.RegenerateApiToken();
                        context.LogInfo($"Generated new API token for user {userName}: {user.ApiToken}");
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}
