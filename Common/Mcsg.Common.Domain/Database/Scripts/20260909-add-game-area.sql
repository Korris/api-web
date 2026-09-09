-- =====================================================================
-- Game area: schema "game" + table game."GamePosts"
-- Matches the CreateTable("GamePosts") block merged into 20250327042022_InitData.cs
-- (team convention: no new migrations, InitData + snapshot edited by hand).
-- Idempotent: safe to re-run on dev / stg / prod.
-- =====================================================================
BEGIN;

CREATE SCHEMA IF NOT EXISTS game;

CREATE TABLE IF NOT EXISTS game."GamePosts" (
    "Id" uuid NOT NULL DEFAULT (gen_random_uuid()),
    "GameUrl" text,
    "CreatedBy" uuid,
    "CreatedOn" timestamp NOT NULL,
    "ModifiedBy" uuid,
    "ModifiedOn" timestamp,
    "IsDelete" boolean NOT NULL,
    "SyncedOn" timestamp,
    "SyncError" character varying(1024),
    "TagData" character varying(64),
    "Permission" integer NOT NULL,
    "Title" character varying(255),
    "HashId" character varying(33) NOT NULL,
    "ThumbnailUrl" character varying(256),
    "CoverUrl" character varying(256),
    "AuthorName" character varying(512),
    "StatusReason" character varying(1024),
    "ExternalCode" character varying(32),
    "Body" text,
    "ShortBody" text,
    "CustomNote" text,
    "ShortCustomNote" text,
    "AuthorId" uuid,
    "UserId" uuid NOT NULL,
    "Type" integer NOT NULL,
    "Status" integer NOT NULL,
    "IsMature" boolean,
    "IsCompleted" boolean,
    "ViewCount" integer NOT NULL,
    "ExternalResource" integer NOT NULL,
    "Hide" integer NOT NULL,
    "SharePostId" uuid,
    "SharePostType" integer,
    CONSTRAINT "PK_GamePosts" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_GamePosts_Users_UserId" FOREIGN KEY ("UserId") REFERENCES identity."Users" ("Id") ON DELETE CASCADE
);

CREATE UNIQUE INDEX IF NOT EXISTS "IX_GamePosts_HashId_UserId_Type_Id" ON game."GamePosts" ("HashId", "UserId", "Type", "Id");
CREATE INDEX IF NOT EXISTS "IX_GamePosts_UserId" ON game."GamePosts" ("UserId");

COMMIT;

-- =====================================================================
-- Game comments + reactions (post-level only). Idempotent, safe to re-run.
-- =====================================================================
BEGIN;

CREATE TABLE IF NOT EXISTS game."GamePostComments" (
    "Id" uuid NOT NULL DEFAULT (gen_random_uuid()),
    "CreatedBy" uuid,
    "CreatedOn" timestamp NOT NULL,
    "ModifiedBy" uuid,
    "ModifiedOn" timestamp,
    "IsDelete" boolean NOT NULL,
    "SyncedOn" timestamp,
    "SyncError" character varying(1024),
    "TagData" character varying(64),
    "Body" text,
    "CustomNote" text,
    "ParentId" uuid,
    "PostId" uuid NOT NULL,
    "AuthorId" uuid NOT NULL,
    "Order" integer NOT NULL,
    "Status" integer NOT NULL,
    "ResourceId" uuid,
    "GifId" text,
    "QuoteId" uuid,
    CONSTRAINT "PK_GamePostComments" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_GamePostComments_GamePosts_PostId" FOREIGN KEY ("PostId") REFERENCES game."GamePosts" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_GamePostComments_Users_AuthorId" FOREIGN KEY ("AuthorId") REFERENCES identity."Users" ("Id") ON DELETE CASCADE
);
CREATE INDEX IF NOT EXISTS "IX_GamePostComments_AuthorId" ON game."GamePostComments" ("AuthorId");
CREATE INDEX IF NOT EXISTS "IX_GamePostComments_PostId_ParentId_AuthorId" ON game."GamePostComments" ("PostId", "ParentId", "AuthorId");

CREATE TABLE IF NOT EXISTS game."GamePostReactions" (
    "Id" uuid NOT NULL DEFAULT (gen_random_uuid()),
    "CreatedBy" uuid,
    "CreatedOn" timestamp NOT NULL,
    "ModifiedBy" uuid,
    "ModifiedOn" timestamp,
    "IsDelete" boolean NOT NULL,
    "SyncedOn" timestamp,
    "SyncError" character varying(1024),
    "TagData" character varying(64),
    "ParentId" uuid,
    "TargetId" uuid NOT NULL,
    "AuthorId" uuid NOT NULL,
    "Type" integer NOT NULL,
    CONSTRAINT "PK_GamePostReactions" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_GamePostReactions_GamePosts_TargetId" FOREIGN KEY ("TargetId") REFERENCES game."GamePosts" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_GamePostReactions_Users_AuthorId" FOREIGN KEY ("AuthorId") REFERENCES identity."Users" ("Id") ON DELETE CASCADE
);
CREATE INDEX IF NOT EXISTS "IX_GamePostReactions_AuthorId" ON game."GamePostReactions" ("AuthorId");
CREATE INDEX IF NOT EXISTS "IX_GamePostReactions_TargetId_ParentId_AuthorId" ON game."GamePostReactions" ("TargetId", "ParentId", "AuthorId");

CREATE TABLE IF NOT EXISTS game."GamePostCommentReactions" (
    "Id" uuid NOT NULL DEFAULT (gen_random_uuid()),
    "CreatedBy" uuid,
    "CreatedOn" timestamp NOT NULL,
    "ModifiedBy" uuid,
    "ModifiedOn" timestamp,
    "IsDelete" boolean NOT NULL,
    "SyncedOn" timestamp,
    "SyncError" character varying(1024),
    "TagData" character varying(64),
    "ParentId" uuid,
    "TargetId" uuid NOT NULL,
    "AuthorId" uuid NOT NULL,
    "Type" integer NOT NULL,
    CONSTRAINT "PK_GamePostCommentReactions" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_GamePostCommentReactions_GamePostComments_TargetId" FOREIGN KEY ("TargetId") REFERENCES game."GamePostComments" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_GamePostCommentReactions_Users_AuthorId" FOREIGN KEY ("AuthorId") REFERENCES identity."Users" ("Id") ON DELETE CASCADE
);
CREATE INDEX IF NOT EXISTS "IX_GamePostCommentReactions_AuthorId" ON game."GamePostCommentReactions" ("AuthorId");
CREATE INDEX IF NOT EXISTS "IX_GamePostCommentReactions_TargetId_ParentId_AuthorId" ON game."GamePostCommentReactions" ("TargetId", "ParentId", "AuthorId");

COMMIT;

-- =====================================================================
-- Game resources (uploaded thumbnail / html files, copy of comic."ComicResources"). Idempotent.
-- =====================================================================
BEGIN;

CREATE TABLE IF NOT EXISTS game."GameResources" (
    "Id" uuid NOT NULL DEFAULT (gen_random_uuid()),
    "PostId" uuid,
    "CreatedBy" uuid,
    "CreatedOn" timestamp NOT NULL,
    "ModifiedBy" uuid,
    "ModifiedOn" timestamp,
    "IsDelete" boolean NOT NULL,
    "SyncedOn" timestamp,
    "SyncError" character varying(1024),
    "TagData" character varying(64),
    "Title" character varying(255),
    "Name" character varying(512) NOT NULL,
    "HashId" character varying(33) NOT NULL,
    "Url" character varying(256),
    "BucketName" character varying(32),
    "MinioInstance" integer,
    "AuthorId" uuid,
    "SubPostId" uuid,
    "Order" integer NOT NULL,
    "Size" double precision NOT NULL,
    "CompressedSize" double precision NOT NULL,
    "Width" integer NOT NULL,
    "Height" integer NOT NULL,
    "Type" integer NOT NULL,
    "LocationType" integer NOT NULL,
    "Status" integer NOT NULL,
    "ExternalUrl" character varying(256),
    "ExternalResource" integer,
    CONSTRAINT "PK_GameResources" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_GameResources_GamePosts_PostId" FOREIGN KEY ("PostId") REFERENCES game."GamePosts" ("Id"),
    CONSTRAINT "FK_GameResources_Users_AuthorId" FOREIGN KEY ("AuthorId") REFERENCES identity."Users" ("Id")
);
CREATE INDEX IF NOT EXISTS "IX_GameResources_AuthorId" ON game."GameResources" ("AuthorId");
CREATE UNIQUE INDEX IF NOT EXISTS "IX_GameResources_HashId" ON game."GameResources" ("HashId");
CREATE UNIQUE INDEX IF NOT EXISTS "IX_GameResources_Name" ON game."GameResources" ("Name");
CREATE INDEX IF NOT EXISTS "IX_GameResources_PostId" ON game."GameResources" ("PostId");

COMMIT;

-- =====================================================================
-- SystemSettings: max game file size (MB). Read by api/game/file/upload-game.
-- Missing or 0 → API falls back to 5 MB; values above 50 are capped to 50.
-- =====================================================================
INSERT INTO system."SystemSettings"
    ("Id", "Key", "Value", "Description", "IsActive", "Order", "DataType", "Group", "MicroService", "IsDelete", "CreatedOn")
SELECT gen_random_uuid(), 'GameFileSize', '5', 'Max size (MB) of an uploaded HTML game file', true, 0, 'double', 'Upload', 'Game', false, now()
WHERE NOT EXISTS (SELECT 1 FROM system."SystemSettings" WHERE "Key" = 'GameFileSize');
