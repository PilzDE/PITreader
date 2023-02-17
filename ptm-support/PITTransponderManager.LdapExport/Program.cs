// Copyright (c) 2026 Pilz GmbH & Co. KG
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

﻿using System.DirectoryServices.Protocols;
using Pilz.PITreader.PitTransponderManager.Model;

namespace Pilz.PITreader.PitTransponderManager.LdapExport
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Missing output file as an argument");
                Environment.Exit(1);
                return;
            }

            string outputPath = args[0];

            if (outputPath == "-online" || outputPath == "--online")
            {
                using (var appController = new PtmApplication())
                {
                    if (!appController.IsRunning)
                    {
                        appController.Start();
                        appController.WaitForRunning(TimeSpan.FromSeconds(15));
                    }

                    var client = new PtmRestApi();
                    var data = await client.GetExportAsync();

                    var outputData = DoSync(data);
                    var apiResult = await client.DoImportAsync(outputData, true);

                    appController.Close();

                    if (apiResult?.ImportErrorMessages?.Any() == true)
                    {
                        Console.WriteLine("Error importing into PTM: " + string.Join(", ", apiResult.ImportErrorMessages.Select(m => m.ErrorMessage)));
                        Environment.Exit(1);
                    }
                }
            }
            else
            {
                PtmExportContainer? inputData = null;
                if (args.Length > 1)
                {
                    string? inputPath = args[1];
                    if (!string.IsNullOrEmpty(inputPath) && File.Exists(inputPath))
                    {
                        inputData = PtmSerializer.Load(inputPath);                        
                    }
                }

                var outputData = DoSync(inputData);
                PtmSerializer.Save(outputData, outputPath);
            }
        }

        private static PtmExportContainer DoSync(PtmExportContainer? inputData)
        {
            var config = new Configuration();

            if (string.IsNullOrEmpty(config.LdapServer))
            {
                Console.WriteLine("Missing LDAP server in config.ini");
                Environment.Exit(1);
                return null;
            }

            if (string.IsNullOrEmpty(config.LdapBaseDN))
            {
                Console.WriteLine("Missing BaseDN in config.ini");
                Environment.Exit(1);
                return null;
            }

            string[] attributes = config.GetAllAttributes();
            if (attributes.Length == 0)
            {
                Console.WriteLine("Missing mapping for user attributes in config.ini");
                Environment.Exit(1);
                return null;
            }

            ushort port;
            ushort.TryParse(config.LdapPort, out port);

            var container = new PtmExportContainer();

            if (inputData != null)
            {
                container.Version.Clear();
                container.Version.AddRange(inputData.Version);
            }

            try
            {
                using (var connector = new LdapConnector(config.LdapServer, port == 0 ? null : port, config.LdapUser, config.LdapPassword))
                {
                    foreach (var entry in connector.PagedSearch(
                        100,
                        config.LdapFilter,
                        config.LdapBaseDN,
                        attributes).SelectMany(s => s.OfType<SearchResultEntry>()))
                    {
                        container.Users.Add(CreateUser(entry, config));
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("Error reading users from LDAP: " + exception.Message);
                Environment.Exit(1);
                return null;
            }

            try
            {
                if (inputData != null) 
                { 
                    foreach (var user in inputData.Users ?? new List<User>())
                    {
                        if (!container.Users.Any(u => u.Email == user.Email))
                        {
                            container.Users.Add(user);
                            user.Active = false;

                            if (config?.BlocklistOnDisable == "true")
                            {
                                var associatedTransponders = (inputData.Transponders ?? new List<Transponder>())
                                    .Where(t => t.User == user.Email);

                                foreach (var transponder in associatedTransponders)
                                {
                                    if (!inputData.Blocklists.SelectMany(b => b.Entries).Any(e => e.Key?.SerialNo == transponder.SerialNo))
                                    {
                                        Blocklist? list = container.Blocklists.FirstOrDefault();
                                        if (list == null)
                                        {
                                            list = inputData.Blocklists.FirstOrDefault() ?? new Blocklist { Name = "Blocklist" };
                                            container.Blocklists.Add(list);
                                        }

                                        list.AddEntry(transponder.SerialNo, "Disabled " + DateTime.Now.ToShortDateString());
                                    }
                                }
                            }
                        }
                    }
                }

                return container;
            }
            catch (Exception exception)
            {
                Console.WriteLine("Error generating PTM export file: " + exception.Message);
                Environment.Exit(1);
                return null;
            }
        }

        private static User CreateUser(SearchResultEntry entry, Configuration configuration)
        {
            Func<string[], Func<SearchResultAttributeCollection?, string, string?>, string?> getMappedValue = (s, f) =>
            {
                if (s == null || s.Length == 0) return null;

                foreach (var i in s)
                {
                    var r = f(entry?.Attributes, i);
                    if (r != null) return r;
                }

                return null;
            };

            return new User
            {
                Active = true,
                Email = getMappedValue(configuration.MappingEmail, LdapHelper.GetStringValue),
                FirstName = getMappedValue(configuration.MappingFirstName, LdapHelper.GetStringValue) ?? string.Empty,
                Surname = getMappedValue(configuration.MappingSurname, LdapHelper.GetStringValue) ?? string.Empty,
                UserID = getMappedValue(configuration.MappingUserID, LdapHelper.GetStringValue),
                PhoneNumber = getMappedValue(configuration.MappingPhoneNumber, LdapHelper.GetStringValue),
                Company = getMappedValue(configuration.MappingCompany, LdapHelper.GetStringValue),
                Role = getMappedValue(configuration.MappingRole, LdapHelper.GetStringValue),
                Description = getMappedValue(configuration.MappingDescription, LdapHelper.GetStringValue),
                StartDate = getMappedValue(configuration.MappingStartDate, LdapHelper.GetDateStringValue),
                EndDate = getMappedValue(configuration.MappingEndDate, LdapHelper.GetDateStringValue)
            };
        }
    }
}
