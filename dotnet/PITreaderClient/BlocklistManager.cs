﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pilz.PITreader.Client.Model;

namespace Pilz.PITreader.Client
{
    /// <summary>
    /// Class to manage (sync) blocklist entries of a device.
    /// </summary>
    public class BlocklistManager
    {
        private readonly PITreaderClient client;

        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        /// <param name="client">Reference to PITreaderClient object</param>
        public BlocklistManager(PITreaderClient client)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
        }

        /// <summary>
        /// Returns all blocklist entries currently stored on the device.
        /// </summary>
        /// <returns></returns>
        public async Task<IList<BlocklistEntry>> GetEntriesAsync()
        {
            var result = await this.client.GetBlocklist();

            if (!result.Success) throw new InvalidOperationException("Error receiving blocklist from PITreader.");

            return result.Data?.Items ?? new List<BlocklistEntry>();
        }

        /// <summary>
        /// Sync entries on the device with entries passed to the function as an argument.
        /// </summary>
        /// <param name="imported">List of imported (to be synced) entries.</param>
        public async Task SyncAsync(IList<BlocklistEntry> imported)
        {
            IList<BlocklistEntry> online = await this.GetEntriesAsync();

            foreach (var entry in online.Where(o => !imported.Any(i => i.Id == o.Id)).ToList())
            {
                // to be deleted
                var result = await DeleteEntryAsync(entry);
                online.Remove(entry);
            }

            foreach (var entry in imported.Where(i => !online.Any(o => i.Id == o.Id)).ToList())
            {
                // to be added
                var result = await AddEntryAsync(entry);
            }

            foreach (var entry in imported.Where(i => online.FirstOrDefault(o => i.Id == o.Id)?.Comment != i.Comment))
            {
                // to be updated
                var result = await UpdateEntryAsync(entry);
            }
        }

        /// <summary>
        /// Modify block list
        /// </summary>
        /// <param name="action">see <see cref="CrudAction"/></param>
        /// <param name="entry">the <see cref="BlocklistEntry"/> to modify</param>
        /// <returns></returns>
        protected async Task<ApiResponse<GenericResponse>> UpdateBlockListAsync(CrudAction action, BlocklistEntry entry)
        {
            var response = await client.UpdateBlocklist(action, entry);
            if (!response.Success)
            {
                throw new Exception($"Error updating blocklist: {response.ErrorData.Message}");
            }
            return response;
        }

        /// <summary>
        /// Adds an entry to the block list
        /// </summary>
        /// <param name="blocklistEntry">see <see cref="BlocklistEntry"/> to add</param>
        /// <returns></returns>
        public async Task<ApiResponse<GenericResponse>> AddEntryAsync(BlocklistEntry blocklistEntry)
        {
            return await UpdateBlockListAsync(CrudAction.Create, blocklistEntry);
        }

        /// <summary>
        /// Update block list entry
        /// </summary>
        /// <param name="blocklistEntry">the <see cref="BlocklistEntry"/> to modify</param>
        /// <returns></returns>
        public async Task<ApiResponse<GenericResponse>> UpdateEntryAsync(BlocklistEntry blocklistEntry)
        {
            return await UpdateBlockListAsync(CrudAction.Edit, blocklistEntry);
        }

        /// <summary>
        /// Delete entry from block list
        /// </summary>
        /// <param name="blocklistEntry">the <see cref="BlocklistEntry"/> to delete</param>
        /// <returns></returns>
        public async Task<ApiResponse<GenericResponse>> DeleteEntryAsync(BlocklistEntry blocklistEntry)
        {
            return await UpdateBlockListAsync(CrudAction.Delete, blocklistEntry);
        }
    }
}