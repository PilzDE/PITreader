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

﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Pilz.PITreader.PitTransponderManager
{
    /// <summary>
    /// Information about a PIT Transponder Manager installation.
    /// </summary>
    public class PtmInstallation
    {
        /// <summary>
        /// The version number as included in the directory name.
        /// </summary>
        public Version Version { get; private set; } = new Version();

        /// <summary>
        /// Complete version name e.g. "PIT Transponder Manager v1.1.1"
        /// </summary>
        public string VersionName { get; private set; } = string.Empty;

        /// <summary>
        /// True, if version is released. Otherwise (alpha or beta build): false
        /// </summary>
        public bool Released { get; private set; }

        /// <summary>
        /// Path to base installation directory.
        /// </summary>
        public string BasePath { get; private set; } = string.Empty;

        /// <summary>
        /// Path to server installation directory.
        /// </summary>
        public string ServerPath { get; private set; } = string.Empty;

        /// <summary>
        /// Path to data directory of installation.
        /// </summary>
        public string DataPath { get; private set; } = string.Empty;

        /// <summary>
        /// Path to Java executable included in the installation.
        /// </summary>
        public string JavaPath { get; private set; } = string.Empty;

        /// <summary>
        /// Path to main jar file of the server application.
        /// </summary>
        public string JarPath { get; private set; } = string.Empty;

        private static PtmInstallation? Build(Version version, string versionName, bool released)
        {
            string baseDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                "Pilz",
                versionName);

            if (!Directory.Exists(baseDir))
            {
                baseDir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                    "Pilz",
                    versionName);
            }

            if (!Directory.Exists(baseDir))
            {
                return null;
            }

            string dataDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "Pilz",
                versionName,
                "server");

            string serverDir = Path.Combine(baseDir, "server");
            if (!Directory.Exists(serverDir))
            {
                return null;
            }

            string? jarFile = Directory.GetFiles(serverDir, "pit-transponder-manager-server-*.jar", SearchOption.AllDirectories).FirstOrDefault();
            if (jarFile == null)
            {
                return null;
            }

            string? javaExe = Directory.GetFiles(baseDir, "java.exe", SearchOption.AllDirectories).FirstOrDefault();
            if (javaExe == null)
            {
                return null;
            }

            return new PtmInstallation
            {
                Version = version,
                VersionName = versionName,
                Released = released,
                BasePath = baseDir,
                ServerPath = serverDir,
                DataPath = dataDir,
                JavaPath = javaExe,
                JarPath = jarFile,
            };
        }

        /// <summary>
        /// Detects PIT Transponder Manager installations on the local system.
        /// </summary>
        /// <returns>Returns a list of PIT Transponder Manager installations on the local system.</returns>
        public static IEnumerable<PtmInstallation> DetectInstallations()
        {
            var programDirectories = Directory.GetDirectories(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Pilz"),
                "PIT Transponder Manager*")
                .Concat(
                    Directory.GetDirectories(
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Pilz"),
                    "PIT Transponder Manager*"));
            
            foreach (var directory in programDirectories.Select(Path.GetFileName).Where(d => d != null))
            {
                var versionMatch = Regex.Match(directory, @"(a|b|v)\s*(\d+(?:\.\d+)*)");
                if (versionMatch.Success)
                {
                    Version testVersion;
                    if (Version.TryParse(versionMatch.Groups[2].Value, out testVersion))
                    {
                        var result = Build(testVersion, directory, versionMatch.Groups[1].Value == "v");
                        if (result != null)
                        {
                            yield return result;
                        }
                    }
                }
            }            
        }
    }
}
