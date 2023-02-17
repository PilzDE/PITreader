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

﻿using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Pilz.PITreader.PitTransponderManager.Service
{
    internal class Service : BackgroundService
    {
        private PtmApplication ptmController = new PtmApplication();

        private readonly ILogger<Service> logger;

        public Service(ILogger<Service> logger)
        {
            this.logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (ptmController.IsRunning)
            {
                Environment.Exit(1);
                return Task.CompletedTask;
            }

            try
            {
                this.ptmController.Start();

                while (!stoppingToken.IsCancellationRequested
                    && !this.ptmController.WaitForExit(500))
                {
                }
            }
            catch (TaskCanceledException)
            {
            }
            catch (Exception exception)
            {
                this.logger?.LogError(exception, "{Message}", exception.Message);

                Environment.Exit(1);
            }
            finally
            {
                this.ptmController.Kill();
            }

            return Task.CompletedTask;
        }
    }
}
