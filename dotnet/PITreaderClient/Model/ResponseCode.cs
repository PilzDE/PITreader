namespace Pilz.PITreader.Client.Model
{
    /// <summary>
    /// Response code of a request to the device.
    /// </summary>
    public enum ResponseCode
    {
        /// <summary>
        /// Unkown or invalid response code.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Request executed successfull.
        /// </summary>
        Ok = 200,

        /// <summary>
        /// Invalid data was submitted in the request.
        /// </summary>
        BadRequest = 400,

        /// <summary>
        /// Access to the API endpoint or one of the parameters in the request is not allowed for the current user.
        /// </summary>
        Forbidden = 403,

        /// <summary>
        /// Internal error of the device (restart the device and try again).
        /// </summary>
        DeviceError = 500
    }
}
