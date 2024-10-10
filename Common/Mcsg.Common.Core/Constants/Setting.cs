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

using Enums;

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
                { "Ana", MicroService.Analytic.ToString() },
                { "Cmc", MicroService.Comic.ToString() },
                { "Job", MicroService.Job.ToString() },
                { "Ide", MicroService.Identity.ToString() },
                { "Med", MicroService.Media.ToString() },
                { "Rea", MicroService.Realtime.ToString() },
                { "Soc", MicroService.Social.ToString() },
                { "Sto", MicroService.Story.ToString() },
                { "Syn", MicroService.Sync.ToString() },
                { "Wal", MicroService.Wallet.ToString() },
                { "Clo", MicroService.CloneSite.ToString() },
                { "Fun", MicroService.Function.ToString() },
                { "Ope", MicroService.OpenId.ToString() },
                { "Wjo", MicroService.WalletJob.ToString() }
            };
        }
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

        /// <summary>
        /// Custom note
        /// </summary>
        public static string CustomNote = "{\"root\":{\"children\":[{\"children\":[],\"direction\":null,\"format\":\"\",\"indent\":0,\"type\":\"paragraph\",\"version\":1}],\"direction\":null,\"format\":\"\",\"indent\":0,\"type\":\"root\",\"version\":1}}";
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
    public class PostConfig
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

    /// <summary>
    /// File location
    /// </summary>
    public class FileLocation
    {
        /// <summary>
        /// Audio
        /// </summary>
        public const string Audio = "audios";

        /// <summary>
        /// Image
        /// </summary>
        public const string Image = "images";

        /// <summary>
        /// Video
        /// </summary>
        public const string Video = "videos";

        /// <summary>
        /// Other
        /// </summary>
        public const string Other = "others";

        /// <summary>
        /// Temp
        /// </summary>
        public const string Temp = "temp";
    }

    /// <summary>
    /// File extension
    /// </summary>
    public class FileExt
    {
        /// <summary>
        /// Audios
        /// </summary>
        public static string[] Audios = { ".mp3", ".wav", ".ogg" };

        /// <summary>
        /// Images
        /// </summary>
        public static string[] Images = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".heic", ".webp", ".heif" };

        /// <summary>
        /// Videos
        /// </summary>
        public static string[] Videos = { ".mp4", ".avi", ".mkv", ".mov", ".webm" };

        /// <summary>
        /// Allow play after upload
        /// </summary>
        public static string[] AllowPlayAfterUpload = { ".mp4", ".webm", ".ogg", ".mp3" };
    }

    /// <summary>
    /// File type
    /// </summary>
    public class FileType
    {
        /// <summary>
        /// Images
        /// </summary>
        public static string[] Images = { "image/jpeg", "image/png", "image/gif", "image/heic" };
    }

    /// <summary>
    /// Resource config
    /// </summary>
    public class ResourceConfig
    {
        /// <summary>
        /// Hash length
        /// </summary>
        public const int HashLength = 32;
    }

    /// <summary>
    /// PostResource type
    /// </summary>
    public class PostResourceType
    {
        /// <summary>
        /// Cover
        /// </summary>
        public const string Cover = "Cover";

        /// <summary>
        /// Thumb
        /// </summary>
        public const string Thumb = "Thumb";
    }

    /// <summary>
    /// Notification type
    /// </summary>
    public class NotificationType
    {
        /// <summary>
        /// Comment
        /// </summary>
        public const string Comment = "Comment";

        /// <summary>
        /// Reply
        /// </summary>
        public const string Reply = "Reply";

        /// <summary>
        /// Video
        /// </summary>
        public const string Video = "Video";

        /// <summary>
        /// Feed
        /// </summary>
        public const string Feed = "Feed";

        /// <summary>
        /// Reaction
        /// </summary>
        public const string Reaction = "Reaction";

        /// <summary>
        /// Mention
        /// </summary>
        public const string Mention = "Mention";

        /// <summary>
        /// FollowUser
        /// </summary>
        public const string FollowUser = "FollowUser";

        /// <summary>
        /// FollowPost
        /// </summary>
        public const string FollowPost = "FollowPost";

        /// <summary>
        /// DonateTransaction
        /// </summary>
        public const string DonateTransaction = "DonateTransaction";

        /// <summary>
        /// DonateTransaction
        /// </summary>
        public const string TransferTransaction = "TransferTransaction";
    }

    /// <summary>
    /// NotificationTarget type
    /// </summary>
    public class NotificationTargetType
    {
        /// <summary>
        /// None
        /// </summary>
        public const string None = "None";

        /// <summary>
        /// Feed
        /// </summary>
        public const string Feed = "Feed";

        /// <summary>
        /// Comic
        /// </summary>
        public const string Comic = "Comic";

        /// <summary>
        /// Story
        /// </summary>
        public const string Story = "Story";

        /// <summary>
        /// SubFeed
        /// </summary>
        public const string SubFeed = "SubFeed";

        /// <summary>
        /// SubComic
        /// </summary>
        public const string SubComic = "SubComic";

        /// <summary>
        /// SubStory
        /// </summary>
        public const string SubStory = "SubStory";

        /// <summary>
        /// CommentOnFeed
        /// </summary>
        public const string CommentOnFeed = "CommentOnFeed";

        /// <summary>
        /// CommentOnComic
        /// </summary>
        public const string CommentOnComic = "CommentOnComic";

        /// <summary>
        /// CommentOnStory
        /// </summary>
        public const string CommentOnStory = "CommentOnStory";

        /// <summary>
        /// CommentOnSubFeed
        /// </summary>
        public const string CommentOnSubFeed = "CommentOnSubFeed";

        /// <summary>
        /// CommentOnSubStory
        /// </summary>
        public const string CommentOnSubComic = "CommentOnSubComic";

        /// <summary>
        /// CommentOnSubStory
        /// </summary>
        public const string CommentOnSubStory = "CommentOnSubStory";

        /// <summary>
        /// ReplyOnFeed
        /// </summary>
        public const string ReplyOnFeed = "ReplyOnFeed";

        /// <summary>
        /// ReplyOnComic
        /// </summary>
        public const string ReplyOnComic = "ReplyOnComic";

        /// <summary>
        /// ReplyOnStory
        /// </summary>
        public const string ReplyOnStory = "ReplyOnStory";

        /// <summary>
        /// ReplyOnSubFeed
        /// </summary>
        public const string ReplyOnSubFeed = "ReplyOnSubFeed";

        /// <summary>
        /// ReplyOnSubComic
        /// </summary>
        public const string ReplyOnSubComic = "ReplyOnSubComic";

        /// <summary>
        /// ReplyOnSubStory
        /// </summary>
        public const string ReplyOnSubStory = "ReplyOnSubStory";

        /// <summary>
        /// FollowUser
        /// </summary>
        public const string FollowUser = "FollowUser";

        /// <summary>
        /// FollowComicPost
        /// </summary>
        public const string FollowComicPost = "FollowComicPost";

        /// <summary>
        /// FollowStoryPost
        /// </summary>
        public const string FollowStoryPost = "FollowStoryPost";

        /// <summary>
        /// Transaction
        /// </summary>
        public const string Transaction = "Transaction";
    }

    /// <summary>
    /// Minio folder
    /// </summary>
    public static class MinioFolder
    {
        /// <summary>
        /// Comic
        /// </summary>
        public const string Comic = "comic";

        /// <summary>
        /// Social
        /// </summary>
        public const string Social = "social";

        /// <summary>
        /// Story
        /// </summary>
        public const string Story = "story";

        /// <summary>
        /// Image
        /// </summary>
        public const string Image = "images";

        /// <summary>
        /// User
        /// </summary>
        public const string User = "users";
    }
}
