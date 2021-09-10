namespace Pilz.PITreader.Client.Model
{
    /// <summary>
    /// API user type
    /// </summary>
    public enum UserType
    {
        /// <summary>
        /// User (session based authentication using login form)
        /// </summary>
        User = 1,

        /// <summary>
        /// API client with API token
        /// </summary>
        ApiClient = 2
    }
}