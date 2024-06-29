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

namespace Mcsg.Common.Core.Constants;

/// <summary>
/// Setting
/// </summary>
public class Setting : SeedWork.Constants.Setting
{
    /// <summary>
    /// MicroServices
    /// </summary>
    public static Dictionary<string, string> MicroServices
    {
        get
        {
            return new Dictionary<string, string>
            {
                { "Adm", "Admin" },
                { "Ana", "Analytic" },
                { "Ide", "Identity" },
                { "Rea", "Realtime" },
                { "Soc", "Social" },
                { "Wal", "Wallet" }
            };
        }
    }

    /// <summary>
    /// The folder name is stored in MinIO
    /// </summary>
    public class FolderMinIO
    {
        /// <summary>
        /// User
        /// </summary>
        public const string User = "users";
    }

    /// <summary>
    /// Default
    /// </summary>
    public class Default
    {
        /// <summary>
        /// Reference number length
        /// </summary>
        public const int ReferenceNumberLength = 12;

        /// <summary>
        /// Chapter price
        /// </summary>
        public const float ChapterPrice = 2000;

        /// <summary>
        /// Minimum point can with draw
        /// </summary>
        public const float MinimumPointCanWithDraw = 50000;

        /// <summary>
        /// Minimum point can deposit
        /// </summary>
        public const float MinimumPointCanDeposit = 10000;

        /// <summary>
        /// Maximum point can deposit
        /// </summary>
        public const float MaximumPointCanDeposit = 999999;

        /// <summary>
        /// Free username length
        /// </summary>
        public const int FreeUserNameLength = 16;
    }

    /// <summary>
    /// Tags
    /// </summary>
    public class Tags
    {
        /// <summary>
        /// Background
        /// </summary>
        public const string Background = "Background";
    }
}
