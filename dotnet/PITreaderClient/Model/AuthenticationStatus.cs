namespace Pilz.PITreader.Client.Model
{
    /// <summary>
    /// Status of authentication process
    /// </summary>
    public enum AuthenticationStatus
    {
        /// <summary>
        /// No transponder key
        /// </summary>
        NoTransponder = 0,

        /// <summary>
        /// Process completed
        /// </summary>
        Completed = 1,

        /// <summary>
        /// Waiting for external authentication
        /// </summary>
        Waiting = 2
    }
}