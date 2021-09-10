namespace Pilz.PITreader.Client
{
    /// <summary>
    /// Authentication type
    /// </summary>
    public enum AuthenticationType
    {
        /// <summary>
        /// "Basic" authentication type (no special function)
        /// </summary>
        Basic = 0,

        /// <summary>
        /// "Single authentication" authentication type
        /// </summary>
        SingleAuthentication = 1,

        /// <summary>
        /// 4-eyes-principle
        /// </summary>
        FourEyesPrinciple = 2
    }
}