namespace Mcsg.Social.Api.Constants
{
    public static class ApiErrorMessage
    {
        // Resource
        public const string NotFoundThumbnail = "No thumbnail found for the URL.";
        public const string NotFileUpload = "No file uploaded.";
        public const string InvalidFile = "Invalid file.";
        public const string InvalidParentFolder = "Invalid parent folder.";
        public const string OnlyMediaFile = "Only media files are allowed.";
        public const string OnlyImageFile = "Only image files are allowed.";

        //Tag
        public const string TAG_NOT_EXIST = "Tag does not exist";

        //Post
        public const string POST_NOT_EXIST = "Post does not exist";
        public const string USER_NOT_PERMISSION = "This user not permission to do this action";
        public const string POST_HAS_DELETED = "This post has deleted";
        public const string POST_HAS_COMPLETED = "This post has completed, can not add more chapter";
        public const string CHAPTER_NOT_EXIST = "Chapter order {0} not exist!";
        public const string POST_REQ_TOTAL_EXCEEDS_LIMIT = "The total number of items cannot exceed 100";
        public const string NEED_PREMIUM_TO_READ = "You need to upgrade to a premium account to view this content";
        public const string NEED_BUY_TO_READ = "You need to upgrade to a premium account to view this content";

        public const string POST_TITLE_LESS_THAN_CHARACTER = "Title much be less than 255 characters";
        public const string POST_NOTE_LESS_THAN_CHARACTER = "Body much be less than 2000 characters";
        public const string POST_DATE_PUBLISH_NULL = "Post has schedule but date publish is null";

        public const string INVALID_AVATAR_IMAGE = "Avatar image is not null";
        public const string INVALID_COVER_PHOTO_IMAGE = "Cover photo image is not null";
        public const string INVALID_AVATAR_FILE_TYPE = "Avatar should be image";
        public const string INVALID_COVER_PHOTO_FILE_TYPE = "Cover photo should be image";
        public const string EXISTING_PROFILE_NAME = "Profile name already exists in the system";
        public const string PROFILE_NAME_NOT_EMPTY = "Profile name is empty";
        public const string NOT_FOUND = "Not found";
        public const string DUPLICATE_USERNAME = "Duplicate username";

        // Affiliate
        public const string INVALID_AFFILIATE_ENTITY_TYPE = "Invalid Affiliate Type request";
    }
}
