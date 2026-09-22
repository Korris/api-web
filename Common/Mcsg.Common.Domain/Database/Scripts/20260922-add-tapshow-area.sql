-- =====================================================================
-- TapShow area: schema "tapshow" + 9 tables.
-- Matches the CreateTable blocks merged into 20250327042022_InitData.cs
-- (team convention: no new migrations, InitData + Designer + snapshot edited by hand).
-- Idempotent: safe to re-run on dev / stg / prod.
-- Order: posts → chapters → characters → segments → choices → resources → comments → reactions.
-- =====================================================================
BEGIN;

CREATE SCHEMA IF NOT EXISTS tapshow;

-- Post (BasePost columns, same as game."GamePosts" without GameUrl)
CREATE TABLE IF NOT EXISTS tapshow."TapShowPosts" (
    "Id" uuid NOT NULL DEFAULT (gen_random_uuid()),
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
    CONSTRAINT "PK_TapShowPosts" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_TapShowPosts_Users_UserId" FOREIGN KEY ("UserId") REFERENCES identity."Users" ("Id") ON DELETE CASCADE
);
CREATE UNIQUE INDEX IF NOT EXISTS "IX_TapShowPosts_HashId_UserId_Type_Id" ON tapshow."TapShowPosts" ("HashId", "UserId", "Type", "Id");
CREATE INDEX IF NOT EXISTS "IX_TapShowPosts_UserId" ON tapshow."TapShowPosts" ("UserId");

-- Chapter (BaseSubPost columns, same as comic."ComicSubPosts" without IsPremium / IsPublishChapterSent / Sort)
CREATE TABLE IF NOT EXISTS tapshow."TapShowChapters" (
    "Id" uuid NOT NULL DEFAULT (gen_random_uuid()),
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
    "Name" character varying(512),
    "HashId" character varying(33) NOT NULL,
    "ThumbnailUrl" character varying(256),
    "Body" text,
    "CreatorNote" text,
    "PostId" uuid NOT NULL,
    "AuthorId" uuid,
    "UserId" uuid NOT NULL,
    "Status" integer NOT NULL,
    "PublishDate" timestamp,
    "ViewCount" integer NOT NULL,
    "IsEnableComment" boolean NOT NULL,
    "ExternalCode" text,
    "IsExclusive" boolean NOT NULL,
    "ExternalResource" integer NOT NULL,
    "Order" real NOT NULL,
    CONSTRAINT "PK_TapShowChapters" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_TapShowChapters_TapShowPosts_PostId" FOREIGN KEY ("PostId") REFERENCES tapshow."TapShowPosts" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_TapShowChapters_Users_UserId" FOREIGN KEY ("UserId") REFERENCES identity."Users" ("Id") ON DELETE CASCADE
);
CREATE UNIQUE INDEX IF NOT EXISTS "IX_TapShowChapters_PostId_HashId_AuthorId" ON tapshow."TapShowChapters" ("PostId", "HashId", "AuthorId");
CREATE INDEX IF NOT EXISTS "IX_TapShowChapters_UserId" ON tapshow."TapShowChapters" ("UserId");
CREATE INDEX IF NOT EXISTS "IX_TapShowChapters_HashId" ON tapshow."TapShowChapters" ("HashId");

-- Character (name + avatar) of a post; segments may reference the speaking character
CREATE TABLE IF NOT EXISTS tapshow."TapShowCharacters" (
    "Id" uuid NOT NULL DEFAULT (gen_random_uuid()),
    "Name" character varying(255) NOT NULL,
    "AvatarUrl" character varying(256),
    "Order" integer NOT NULL,
    "PostId" uuid NOT NULL,
    "CreatedBy" uuid,
    "CreatedOn" timestamp NOT NULL,
    "ModifiedBy" uuid,
    "ModifiedOn" timestamp,
    "IsDelete" boolean NOT NULL,
    "SyncedOn" timestamp,
    "SyncError" character varying(1024),
    "TagData" character varying(64),
    CONSTRAINT "PK_TapShowCharacters" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_TapShowCharacters_TapShowPosts_PostId" FOREIGN KEY ("PostId") REFERENCES tapshow."TapShowPosts" ("Id") ON DELETE CASCADE
);
CREATE INDEX IF NOT EXISTS "IX_TapShowCharacters_PostId_Order" ON tapshow."TapShowCharacters" ("PostId", "Order");

-- Segment: one screen (image + narration) of a chapter; lowest "Order" = chapter entry point
CREATE TABLE IF NOT EXISTS tapshow."TapShowSegments" (
    "Id" uuid NOT NULL DEFAULT (gen_random_uuid()),
    "Title" character varying(255),
    "ImageUrl" character varying(256),
    "Narration" text,
    "AudioUrl" character varying(256),
    "Order" real NOT NULL,
    "IsEnding" boolean NOT NULL,
    "ChapterId" uuid NOT NULL,
    "CharacterId" uuid,
    "CreatedBy" uuid,
    "CreatedOn" timestamp NOT NULL,
    "ModifiedBy" uuid,
    "ModifiedOn" timestamp,
    "IsDelete" boolean NOT NULL,
    "SyncedOn" timestamp,
    "SyncError" character varying(1024),
    "TagData" character varying(64),
    CONSTRAINT "PK_TapShowSegments" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_TapShowSegments_TapShowChapters_ChapterId" FOREIGN KEY ("ChapterId") REFERENCES tapshow."TapShowChapters" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_TapShowSegments_TapShowCharacters_CharacterId" FOREIGN KEY ("CharacterId") REFERENCES tapshow."TapShowCharacters" ("Id")
);
CREATE INDEX IF NOT EXISTS "IX_TapShowSegments_ChapterId_Order" ON tapshow."TapShowSegments" ("ChapterId", "Order");
CREATE INDEX IF NOT EXISTS "IX_TapShowSegments_CharacterId" ON tapshow."TapShowSegments" ("CharacterId");

-- Choice: branch from "SegmentId" to "TargetSegmentId" (same chapter); a segment with no choices is an ending
CREATE TABLE IF NOT EXISTS tapshow."TapShowSegmentChoices" (
    "Id" uuid NOT NULL DEFAULT (gen_random_uuid()),
    "Label" character varying(255),
    "Order" integer NOT NULL,
    "SegmentId" uuid NOT NULL,
    "TargetSegmentId" uuid NOT NULL,
    "CreatedBy" uuid,
    "CreatedOn" timestamp NOT NULL,
    "ModifiedBy" uuid,
    "ModifiedOn" timestamp,
    "IsDelete" boolean NOT NULL,
    "SyncedOn" timestamp,
    "SyncError" character varying(1024),
    "TagData" character varying(64),
    CONSTRAINT "PK_TapShowSegmentChoices" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_TapShowSegmentChoices_TapShowSegments_SegmentId" FOREIGN KEY ("SegmentId") REFERENCES tapshow."TapShowSegments" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_TapShowSegmentChoices_TapShowSegments_TargetSegmentId" FOREIGN KEY ("TargetSegmentId") REFERENCES tapshow."TapShowSegments" ("Id") ON DELETE CASCADE
);
CREATE INDEX IF NOT EXISTS "IX_TapShowSegmentChoices_SegmentId_Order" ON tapshow."TapShowSegmentChoices" ("SegmentId", "Order");
CREATE INDEX IF NOT EXISTS "IX_TapShowSegmentChoices_TargetSegmentId" ON tapshow."TapShowSegmentChoices" ("TargetSegmentId");

-- Resources (uploaded images: post thumbnail or segment image), copy of game."GameResources" + SegmentId
CREATE TABLE IF NOT EXISTS tapshow."TapShowResources" (
    "Id" uuid NOT NULL DEFAULT (gen_random_uuid()),
    "PostId" uuid,
    "SegmentId" uuid,
    "CharacterId" uuid,
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
    CONSTRAINT "PK_TapShowResources" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_TapShowResources_TapShowPosts_PostId" FOREIGN KEY ("PostId") REFERENCES tapshow."TapShowPosts" ("Id"),
    CONSTRAINT "FK_TapShowResources_TapShowSegments_SegmentId" FOREIGN KEY ("SegmentId") REFERENCES tapshow."TapShowSegments" ("Id"),
    CONSTRAINT "FK_TapShowResources_TapShowCharacters_CharacterId" FOREIGN KEY ("CharacterId") REFERENCES tapshow."TapShowCharacters" ("Id"),
    CONSTRAINT "FK_TapShowResources_Users_AuthorId" FOREIGN KEY ("AuthorId") REFERENCES identity."Users" ("Id")
);
CREATE INDEX IF NOT EXISTS "IX_TapShowResources_AuthorId" ON tapshow."TapShowResources" ("AuthorId");
CREATE UNIQUE INDEX IF NOT EXISTS "IX_TapShowResources_HashId" ON tapshow."TapShowResources" ("HashId");
CREATE UNIQUE INDEX IF NOT EXISTS "IX_TapShowResources_Name" ON tapshow."TapShowResources" ("Name");
CREATE INDEX IF NOT EXISTS "IX_TapShowResources_PostId" ON tapshow."TapShowResources" ("PostId");
CREATE INDEX IF NOT EXISTS "IX_TapShowResources_SegmentId" ON tapshow."TapShowResources" ("SegmentId");
CREATE INDEX IF NOT EXISTS "IX_TapShowResources_CharacterId" ON tapshow."TapShowResources" ("CharacterId");

COMMIT;

-- =====================================================================
-- Comments + reactions (post-level only, copy of the game tables). Idempotent.
-- =====================================================================
BEGIN;

CREATE TABLE IF NOT EXISTS tapshow."TapShowPostComments" (
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
    CONSTRAINT "PK_TapShowPostComments" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_TapShowPostComments_TapShowPosts_PostId" FOREIGN KEY ("PostId") REFERENCES tapshow."TapShowPosts" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_TapShowPostComments_Users_AuthorId" FOREIGN KEY ("AuthorId") REFERENCES identity."Users" ("Id") ON DELETE CASCADE
);
CREATE INDEX IF NOT EXISTS "IX_TapShowPostComments_AuthorId" ON tapshow."TapShowPostComments" ("AuthorId");
CREATE INDEX IF NOT EXISTS "IX_TapShowPostComments_PostId_ParentId_AuthorId" ON tapshow."TapShowPostComments" ("PostId", "ParentId", "AuthorId");

CREATE TABLE IF NOT EXISTS tapshow."TapShowPostReactions" (
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
    CONSTRAINT "PK_TapShowPostReactions" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_TapShowPostReactions_TapShowPosts_TargetId" FOREIGN KEY ("TargetId") REFERENCES tapshow."TapShowPosts" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_TapShowPostReactions_Users_AuthorId" FOREIGN KEY ("AuthorId") REFERENCES identity."Users" ("Id") ON DELETE CASCADE
);
CREATE INDEX IF NOT EXISTS "IX_TapShowPostReactions_AuthorId" ON tapshow."TapShowPostReactions" ("AuthorId");
CREATE INDEX IF NOT EXISTS "IX_TapShowPostReactions_TargetId_ParentId_AuthorId" ON tapshow."TapShowPostReactions" ("TargetId", "ParentId", "AuthorId");

CREATE TABLE IF NOT EXISTS tapshow."TapShowPostCommentReactions" (
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
    CONSTRAINT "PK_TapShowPostCommentReactions" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_TapShowPostCommentReactions_TapShowPostComments_TargetId" FOREIGN KEY ("TargetId") REFERENCES tapshow."TapShowPostComments" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_TapShowPostCommentReactions_Users_AuthorId" FOREIGN KEY ("AuthorId") REFERENCES identity."Users" ("Id") ON DELETE CASCADE
);
CREATE INDEX IF NOT EXISTS "IX_TapShowPostCommentReactions_AuthorId" ON tapshow."TapShowPostCommentReactions" ("AuthorId");
CREATE INDEX IF NOT EXISTS "IX_TapShowPostCommentReactions_TargetId_ParentId_AuthorId" ON tapshow."TapShowPostCommentReactions" ("TargetId", "ParentId", "AuthorId");

COMMIT;

-- =====================================================================
-- SystemSettings: max segment voice-over audio size (MB). Read by api/tapshow/file/upload-audio.
-- Missing or 0 → API falls back to 10 MB; values above 30 are capped to 30.
-- =====================================================================
INSERT INTO system."SystemSettings"
    ("Id", "Key", "Value", "Description", "IsActive", "Order", "DataType", "Group", "MicroService", "IsDelete", "CreatedOn")
SELECT gen_random_uuid(), 'TapShowAudioSize', '10', 'Max size (MB) of a TapShow segment voice-over audio file', true, 0, 'double', 'Upload', 'TapShow', false, now()
WHERE NOT EXISTS (SELECT 1 FROM system."SystemSettings" WHERE "Key" = 'TapShowAudioSize');
