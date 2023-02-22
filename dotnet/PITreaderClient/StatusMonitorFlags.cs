using System;

namespace Pilz.PITreader.Client
{
    /// <summary>
    /// Status monitoring change types
    /// </summary>
    [Flags]
    public enum StatusMonitorFlags
    {
        /// <summary>
        /// Default none value
        /// </summary>
        None = 0,

        /// <summary>
        /// Device status change
        /// </summary>
        StatusChange = 1 << 0,

        /// <summary>
        /// Led status change
        /// </summary>
        LedChange = 1 << 1,

        /// <summary>
        /// Configuration settings change
        /// </summary>
        ConfigurationChange = 1 << 2,

        /// <summary>
        /// Transponder/authentication status change
        /// </summary>
        TransponderChange = 1 << 3,

        /// <summary>
        /// Diagnostic status/log change
        /// </summary>
        DiagnosticChange = 1 << 4,

        /// <summary>
        /// Blocklist change
        /// </summary>
        BlocklistChange = 1 << 5,

        /// <summary>
        /// Permission list change
        /// </summary>
        PermissionListChange = 1 << 6,

        /// <summary>
        /// User data config change
        /// </summary>
        UserDataConfigChange = 1 << 7,

        /// <summary>
        /// Combination of all flags
        /// </summary>
        All = StatusChange 
            | LedChange 
            | ConfigurationChange 
            | TransponderChange 
            | DiagnosticChange
            | BlocklistChange
            | PermissionListChange
            | UserDataConfigChange
    }
}
