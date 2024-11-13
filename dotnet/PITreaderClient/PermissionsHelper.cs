using System.Collections.Generic;
using System.Linq;
using Pilz.PITreader.Client.Model;

namespace Pilz.PITreader.Client
{
    /// <summary>
    /// Helper for <see cref="Model.Permission"/>
    /// </summary>
    public static class PermissionsHelper
    {
        private static readonly IDictionary<int, Permission> permissionMapping = new Dictionary<int, Permission>
        {
            { 0, Permission.Permission_0 },
            { 1, Permission.Permission_1 },
            { 2, Permission.Permission_2 },
            { 3, Permission.Permission_3 },
            { 4, Permission.Permission_4 },
            { 5, Permission.Permission_5 },
            { 6, Permission.Permission_6 },
            { 7, Permission.Permission_7 },
            { 8, Permission.Permission_8 },
            { 9, Permission.Permission_9 },
            { 10, Permission.Permission_10 },
            { 11, Permission.Permission_11 },
            { 12, Permission.Permission_12 },
            { 13, Permission.Permission_13 },
            { 14, Permission.Permission_14 },
            { 15, Permission.Permission_15 },
            { 16, Permission.Permission_16 },
            { 17, Permission.Permission_17 },
            { 18, Permission.Permission_18 },
            { 19, Permission.Permission_19 },
            { 20, Permission.Permission_20 },
            { 21, Permission.Permission_21 },
            { 22, Permission.Permission_22 },
            { 23, Permission.Permission_23 },
            { 24, Permission.Permission_24 },
            { 25, Permission.Permission_25 },
            { 26, Permission.Permission_26 },
            { 27, Permission.Permission_27 },
            { 28, Permission.Permission_28 },
            { 29, Permission.Permission_29 },
            { 30, Permission.Permission_30 },
            { 31, Permission.Permission_31 },
            { 32, Permission.Permission_32 },
            { 33, Permission.Permission_33 },
            { 34, Permission.Permission_34 },
            { 35, Permission.Permission_35 },
            { 36, Permission.Permission_36 },
            { 37, Permission.Permission_37 },
            { 38, Permission.Permission_38 },
            { 39, Permission.Permission_39 },
            { 40, Permission.Permission_40 },
            { 41, Permission.Permission_41 },
            { 42, Permission.Permission_42 },
            { 43, Permission.Permission_43 },
            { 44, Permission.Permission_44 },
            { 45, Permission.Permission_45 },
            { 46, Permission.Permission_46 },
            { 47, Permission.Permission_47 },
            { 48, Permission.Permission_48 },
            { 49, Permission.Permission_49 },
            { 50, Permission.Permission_50 },
            { 51, Permission.Permission_51 },
            { 52, Permission.Permission_52 },
            { 53, Permission.Permission_53 },
            { 54, Permission.Permission_54 },
            { 55, Permission.Permission_55 },
            { 56, Permission.Permission_56 },
            { 57, Permission.Permission_57 },
            { 58, Permission.Permission_58 },
            { 59, Permission.Permission_59 },
            { 60, Permission.Permission_60 },
            { 61, Permission.Permission_61 },
            { 62, Permission.Permission_62 },
            { 63, Permission.Permission_63 },
            { 64, Permission.Permission_64 },
        };

        /// <summary>
        /// Converts a permission code to the corresponding value (0 to 64).
        /// </summary>
        /// <param name="permission">The permission code.</param>
        /// <returns>The corresponding value (0-64) or <c>null</c>, if no distinct mapping exists.</returns>
        public static int? ToValue(this Permission permission)
        {
            IEnumerable<KeyValuePair<int, Permission>> matches = permissionMapping.Where(x => x.Value == permission);

            if (matches.Count() == 1)
            {
                return matches.First().Key;
            }

            return default(int?);
        }
    }
}
