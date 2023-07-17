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

using System.Security.Cryptography;

namespace Pilz.PITreader.CommissioningTool
{
    internal class PasswordGenerator
    {
        private const string UpperCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        private const string LowerCharacters = "abcdefghijklmnopqrstuvwxyz";

        private const string NumericCharacters = "0123456789";

        private const string SpecialCharacters = "!\"§$%&/()=?+#*{[]}\\<>|.:,;-_";

        public const string DefaultPasswordCharacters = LowerCharacters + UpperCharacters + NumericCharacters + SpecialCharacters;

        public static string GenerateSecurePassword(int length, string characters = DefaultPasswordCharacters)
        {
            var buffer = new char[length];

            do
            {
                for (int index = 0; index < length; index++)
                {
                    buffer[index] = characters[RandomNumberGenerator.GetInt32(characters.Length)];
                }
            } while (!buffer.Any(c => UpperCharacters.Contains(c))
                    || !buffer.Any(c => LowerCharacters.Contains(c))
                    || !buffer.Any(c => NumericCharacters.Contains(c))
                    || !buffer.Any(c => SpecialCharacters.Contains(c)));

            return new string(buffer);
        }
    }
}
