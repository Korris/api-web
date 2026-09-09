using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

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
                name: "document");

            migrationBuilder.EnsureSchema(
                name: "system");

            migrationBuilder.EnsureSchema(
                name: "openid");

            migrationBuilder.EnsureSchema(
                name: "social");

            migrationBuilder.EnsureSchema(
                name: "story");

            migrationBuilder.EnsureSchema(
                name: "game");

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
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
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
                name: "ComicReports",
                schema: "comic",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityType = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ExpiredBlock = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReasonType = table.Column<int>(type: "integer", nullable: true),
                    ReasonText = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComicReports", x => x.Id);
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
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
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
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
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
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DocumentMetaDatas",
                schema: "document",
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
                    table.PrimaryKey("PK_DocumentMetaDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DocumentReports",
                schema: "document",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityType = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ExpiredBlock = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReasonType = table.Column<int>(type: "integer", nullable: true),
                    ReasonText = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentReports", x => x.Id);
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
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
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
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
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
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationObjects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OpenIdApplications",
                schema: "openid",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ApplicationType = table.Column<string>(type: "text", nullable: true),
                    ClientId = table.Column<string>(type: "text", nullable: true),
                    ClientSecret = table.Column<string>(type: "text", nullable: true),
                    ClientType = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyToken = table.Column<string>(type: "text", nullable: true),
                    ConsentType = table.Column<string>(type: "text", nullable: true),
                    DisplayName = table.Column<string>(type: "text", nullable: true),
                    DisplayNames = table.Column<string>(type: "text", nullable: true),
                    JsonWebKeySet = table.Column<string>(type: "text", nullable: true),
                    Permissions = table.Column<string>(type: "text", nullable: true),
                    PostLogoutRedirectUris = table.Column<string>(type: "text", nullable: true),
                    Properties = table.Column<string>(type: "text", nullable: true),
                    RedirectUris = table.Column<string>(type: "text", nullable: true),
                    Requirements = table.Column<string>(type: "text", nullable: true),
                    Settings = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpenIdApplications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OpenIdScopes",
                schema: "openid",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyToken = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Descriptions = table.Column<string>(type: "text", nullable: true),
                    DisplayName = table.Column<string>(type: "text", nullable: true),
                    DisplayNames = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Properties = table.Column<string>(type: "text", nullable: true),
                    Resources = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpenIdScopes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    DisplayName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
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
                name: "SocialReports",
                schema: "social",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityType = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ExpiredBlock = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReasonType = table.Column<int>(type: "integer", nullable: true),
                    ReasonText = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocialReports", x => x.Id);
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
                name: "StoryReports",
                schema: "story",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityType = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ExpiredBlock = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReasonType = table.Column<int>(type: "integer", nullable: true),
                    ReasonText = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryReports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemConfigs",
                schema: "system",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Description = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    Key = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true),
                    DataType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemConfigs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemSettings",
                schema: "system",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Description = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    Key = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    DataType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    Group = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    MicroService = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserAuthenticators",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    SecretKey = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsLogin = table.Column<bool>(type: "boolean", nullable: false),
                    IsTransaction = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAuthenticators", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserNameHistories",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
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
                    Token = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Destination = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Code = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: true),
                    ExpiryTime = table.Column<DateTime>(type: "timestamp", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserOtps", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserRecoveries",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    SecretKey = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRecoveries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserRefreshTokens",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RefreshToken = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
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
                    ProfileId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    FirstName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    LastName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp", nullable: true),
                    Gender = table.Column<int>(type: "integer", nullable: true),
                    RefreshToken = table.Column<string>(type: "text", nullable: true),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "timestamp", nullable: true),
                    ReferralCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Avatar = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ActivedDate = table.Column<DateTime>(type: "timestamp", nullable: true),
                    LastLoginDate = table.Column<DateTime>(type: "timestamp", nullable: true),
                    StatusReason = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    CoverPhoto = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Location = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    PremiumDate = table.Column<DateTime>(type: "timestamp", nullable: true),
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
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Language = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: true),
                    NextCheckPremium = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsExpiredSubscriptionSent = table.Column<bool>(type: "boolean", nullable: true),
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
                    SocialId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Type = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    PhoneNumber = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    FirstName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    LastName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    IsRegisterBySocial = table.Column<bool>(type: "boolean", nullable: false),
                    RegisterBySocialPlatform = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSocials", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OpenIdAuthorizations",
                schema: "openid",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ApplicationId = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyToken = table.Column<string>(type: "text", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Properties = table.Column<string>(type: "text", nullable: true),
                    Scopes = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    Subject = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpenIdAuthorizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OpenIdAuthorizations_OpenIdApplications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalSchema: "openid",
                        principalTable: "OpenIdApplications",
                        principalColumn: "Id");
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
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Permission = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    HashId = table.Column<string>(type: "character varying(33)", maxLength: 33, nullable: false),
                    ThumbnailUrl = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    CoverUrl = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    AuthorName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    StatusReason = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    ExternalCode = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    Body = table.Column<string>(type: "text", nullable: true),
                    ShortBody = table.Column<string>(type: "text", nullable: true),
                    CustomNote = table.Column<string>(type: "text", nullable: true),
                    ShortCustomNote = table.Column<string>(type: "text", nullable: true),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    IsMature = table.Column<bool>(type: "boolean", nullable: true),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: true),
                    ViewCount = table.Column<int>(type: "integer", nullable: false),
                    ExternalResource = table.Column<int>(type: "integer", nullable: false),
                    Hide = table.Column<int>(type: "integer", nullable: false),
                    SharePostId = table.Column<Guid>(type: "uuid", nullable: true),
                    SharePostType = table.Column<int>(type: "integer", nullable: true)
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
                name: "ComicReportDetails",
                schema: "comic",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ReportId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ReasonType = table.Column<int>(type: "integer", nullable: false),
                    ReasonText = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComicReportDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComicReportDetails_ComicReports_ReportId",
                        column: x => x.ReportId,
                        principalSchema: "comic",
                        principalTable: "ComicReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComicReportDetails_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentPosts",
                schema: "document",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    IsAllowDownload = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Permission = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    HashId = table.Column<string>(type: "character varying(33)", maxLength: 33, nullable: false),
                    ThumbnailUrl = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    CoverUrl = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    AuthorName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    StatusReason = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    ExternalCode = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    Body = table.Column<string>(type: "text", nullable: true),
                    ShortBody = table.Column<string>(type: "text", nullable: true),
                    CustomNote = table.Column<string>(type: "text", nullable: true),
                    ShortCustomNote = table.Column<string>(type: "text", nullable: true),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    IsMature = table.Column<bool>(type: "boolean", nullable: true),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: true),
                    ViewCount = table.Column<int>(type: "integer", nullable: false),
                    ExternalResource = table.Column<int>(type: "integer", nullable: false),
                    Hide = table.Column<int>(type: "integer", nullable: false),
                    SharePostId = table.Column<Guid>(type: "uuid", nullable: true),
                    SharePostType = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentPosts_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentReportDetails",
                schema: "document",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ReportId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ReasonType = table.Column<int>(type: "integer", nullable: false),
                    ReasonText = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentReportDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentReportDetails_DocumentReports_ReportId",
                        column: x => x.ReportId,
                        principalSchema: "document",
                        principalTable: "DocumentReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentReportDetails_Users_UserId",
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
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
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
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
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
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
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
                    IsLongText = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Permission = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    HashId = table.Column<string>(type: "character varying(33)", maxLength: 33, nullable: false),
                    ThumbnailUrl = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    CoverUrl = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    AuthorName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    StatusReason = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    ExternalCode = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    Body = table.Column<string>(type: "text", nullable: true),
                    ShortBody = table.Column<string>(type: "text", nullable: true),
                    CustomNote = table.Column<string>(type: "text", nullable: true),
                    ShortCustomNote = table.Column<string>(type: "text", nullable: true),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    IsMature = table.Column<bool>(type: "boolean", nullable: true),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: true),
                    ViewCount = table.Column<int>(type: "integer", nullable: false),
                    ExternalResource = table.Column<int>(type: "integer", nullable: false),
                    Hide = table.Column<int>(type: "integer", nullable: false),
                    SharePostId = table.Column<Guid>(type: "uuid", nullable: true),
                    SharePostType = table.Column<int>(type: "integer", nullable: true)
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
                name: "SocialReportDetails",
                schema: "social",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ReportId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ReasonType = table.Column<int>(type: "integer", nullable: false),
                    ReasonText = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocialReportDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SocialReportDetails_SocialReports_ReportId",
                        column: x => x.ReportId,
                        principalSchema: "social",
                        principalTable: "SocialReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SocialReportDetails_Users_UserId",
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
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Permission = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    HashId = table.Column<string>(type: "character varying(33)", maxLength: 33, nullable: false),
                    ThumbnailUrl = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    CoverUrl = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    AuthorName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    StatusReason = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    ExternalCode = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    Body = table.Column<string>(type: "text", nullable: true),
                    ShortBody = table.Column<string>(type: "text", nullable: true),
                    CustomNote = table.Column<string>(type: "text", nullable: true),
                    ShortCustomNote = table.Column<string>(type: "text", nullable: true),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    IsMature = table.Column<bool>(type: "boolean", nullable: true),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: true),
                    ViewCount = table.Column<int>(type: "integer", nullable: false),
                    ExternalResource = table.Column<int>(type: "integer", nullable: false),
                    Hide = table.Column<int>(type: "integer", nullable: false),
                    SharePostId = table.Column<Guid>(type: "uuid", nullable: true),
                    SharePostType = table.Column<int>(type: "integer", nullable: true)
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
                name: "StoryReportDetails",
                schema: "story",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ReportId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ReasonType = table.Column<int>(type: "integer", nullable: false),
                    ReasonText = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryReportDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryReportDetails_StoryReports_ReportId",
                        column: x => x.ReportId,
                        principalSchema: "story",
                        principalTable: "StoryReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoryReportDetails_Users_UserId",
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
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
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
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
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
                name: "UserBlocks",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    UserId1 = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId2 = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Reason = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    StartDate = table.Column<DateTime>(type: "timestamp", nullable: true),
                    EndDate = table.Column<DateTime>(type: "timestamp", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBlocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserBlocks_Users_UserId1",
                        column: x => x.UserId1,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserBlocks_Users_UserId2",
                        column: x => x.UserId2,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
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
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
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
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
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
                name: "OpenIdTokens",
                schema: "openid",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ApplicationId = table.Column<string>(type: "text", nullable: true),
                    AuthorizationId = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyToken = table.Column<string>(type: "text", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExpirationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Payload = table.Column<string>(type: "text", nullable: true),
                    Properties = table.Column<string>(type: "text", nullable: true),
                    RedemptionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReferenceId = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    Subject = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpenIdTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OpenIdTokens_OpenIdApplications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalSchema: "openid",
                        principalTable: "OpenIdApplications",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OpenIdTokens_OpenIdAuthorizations_AuthorizationId",
                        column: x => x.AuthorizationId,
                        principalSchema: "openid",
                        principalTable: "OpenIdAuthorizations",
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
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
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
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
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
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
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
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
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
                name: "ComicPostShares",
                schema: "comic",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    PostId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComicPostShares", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComicPostShares_ComicPosts_PostId",
                        column: x => x.PostId,
                        principalSchema: "comic",
                        principalTable: "ComicPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComicPostShares_Users_UserId",
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
                    IsPremium = table.Column<bool>(type: "boolean", nullable: false),
                    IsPublishChapterSent = table.Column<bool>(type: "boolean", nullable: true),
                    Sort = table.Column<float>(type: "real", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Permission = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    HashId = table.Column<string>(type: "character varying(33)", maxLength: 33, nullable: true),
                    ThumbnailUrl = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
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
                    ExternalResource = table.Column<int>(type: "integer", nullable: false),
                    Order = table.Column<float>(type: "real", nullable: false)
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
                name: "DocumentPostFavorites",
                schema: "document",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    PostId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentPostFavorites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentPostFavorites_DocumentPosts_PostId",
                        column: x => x.PostId,
                        principalSchema: "document",
                        principalTable: "DocumentPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentPostFavorites_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentPostHides",
                schema: "document",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    PostId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentPostHides", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentPostHides_DocumentPosts_PostId",
                        column: x => x.PostId,
                        principalSchema: "document",
                        principalTable: "DocumentPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentPostHides_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentPostLinks",
                schema: "document",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    HashId = table.Column<string>(type: "character varying(33)", maxLength: 33, nullable: true),
                    Url = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Description = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    PostId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentPostLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentPostLinks_DocumentPosts_PostId",
                        column: x => x.PostId,
                        principalSchema: "document",
                        principalTable: "DocumentPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentPostReactions",
                schema: "document",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    TargetId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentPostReactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentPostReactions_DocumentPosts_TargetId",
                        column: x => x.TargetId,
                        principalSchema: "document",
                        principalTable: "DocumentPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentPostReactions_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentPostShares",
                schema: "document",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    PostId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentPostShares", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentPostShares_DocumentPosts_PostId",
                        column: x => x.PostId,
                        principalSchema: "document",
                        principalTable: "DocumentPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentPostShares_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentSubPosts",
                schema: "document",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    IsPremium = table.Column<bool>(type: "boolean", nullable: false),
                    IsPublishChapterSent = table.Column<bool>(type: "boolean", nullable: true),
                    Sort = table.Column<float>(type: "real", nullable: false),
                    IsAllowDownload = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Permission = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    HashId = table.Column<string>(type: "character varying(33)", maxLength: 33, nullable: true),
                    ThumbnailUrl = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
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
                    ExternalResource = table.Column<int>(type: "integer", nullable: false),
                    Order = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentSubPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentSubPosts_DocumentPosts_PostId",
                        column: x => x.PostId,
                        principalSchema: "document",
                        principalTable: "DocumentPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentSubPosts_Users_UserId",
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
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
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
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
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
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
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
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
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
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
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
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
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
                name: "SocialPostShares",
                schema: "social",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    PostId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocialPostShares", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SocialPostShares_SocialPosts_PostId",
                        column: x => x.PostId,
                        principalSchema: "social",
                        principalTable: "SocialPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SocialPostShares_Users_UserId",
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
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Permission = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    HashId = table.Column<string>(type: "character varying(33)", maxLength: 33, nullable: true),
                    ThumbnailUrl = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
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
                    ExternalResource = table.Column<int>(type: "integer", nullable: false),
                    Order = table.Column<float>(type: "real", nullable: false)
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
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
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
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
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
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
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
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
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
                name: "StoryPostShares",
                schema: "story",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    PostId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryPostShares", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryPostShares_StoryPosts_PostId",
                        column: x => x.PostId,
                        principalSchema: "story",
                        principalTable: "StoryPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoryPostShares_Users_UserId",
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
                    IsPremium = table.Column<bool>(type: "boolean", nullable: false),
                    IsPublishChapterSent = table.Column<bool>(type: "boolean", nullable: true),
                    Sort = table.Column<float>(type: "real", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Permission = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    HashId = table.Column<string>(type: "character varying(33)", maxLength: 33, nullable: true),
                    ThumbnailUrl = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
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
                    ExternalResource = table.Column<int>(type: "integer", nullable: false),
                    Order = table.Column<float>(type: "real", nullable: false)
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
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
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
                name: "DocumentTagPosts",
                schema: "document",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    TagId = table.Column<Guid>(type: "uuid", nullable: false),
                    PostId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentTagPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentTagPosts_DocumentPosts_PostId",
                        column: x => x.PostId,
                        principalSchema: "document",
                        principalTable: "DocumentPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentTagPosts_Tags_TagId",
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
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
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
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
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
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
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
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
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
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
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
                name: "DocumentResources",
                schema: "document",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
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
                    table.PrimaryKey("PK_DocumentResources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentResources_DocumentSubPosts_SubPostId",
                        column: x => x.SubPostId,
                        principalSchema: "document",
                        principalTable: "DocumentSubPosts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DocumentResources_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DocumentSubPostReactions",
                schema: "document",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    TargetId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentSubPostReactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentSubPostReactions_DocumentSubPosts_TargetId",
                        column: x => x.TargetId,
                        principalSchema: "document",
                        principalTable: "DocumentSubPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentSubPostReactions_Users_AuthorId",
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
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
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
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
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
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
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
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
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
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
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
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
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
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
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
                name: "DocumentPostComments",
                schema: "document",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
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
                    table.PrimaryKey("PK_DocumentPostComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentPostComments_DocumentPosts_PostId",
                        column: x => x.PostId,
                        principalSchema: "document",
                        principalTable: "DocumentPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentPostComments_DocumentResources_ResourceId",
                        column: x => x.ResourceId,
                        principalSchema: "document",
                        principalTable: "DocumentResources",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DocumentPostComments_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentSubPostComments",
                schema: "document",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
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
                    table.PrimaryKey("PK_DocumentSubPostComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentSubPostComments_DocumentResources_ResourceId",
                        column: x => x.ResourceId,
                        principalSchema: "document",
                        principalTable: "DocumentResources",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DocumentSubPostComments_DocumentSubPosts_PostId",
                        column: x => x.PostId,
                        principalSchema: "document",
                        principalTable: "DocumentSubPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentSubPostComments_Users_AuthorId",
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
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
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
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
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
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
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
                    ParagraphId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
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
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
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
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
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
                name: "DocumentPostCommentReactions",
                schema: "document",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    TargetId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentPostCommentReactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentPostCommentReactions_DocumentPostComments_TargetId",
                        column: x => x.TargetId,
                        principalSchema: "document",
                        principalTable: "DocumentPostComments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentPostCommentReactions_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentSubPostCommentReactions",
                schema: "document",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    TargetId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentSubPostCommentReactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentSubPostCommentReactions_DocumentSubPostComments_Tar~",
                        column: x => x.TargetId,
                        principalSchema: "document",
                        principalTable: "DocumentSubPostComments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentSubPostCommentReactions_Users_AuthorId",
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
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
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
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
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
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
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
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
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

            migrationBuilder.InsertData(
                schema: "identity",
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "DisplayName", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("35e7cb92-d601-4d51-83f7-d327242c8f7e"), null, "Content Admin", "ContentAdmin", "CONTENTADMIN" },
                    { new Guid("53a787ef-f614-4425-95ed-c1905a917b85"), null, "User", "User", "USER" },
                    { new Guid("ae2fac1e-dbbd-4ed9-b4c9-160375bf28d5"), null, "System Admin", "SystemAdmin", "SYSTEMADMIN" },
                    { new Guid("b0632b0e-8ebd-4303-8ec6-e100ba4204e4"), null, "Admin", "Admin", "ADMIN" }
                });

            migrationBuilder.InsertData(
                schema: "identity",
                table: "UserNameHistories",
                columns: new[] { "Id", "CreatedBy", "CreatedOn", "IsDelete", "ModifiedBy", "ModifiedOn", "SyncError", "SyncedOn", "TagData", "UserId", "UserName" },
                values: new object[,]
                {
                    { new Guid("07efe4ce-85cb-4949-a2f0-94bad91383b5"), null, new DateTime(2024, 8, 8, 18, 45, 4, 0, DateTimeKind.Unspecified), false, null, null, null, null, null, new Guid("ff5727ac-4b12-4e03-9e02-25bfb9086cbf"), "systemadmin" },
                    { new Guid("c17430aa-37b4-47fc-a461-7faea85cf6ca"), null, new DateTime(2024, 8, 8, 18, 45, 4, 0, DateTimeKind.Unspecified), false, null, null, null, null, null, new Guid("00000000-0000-0000-0000-000000000001"), "system" },
                    { new Guid("c80574fd-9a67-43ca-8d6f-83d4d2b67701"), null, new DateTime(2024, 8, 8, 18, 45, 4, 0, DateTimeKind.Unspecified), false, null, null, null, null, null, new Guid("ff09e6eb-8ff5-4073-b8f7-a1bc7dc8d4cb"), "admin" },
                    { new Guid("f087ae65-c26d-42ed-8698-f6d2758097f2"), null, new DateTime(2024, 8, 8, 18, 45, 4, 0, DateTimeKind.Unspecified), false, null, null, null, null, null, new Guid("1a07b416-f417-475b-9cef-f50ee8591a91"), "contentadmin" }
                });

            migrationBuilder.InsertData(
                schema: "identity",
                table: "Users",
                columns: new[] { "Id", "AccessFailedCount", "ActivedDate", "Avatar", "ConcurrencyStamp", "CoverPhoto", "CreatedBy", "CreatedIp", "CreatedOn", "DateOfBirth", "DeletedAt", "DeletedBy", "Email", "EmailConfirmed", "FirstName", "Gender", "IsActiveEarning", "IsDelete", "IsExpiredSubscriptionSent", "IsWalletShowing", "Language", "LastLoginDate", "LastLoginIp", "LastName", "Location", "LockoutEnabled", "LockoutEnd", "MinioInstance", "ModifiedBy", "ModifiedOn", "NextCheckPremium", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "PremiumDate", "ProfileId", "ProfileName", "ReferralCode", "RefreshToken", "RefreshTokenExpiryTime", "SecurityStamp", "Status", "StatusReason", "StorageLimit", "SyncError", "SyncedOn", "TagData", "TwoFactorEnabled", "Type", "UserName" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000001"), 0, null, null, "a625d884-4387-48cf-b584-3a9b1b228832", null, null, null, new DateTime(2024, 8, 8, 18, 45, 4, 0, DateTimeKind.Unspecified), null, null, null, "system@focfoc.com", false, null, null, false, false, null, false, null, null, null, null, null, false, null, 0, null, null, null, "SYSTEM@FOCFOC.COM", "SYSTEM", "AQAAAAIAAYagAAAAEIvW0O+erQdOT5gTf8NMInqD6siJ/HJy+3cYsZ0V6NmQFcm4HWvysYxajMisoF6M5A==", null, false, null, "system", "system", null, null, null, "64be2bdf-b79b-4c0a-9dec-419cda1b67d5", 1, null, 0, null, null, null, false, 3, "system" },
                    { new Guid("1a07b416-f417-475b-9cef-f50ee8591a91"), 0, null, null, "a625d884-4387-48cf-b584-3a9b1b228832", null, null, null, new DateTime(2024, 8, 8, 18, 45, 4, 0, DateTimeKind.Unspecified), null, null, null, "contentadmin@focfoc.com", false, null, null, false, false, null, false, null, null, null, null, null, false, null, 0, null, null, null, "CONTENTADMIN@FOCFOC.COM", "CONTENTADMIN", "AQAAAAIAAYagAAAAEIvW0O+erQdOT5gTf8NMInqD6siJ/HJy+3cYsZ0V6NmQFcm4HWvysYxajMisoF6M5A==", null, false, null, "contentadmin", "contentadmin", null, null, null, "64be2bdf-b79b-4c0a-9dec-419cda1b67d5", 1, null, 0, null, null, null, false, 3, "contentadmin" },
                    { new Guid("ff09e6eb-8ff5-4073-b8f7-a1bc7dc8d4cb"), 0, null, null, "a625d884-4387-48cf-b584-3a9b1b228832", null, null, null, new DateTime(2024, 8, 8, 18, 45, 4, 0, DateTimeKind.Unspecified), null, null, null, "admin@focfoc.com", false, null, null, false, false, null, false, null, null, null, null, null, false, null, 0, null, null, null, "ADMIN@FOCFOC.COM", "ADMIN", "AQAAAAIAAYagAAAAEIvW0O+erQdOT5gTf8NMInqD6siJ/HJy+3cYsZ0V6NmQFcm4HWvysYxajMisoF6M5A==", null, false, null, "admin", "admin", null, null, null, "64be2bdf-b79b-4c0a-9dec-419cda1b67d5", 1, null, 0, null, null, null, false, 4, "admin" },
                    { new Guid("ff5727ac-4b12-4e03-9e02-25bfb9086cbf"), 0, null, null, "a625d884-4387-48cf-b584-3a9b1b228832", null, null, null, new DateTime(2024, 8, 8, 18, 45, 4, 0, DateTimeKind.Unspecified), null, null, null, "systemadmin@focfoc.com", false, null, null, false, false, null, false, null, null, null, null, null, false, null, 0, null, null, null, "SYSTEMADMIN@FOCFOC.COM", "SYSTEMADMIN", "AQAAAAIAAYagAAAAEIvW0O+erQdOT5gTf8NMInqD6siJ/HJy+3cYsZ0V6NmQFcm4HWvysYxajMisoF6M5A==", null, false, null, "systemadmin", "systemadmin", null, null, null, "64be2bdf-b79b-4c0a-9dec-419cda1b67d5", 1, null, 0, null, null, null, false, 5, "systemadmin" }
                });

            migrationBuilder.InsertData(
                schema: "identity",
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { new Guid("35e7cb92-d601-4d51-83f7-d327242c8f7e"), new Guid("1a07b416-f417-475b-9cef-f50ee8591a91") },
                    { new Guid("b0632b0e-8ebd-4303-8ec6-e100ba4204e4"), new Guid("ff09e6eb-8ff5-4073-b8f7-a1bc7dc8d4cb") },
                    { new Guid("ae2fac1e-dbbd-4ed9-b4c9-160375bf28d5"), new Guid("ff5727ac-4b12-4e03-9e02-25bfb9086cbf") }
                });

            migrationBuilder.CreateTable(
                name: "GamePosts",
                schema: "game",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    GameUrl = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Permission = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    HashId = table.Column<string>(type: "character varying(33)", maxLength: 33, nullable: false),
                    ThumbnailUrl = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    CoverUrl = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    AuthorName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    StatusReason = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    ExternalCode = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    Body = table.Column<string>(type: "text", nullable: true),
                    ShortBody = table.Column<string>(type: "text", nullable: true),
                    CustomNote = table.Column<string>(type: "text", nullable: true),
                    ShortCustomNote = table.Column<string>(type: "text", nullable: true),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    IsMature = table.Column<bool>(type: "boolean", nullable: true),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: true),
                    ViewCount = table.Column<int>(type: "integer", nullable: false),
                    ExternalResource = table.Column<int>(type: "integer", nullable: false),
                    Hide = table.Column<int>(type: "integer", nullable: false),
                    SharePostId = table.Column<Guid>(type: "uuid", nullable: true),
                    SharePostType = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GamePosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GamePosts_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GamePostComments",
                schema: "game",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
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
                    table.PrimaryKey("PK_GamePostComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GamePostComments_GamePosts_PostId",
                        column: x => x.PostId,
                        principalSchema: "game",
                        principalTable: "GamePosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GamePostComments_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GamePostReactions",
                schema: "game",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    TargetId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GamePostReactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GamePostReactions_GamePosts_TargetId",
                        column: x => x.TargetId,
                        principalSchema: "game",
                        principalTable: "GamePosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GamePostReactions_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GamePostCommentReactions",
                schema: "game",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    TargetId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GamePostCommentReactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GamePostCommentReactions_GamePostComments_TargetId",
                        column: x => x.TargetId,
                        principalSchema: "game",
                        principalTable: "GamePostComments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GamePostCommentReactions_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GamePostCommentReactions_AuthorId",
                schema: "game",
                table: "GamePostCommentReactions",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_GamePostCommentReactions_TargetId_ParentId_AuthorId",
                schema: "game",
                table: "GamePostCommentReactions",
                columns: new[] { "TargetId", "ParentId", "AuthorId" });

            migrationBuilder.CreateIndex(
                name: "IX_GamePostComments_AuthorId",
                schema: "game",
                table: "GamePostComments",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_GamePostComments_PostId_ParentId_AuthorId",
                schema: "game",
                table: "GamePostComments",
                columns: new[] { "PostId", "ParentId", "AuthorId" });

            migrationBuilder.CreateIndex(
                name: "IX_GamePostReactions_AuthorId",
                schema: "game",
                table: "GamePostReactions",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_GamePostReactions_TargetId_ParentId_AuthorId",
                schema: "game",
                table: "GamePostReactions",
                columns: new[] { "TargetId", "ParentId", "AuthorId" });

            migrationBuilder.CreateTable(
                name: "GameResources",
                schema: "game",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    PostId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedOn = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SyncError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TagData = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
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
                    table.PrimaryKey("PK_GameResources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameResources_GamePosts_PostId",
                        column: x => x.PostId,
                        principalSchema: "game",
                        principalTable: "GamePosts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GameResources_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameResources_AuthorId",
                schema: "game",
                table: "GameResources",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_GameResources_HashId",
                schema: "game",
                table: "GameResources",
                column: "HashId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GameResources_Name",
                schema: "game",
                table: "GameResources",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GameResources_PostId",
                schema: "game",
                table: "GameResources",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_GamePosts_HashId_UserId_Type_Id",
                schema: "game",
                table: "GamePosts",
                columns: new[] { "HashId", "UserId", "Type", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GamePosts_UserId",
                schema: "game",
                table: "GamePosts",
                column: "UserId");


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
                name: "IX_ComicPostShares_PostId",
                schema: "comic",
                table: "ComicPostShares",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_ComicPostShares_UserId",
                schema: "comic",
                table: "ComicPostShares",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ComicReportDetails_ReportId",
                schema: "comic",
                table: "ComicReportDetails",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "IX_ComicReportDetails_UserId",
                schema: "comic",
                table: "ComicReportDetails",
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
                name: "IX_DocumentPostCommentReactions_AuthorId",
                schema: "document",
                table: "DocumentPostCommentReactions",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentPostCommentReactions_TargetId_ParentId_AuthorId",
                schema: "document",
                table: "DocumentPostCommentReactions",
                columns: new[] { "TargetId", "ParentId", "AuthorId" });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentPostComments_AuthorId",
                schema: "document",
                table: "DocumentPostComments",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentPostComments_PostId_ParentId_AuthorId",
                schema: "document",
                table: "DocumentPostComments",
                columns: new[] { "PostId", "ParentId", "AuthorId" });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentPostComments_ResourceId",
                schema: "document",
                table: "DocumentPostComments",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentPostFavorites_PostId",
                schema: "document",
                table: "DocumentPostFavorites",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentPostFavorites_UserId",
                schema: "document",
                table: "DocumentPostFavorites",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentPostHides_PostId",
                schema: "document",
                table: "DocumentPostHides",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentPostHides_UserId",
                schema: "document",
                table: "DocumentPostHides",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentPostLinks_PostId",
                schema: "document",
                table: "DocumentPostLinks",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentPostReactions_AuthorId",
                schema: "document",
                table: "DocumentPostReactions",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentPostReactions_TargetId_ParentId_AuthorId",
                schema: "document",
                table: "DocumentPostReactions",
                columns: new[] { "TargetId", "ParentId", "AuthorId" });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentPosts_HashId_UserId_Type_Id",
                schema: "document",
                table: "DocumentPosts",
                columns: new[] { "HashId", "UserId", "Type", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentPosts_UserId",
                schema: "document",
                table: "DocumentPosts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentPostShares_PostId",
                schema: "document",
                table: "DocumentPostShares",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentPostShares_UserId",
                schema: "document",
                table: "DocumentPostShares",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentReportDetails_ReportId",
                schema: "document",
                table: "DocumentReportDetails",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentReportDetails_UserId",
                schema: "document",
                table: "DocumentReportDetails",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentResources_AuthorId",
                schema: "document",
                table: "DocumentResources",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentResources_HashId",
                schema: "document",
                table: "DocumentResources",
                column: "HashId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentResources_Name",
                schema: "document",
                table: "DocumentResources",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentResources_SubPostId",
                schema: "document",
                table: "DocumentResources",
                column: "SubPostId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentSubPostCommentReactions_AuthorId",
                schema: "document",
                table: "DocumentSubPostCommentReactions",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentSubPostCommentReactions_TargetId_ParentId_AuthorId",
                schema: "document",
                table: "DocumentSubPostCommentReactions",
                columns: new[] { "TargetId", "ParentId", "AuthorId" });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentSubPostComments_AuthorId",
                schema: "document",
                table: "DocumentSubPostComments",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentSubPostComments_PostId_ParentId_AuthorId",
                schema: "document",
                table: "DocumentSubPostComments",
                columns: new[] { "PostId", "ParentId", "AuthorId" });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentSubPostComments_ResourceId",
                schema: "document",
                table: "DocumentSubPostComments",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentSubPostReactions_AuthorId",
                schema: "document",
                table: "DocumentSubPostReactions",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentSubPostReactions_TargetId_ParentId_AuthorId",
                schema: "document",
                table: "DocumentSubPostReactions",
                columns: new[] { "TargetId", "ParentId", "AuthorId" });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentSubPosts_PostId_HashId_AuthorId",
                schema: "document",
                table: "DocumentSubPosts",
                columns: new[] { "PostId", "HashId", "AuthorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentSubPosts_UserId",
                schema: "document",
                table: "DocumentSubPosts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTagPosts_PostId",
                schema: "document",
                table: "DocumentTagPosts",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTagPosts_TagId_PostId",
                schema: "document",
                table: "DocumentTagPosts",
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
                name: "IX_OpenIdAuthorizations_ApplicationId",
                schema: "openid",
                table: "OpenIdAuthorizations",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_OpenIdTokens_ApplicationId",
                schema: "openid",
                table: "OpenIdTokens",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_OpenIdTokens_AuthorizationId",
                schema: "openid",
                table: "OpenIdTokens",
                column: "AuthorizationId");

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
                name: "IX_SocialPostShares_PostId",
                schema: "social",
                table: "SocialPostShares",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_SocialPostShares_UserId",
                schema: "social",
                table: "SocialPostShares",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SocialReportDetails_ReportId",
                schema: "social",
                table: "SocialReportDetails",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "IX_SocialReportDetails_UserId",
                schema: "social",
                table: "SocialReportDetails",
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
                name: "IX_StoryPostShares_PostId",
                schema: "story",
                table: "StoryPostShares",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryPostShares_UserId",
                schema: "story",
                table: "StoryPostShares",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryReportDetails_ReportId",
                schema: "story",
                table: "StoryReportDetails",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryReportDetails_UserId",
                schema: "story",
                table: "StoryReportDetails",
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
                name: "IX_SystemConfigs_Key",
                schema: "system",
                table: "SystemConfigs",
                column: "Key",
                unique: true);

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
                name: "IX_UserBlocks_UserId1",
                schema: "identity",
                table: "UserBlocks",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_UserBlocks_UserId2",
                schema: "identity",
                table: "UserBlocks",
                column: "UserId2");

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
                name: "GameResources",
                schema: "game");

            migrationBuilder.DropTable(
                name: "GamePostCommentReactions",
                schema: "game");

            migrationBuilder.DropTable(
                name: "GamePostReactions",
                schema: "game");

            migrationBuilder.DropTable(
                name: "GamePostComments",
                schema: "game");

            migrationBuilder.DropTable(
                name: "GamePosts",
                schema: "game");

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
                name: "ComicPostShares",
                schema: "comic");

            migrationBuilder.DropTable(
                name: "ComicReportDetails",
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
                name: "DocumentMetaDatas",
                schema: "document");

            migrationBuilder.DropTable(
                name: "DocumentPostCommentReactions",
                schema: "document");

            migrationBuilder.DropTable(
                name: "DocumentPostFavorites",
                schema: "document");

            migrationBuilder.DropTable(
                name: "DocumentPostHides",
                schema: "document");

            migrationBuilder.DropTable(
                name: "DocumentPostLinks",
                schema: "document");

            migrationBuilder.DropTable(
                name: "DocumentPostReactions",
                schema: "document");

            migrationBuilder.DropTable(
                name: "DocumentPostShares",
                schema: "document");

            migrationBuilder.DropTable(
                name: "DocumentReportDetails",
                schema: "document");

            migrationBuilder.DropTable(
                name: "DocumentSubPostCommentReactions",
                schema: "document");

            migrationBuilder.DropTable(
                name: "DocumentSubPostReactions",
                schema: "document");

            migrationBuilder.DropTable(
                name: "DocumentTagPosts",
                schema: "document");

            migrationBuilder.DropTable(
                name: "Jobs",
                schema: "system");

            migrationBuilder.DropTable(
                name: "Mentions");

            migrationBuilder.DropTable(
                name: "Notifications",
                schema: "system");

            migrationBuilder.DropTable(
                name: "OpenIdScopes",
                schema: "openid");

            migrationBuilder.DropTable(
                name: "OpenIdTokens",
                schema: "openid");

            migrationBuilder.DropTable(
                name: "Ratings",
                schema: "system");

            migrationBuilder.DropTable(
                name: "RoleClaims",
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
                name: "SocialPostShares",
                schema: "social");

            migrationBuilder.DropTable(
                name: "SocialReportDetails",
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
                name: "StoryPostShares",
                schema: "story");

            migrationBuilder.DropTable(
                name: "StoryReportDetails",
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
                name: "SystemConfigs",
                schema: "system");

            migrationBuilder.DropTable(
                name: "SystemResources",
                schema: "system");

            migrationBuilder.DropTable(
                name: "SystemSettingHistories",
                schema: "system");

            migrationBuilder.DropTable(
                name: "TagFavorites");

            migrationBuilder.DropTable(
                name: "UserAuthenticators",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "UserBlocks",
                schema: "identity");

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
                name: "UserRecoveries",
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
                name: "ComicReports",
                schema: "comic");

            migrationBuilder.DropTable(
                name: "ComicSubPostComments",
                schema: "comic");

            migrationBuilder.DropTable(
                name: "DocumentPostComments",
                schema: "document");

            migrationBuilder.DropTable(
                name: "DocumentReports",
                schema: "document");

            migrationBuilder.DropTable(
                name: "DocumentSubPostComments",
                schema: "document");

            migrationBuilder.DropTable(
                name: "NotificationObjects",
                schema: "system");

            migrationBuilder.DropTable(
                name: "OpenIdAuthorizations",
                schema: "openid");

            migrationBuilder.DropTable(
                name: "SocialPostComments",
                schema: "social");

            migrationBuilder.DropTable(
                name: "SocialReports",
                schema: "social");

            migrationBuilder.DropTable(
                name: "SocialSubPostComments",
                schema: "social");

            migrationBuilder.DropTable(
                name: "StoryPostComments",
                schema: "story");

            migrationBuilder.DropTable(
                name: "StoryReports",
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
                name: "DocumentResources",
                schema: "document");

            migrationBuilder.DropTable(
                name: "OpenIdApplications",
                schema: "openid");

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
                name: "DocumentSubPosts",
                schema: "document");

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
                name: "DocumentPosts",
                schema: "document");

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
