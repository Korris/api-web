#region Information
/*
 * Author       : Toan Nguyen Van
 * Email        : nvt87x@gmail.com
 * Phone        : +84 345 515 010
 * ------------------------------- *
 * Create       : 2024-Jan-21 07:03
 * Update       : 2024-Jan-21 07:03
 * Checklist    : 1.0
 * Status       : New
 */
#endregion

namespace Mcsg.Common.SeedWork.Constants;

/// <summary>
/// Setting
/// </summary>
public class Setting
{
    #region -- Commons --

    /// <summary>
    /// Payload
    /// </summary>
    public const string Payload = "payload";

    /// <summary>
    /// The prefix for common environment variables
    /// </summary>
    public const string CommonPrefix = "Focfoc_Com";

    /// <summary>
    /// Password for test
    /// </summary>
    public const string Password4Test = "12345678";

    /// <summary>
    /// Page size
    /// </summary>
    public const ushort PageSize = 10;

    /// <summary>
    /// Original suffix file name
    /// </summary>
    public const string OriginalSuffixFileName = "-original";

    /// <summary>
    /// Role
    /// </summary>
    public class McsgRole
    {
        /// <summary>
        /// Customer
        /// </summary>
        public const string User = "Mcsg.User";

        /// <summary>
        /// SuperAdmin
        /// </summary>
        public const string SysAdmin = "Mcsg.SysAdmin";

        /// <summary>
        /// Admin
        /// </summary>
        public const string Admin = "Mcsg.Admin";

        /// <summary>
        /// ContentAdmin
        /// </summary>
        public const string ContentAdmin = "Mcsg.ContentAdmin";
    }

    /// <summary>
    /// Policy
    /// </summary>
    public class Policy
    {
        /// <summary>
        /// Admin
        /// </summary>
        public const string Admin = "Admin";
    }

    /// <summary>
    /// Created by
    /// </summary>
    public class CreatedBy
    {
        /// <summary>
        /// System
        /// </summary>
        public static Guid System = new("00000000-0000-0000-0000-000000000001");
    }

    #endregion
}
