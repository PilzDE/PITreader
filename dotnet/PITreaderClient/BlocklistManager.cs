using CsvHelper;
using CsvHelper.Configuration;
using Pilz.PITreader.Client.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
        /// Sync entries on the device with entries contained in the specified CSV file.
        /// </summary>
        /// <param name="csvFilePath">Path to CSV file.</param>
        public async Task SyncFromCsvAsync(string csvFilePath)
        {
            IList<BlocklistEntry> imported = new List<BlocklistEntry>();
            using (var fileReader = File.OpenText(csvFilePath))
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    DetectDelimiter = true,
                    HasHeaderRecord = true
                };

                var csvReader = new CsvReader(fileReader, config);

                csvReader.Read();
                csvReader.ReadHeader();

                while (csvReader.Read())
                {
                    imported.Add(new BlocklistEntry
                    {
                        Id = csvReader.GetField<string>(0),
                        Comment = csvReader.GetField<string>(1)
                    });
                }
            }

            IList<BlocklistEntry> online = await this.GetEntriesAsync();

            foreach (var entry in online.Where(o => !imported.Any(i => i.Id == o.Id)).ToList())
            {
                // to be deleted
                var result = await this.client.UpdateBlocklist(CrudAction.Delete, entry);
                online.Remove(entry);
            }

            foreach (var entry in imported.Where(i => !online.Any(o => i.Id == o.Id)).ToList())
            {
                // to be added
                var result = await this.client.UpdateBlocklist(CrudAction.Create, entry);
            }

            foreach (var entry in imported.Where(i => online.FirstOrDefault(o => i.Id == o.Id)?.Comment != i.Comment))
            {
                // to be updated
                var result = await this.client.UpdateBlocklist(CrudAction.Edit, entry);
            }
        }
    }
}
