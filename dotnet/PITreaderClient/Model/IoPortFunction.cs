namespace Pilz.PITreader.Client.Model
{
    /// <summary>
    /// Mode of 24 V I/O port
    /// </summary>
    public enum IoPortFunction
    {
        /// <summary>
        /// No function
        /// </summary>
        None = 0,

        /// <summary>
        /// Authentication status (output)
        /// </summary>
        Output_AuthenticationStatus = 1,

        /// <summary>
        /// Lock authentication (input)
        /// </summary>
        Input_LockAuthentication = 2
    }
}