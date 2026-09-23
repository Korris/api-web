# Game Area (Mcsg.Api/Areas/Game)

Trimmed copy of the Comic area: one post = one HTML game. Post CRUD, uploads, comments (1 level of replies), reactions. No chapters, favorites, reports, tags, sounds, Analytic sync, Realtime notifications.

## Data (schema `game`)
| Table | Copy of | Note |
|---|---|---|
| `GamePosts` | ComicPosts | + `GameUrl` text. `CoverUrl` column exists (BasePost) but unused |
| `GameResources` | ComicResources | + `PostId` (nullable FK). Temp row `IsDelete = true` until attached to a post. `SubPostId` unused |
| `GamePostComments` | ComicPostComments | no Resource FK |
| `GamePostReactions`, `GamePostCommentReactions` | Comic counterparts | one row per user per target |
| `GameTagPosts` | ComicTagPosts | `PostId`, `TagId` → shared `public."Tags"`. Soft-deleted when a tag is removed from the post. Script `Database/Scripts/20260923-add-game-tapshow-hashtags.sql` |

- `PostType.Game = 6` (appended, existing ints unchanged).
- Team convention: no new migrations. Tables are merged into `20250327042022_InitData.cs` + Designer + snapshot. Snapshot verified equal to the model (temp `migrations add` produces 0 operations).
- Existing DB: run `Common/Mcsg.Common.Domain/Database/Scripts/20260909-add-game-area.sql` (idempotent, 4 blocks) before deploying.
- `DataSeeder` now uses static GUIDs for `UserNameHistories` seeds, so `dotnet ef` no longer churns seed data.

## Endpoints (`api/game/...`)
Flow, same as Comic: upload files first, then create the post with the returned `hashId`s.

| Method | Route | Auth | Note |
|---|---|---|---|
| POST | `file/upload-media` | yes | multipart `File`; image only; re-encoded JPEG; returns `{ hashId, url, width, height, size }` |
| POST | `file/upload-game` | yes | multipart `File`; `.html` only, max = SystemSettings `GameFileSize` MB (default 30, ceiling 50), stored `text/html`; returns `{ hashId, url, ... }` |
| POST | `game` | yes | body: `title, summary, thumbnailHashId, gameHashId, isCurrentUserAuthor, authorName, isMature, permission, tags?[]`; `tags`: names with or without `#`, max 10, letters / digits / `_`, stored lower-case; response carries `tags[]` |
| PUT | `game/{hashId}` | owner | same body; a new hashId replaces the file, the old object is deleted from Minio. `tags` is the full set: names left out are unlinked, null / empty = no tags |
| DELETE | `game/{hashId}` | owner | soft delete post + resources, objects deleted from Minio |
| GET | `game/{hashId}` | no | includes `commentCount`, `reaction`; non-public / Private → owner only (404 otherwise) |
| GET | `game/list?PageNumber&PageSize&ProfileName&Keyword&HashTag` | no | Public + non-Private, newest first, max 50/page; mobile hides mature; `HashTag` = one tag name (no `#`) |
| GET | `game/my?PageNumber&PageSize` | yes | own posts, all statuses |
| GET | `comment/post/{hashId}?PageNumber&PageSize` | no | root comments, newest first, each with `replyCount` + `reaction` |
| GET | `comment/{id}/replies?PageNumber&PageSize` | no | replies, oldest first |
| POST | `comment` | yes | body: `postHashId, parentId?, body` |
| PUT | `comment/{id}` | author | body: `body` |
| DELETE | `comment/{id}` | author or post owner | root delete hides replies |
| GET | `reaction/post/{postId}`, `reaction/comment/{commentId}` | no | `{ targetId, totalReacts, currentUserReactType, reactions[], mostReactionType }` |
| POST | `reaction/post`, `reaction/comment` | yes | body: `targetId, type (0..6)`; upsert per user |
| DELETE | `reaction/post/{id}`, `reaction/comment/{id}` | yes | remove own reaction |

## Permission rules
- `Private`: never listed publicly, detail returns 404 to anyone but the owner.
- `Premium`: listed and viewable, but `gameUrl` is null unless the viewer is premium (JWT `isPremium`) or the owner.

## Storage
- Minio instance `Default`, bucket `{bucket}-public`, keys `game/{userFolder}/thumbnails/{hash}.jpg` and `game/{userFolder}/games/{hash}.html`.
- A post may only reference resources with `AuthorId` = current user, `PostId` null, `IsDelete = true` and the right `Type`; anything else → 400.
- Upload verified with `StatObject` (Minio SDK silent-failure workaround).
- Orphan uploads (never attached) stay as temp rows. Cleanup when needed:
```sql
SELECT "Url", "BucketName" FROM game."GameResources"
WHERE "PostId" IS NULL AND "IsDelete" = true AND "CreatedOn" < now() - interval '2 days';
```

## Security (must-read for frontend)
- The `.html` is user-controlled script. API never returns HTML content, only the URL on the media domain.
- Embed ONLY as `<iframe sandbox="allow-scripts" src="{gameUrl}">` (no `allow-same-origin`, no inline injection into the app DOM). Otherwise the game can read app cookies / tokens.

## Deploy checklist
1. Run the SQL script on the target DB.
2. Add the `/api/game/` prefix in `ntada-azure-infra/helm_local/values-{env}.yaml` (copy the `/api/comic/` rule, keep the upload body-size annotation).
3. Bucket `{bucket}-public` must exist (Social already uses it).
4. Recommended: serve `game/*/games/*.html` with `Content-Security-Policy: sandbox allow-scripts; frame-ancestors <app origins>` and `X-Content-Type-Options: nosniff` at ingress / Minio; never add the media host to API CORS `Origins`.

Seeding: `Database/Seeds/seed-comments-social-story-comic.sql` and `seed-reactions-social-story-comic.sql` also fill the game tables.
