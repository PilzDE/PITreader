namespace Pilz.PITreader.Client.Model
{
    /// <summary>
    /// Request to create, update or delete a blocklist entry.
    /// </summary>
    public class BlocklistCrudRequest : GenericCrudRequest<SecurityId, BlocklistEntry>
    {
    }
}
