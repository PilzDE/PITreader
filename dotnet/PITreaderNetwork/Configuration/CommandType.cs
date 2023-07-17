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

namespace Pilz.PITreader.Network.Configuration
{
    /// <summary>
    /// Multicast Configuration Command Types
    /// </summary>
    public enum CommandType : UInt32
    {
        /// <summary>
        /// None / not a command
        /// </summary>
        None = 0,

        /// <summary>
        /// Identify device (flash led).
        /// </summary>
        Identify = 1,

        /// <summary>
        /// Set IPv4 configuration of device.
        /// </summary>
        SetIPv4 = 2,

        /// <summary>
        /// Ping device to check state (e.g. authentication required).
        /// </summary>
        Ping = 3
    }
}
