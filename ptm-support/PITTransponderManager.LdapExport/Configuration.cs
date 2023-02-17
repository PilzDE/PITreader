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

﻿using System.Reflection;

namespace Pilz.PITreader.PitTransponderManager.LdapExport
{
    internal class Configuration
    {
        private const string ConnectionSectionKey = "Connection";

        private const string MappingSectionKey = "Mapping";

        private const string OptionsSectionKey = "Options";

        public Dictionary<string, string> DefaultSection => Sections[string.Empty];

        public readonly Dictionary<string, Dictionary<string, string>> Sections = new Dictionary<string, Dictionary<string, string>>
        {
            { string.Empty, new Dictionary<string, string>() },
            { ConnectionSectionKey, new Dictionary<string, string>() },
            { MappingSectionKey, new Dictionary<string, string>() },
            { OptionsSectionKey, new Dictionary<string, string>() },
        };

        public Configuration()
        {
            string? basePath = Assembly.GetExecutingAssembly().Location ?? Assembly.GetEntryAssembly()?.Location;
            if (basePath != null)
            {
                basePath = Path.GetDirectoryName(basePath);
            }

            string iniPath = string.IsNullOrEmpty(basePath) ? "config.ini" : Path.Combine(basePath, "config.ini");
            using (var reader = new StreamReader(iniPath))
            {
                string section = string.Empty;
                while (!reader.EndOfStream)
                {
                    string? line = reader.ReadLine()?.Trim();
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        if (line.StartsWith("[") && line.EndsWith("]"))
                        {
                            section = line.Substring(1, line.Length - 2);
                            if (!Sections.ContainsKey(section))
                            {
                                Sections.Add(section, new Dictionary<string, string>());
                            }
                        }
                        else
                        {
                            string[] keyValue = line.Split("=", 2, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                            if (keyValue.Length == 2)
                            {
                                if (!Sections[section].ContainsKey(keyValue[0]))
                                {
                                    if (keyValue[1].StartsWith("\"") && keyValue[1].EndsWith("\""))
                                    {
                                        keyValue[1] = keyValue[1].Substring(1, keyValue[1].Length - 2);
                                    }

                                    Sections[section].Add(keyValue[0], keyValue[1]);
                                }
                            }
                        }
                    }
                }
            }
        }

        public string? LdapServer => this.Sections[ConnectionSectionKey].GetValueOrDefault("Server");

        public string? LdapPort => this.Sections[ConnectionSectionKey].GetValueOrDefault("Port");

        public string? LdapUser => this.Sections[ConnectionSectionKey].GetValueOrDefault("User");

        public string? LdapPassword => this.Sections[ConnectionSectionKey].GetValueOrDefault("Password");

        public string? LdapBaseDN => this.Sections[ConnectionSectionKey].GetValueOrDefault("BaseDN");

        public string? LdapFilter => this.Sections[ConnectionSectionKey].GetValueOrDefault("Filter");

        public string? BlocklistOnDisable => this.Sections[OptionsSectionKey].GetValueOrDefault("BlocklistOnDisable");

        public string[] MappingEmail => this.Sections[MappingSectionKey].GetValueOrDefault("Email")?.Split(',') ?? new string[] { };

        public string[] MappingFirstName => this.Sections[MappingSectionKey].GetValueOrDefault("FirstName")?.Split(',') ?? new string[] { };

        public string[] MappingSurname => this.Sections[MappingSectionKey].GetValueOrDefault("Surname")?.Split(',') ?? new string[] { };

        public string[] MappingUserID => this.Sections[MappingSectionKey].GetValueOrDefault("UserID")?.Split(',') ?? new string[] { };

        public string[] MappingPhoneNumber => this.Sections[MappingSectionKey].GetValueOrDefault("PhoneNumber")?.Split(',') ?? new string[] { };

        public string[] MappingCompany => this.Sections[MappingSectionKey].GetValueOrDefault("Company")?.Split(',') ?? new string[] { };

        public string[] MappingRole => this.Sections[MappingSectionKey].GetValueOrDefault("Role")?.Split(',') ?? new string[] { };

        public string[] MappingDescription => this.Sections[MappingSectionKey].GetValueOrDefault("Description")?.Split(',') ?? new string[] { };

        public string[] MappingStartDate => this.Sections[MappingSectionKey].GetValueOrDefault("StartDate")?.Split(',') ?? new string[] { };

        public string[] MappingEndDate => this.Sections[MappingSectionKey].GetValueOrDefault("EndDate")?.Split(',') ?? new string[] { };

        public string[] GetAllAttributes()
        {
            return MappingEmail
                .Concat(MappingFirstName)
                .Concat(MappingSurname)
                .Concat(MappingUserID)
                .Concat(MappingPhoneNumber)
                .Concat(MappingCompany)
                .Concat(MappingRole)
                .Concat(MappingDescription)
                .Concat(MappingStartDate)
                .Concat(MappingEndDate)
                .Distinct()
                .ToArray();
        }
    }
}
