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

        /// <summary>
        /// Value length limit
        /// </summary>
        public const int ValueLengthLimit = 4;

        /// <summary>
        /// Multipart body length limit
        /// </summary>
        public const int MultipartBodyLengthLimit = 128;

        /// <summary>
        /// Reward point
        /// </summary>
        public const int RewardPoint = 1000;
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

    /// <summary>
    /// Security claim
    /// </summary>
    public class SecurityClaim
    {
        /// <summary>
        /// User name
        /// </summary>
        public const string UserName = "UserName";

        /// <summary>
        /// User ID
        /// </summary>
        public const string UserId = "UserId";

        /// <summary>
        /// Profile name
        /// </summary>
        public const string ProfileName = "ProfileName";

        /// <summary>
        /// User avatar
        /// </summary>
        public const string UserAvatar = "UserAvatar";
    }

    /// <summary>
    /// Media config
    /// </summary>
    public class MediaConfig
    {
        /// <summary>
        /// Public image URL path
        /// </summary>
        public const string PublicImageUrlPath = "image?p={0}";

        /// <summary>
        /// Image URL path
        /// </summary>
        public const string ImageUrlPath = "image?i={0}";

        /// <summary>
        /// Video URL path
        /// </summary>
        public const string VideoUrlPath = "watch?v={0}";

        /// <summary>
        /// Audio URL path
        /// </summary>
        public const string AudioUrlPath = "watch?a={0}";
    }

    /// <summary>
    /// Post config
    /// </summary>
    public static class PostConfig
    {
        /// <summary>
        /// Hash length
        /// </summary>
        public const int HashLength = 12;

        /// <summary>
        /// Sub hash length
        /// </summary>
        public const int SubHashLength = 16;

        /// <summary>
        /// Tag maximum length
        /// </summary>
        public const int TagMaxLength = 32;

        /// <summary>
        /// Item count maximum
        /// </summary>
        public const int ItemCountMax = 100;

        /// <summary>
        /// Tag suggest maximum length
        /// </summary>
        public const int TagSuggestMaxLength = 6;

        /// <summary>
        /// Valid video size (8MB)
        /// </summary>
        public const int ValidVideoSize = 8;
    }
}
