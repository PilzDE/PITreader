namespace Pilz.PITreader.Client.Model
{
    /// <summary>
    /// Action for <see cref="GenericCrudRequest{TKey, TData}"/>
    /// </summary>
    public enum CrudAction
    {
        /// <summary>
        /// Create a new item.
        /// </summary>
        Create,

        /// <summary>
        /// Edit an existing item.
        /// </summary>
        Edit,

        /// <summary>
        /// Delete an existing item.
        /// </summary>
        Delete
    }
}