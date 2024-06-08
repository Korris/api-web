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
    public const string CommonPrefix = "Bumcheo_Com";

    /// <summary>
    /// Password for test
    /// </summary>
    public const string Password4Test = "12345678";

    /// <summary>
    /// Page size
    /// </summary>
    public const ushort PageSize = 10;

    /// <summary>
    /// Role
    /// </summary>
    public class Role
    {
        /// <summary>
        /// SuperAdmin
        /// </summary>
        public const string SuperAdmin = "SuperAdmin";

        /// <summary>
        /// Admin
        /// </summary>
        public const string Admin = "Admin";

        /// <summary>
        /// SuperTenant
        /// </summary>
        public const string SuperTenant = "SuperTenant";

        /// <summary>
        /// Tenant
        /// </summary>
        public const string Tenant = "Tenant";

        /// <summary>
        /// Customer
        /// </summary>
        public const string Customer = "Customer";
    }

    /// <summary>
    /// Policy
    /// </summary>
    public class Policy
    {
        /// <summary>
        /// AppAdmin
        /// </summary>
        public const string AppAdmin = "AppAdmin";

        /// <summary>
        /// ClientAdmin
        /// </summary>
        public const string ClientAdmin = "ClientAdmin";

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
        public const string System = "System";

        /// <summary>
        /// gRPC
        /// </summary>
        public const string Grpc = "gRPC";
    }

    #endregion
}
