using System.Text.Json.Serialization;

namespace Pilz.PITreader.Client.Model
{
    /// <summary>
    /// Request to create, update or delete an item in a list stored on the device.
    /// </summary>
    /// <typeparam name="TKey">Type of the id parameter (integer or <see cref="SecurityId"/>)</typeparam>
    /// <typeparam name="TData">Type of the data entry.</typeparam>
    public class GenericCrudRequest<TKey, TData> where TData: class
    {
        /// <summary>
        /// The id represention the item.
        /// </summary>
        [JsonPropertyName("id")]
        public TKey Id { get; set; }

        /// <summary>
        /// The action to be performed.
        /// </summary>
        [JsonPropertyName("action")]
        public CrudAction Action { get; set; }

        /// <summary>
        /// Data for the item.
        /// </summary>
        [JsonPropertyName("data")]
        public TData Data { get; set; }
    }
}
