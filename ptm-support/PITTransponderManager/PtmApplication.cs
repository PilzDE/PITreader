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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Pilz.PITreader.PitTransponderManager
{
    /// <summary>
    /// Starts/stops PIT Transponder Manager tool in the background.
    /// </summary>
    public class PtmApplication : IDisposable
    {
        private bool disposed;

        private Process? process;

        private const int OutputLineCount = 500;

        private readonly Queue<string?> processStandardOutput = new Queue<string?>();

        private readonly Queue<string?> processErrorOutput = new Queue<string?>();

        /// <summary>
        /// Returns true, if PTM is running and the REST API is accessible.
        /// </summary>
        public bool IsRunning
        {
            get
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(1);

                    try
                    {
                        var response = client.GetAsync(PtmRestApi.BaseUrl).ConfigureAwait(false).GetAwaiter().GetResult();
                        return response.IsSuccessStatusCode;
                    }
                    catch (TaskCanceledException)
                    {
                        return false;
                    }
                }
            }
        }

        public bool WaitForRunning(TimeSpan timeout)
        {
            using (var cts = new CancellationTokenSource((int)timeout.TotalMilliseconds))
            {
                while (!this.IsRunning && !cts.IsCancellationRequested)
                {
                    Thread.Sleep(500);
                }
            }

            return this.IsRunning;
        }

        /// <summary>
        /// Closes the started PTM instance gracefully.
        /// </summary>
        public void Close()
        {
            if (this.process != null)
            {
                try
                {
                    this.process.Kill();
                    this.process.WaitForExit();

                    this.process = null;
                }
                catch (InvalidOperationException)
                {
                    // pass
                }
            }
        }

        /// <summary>
        /// Kills the started PTM instance.
        /// </summary>
        public void Kill()
        {
            if (this.process != null)
            {
                this.process.Kill();
                this.process = null;
            }
        }

        public bool WaitForExit(int milliseconds)
        {
            if (this.process != null && !this.process.HasExited)
            {
                return this.process.WaitForExit(milliseconds);
            }

            return true;
        }

        /// <summary>
        /// Starts an instance of PTM.
        /// </summary>
        public void Start()
        {
            this.Kill();

            var installation = PtmInstallation.DetectInstallations()
                .OrderByDescending(i => i.Version)
                .ThenByDescending(i => i.Released)
                .FirstOrDefault();
            if (installation == null)
            {
                return;
            }

            string[] arguments = new string[]
            {
                "\"-Duser.dir=" + installation.ServerPath + "\"",
                 "-Dfile.encoding=UTF-8",
                 "-Xms128m",
                 "-noverify",
                 "-Xmx512m",
                 "-Djava.net.preferIPv4Stack=true",
                 "-jar \"" + installation.JarPath + "\"",
                 "\"--spring.config.location=" + Path.Combine(installation.DataPath, "application.properties") + "\""
            };

            var pi = new ProcessStartInfo(installation.JavaPath)
            {
                WorkingDirectory = installation.BasePath,
                Arguments = string.Join(" ", arguments),
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            this.process = new Process
            {
                StartInfo = pi
            };

            this.process.OutputDataReceived += this.OnOutputDataReceived;
            this.process.ErrorDataReceived += this.OnErrorDataReceived;

            this.process.Start();

            this.process.BeginOutputReadLine();
            this.process.BeginErrorReadLine();
        }

        private void OnOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            this.processStandardOutput.Enqueue(e.Data);

            while (this.processStandardOutput.Count > OutputLineCount)
            {
                this.processStandardOutput.Dequeue();
            }
        }

        private void OnErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            this.processErrorOutput.Enqueue(e.Data);

            while (this.processErrorOutput.Count > OutputLineCount)
            {
                this.processErrorOutput.Dequeue();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.Kill();
                }

                this.disposed = true;
            }
        }

        void IDisposable.Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
