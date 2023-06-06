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
using System.Text;

namespace Pilz.PITreader.Client
{
    /// <summary>
    /// PITreader device or update package version.
    /// </summary>
    public class FirmwareVersion : IComparable, IComparable<FirmwareVersion>, IComparable<Version>
    {
        /// <summary>
        /// Create a new FirmwareVersion instance.
        /// </summary>
        public FirmwareVersion()
        {
        }

        /// <summary>
        /// Create a new FirmwareVersion instance.
        /// </summary>
        /// <param name="version">Base version</param>
        /// <exception cref="ArgumentNullException"></exception>
        public FirmwareVersion(Version version)
        {
            if (version is null)
            {
                throw new ArgumentNullException(nameof(version));
            }

            this.Major = version.Major;
            this.Minor = version.Minor;
            this.Patch = version.Build;

            if (version.Revision > 0)
            {
                this.Build = (uint)version.Revision;
            }
        }

        /// <summary>
        /// Create a new FirmwareVersion instance.
        /// </summary>
        /// <param name="major"></param>
        /// <param name="minor"></param>
        /// <param name="patch"></param>
        public FirmwareVersion(int major, int minor, int patch)
        {
            this.Major = major;
            this.Minor = minor;
            this.Patch = patch;
        }

        /// <summary>
        /// Create a new FirmwareVersion instance.
        /// </summary>
        /// <param name="major"></param>
        /// <param name="minor"></param>
        /// <param name="patch"></param>
        /// <param name="build"></param>
        public FirmwareVersion(int major, int minor, int patch, uint build)
        {
            this.Major = major;
            this.Minor = minor;
            this.Patch = patch;
            this.Build = build;
        }

        /// <summary>
        /// Major component of firmware version.
        /// </summary>
        public int Major { get; set; }

        /// <summary>
        /// Minor component of firmware version.
        /// </summary>
        public int Minor { get; set; }

        /// <summary>
        /// Patch component of firmware version.
        /// </summary>
        public int Patch { get; set; }

        /// <summary>
        /// (Optional) Build component of firmware version.
        /// </summary>
        public uint? Build { get; set; }

        /// <summary>
        /// Released flag (true/false)
        /// </summary>
        public bool? Released { get; set; }

        /// <inheritdoc/>
        public int CompareTo(object obj)
        {
            var firmwareVersion = obj as FirmwareVersion;
            if (firmwareVersion != null)
            {
                return this.CompareTo(firmwareVersion);
            }

            var version = obj as Version;
            if (version != null)
            {
                return this.CompareTo(version);
            }

            throw new ArgumentException();
        }

        /// <inheritdoc/>
        public int CompareTo(FirmwareVersion other)
        {
            if (other is null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (this.Major > other.Major) return 1;
            if (this.Major < other.Major) return -1;

            if (this.Minor > other.Minor) return 1;
            if (this.Minor < other.Minor) return -1;

            if (this.Patch > other.Patch) return 1;
            if (this.Patch < other.Patch) return -1;

            if (this.Build.HasValue && other.Build.HasValue)
            {
                if (this.Build > other.Build) return 1;
                if (this.Build < other.Build) return -1;
            }

            if (this.Released.HasValue && this.Released.Value && (!other.Released.HasValue || !other.Released.Value))
                return 1;
            if (other.Released.HasValue && other.Released.Value && (!this.Released.HasValue || !this.Released.Value))
                return -1;

            if (!(this.Released ?? false) && !(other.Released ?? false))
            {
                if ((this.Build ?? 0) > (other.Build ?? 0)) return 1;
                if ((this.Build ?? 0) < (other.Build ?? 0)) return -1;
            }

            return 0;
        }

        /// <inheritdoc/>
        public int CompareTo(Version other)
        {
            if (other is null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (this.Major > other.Major) return 1;
            if (this.Major < other.Major) return -1;

            if (this.Minor > other.Minor) return 1;
            if (this.Minor < other.Minor) return -1;

            if (this.Patch > other.Build) return 1;
            if (this.Patch < other.Build) return -1;

            if (this.Build > other.Revision) return 1;
            if (this.Build < other.Revision) return -1;

            return 0;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is FirmwareVersion version &&
                   this.Major == version.Major &&
                   this.Minor == version.Minor &&
                   this.Patch == version.Patch &&
                   this.Build == version.Build &&
                   this.Released == version.Released;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int hashCode = -852753854;
            hashCode = hashCode * -1521134295 + this.Major.GetHashCode();
            hashCode = hashCode * -1521134295 + this.Minor.GetHashCode();
            hashCode = hashCode * -1521134295 + this.Patch.GetHashCode();
            hashCode = hashCode * -1521134295 + this.Build.GetHashCode();
            hashCode = hashCode * -1521134295 + this.Released.GetHashCode();
            return hashCode;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"{this.Major:00}.{this.Minor:00}.{this.Patch:00}");

            if (this.Build != null && this.Build.Value > 0)
            {
                sb.Append($".{this.Build.Value}");
            }

            if (this.Released.HasValue && !this.Released.Value)
            {
                sb.Append(" Unreleased");
            }

            return sb.ToString();
        }

        /// <inheritdoc/>
        public static bool operator ==(FirmwareVersion a, FirmwareVersion b)
        {
            if (a is null && b is null) return true;
            if (a is null || b is null) return false;

            return a.CompareTo(b) == 0;
        }

        /// <inheritdoc/>
        public static bool operator !=(FirmwareVersion a, FirmwareVersion b)
        {
            if (a is null && b is null) return false;
            if (a is null || b is null) return true;

            return a.CompareTo(b) != 0;
        }

        public static bool operator >(FirmwareVersion a, FirmwareVersion b)
        {
            if (a is null && b is null) return false;
            if (a is null) return false;
            if (b is null) return true;

            return a.CompareTo(b) > 0;
        }

        /// <inheritdoc/>
        public static bool operator <(FirmwareVersion a, FirmwareVersion b)
        {
            if (a is null && b is null) return false;
            if (a is null) return true;
            if (b is null) return false;

            return a.CompareTo(b) < 0;
        }

        /// <inheritdoc/>
        public static bool operator >=(FirmwareVersion a, FirmwareVersion b)
        {
            if (a is null && b is null) return true;
            if (a is null) return false;
            if (b is null) return true;

            return a.CompareTo(b) >= 0;
        }

        /// <inheritdoc/>
        public static bool operator <=(FirmwareVersion a, FirmwareVersion b)
        {
            if (a is null && b is null) return true;
            if (a is null) return true;
            if (b is null) return false;

            return a.CompareTo(b) <= 0;
        }
    }
}
