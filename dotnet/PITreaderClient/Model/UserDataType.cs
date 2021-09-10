namespace Pilz.PITreader.Client.Model
{
    /// <summary>
    /// Data type for user data parameters
    /// </summary>
    public enum UserDataType
    {
        /// <summary>
        /// A string of characters
        /// </summary>
        STRING = 1,

        /// <summary>
        /// Unsigned 8-bit number (range 0 to 255)
        /// </summary>
        INT8U = 10,

        /// <summary>
        /// Signed 8-bit number (range -128 to 127)
        /// </summary>
        INT8S = 11,

        /// <summary>
        /// Unsigned 16-bit number (range 0 to 65535)
        /// </summary>
        INT16U = 12,

        /// <summary>
        /// Signed 16-bit number (range -32768 to 32767)
        /// </summary>
        INT16S = 13,

        /// <summary>
        /// Unsigned 32-bit number (range 0 to 4294967295)
        /// </summary>
        INT32U = 14,

        /// <summary>
        /// Signed 32-bit number (range -2147483648 to 2147483647)
        /// </summary>
        INT32S = 15,

        /// <summary>
        /// Date/time string formatted according to RFC 3339 (%yyyy-%MM-%dd'T'%HH:%mm:%ss'Z')
        /// </summary>
        DATETIME = 20,

        /// <summary>
        /// Permission (32 bit code word)
        /// </summary>
        PERMISSION = 30
    }
}