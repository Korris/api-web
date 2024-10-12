namespace Mcsg.Story.Api.Constants;

public static class ApiErrorCode
{
    // Resource
    public const string NotFoundThumbnail = "ERR_API_000001";
    public const string InvalidFile = "ERR_API_000003";
    public const string OnlyMediaFile = "ERR_API_000005";

    //Tag
    public const string TAG_NOT_EXIST = "ERR_API_100001";

    //Post
    public const string POST_HAS_COMPLETED = "ERR_API_200007";
    public const string CHAPTER_NOT_EXIST = "ERR_API_200008";
    public const string POST_REQ_TOTAL_EXCEEDS_LIMIT = "ERR_API_200009";
    public const string NEED_PREMIUM_TO_READ = "ERR_API_200010";
    public const string NEED_BUY_TO_READ = "ERR_API_200011";

    public const string POST_TITLE_LESS_THAN_CHARACTER = "ERR_API_200004";
    public const string POST_NOTE_LESS_THAN_CHARACTER = "ERR_API_200005";
    public const string POST_DATE_PUBLISH_NULL = "ERR_API_200006";

    public const string INVALID_AVATAR_IMAGE = "ERR_API_300001";
    public const string INVALID_COVER_PHOTO_IMAGE = "ERR_API_300002";
    public const string INVALID_FILE_TYPE = "ERR_API_300003";
    public const string EXISTING_PROFILE_NAME = "ERR_API_300004";
    public const string PROFILE_NAME_NOT_EMPTY = "ERR_API_300005";
    public const string NOT_FOUND = "ERR_API_300006";
    public const string DUPLICATE_USERNAME = "ERR_API_300007";
    public const string NEED_PREMIUM_TO_EDIT = "ERR_API_300008";
    public const string CHAPTER_EXISTED = "ERR_API_300009";

    // User
    public const string ALREADY_EXISTS = "ERR_API_300001";
    public const string INVALID_OPERATION = "ERR_API_300002";

    // Affiliate
    public const string INVALID_AFFILIATE_ENTITY_TYPE = "ERR_API_300006";
}
