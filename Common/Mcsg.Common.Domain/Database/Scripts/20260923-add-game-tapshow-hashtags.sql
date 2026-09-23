-- =====================================================================
-- Hashtags for Game and TapShow posts: two link tables, same shape as comic."ComicTagPosts".
-- Matches the CreateTable blocks merged into 20250327042022_InitData.cs
-- (team convention: no new migrations, InitData + Designer + snapshot edited by hand).
-- Idempotent: safe to re-run on dev / stg / prod. Requires public."Tags", game."GamePosts", tapshow."TapShowPosts".
-- =====================================================================
BEGIN;

CREATE TABLE IF NOT EXISTS game."GameTagPosts" (
    "Id" uuid NOT NULL DEFAULT (gen_random_uuid()),
    "CreatedBy" uuid,
    "CreatedOn" timestamp NOT NULL,
    "ModifiedBy" uuid,
    "ModifiedOn" timestamp,
    "IsDelete" boolean NOT NULL,
    "SyncedOn" timestamp,
    "SyncError" character varying(1024),
    "TagData" character varying(64),
    "TagId" uuid NOT NULL,
    "PostId" uuid NOT NULL,
    CONSTRAINT "PK_GameTagPosts" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_GameTagPosts_GamePosts_PostId" FOREIGN KEY ("PostId") REFERENCES game."GamePosts" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_GameTagPosts_Tags_TagId" FOREIGN KEY ("TagId") REFERENCES public."Tags" ("Id") ON DELETE CASCADE
);
CREATE INDEX IF NOT EXISTS "IX_GameTagPosts_PostId" ON game."GameTagPosts" ("PostId");
CREATE INDEX IF NOT EXISTS "IX_GameTagPosts_TagId_PostId" ON game."GameTagPosts" ("TagId", "PostId");

CREATE TABLE IF NOT EXISTS tapshow."TapShowTagPosts" (
    "Id" uuid NOT NULL DEFAULT (gen_random_uuid()),
    "CreatedBy" uuid,
    "CreatedOn" timestamp NOT NULL,
    "ModifiedBy" uuid,
    "ModifiedOn" timestamp,
    "IsDelete" boolean NOT NULL,
    "SyncedOn" timestamp,
    "SyncError" character varying(1024),
    "TagData" character varying(64),
    "TagId" uuid NOT NULL,
    "PostId" uuid NOT NULL,
    CONSTRAINT "PK_TapShowTagPosts" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_TapShowTagPosts_TapShowPosts_PostId" FOREIGN KEY ("PostId") REFERENCES tapshow."TapShowPosts" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_TapShowTagPosts_Tags_TagId" FOREIGN KEY ("TagId") REFERENCES public."Tags" ("Id") ON DELETE CASCADE
);
CREATE INDEX IF NOT EXISTS "IX_TapShowTagPosts_PostId" ON tapshow."TapShowTagPosts" ("PostId");
CREATE INDEX IF NOT EXISTS "IX_TapShowTagPosts_TagId_PostId" ON tapshow."TapShowTagPosts" ("TagId", "PostId");

COMMIT;
