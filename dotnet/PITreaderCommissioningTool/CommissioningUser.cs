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

namespace Pilz.PITreader.CommissioningTool
{
    /// <summary>
    /// User for device access during the commissioning operation.
    /// </summary>
    internal class CommissioningUser
    {
        /// <summary>
        /// Creates a new commissioning user instance.
        /// </summary>
        /// <param name="name"></param>
        public CommissioningUser(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// The user name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// If set to true, username and password based interactive login is used.
        /// Otherwise authentication via an API token is used.
        /// </summary>
        public bool InteractiveLogin { get; set; }

        /// <summary>
        /// Password or API token.
        /// </summary>
        public string? PasswordOrApiToken { get; set; }

        /// <summary>
        /// If set to true, the user with specified name should be deleted at the end of the commissioning process from the device.
        /// </summary>
        public bool DeleteOnCleanup { get; set; }
    }
}
