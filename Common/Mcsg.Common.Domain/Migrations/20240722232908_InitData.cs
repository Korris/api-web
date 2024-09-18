using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Mcsg.Common.Domain.Migrations
{
    /// <inheritdoc />
    public partial class InitData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "comic");

            migrationBuilder.EnsureSchema(
                name: "identity");

            migrationBuilder.EnsureSchema(
                name: "system");

            migrationBuilder.EnsureSchema(
                name: "social");

            migrationBuilder.EnsureSchema(
                name: "story");

            migrationBuilder.CreateTable(
                name: "BackgroundMedias",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Url = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Thumbnail = table.Column<string>(type: "text", nullable: true),
                    ArtistName = table.Column<string>(type: "text", nullable: true),
                    DurationSeconds = table.Column<int>(type: "integer", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BackgroundMedias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ComicMetaDatas",
                schema: "comic",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Url = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Description = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    Domain = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    PostId = table.Column<Guid>(type: "uuid", nullable: true),
                    SubPostId = table.Column<Guid>(type: "uuid", nullable: true),
                    PostCommentId = table.Column<Guid>(type: "uuid", nullable: true),
                    SubPostCommentId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComicMetaDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CrawComicChapters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    Url = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    SourceComic = table.Column<Guid>(type: "uuid", nullable: false),
                    ExternalCode = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ExternalLastedUpdate = table.Column<string>(type: "text", nullable: true),
                    ExternalLastedUpdateDate = table.Column<DateTime>(type: "timestamp", nullable: true),
                    View = table.Column<string>(type: "text", nullable: true),
                    ViewNumber = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrawComicChapters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CrawComics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    Url = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Description = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    Author = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    Category = table.Column<string>(type: "text", nullable: true),
                    View = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    Follow = table.Column<string>(type: "text", nullable: true),
                    Rating = table.Column<string>(type: "text", nullable: true),
                    TotalChapter = table.Column<int>(type: "integer", nullable: false),
                    Avatar = table.Column<string>(type: "text", nullable: true),
                    ExternalCode = table.Column<string>(type: "text", nullable: true),
                    ExternalResource = table.Column<int>(type: "integer", nullable: false),
                    ExternalLastedUpdate = table.Column<string>(type: "text", nullable: true),
                    CrawStatus = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrawComics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Devices",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Token = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    UserType = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Jobs",
                schema: "system",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    JobType = table.Column<int>(type: "integer", nullable: false),
                    JobCategory = table.Column<int>(type: "integer", nullable: false),
                    Data = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Error = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Mentions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityType = table.Column<int>(type: "integer", nullable: false),
                    LocationId = table.Column<Guid>(type: "uuid", nullable: false),
                    LocationType = table.Column<int>(type: "integer", nullable: false),
                    Length = table.Column<int>(type: "integer", nullable: false),
                    Offset = table.Column<int>(type: "integer", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mentions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NotificationObjects",
                schema: "system",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    EntityType = table.Column<int>(type: "integer", nullable: false),
                    Action = table.Column<int>(type: "integer", nullable: false),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: true),
                    EntityHashId = table.Column<string>(type: "text", nullable: true),
                    LocationId = table.Column<Guid>(type: "uuid", nullable: true),
                    LocationHashId = table.Column<string>(type: "text", nullable: true),
                    ActorId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationObjects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    DisplayName = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sessions",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    LoginDateUtc = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ExpiredDateUtc = table.Column<DateTime>(type: "timestamp", nullable: false),
                    UserName = table.Column<string>(type: "text", nullable: false),
                    ProfileName = table.Column<string>(type: "text", nullable: true),
                    ProfileId = table.Column<string>(type: "text", nullable: true),
                    UserAvatar = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: true),
                    FirstName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Roles = table.Column<string>(type: "text", nullable: true),
                    Claims = table.Column<string>(type: "text", nullable: true),
                    LastActionDateUtc = table.Column<DateTime>(type: "timestamp", nullable: false),
                    PremiumDate = table.Column<DateOnly>(type: "date", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SmartCountActions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityType = table.Column<int>(type: "integer", nullable: false),
                    SubType = table.Column<int>(type: "integer", nullable: true),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    ActionType = table.Column<int>(type: "integer", nullable: false),
                    Count = table.Column<int>(type: "integer", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmartCountActions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SmartLookups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityType = table.Column<int>(type: "integer", nullable: false),
                    Keyword = table.Column<string>(type: "text", nullable: true),
                    KeywordType = table.Column<int>(type: "integer", nullable: false),
                    CountCriteria = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmartLookups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SocialMetaDatas",
                schema: "social",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Url = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Description = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    Domain = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    PostId = table.Column<Guid>(type: "uuid", nullable: true),
                    SubPostId = table.Column<Guid>(type: "uuid", nullable: true),
                    PostCommentId = table.Column<Guid>(type: "uuid", nullable: true),
                    SubPostCommentId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocialMetaDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StoryMetaDatas",
                schema: "story",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Url = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Description = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    Domain = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    PostId = table.Column<Guid>(type: "uuid", nullable: true),
                    SubPostId = table.Column<Guid>(type: "uuid", nullable: true),
                    PostCommentId = table.Column<Guid>(type: "uuid", nullable: true),
                    SubPostCommentId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryMetaDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemSettings",
                schema: "system",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Description = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    Key = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserNameHistories",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserName = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserNameHistories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserOtps",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    OtpType = table.Column<int>(type: "integer", nullable: false),
                    Token = table.Column<string>(type: "text", nullable: true),
                    Destination = table.Column<string>(type: "text", nullable: true),
                    Code = table.Column<string>(type: "text", nullable: true),
                    ExpiryTime = table.Column<DateTime>(type: "timestamp", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserOtps", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserRefreshTokens",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RefreshToken = table.Column<string>(type: "text", nullable: false),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "timestamp", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRefreshTokens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    ProfileName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ProfileId = table.Column<string>(type: "text", nullable: true),
                    FirstName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    LastName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp", nullable: true),
                    Gender = table.Column<int>(type: "integer", nullable: true),
                    RefreshToken = table.Column<string>(type: "text", nullable: true),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "timestamp", nullable: true),
                    ReferralCode = table.Column<string>(type: "text", nullable: true),
                    Avatar = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ActivedDate = table.Column<DateTime>(type: "timestamp", nullable: true),
                    LastLoginDate = table.Column<DateTime>(type: "timestamp", nullable: true),
                    StatusReason = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    CoverPhoto = table.Column<string>(type: "text", nullable: true),
                    Location = table.Column<string>(type: "text", nullable: true),
                    PremiumDate = table.Column<DateOnly>(type: "date", nullable: true),
                    IsActiveEarning = table.Column<bool>(type: "boolean", nullable: false),
                    IsWalletShowing = table.Column<bool>(type: "boolean", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    CreatedIp = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    LastLoginIp = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    MinioInstance = table.Column<int>(type: "integer", nullable: false),
                    StorageLimit = table.Column<int>(type: "integer", nullable: false),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserSocials",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    SocialId = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    FirstName = table.Column<string>(type: "text", nullable: true),
                    LastName = table.Column<string>(type: "text", nullable: true),
                    IsRegisterBySocial = table.Column<bool>(type: "boolean", nullable: false),
                    RegisterBySocialPlatform = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSocials", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoleClaims",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleClaims_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "identity",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComicPosts",
                schema: "comic",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    Permission = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    HashId = table.Column<string>(type: "character varying(33)", maxLength: 33, nullable: false),
                    ThumbnailUrl = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    CoverUrl = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    AuthorName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    StatusReason = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    ExternalCode = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    Body = table.Column<string>(type: "text", nullable: true),
                    CustomNote = table.Column<string>(type: "text", nullable: true),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    IsMature = table.Column<bool>(type: "boolean", nullable: true),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: true),
                    ViewCount = table.Column<int>(type: "integer", nullable: false),
                    ExternalResource = table.Column<int>(type: "integer", nullable: false),
                    Hide = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComicPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComicPosts_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Feedbacks",
                schema: "system",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Comment = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedbacks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Feedbacks_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                schema: "system",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    NotificationObjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReceiverId = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_NotificationObjects_NotificationObjectId",
                        column: x => x.NotificationObjectId,
                        principalSchema: "system",
                        principalTable: "NotificationObjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_ReceiverId",
                        column: x => x.ReceiverId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Ratings",
                schema: "system",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Satisfaction = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Comment = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ratings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ratings_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SmartLookupUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityType = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Keyword = table.Column<string>(type: "text", nullable: false),
                    KeywordType = table.Column<int>(type: "integer", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmartLookupUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SmartLookupUsers_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SocialPosts",
                schema: "social",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    Permission = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    HashId = table.Column<string>(type: "character varying(33)", maxLength: 33, nullable: false),
                    ThumbnailUrl = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    CoverUrl = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    AuthorName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    StatusReason = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    ExternalCode = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    Body = table.Column<string>(type: "text", nullable: true),
                    CustomNote = table.Column<string>(type: "text", nullable: true),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    IsMature = table.Column<bool>(type: "boolean", nullable: true),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: true),
                    ViewCount = table.Column<int>(type: "integer", nullable: false),
                    ExternalResource = table.Column<int>(type: "integer", nullable: false),
                    Hide = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocialPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SocialPosts_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryPosts",
                schema: "story",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    Permission = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    HashId = table.Column<string>(type: "character varying(33)", maxLength: 33, nullable: false),
                    ThumbnailUrl = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    CoverUrl = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    AuthorName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    StatusReason = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    ExternalCode = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    Body = table.Column<string>(type: "text", nullable: true),
                    CustomNote = table.Column<string>(type: "text", nullable: true),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    IsMature = table.Column<bool>(type: "boolean", nullable: true),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: true),
                    ViewCount = table.Column<int>(type: "integer", nullable: false),
                    ExternalResource = table.Column<int>(type: "integer", nullable: false),
                    Hide = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryPosts_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SystemSettingHistories",
                schema: "system",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    SystemSettingId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    OldValue = table.Column<string>(type: "text", nullable: true),
                    NewValue = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemSettingHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SystemSettingHistories_SystemSettings_SystemSettingId",
                        column: x => x.SystemSettingId,
                        principalSchema: "system",
                        principalTable: "SystemSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SystemSettingHistories_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tags_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserClaims",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserClaims_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserFollows",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    UserFollowerId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserFollowingId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFollows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserFollows_Users_UserFollowerId",
                        column: x => x.UserFollowerId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserFollows_Users_UserFollowingId",
                        column: x => x.UserFollowingId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLogins",
                schema: "identity",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_UserLogins_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserReferrals",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    UserReferrerId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserRefereeId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserReferrals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserReferrals_Users_UserRefereeId",
                        column: x => x.UserRefereeId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserReferrals_Users_UserReferrerId",
                        column: x => x.UserReferrerId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRelations",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    UserId1 = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId2 = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRelations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRelations_Users_UserId1",
                        column: x => x.UserId1,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRelations_Users_UserId2",
                        column: x => x.UserId2,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                schema: "identity",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "identity",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTokens",
                schema: "identity",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_UserTokens_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ViewHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    EntityType = table.Column<int>(type: "integer", nullable: false),
                    SubType = table.Column<int>(type: "integer", nullable: true),
                    IpAddress = table.Column<string>(type: "text", nullable: true),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    UsedId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViewHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ViewHistories_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ComicPostFavorites",
                schema: "comic",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    PostId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComicPostFavorites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComicPostFavorites_ComicPosts_PostId",
                        column: x => x.PostId,
                        principalSchema: "comic",
                        principalTable: "ComicPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComicPostFavorites_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComicPostHides",
                schema: "comic",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    PostId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComicPostHides", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComicPostHides_ComicPosts_PostId",
                        column: x => x.PostId,
                        principalSchema: "comic",
                        principalTable: "ComicPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComicPostHides_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComicPostLinks",
                schema: "comic",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    HashId = table.Column<string>(type: "character varying(33)", maxLength: 33, nullable: true),
                    Url = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Description = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    PostId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComicPostLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComicPostLinks_ComicPosts_PostId",
                        column: x => x.PostId,
                        principalSchema: "comic",
                        principalTable: "ComicPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComicPostReactions",
                schema: "comic",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    TargetId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComicPostReactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComicPostReactions_ComicPosts_TargetId",
                        column: x => x.TargetId,
                        principalSchema: "comic",
                        principalTable: "ComicPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComicPostReactions_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComicPostReports",
                schema: "comic",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    PostId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReasonType = table.Column<int>(type: "integer", nullable: false),
                    ReasonText = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComicPostReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComicPostReports_ComicPosts_PostId",
                        column: x => x.PostId,
                        principalSchema: "comic",
                        principalTable: "ComicPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComicPostReports_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComicSubPosts",
                schema: "comic",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Order = table.Column<float>(type: "real", nullable: false),
                    IsPremium = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    Permission = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    HashId = table.Column<string>(type: "character varying(33)", maxLength: 33, nullable: true),
                    Body = table.Column<string>(type: "text", nullable: true),
                    CreatorNote = table.Column<string>(type: "text", nullable: true),
                    PostId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    PublishDate = table.Column<DateTime>(type: "timestamp", nullable: true),
                    ViewCount = table.Column<int>(type: "integer", nullable: false),
                    IsEnableComment = table.Column<bool>(type: "boolean", nullable: false),
                    ExternalCode = table.Column<string>(type: "text", nullable: true),
                    IsExclusive = table.Column<bool>(type: "boolean", nullable: false),
                    ExternalResource = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComicSubPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComicSubPosts_ComicPosts_PostId",
                        column: x => x.PostId,
                        principalSchema: "comic",
                        principalTable: "ComicPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComicSubPosts_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SystemResources",
                schema: "system",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    HashId = table.Column<string>(type: "character varying(33)", maxLength: 33, nullable: false),
                    Url = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    BucketName = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    MinioInstance = table.Column<int>(type: "integer", nullable: true),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: true),
                    FeedbackId = table.Column<Guid>(type: "uuid", nullable: true),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    Size = table.Column<double>(type: "double precision", nullable: false),
                    CompressedSize = table.Column<double>(type: "double precision", nullable: false),
                    Width = table.Column<int>(type: "integer", nullable: false),
                    Height = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemResources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SystemResources_Feedbacks_FeedbackId",
                        column: x => x.FeedbackId,
                        principalSchema: "system",
                        principalTable: "Feedbacks",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BackgroundMediaPosts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    BackgroundMediaId = table.Column<Guid>(type: "uuid", nullable: false),
                    PostId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BackgroundMediaPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BackgroundMediaPosts_BackgroundMedias_BackgroundMediaId",
                        column: x => x.BackgroundMediaId,
                        principalTable: "BackgroundMedias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BackgroundMediaPosts_SocialPosts_PostId",
                        column: x => x.PostId,
                        principalSchema: "social",
                        principalTable: "SocialPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SocialPostFavorites",
                schema: "social",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    PostId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocialPostFavorites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SocialPostFavorites_SocialPosts_PostId",
                        column: x => x.PostId,
                        principalSchema: "social",
                        principalTable: "SocialPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SocialPostFavorites_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SocialPostHides",
                schema: "social",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    PostId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocialPostHides", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SocialPostHides_SocialPosts_PostId",
                        column: x => x.PostId,
                        principalSchema: "social",
                        principalTable: "SocialPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SocialPostHides_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SocialPostLinks",
                schema: "social",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    HashId = table.Column<string>(type: "character varying(33)", maxLength: 33, nullable: true),
                    Url = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Description = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    PostId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocialPostLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SocialPostLinks_SocialPosts_PostId",
                        column: x => x.PostId,
                        principalSchema: "social",
                        principalTable: "SocialPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SocialPostReactions",
                schema: "social",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    TargetId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocialPostReactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SocialPostReactions_SocialPosts_TargetId",
                        column: x => x.TargetId,
                        principalSchema: "social",
                        principalTable: "SocialPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SocialPostReactions_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SocialPostReports",
                schema: "social",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    PostId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReasonType = table.Column<int>(type: "integer", nullable: false),
                    ReasonText = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocialPostReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SocialPostReports_SocialPosts_PostId",
                        column: x => x.PostId,
                        principalSchema: "social",
                        principalTable: "SocialPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SocialPostReports_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SocialSubPosts",
                schema: "social",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    Permission = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    HashId = table.Column<string>(type: "character varying(33)", maxLength: 33, nullable: true),
                    Body = table.Column<string>(type: "text", nullable: true),
                    CreatorNote = table.Column<string>(type: "text", nullable: true),
                    PostId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    PublishDate = table.Column<DateTime>(type: "timestamp", nullable: true),
                    ViewCount = table.Column<int>(type: "integer", nullable: false),
                    IsEnableComment = table.Column<bool>(type: "boolean", nullable: false),
                    ExternalCode = table.Column<string>(type: "text", nullable: true),
                    IsExclusive = table.Column<bool>(type: "boolean", nullable: false),
                    ExternalResource = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocialSubPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SocialSubPosts_SocialPosts_PostId",
                        column: x => x.PostId,
                        principalSchema: "social",
                        principalTable: "SocialPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SocialSubPosts_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryPostFavorites",
                schema: "story",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    PostId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryPostFavorites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryPostFavorites_StoryPosts_PostId",
                        column: x => x.PostId,
                        principalSchema: "story",
                        principalTable: "StoryPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoryPostFavorites_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryPostHides",
                schema: "story",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    PostId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryPostHides", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryPostHides_StoryPosts_PostId",
                        column: x => x.PostId,
                        principalSchema: "story",
                        principalTable: "StoryPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoryPostHides_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryPostLinks",
                schema: "story",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    HashId = table.Column<string>(type: "character varying(33)", maxLength: 33, nullable: true),
                    Url = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Description = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    PostId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryPostLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryPostLinks_StoryPosts_PostId",
                        column: x => x.PostId,
                        principalSchema: "story",
                        principalTable: "StoryPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryPostReactions",
                schema: "story",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    TargetId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryPostReactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryPostReactions_StoryPosts_TargetId",
                        column: x => x.TargetId,
                        principalSchema: "story",
                        principalTable: "StoryPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoryPostReactions_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryPostReports",
                schema: "story",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    PostId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReasonType = table.Column<int>(type: "integer", nullable: false),
                    ReasonText = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryPostReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryPostReports_StoryPosts_PostId",
                        column: x => x.PostId,
                        principalSchema: "story",
                        principalTable: "StoryPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoryPostReports_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StorySubPosts",
                schema: "story",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Order = table.Column<float>(type: "real", nullable: false),
                    IsPremium = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    Permission = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    HashId = table.Column<string>(type: "character varying(33)", maxLength: 33, nullable: true),
                    Body = table.Column<string>(type: "text", nullable: true),
                    CreatorNote = table.Column<string>(type: "text", nullable: true),
                    PostId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    PublishDate = table.Column<DateTime>(type: "timestamp", nullable: true),
                    ViewCount = table.Column<int>(type: "integer", nullable: false),
                    IsEnableComment = table.Column<bool>(type: "boolean", nullable: false),
                    ExternalCode = table.Column<string>(type: "text", nullable: true),
                    IsExclusive = table.Column<bool>(type: "boolean", nullable: false),
                    ExternalResource = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StorySubPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StorySubPosts_StoryPosts_PostId",
                        column: x => x.PostId,
                        principalSchema: "story",
                        principalTable: "StoryPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StorySubPosts_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComicTagPosts",
                schema: "comic",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    TagId = table.Column<Guid>(type: "uuid", nullable: false),
                    PostId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComicTagPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComicTagPosts_ComicPosts_PostId",
                        column: x => x.PostId,
                        principalSchema: "comic",
                        principalTable: "ComicPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComicTagPosts_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SocialTagPosts",
                schema: "social",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    TagId = table.Column<Guid>(type: "uuid", nullable: false),
                    PostId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocialTagPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SocialTagPosts_SocialPosts_PostId",
                        column: x => x.PostId,
                        principalSchema: "social",
                        principalTable: "SocialPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SocialTagPosts_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryTagPosts",
                schema: "story",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    TagId = table.Column<Guid>(type: "uuid", nullable: false),
                    PostId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryTagPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryTagPosts_StoryPosts_PostId",
                        column: x => x.PostId,
                        principalSchema: "story",
                        principalTable: "StoryPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoryTagPosts_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TagFavorites",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    TagId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagFavorites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TagFavorites_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TagFavorites_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComicResources",
                schema: "comic",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    Title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    HashId = table.Column<string>(type: "character varying(33)", maxLength: 33, nullable: false),
                    Url = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    BucketName = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    MinioInstance = table.Column<int>(type: "integer", nullable: true),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: true),
                    SubPostId = table.Column<Guid>(type: "uuid", nullable: true),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    Size = table.Column<double>(type: "double precision", nullable: false),
                    CompressedSize = table.Column<double>(type: "double precision", nullable: false),
                    Width = table.Column<int>(type: "integer", nullable: false),
                    Height = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    LocationType = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ExternalUrl = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ExternalResource = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComicResources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComicResources_ComicSubPosts_SubPostId",
                        column: x => x.SubPostId,
                        principalSchema: "comic",
                        principalTable: "ComicSubPosts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ComicResources_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ComicSubPostReactions",
                schema: "comic",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    TargetId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComicSubPostReactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComicSubPostReactions_ComicSubPosts_TargetId",
                        column: x => x.TargetId,
                        principalSchema: "comic",
                        principalTable: "ComicSubPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComicSubPostReactions_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SocialResources",
                schema: "social",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    Title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    HashId = table.Column<string>(type: "character varying(33)", maxLength: 33, nullable: false),
                    Url = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    BucketName = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    MinioInstance = table.Column<int>(type: "integer", nullable: true),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: true),
                    SubPostId = table.Column<Guid>(type: "uuid", nullable: true),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    Size = table.Column<double>(type: "double precision", nullable: false),
                    CompressedSize = table.Column<double>(type: "double precision", nullable: false),
                    Width = table.Column<int>(type: "integer", nullable: false),
                    Height = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    LocationType = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ExternalUrl = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ExternalResource = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocialResources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SocialResources_SocialSubPosts_SubPostId",
                        column: x => x.SubPostId,
                        principalSchema: "social",
                        principalTable: "SocialSubPosts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SocialResources_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SocialSubPostReactions",
                schema: "social",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    TargetId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocialSubPostReactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SocialSubPostReactions_SocialSubPosts_TargetId",
                        column: x => x.TargetId,
                        principalSchema: "social",
                        principalTable: "SocialSubPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SocialSubPostReactions_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserExclusiveSubPosts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubPostId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserExclusiveSubPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserExclusiveSubPosts_SocialSubPosts_SubPostId",
                        column: x => x.SubPostId,
                        principalSchema: "social",
                        principalTable: "SocialSubPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserExclusiveSubPosts_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryResources",
                schema: "story",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    Title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    HashId = table.Column<string>(type: "character varying(33)", maxLength: 33, nullable: false),
                    Url = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    BucketName = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    MinioInstance = table.Column<int>(type: "integer", nullable: true),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: true),
                    SubPostId = table.Column<Guid>(type: "uuid", nullable: true),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    Size = table.Column<double>(type: "double precision", nullable: false),
                    CompressedSize = table.Column<double>(type: "double precision", nullable: false),
                    Width = table.Column<int>(type: "integer", nullable: false),
                    Height = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    LocationType = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ExternalUrl = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ExternalResource = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryResources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryResources_StorySubPosts_SubPostId",
                        column: x => x.SubPostId,
                        principalSchema: "story",
                        principalTable: "StorySubPosts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StoryResources_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StorySubPostReactions",
                schema: "story",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    TargetId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StorySubPostReactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StorySubPostReactions_StorySubPosts_TargetId",
                        column: x => x.TargetId,
                        principalSchema: "story",
                        principalTable: "StorySubPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StorySubPostReactions_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComicPostComments",
                schema: "comic",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    Body = table.Column<string>(type: "text", nullable: true),
                    CustomNote = table.Column<string>(type: "text", nullable: true),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    PostId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ResourceId = table.Column<Guid>(type: "uuid", nullable: true),
                    GifId = table.Column<string>(type: "text", nullable: true),
                    QuoteId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComicPostComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComicPostComments_ComicPosts_PostId",
                        column: x => x.PostId,
                        principalSchema: "comic",
                        principalTable: "ComicPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComicPostComments_ComicResources_ResourceId",
                        column: x => x.ResourceId,
                        principalSchema: "comic",
                        principalTable: "ComicResources",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ComicPostComments_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComicSubPostComments",
                schema: "comic",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    Body = table.Column<string>(type: "text", nullable: true),
                    CustomNote = table.Column<string>(type: "text", nullable: true),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    PostId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ResourceId = table.Column<Guid>(type: "uuid", nullable: true),
                    GifId = table.Column<string>(type: "text", nullable: true),
                    QuoteId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComicSubPostComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComicSubPostComments_ComicResources_ResourceId",
                        column: x => x.ResourceId,
                        principalSchema: "comic",
                        principalTable: "ComicResources",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ComicSubPostComments_ComicSubPosts_PostId",
                        column: x => x.PostId,
                        principalSchema: "comic",
                        principalTable: "ComicSubPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComicSubPostComments_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SocialPostComments",
                schema: "social",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    Body = table.Column<string>(type: "text", nullable: true),
                    CustomNote = table.Column<string>(type: "text", nullable: true),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    PostId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ResourceId = table.Column<Guid>(type: "uuid", nullable: true),
                    GifId = table.Column<string>(type: "text", nullable: true),
                    QuoteId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocialPostComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SocialPostComments_SocialPosts_PostId",
                        column: x => x.PostId,
                        principalSchema: "social",
                        principalTable: "SocialPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SocialPostComments_SocialResources_ResourceId",
                        column: x => x.ResourceId,
                        principalSchema: "social",
                        principalTable: "SocialResources",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SocialPostComments_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SocialSubPostComments",
                schema: "social",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    Body = table.Column<string>(type: "text", nullable: true),
                    CustomNote = table.Column<string>(type: "text", nullable: true),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    PostId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ResourceId = table.Column<Guid>(type: "uuid", nullable: true),
                    GifId = table.Column<string>(type: "text", nullable: true),
                    QuoteId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocialSubPostComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SocialSubPostComments_SocialResources_ResourceId",
                        column: x => x.ResourceId,
                        principalSchema: "social",
                        principalTable: "SocialResources",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SocialSubPostComments_SocialSubPosts_PostId",
                        column: x => x.PostId,
                        principalSchema: "social",
                        principalTable: "SocialSubPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SocialSubPostComments_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryPostComments",
                schema: "story",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    Body = table.Column<string>(type: "text", nullable: true),
                    CustomNote = table.Column<string>(type: "text", nullable: true),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    PostId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ResourceId = table.Column<Guid>(type: "uuid", nullable: true),
                    GifId = table.Column<string>(type: "text", nullable: true),
                    QuoteId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryPostComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryPostComments_StoryPosts_PostId",
                        column: x => x.PostId,
                        principalSchema: "story",
                        principalTable: "StoryPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoryPostComments_StoryResources_ResourceId",
                        column: x => x.ResourceId,
                        principalSchema: "story",
                        principalTable: "StoryResources",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StoryPostComments_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StorySubPostComments",
                schema: "story",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    Body = table.Column<string>(type: "text", nullable: true),
                    CustomNote = table.Column<string>(type: "text", nullable: true),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    PostId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ResourceId = table.Column<Guid>(type: "uuid", nullable: true),
                    GifId = table.Column<string>(type: "text", nullable: true),
                    QuoteId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StorySubPostComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StorySubPostComments_StoryResources_ResourceId",
                        column: x => x.ResourceId,
                        principalSchema: "story",
                        principalTable: "StoryResources",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StorySubPostComments_StorySubPosts_PostId",
                        column: x => x.PostId,
                        principalSchema: "story",
                        principalTable: "StorySubPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StorySubPostComments_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComicPostCommentReactions",
                schema: "comic",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    TargetId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComicPostCommentReactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComicPostCommentReactions_ComicPostComments_TargetId",
                        column: x => x.TargetId,
                        principalSchema: "comic",
                        principalTable: "ComicPostComments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComicPostCommentReactions_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComicSubPostCommentReactions",
                schema: "comic",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    TargetId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComicSubPostCommentReactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComicSubPostCommentReactions_ComicSubPostComments_TargetId",
                        column: x => x.TargetId,
                        principalSchema: "comic",
                        principalTable: "ComicSubPostComments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComicSubPostCommentReactions_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SocialPostCommentReactions",
                schema: "social",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    TargetId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocialPostCommentReactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SocialPostCommentReactions_SocialPostComments_TargetId",
                        column: x => x.TargetId,
                        principalSchema: "social",
                        principalTable: "SocialPostComments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SocialPostCommentReactions_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SocialSubPostCommentReactions",
                schema: "social",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    TargetId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocialSubPostCommentReactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SocialSubPostCommentReactions_SocialSubPostComments_TargetId",
                        column: x => x.TargetId,
                        principalSchema: "social",
                        principalTable: "SocialSubPostComments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SocialSubPostCommentReactions_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryPostCommentReactions",
                schema: "story",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    TargetId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryPostCommentReactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryPostCommentReactions_StoryPostComments_TargetId",
                        column: x => x.TargetId,
                        principalSchema: "story",
                        principalTable: "StoryPostComments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoryPostCommentReactions_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StorySubPostCommentReactions",
                schema: "story",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    TargetId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StorySubPostCommentReactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StorySubPostCommentReactions_StorySubPostComments_TargetId",
                        column: x => x.TargetId,
                        principalSchema: "story",
                        principalTable: "StorySubPostComments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StorySubPostCommentReactions_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BackgroundMediaPosts_BackgroundMediaId",
                table: "BackgroundMediaPosts",
                column: "BackgroundMediaId");

            migrationBuilder.CreateIndex(
                name: "IX_BackgroundMediaPosts_PostId",
                table: "BackgroundMediaPosts",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_ComicPostCommentReactions_AuthorId",
                schema: "comic",
                table: "ComicPostCommentReactions",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_ComicPostCommentReactions_TargetId_ParentId_AuthorId",
                schema: "comic",
                table: "ComicPostCommentReactions",
                columns: new[] { "TargetId", "ParentId", "AuthorId" });

            migrationBuilder.CreateIndex(
                name: "IX_ComicPostComments_AuthorId",
                schema: "comic",
                table: "ComicPostComments",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_ComicPostComments_PostId_ParentId_AuthorId",
                schema: "comic",
                table: "ComicPostComments",
                columns: new[] { "PostId", "ParentId", "AuthorId" });

            migrationBuilder.CreateIndex(
                name: "IX_ComicPostComments_ResourceId",
                schema: "comic",
                table: "ComicPostComments",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_ComicPostFavorites_PostId",
                schema: "comic",
                table: "ComicPostFavorites",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_ComicPostFavorites_UserId",
                schema: "comic",
                table: "ComicPostFavorites",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ComicPostHides_PostId",
                schema: "comic",
                table: "ComicPostHides",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_ComicPostHides_UserId",
                schema: "comic",
                table: "ComicPostHides",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ComicPostLinks_PostId",
                schema: "comic",
                table: "ComicPostLinks",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_ComicPostReactions_AuthorId",
                schema: "comic",
                table: "ComicPostReactions",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_ComicPostReactions_TargetId_ParentId_AuthorId",
                schema: "comic",
                table: "ComicPostReactions",
                columns: new[] { "TargetId", "ParentId", "AuthorId" });

            migrationBuilder.CreateIndex(
                name: "IX_ComicPostReports_PostId",
                schema: "comic",
                table: "ComicPostReports",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_ComicPostReports_UserId",
                schema: "comic",
                table: "ComicPostReports",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ComicPosts_HashId_UserId_Type_Id",
                schema: "comic",
                table: "ComicPosts",
                columns: new[] { "HashId", "UserId", "Type", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ComicPosts_UserId",
                schema: "comic",
                table: "ComicPosts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ComicResources_AuthorId",
                schema: "comic",
                table: "ComicResources",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_ComicResources_HashId",
                schema: "comic",
                table: "ComicResources",
                column: "HashId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ComicResources_Name",
                schema: "comic",
                table: "ComicResources",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ComicResources_SubPostId",
                schema: "comic",
                table: "ComicResources",
                column: "SubPostId");

            migrationBuilder.CreateIndex(
                name: "IX_ComicSubPostCommentReactions_AuthorId",
                schema: "comic",
                table: "ComicSubPostCommentReactions",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_ComicSubPostCommentReactions_TargetId_ParentId_AuthorId",
                schema: "comic",
                table: "ComicSubPostCommentReactions",
                columns: new[] { "TargetId", "ParentId", "AuthorId" });

            migrationBuilder.CreateIndex(
                name: "IX_ComicSubPostComments_AuthorId",
                schema: "comic",
                table: "ComicSubPostComments",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_ComicSubPostComments_PostId_ParentId_AuthorId",
                schema: "comic",
                table: "ComicSubPostComments",
                columns: new[] { "PostId", "ParentId", "AuthorId" });

            migrationBuilder.CreateIndex(
                name: "IX_ComicSubPostComments_ResourceId",
                schema: "comic",
                table: "ComicSubPostComments",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_ComicSubPostReactions_AuthorId",
                schema: "comic",
                table: "ComicSubPostReactions",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_ComicSubPostReactions_TargetId_ParentId_AuthorId",
                schema: "comic",
                table: "ComicSubPostReactions",
                columns: new[] { "TargetId", "ParentId", "AuthorId" });

            migrationBuilder.CreateIndex(
                name: "IX_ComicSubPosts_PostId_HashId_AuthorId",
                schema: "comic",
                table: "ComicSubPosts",
                columns: new[] { "PostId", "HashId", "AuthorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ComicSubPosts_UserId",
                schema: "comic",
                table: "ComicSubPosts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ComicTagPosts_PostId",
                schema: "comic",
                table: "ComicTagPosts",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_ComicTagPosts_TagId_PostId",
                schema: "comic",
                table: "ComicTagPosts",
                columns: new[] { "TagId", "PostId" });

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_UserId",
                schema: "system",
                table: "Feedbacks",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_NotificationObjectId",
                schema: "system",
                table: "Notifications",
                column: "NotificationObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_ReceiverId",
                schema: "system",
                table: "Notifications",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_UserId",
                schema: "system",
                table: "Ratings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleClaims_RoleId",
                schema: "identity",
                table: "RoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "identity",
                table: "Roles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SmartCountActions_EntityId_ActionType_Date",
                table: "SmartCountActions",
                columns: new[] { "EntityId", "ActionType", "Date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SmartLookupUsers_UserId",
                table: "SmartLookupUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SocialPostCommentReactions_AuthorId",
                schema: "social",
                table: "SocialPostCommentReactions",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_SocialPostCommentReactions_TargetId_ParentId_AuthorId",
                schema: "social",
                table: "SocialPostCommentReactions",
                columns: new[] { "TargetId", "ParentId", "AuthorId" });

            migrationBuilder.CreateIndex(
                name: "IX_SocialPostComments_AuthorId",
                schema: "social",
                table: "SocialPostComments",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_SocialPostComments_PostId_ParentId_AuthorId",
                schema: "social",
                table: "SocialPostComments",
                columns: new[] { "PostId", "ParentId", "AuthorId" });

            migrationBuilder.CreateIndex(
                name: "IX_SocialPostComments_ResourceId",
                schema: "social",
                table: "SocialPostComments",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_SocialPostFavorites_PostId",
                schema: "social",
                table: "SocialPostFavorites",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_SocialPostFavorites_UserId",
                schema: "social",
                table: "SocialPostFavorites",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SocialPostHides_PostId",
                schema: "social",
                table: "SocialPostHides",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_SocialPostHides_UserId",
                schema: "social",
                table: "SocialPostHides",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SocialPostLinks_PostId",
                schema: "social",
                table: "SocialPostLinks",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_SocialPostReactions_AuthorId",
                schema: "social",
                table: "SocialPostReactions",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_SocialPostReactions_TargetId_ParentId_AuthorId",
                schema: "social",
                table: "SocialPostReactions",
                columns: new[] { "TargetId", "ParentId", "AuthorId" });

            migrationBuilder.CreateIndex(
                name: "IX_SocialPostReports_PostId",
                schema: "social",
                table: "SocialPostReports",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_SocialPostReports_UserId",
                schema: "social",
                table: "SocialPostReports",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SocialPosts_HashId_UserId_Type_Id",
                schema: "social",
                table: "SocialPosts",
                columns: new[] { "HashId", "UserId", "Type", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SocialPosts_UserId",
                schema: "social",
                table: "SocialPosts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SocialResources_AuthorId",
                schema: "social",
                table: "SocialResources",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_SocialResources_HashId",
                schema: "social",
                table: "SocialResources",
                column: "HashId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SocialResources_Name",
                schema: "social",
                table: "SocialResources",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SocialResources_SubPostId",
                schema: "social",
                table: "SocialResources",
                column: "SubPostId");

            migrationBuilder.CreateIndex(
                name: "IX_SocialSubPostCommentReactions_AuthorId",
                schema: "social",
                table: "SocialSubPostCommentReactions",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_SocialSubPostCommentReactions_TargetId_ParentId_AuthorId",
                schema: "social",
                table: "SocialSubPostCommentReactions",
                columns: new[] { "TargetId", "ParentId", "AuthorId" });

            migrationBuilder.CreateIndex(
                name: "IX_SocialSubPostComments_AuthorId",
                schema: "social",
                table: "SocialSubPostComments",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_SocialSubPostComments_PostId_ParentId_AuthorId",
                schema: "social",
                table: "SocialSubPostComments",
                columns: new[] { "PostId", "ParentId", "AuthorId" });

            migrationBuilder.CreateIndex(
                name: "IX_SocialSubPostComments_ResourceId",
                schema: "social",
                table: "SocialSubPostComments",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_SocialSubPostReactions_AuthorId",
                schema: "social",
                table: "SocialSubPostReactions",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_SocialSubPostReactions_TargetId_ParentId_AuthorId",
                schema: "social",
                table: "SocialSubPostReactions",
                columns: new[] { "TargetId", "ParentId", "AuthorId" });

            migrationBuilder.CreateIndex(
                name: "IX_SocialSubPosts_PostId_HashId_AuthorId",
                schema: "social",
                table: "SocialSubPosts",
                columns: new[] { "PostId", "HashId", "AuthorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SocialSubPosts_UserId",
                schema: "social",
                table: "SocialSubPosts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SocialTagPosts_PostId",
                schema: "social",
                table: "SocialTagPosts",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_SocialTagPosts_TagId_PostId",
                schema: "social",
                table: "SocialTagPosts",
                columns: new[] { "TagId", "PostId" });

            migrationBuilder.CreateIndex(
                name: "IX_StoryPostCommentReactions_AuthorId",
                schema: "story",
                table: "StoryPostCommentReactions",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryPostCommentReactions_TargetId_ParentId_AuthorId",
                schema: "story",
                table: "StoryPostCommentReactions",
                columns: new[] { "TargetId", "ParentId", "AuthorId" });

            migrationBuilder.CreateIndex(
                name: "IX_StoryPostComments_AuthorId",
                schema: "story",
                table: "StoryPostComments",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryPostComments_PostId_ParentId_AuthorId",
                schema: "story",
                table: "StoryPostComments",
                columns: new[] { "PostId", "ParentId", "AuthorId" });

            migrationBuilder.CreateIndex(
                name: "IX_StoryPostComments_ResourceId",
                schema: "story",
                table: "StoryPostComments",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryPostFavorites_PostId",
                schema: "story",
                table: "StoryPostFavorites",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryPostFavorites_UserId",
                schema: "story",
                table: "StoryPostFavorites",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryPostHides_PostId",
                schema: "story",
                table: "StoryPostHides",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryPostHides_UserId",
                schema: "story",
                table: "StoryPostHides",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryPostLinks_PostId",
                schema: "story",
                table: "StoryPostLinks",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryPostReactions_AuthorId",
                schema: "story",
                table: "StoryPostReactions",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryPostReactions_TargetId_ParentId_AuthorId",
                schema: "story",
                table: "StoryPostReactions",
                columns: new[] { "TargetId", "ParentId", "AuthorId" });

            migrationBuilder.CreateIndex(
                name: "IX_StoryPostReports_PostId",
                schema: "story",
                table: "StoryPostReports",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryPostReports_UserId",
                schema: "story",
                table: "StoryPostReports",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryPosts_HashId_UserId_Type_Id",
                schema: "story",
                table: "StoryPosts",
                columns: new[] { "HashId", "UserId", "Type", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryPosts_UserId",
                schema: "story",
                table: "StoryPosts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryResources_AuthorId",
                schema: "story",
                table: "StoryResources",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryResources_HashId",
                schema: "story",
                table: "StoryResources",
                column: "HashId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryResources_Name",
                schema: "story",
                table: "StoryResources",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryResources_SubPostId",
                schema: "story",
                table: "StoryResources",
                column: "SubPostId");

            migrationBuilder.CreateIndex(
                name: "IX_StorySubPostCommentReactions_AuthorId",
                schema: "story",
                table: "StorySubPostCommentReactions",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_StorySubPostCommentReactions_TargetId_ParentId_AuthorId",
                schema: "story",
                table: "StorySubPostCommentReactions",
                columns: new[] { "TargetId", "ParentId", "AuthorId" });

            migrationBuilder.CreateIndex(
                name: "IX_StorySubPostComments_AuthorId",
                schema: "story",
                table: "StorySubPostComments",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_StorySubPostComments_PostId_ParentId_AuthorId",
                schema: "story",
                table: "StorySubPostComments",
                columns: new[] { "PostId", "ParentId", "AuthorId" });

            migrationBuilder.CreateIndex(
                name: "IX_StorySubPostComments_ResourceId",
                schema: "story",
                table: "StorySubPostComments",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_StorySubPostReactions_AuthorId",
                schema: "story",
                table: "StorySubPostReactions",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_StorySubPostReactions_TargetId_ParentId_AuthorId",
                schema: "story",
                table: "StorySubPostReactions",
                columns: new[] { "TargetId", "ParentId", "AuthorId" });

            migrationBuilder.CreateIndex(
                name: "IX_StorySubPosts_PostId_HashId_AuthorId",
                schema: "story",
                table: "StorySubPosts",
                columns: new[] { "PostId", "HashId", "AuthorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StorySubPosts_UserId",
                schema: "story",
                table: "StorySubPosts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryTagPosts_PostId",
                schema: "story",
                table: "StoryTagPosts",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryTagPosts_TagId_PostId",
                schema: "story",
                table: "StoryTagPosts",
                columns: new[] { "TagId", "PostId" });

            migrationBuilder.CreateIndex(
                name: "IX_SystemResources_FeedbackId",
                schema: "system",
                table: "SystemResources",
                column: "FeedbackId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemResources_HashId",
                schema: "system",
                table: "SystemResources",
                column: "HashId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SystemResources_Name",
                schema: "system",
                table: "SystemResources",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SystemSettingHistories_SystemSettingId",
                schema: "system",
                table: "SystemSettingHistories",
                column: "SystemSettingId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemSettingHistories_UserId",
                schema: "system",
                table: "SystemSettingHistories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemSettings_Key",
                schema: "system",
                table: "SystemSettings",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TagFavorites_TagId",
                table: "TagFavorites",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_TagFavorites_UserId",
                table: "TagFavorites",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_AuthorId",
                table: "Tags",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Name",
                table: "Tags",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_UserId",
                schema: "identity",
                table: "UserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserExclusiveSubPosts_SubPostId",
                table: "UserExclusiveSubPosts",
                column: "SubPostId");

            migrationBuilder.CreateIndex(
                name: "IX_UserExclusiveSubPosts_UserId",
                table: "UserExclusiveSubPosts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFollows_UserFollowerId",
                schema: "identity",
                table: "UserFollows",
                column: "UserFollowerId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFollows_UserFollowingId",
                schema: "identity",
                table: "UserFollows",
                column: "UserFollowingId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogins_UserId",
                schema: "identity",
                table: "UserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserReferrals_UserRefereeId",
                schema: "identity",
                table: "UserReferrals",
                column: "UserRefereeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserReferrals_UserReferrerId",
                schema: "identity",
                table: "UserReferrals",
                column: "UserReferrerId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRelations_UserId1",
                schema: "identity",
                table: "UserRelations",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_UserRelations_UserId2",
                schema: "identity",
                table: "UserRelations",
                column: "UserId2");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                schema: "identity",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "identity",
                table: "Users",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ReferralCode",
                schema: "identity",
                table: "Users",
                column: "ReferralCode");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "identity",
                table: "Users",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ViewHistories_EntityId_UsedId_EntityType_CreatedOn",
                table: "ViewHistories",
                columns: new[] { "EntityId", "UsedId", "EntityType", "CreatedOn" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ViewHistories_UserId",
                table: "ViewHistories",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BackgroundMediaPosts");

            migrationBuilder.DropTable(
                name: "ComicMetaDatas",
                schema: "comic");

            migrationBuilder.DropTable(
                name: "ComicPostCommentReactions",
                schema: "comic");

            migrationBuilder.DropTable(
                name: "ComicPostFavorites",
                schema: "comic");

            migrationBuilder.DropTable(
                name: "ComicPostHides",
                schema: "comic");

            migrationBuilder.DropTable(
                name: "ComicPostLinks",
                schema: "comic");

            migrationBuilder.DropTable(
                name: "ComicPostReactions",
                schema: "comic");

            migrationBuilder.DropTable(
                name: "ComicPostReports",
                schema: "comic");

            migrationBuilder.DropTable(
                name: "ComicSubPostCommentReactions",
                schema: "comic");

            migrationBuilder.DropTable(
                name: "ComicSubPostReactions",
                schema: "comic");

            migrationBuilder.DropTable(
                name: "ComicTagPosts",
                schema: "comic");

            migrationBuilder.DropTable(
                name: "CrawComicChapters");

            migrationBuilder.DropTable(
                name: "CrawComics");

            migrationBuilder.DropTable(
                name: "Devices",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "Jobs",
                schema: "system");

            migrationBuilder.DropTable(
                name: "Mentions");

            migrationBuilder.DropTable(
                name: "Notifications",
                schema: "system");

            migrationBuilder.DropTable(
                name: "Ratings",
                schema: "system");

            migrationBuilder.DropTable(
                name: "RoleClaims",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "Sessions",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "SmartCountActions");

            migrationBuilder.DropTable(
                name: "SmartLookups");

            migrationBuilder.DropTable(
                name: "SmartLookupUsers");

            migrationBuilder.DropTable(
                name: "SocialMetaDatas",
                schema: "social");

            migrationBuilder.DropTable(
                name: "SocialPostCommentReactions",
                schema: "social");

            migrationBuilder.DropTable(
                name: "SocialPostFavorites",
                schema: "social");

            migrationBuilder.DropTable(
                name: "SocialPostHides",
                schema: "social");

            migrationBuilder.DropTable(
                name: "SocialPostLinks",
                schema: "social");

            migrationBuilder.DropTable(
                name: "SocialPostReactions",
                schema: "social");

            migrationBuilder.DropTable(
                name: "SocialPostReports",
                schema: "social");

            migrationBuilder.DropTable(
                name: "SocialSubPostCommentReactions",
                schema: "social");

            migrationBuilder.DropTable(
                name: "SocialSubPostReactions",
                schema: "social");

            migrationBuilder.DropTable(
                name: "SocialTagPosts",
                schema: "social");

            migrationBuilder.DropTable(
                name: "StoryMetaDatas",
                schema: "story");

            migrationBuilder.DropTable(
                name: "StoryPostCommentReactions",
                schema: "story");

            migrationBuilder.DropTable(
                name: "StoryPostFavorites",
                schema: "story");

            migrationBuilder.DropTable(
                name: "StoryPostHides",
                schema: "story");

            migrationBuilder.DropTable(
                name: "StoryPostLinks",
                schema: "story");

            migrationBuilder.DropTable(
                name: "StoryPostReactions",
                schema: "story");

            migrationBuilder.DropTable(
                name: "StoryPostReports",
                schema: "story");

            migrationBuilder.DropTable(
                name: "StorySubPostCommentReactions",
                schema: "story");

            migrationBuilder.DropTable(
                name: "StorySubPostReactions",
                schema: "story");

            migrationBuilder.DropTable(
                name: "StoryTagPosts",
                schema: "story");

            migrationBuilder.DropTable(
                name: "SystemResources",
                schema: "system");

            migrationBuilder.DropTable(
                name: "SystemSettingHistories",
                schema: "system");

            migrationBuilder.DropTable(
                name: "TagFavorites");

            migrationBuilder.DropTable(
                name: "UserClaims",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "UserExclusiveSubPosts");

            migrationBuilder.DropTable(
                name: "UserFollows",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "UserLogins",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "UserNameHistories",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "UserOtps",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "UserReferrals",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "UserRefreshTokens",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "UserRelations",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "UserRoles",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "UserSocials",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "UserTokens",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "ViewHistories");

            migrationBuilder.DropTable(
                name: "BackgroundMedias");

            migrationBuilder.DropTable(
                name: "ComicPostComments",
                schema: "comic");

            migrationBuilder.DropTable(
                name: "ComicSubPostComments",
                schema: "comic");

            migrationBuilder.DropTable(
                name: "NotificationObjects",
                schema: "system");

            migrationBuilder.DropTable(
                name: "SocialPostComments",
                schema: "social");

            migrationBuilder.DropTable(
                name: "SocialSubPostComments",
                schema: "social");

            migrationBuilder.DropTable(
                name: "StoryPostComments",
                schema: "story");

            migrationBuilder.DropTable(
                name: "StorySubPostComments",
                schema: "story");

            migrationBuilder.DropTable(
                name: "Feedbacks",
                schema: "system");

            migrationBuilder.DropTable(
                name: "SystemSettings",
                schema: "system");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "ComicResources",
                schema: "comic");

            migrationBuilder.DropTable(
                name: "SocialResources",
                schema: "social");

            migrationBuilder.DropTable(
                name: "StoryResources",
                schema: "story");

            migrationBuilder.DropTable(
                name: "ComicSubPosts",
                schema: "comic");

            migrationBuilder.DropTable(
                name: "SocialSubPosts",
                schema: "social");

            migrationBuilder.DropTable(
                name: "StorySubPosts",
                schema: "story");

            migrationBuilder.DropTable(
                name: "ComicPosts",
                schema: "comic");

            migrationBuilder.DropTable(
                name: "SocialPosts",
                schema: "social");

            migrationBuilder.DropTable(
                name: "StoryPosts",
                schema: "story");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "identity");
        }
    }
}
