// Copyright (c) 2022 Pilz GmbH & Co. KG
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

namespace Pilz.PITreader.Client.Model
{
    /// <summary>
    /// Failure reasons of the authentication process running for transponders of the PITreader.
    /// </summary>
    public enum AuthenticationFailureReason
    {
        /// <summary>
        /// No error
        /// </summary>
        None = 0,

        /// <summary>
        /// No transponder key inserted
        /// </summary>
        NoTransponder = 1,

        /// <summary>
        /// Permission "0"
        /// The transponder key has no permissions for device groups.
        /// </summary>
        Permission_0 = 2,

        /// <summary>
        /// The validity of the transponder key is outside the validity period.
        /// (Start date/end date)
        /// </summary>
        TimeLimitation = 3,

        /// <summary>
        /// The transponder key is included in the block list.
        /// </summary>
        Blocklist = 4,

        /// <summary>
        /// No permission has been stored for the security-ID yet. ("External" authentication mode)
        /// </summary>
        ExternalModeWating = 5,

        /// <summary>
        /// Authentication is locked by the 24 V I/O port.
        /// </summary>
        IoPortLock = 6,

        /// <summary>
        /// The "Single authentication" authentication type is configured and authentication is locked by another registered transponder key.
        /// </summary>
        SingleAuthLock = 7,

        /// <summary>
        /// The "4 Eyes Principle" authentication type is configured and the second transponder key was not authenticated yes.
        /// </summary>
        FourEyesFirstKey = 8
    }
}