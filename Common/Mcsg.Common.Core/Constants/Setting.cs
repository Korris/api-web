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
                { "Cre", "Credential" },
                { "Hea", "Health" },
                { "Not", "Notification" },
                { "Ser", "Services" },
                { "Sho", "Shop" },
                { "Soc", "Social" }
            };
        }
    }

    /// <summary>
    /// Developer name
    /// </summary>
    public class DevName
    {
        /// <summary>
        /// ComBreed
        /// </summary>
        public const string ComBreed = "CBR";

        /// <summary>
        /// ComPet
        /// </summary>
        public const string ComPet = "CPT";

        /// <summary>
        /// ComUser
        /// </summary>
        public const string ComUser = "CUS";

        /// <summary>
        /// CreMenu
        /// </summary>
        public const string CreMenu = "CMN";

        /// <summary>
        /// NotDevice
        /// </summary>
        public const string NotDevice = "NDV";

        /// <summary>
        /// SerEnterprise
        /// </summary>
        public const string SerEnterprise = "SET";

        /// <summary>
        /// SerLocation
        /// </summary>
        public const string SerLocation = "SLC";

        /// <summary>
        /// SocHashtag
        /// </summary>
        public const string SocHashtag = "SHT";
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
        /// Enterprise thumbnail
        /// </summary>
        public const string EnterpriseThumbnail = "users/UA000001/Enterprises/default.png";

        /// <summary>
        /// Service thumbnail
        /// </summary>
        public const string ServiceThumbnail = "users/UA000001/Services/default.png";
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

        /// <summary>
        /// Posts
        /// </summary>
        public const string Posts = "Posts";

        /// <summary>
        /// Moments
        /// </summary>
        public const string Moments = "Moments";

        /// <summary>
        /// Files
        /// </summary>
        public const string Files = "Files";

        /// <summary>
        /// Comments
        /// </summary>
        public const string Comments = "Comments";

        /// <summary>
        /// CommentSubs
        /// </summary>
        public const string CommentSubs = "CommentSubs";
    }
}
