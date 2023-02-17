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

namespace Pilz.PITreader.PitTransponderManager.LdapExport
{
    internal class LdapConnector : IDisposable
    {
        private readonly LdapConnection connection;

        private bool disposed;

        public LdapConnector(string server, ushort? port, string? username, string? password)
        {
            this.connection = new LdapConnection(port == null ? new LdapDirectoryIdentifier(server) : new LdapDirectoryIdentifier(server, port.Value));
            connection.AutoBind = true;

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                this.connection.Credential = new System.Net.NetworkCredential(username, password);
            }
        }

        public IEnumerable<SearchResultEntryCollection> PagedSearch(int pageSize, string? searchFilter, string baseDn, string[] attributesToLoad)
        {
            var searchRequest = new SearchRequest(baseDn, searchFilter ?? "(cn=*)", SearchScope.Subtree, attributesToLoad);

            var pageResultRequestControl = new PageResultRequestControl(pageSize);
            searchRequest.Controls.Add(pageResultRequestControl);

            while (true)
            {
                var searchResponse = (SearchResponse)this.connection.SendRequest(searchRequest);

                if (searchResponse == null)
                    break;

                var pageResponse = (PageResultResponseControl)searchResponse.Controls[0];

                yield return searchResponse.Entries;

                if (pageResponse.Cookie.Length == 0)
                    break;

                pageResultRequestControl.Cookie = pageResponse.Cookie;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.connection.Dispose();
                }

                this.disposed = true;
            }
        }        

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
