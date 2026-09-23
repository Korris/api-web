# TapShow Area (Mcsg.Api/Areas/TapShow)

Interactive branching visual story: **post → chapters → segments (image + narration) → choices**. Each segment is one of `Next` (continue to the next segment by `order`), `Choice` (reader picks a branch) or `Ending` (story stops); several endings per chapter are the point. Cloned from the Game area (itself a trimmed Comic copy): post CRUD, uploads, comments (1 level of replies), reactions. No favorites, reports, tags, feed aggregation, view counting, chapter-level comments.

## Data (schema `tapshow`)
| Table | Base | Note |
|---|---|---|
| `TapShowPosts` | BasePost | `PostType.TapShow = 7`. `CoverUrl` unused |
| `TapShowChapters` | BaseSubPost | `HashId`, `Title`, `Order` (float), `Status` Draft/Public, `PublishDate` set on first publish |
| `TapShowCharacters` | AuditableEntity | `PostId`, `Name`, `AvatarUrl?`, `Order`. Characters of a post (name + avatar) |
| `TapShowSegments` | AuditableEntity | `ChapterId`, `CharacterId?` (speaker, same post), `Title?`, `ImageUrl?`, `Narration?` (dialogue), `AudioUrl?` (voice-over), `Order` (float), `IsEnding`. Lowest `Order` = chapter entry point |
| `TapShowSegmentChoices` | AuditableEntity | `SegmentId` → `TargetSegmentId` (same chapter), `Label`, `Order`. Both FKs cascade. Only Choice segments have rows |
| `TapShowResources` | BaseResource | + `PostId?`, `SegmentId?`, `CharacterId?`. Temp row `IsDelete = true` until attached. Thumbnail: `PostId` only; segment image / audio: `PostId` + `SegmentId` (distinguished by `Type` Image / Audio); avatar: `PostId` + `CharacterId` |
| `TapShowPostComments`, `TapShowPostReactions`, `TapShowPostCommentReactions` | Game counterparts | post-level only |

- Team convention: no new migrations. Tables merged into `20250327042022_InitData.cs` + Designer + snapshot. Verified: temp `dotnet ef migrations add` produces an empty `Up()`.
- How it was done (repeatable): `cd Common/Mcsg.Common.Domain && dotnet ef migrations add Tmp -- "Host=x;Database=x;Username=x;Password=x"` (factory needs a connection string arg, no DB access), copy `Up()` blocks into InitData, `git diff` of the snapshot applied as a patch to the Designer, delete the temp migration, rebuild, re-run `migrations add` → empty, delete again.
- Existing DB: run `Common/Mcsg.Common.Domain/Database/Scripts/20260922-add-tapshow-area.sql` (idempotent) before deploying.

## Endpoints (`api/tapshow/...`)
Flow: upload images first, then reference them by `hashId`. All write endpoints are owner-only (post owner).

| Method | Route | Auth | Note |
|---|---|---|---|
| POST | `file/upload-media` | yes | multipart `File`; image only; re-encoded JPEG; limit = SystemSettings `ThumbnailCoverSize` MB; returns `{ hashId, url, width, height, size }` |
| POST | `file/upload-audio` | yes | multipart `File`; mp3 / m4a / aac / wav / ogg; limit = SystemSettings `TapShowAudioSize` MB (default 10, ceiling 30); returns `{ hashId, url, size }` → `audioHashId` of a segment |
| POST | `tapshow` | yes | `title, summary, thumbnailHashId, isCurrentUserAuthor, authorName, isMature, isCompleted, permission, characters?[]`; each `{ name, avatarHashId?, order? }` created in the same transaction (max 50); response carries `characters[]` with ids |
| PUT | `tapshow/{hashId}` | owner | same body without `characters` (ignored if sent); new `thumbnailHashId` replaces the file (old object deleted). Characters are changed only through `character` endpoints |
| DELETE | `tapshow/{hashId}` | owner | soft-deletes post + chapters + segments + choices, all images removed from Minio |
| GET | `tapshow/{hashId}` | no | `chapterCount` (public chapters; owner: all), `commentCount`, `reaction`, `characters[]` (null in lists); non-public / Private → owner only (404 otherwise) |
| GET | `tapshow/list?PageNumber&PageSize&ProfileName&Keyword` | no | Public + non-Private, newest first, max 50/page; mobile hides mature |
| GET | `tapshow/my?PageNumber&PageSize` | yes | own posts, all statuses |
| GET | `chapter/post/{postHashId}` | no | chapters by `order`; owner sees Draft too; each with `segmentCount`, `endingCount` |
| GET | `chapter/{hashId}` | no | chapter + `startSegmentId` + `segments[]` (each with `kind` Next/Choice/Ending, `nextSegmentId`, `choices[]`, `characterId/characterName/characterAvatarUrl`). Draft → owner only. Premium post + viewer not premium/owner → `isLocked: true`, `segments: []`. `imageHashId` only for the owner |
| POST | `chapter` | owner | `postHashId, title, order?, status (Draft/Public)`; `order` null → appended |
| PUT | `chapter/{hashId}` | owner | `title, order?, status` |
| DELETE | `chapter/{hashId}` | owner | soft-deletes segments + choices, images removed |
| GET | `character/post/{postHashId}` | no | characters by `order`; same post visibility as chapter list; `avatarHashId` only for the owner |
| POST | `character` | owner | `postHashId, name, avatarHashId?, order?`; `order` null → appended |
| PUT | `character/{id}` | owner | `name, avatarHashId?, order?`; `avatarHashId`: same = keep, new = replace, null = remove |
| DELETE | `character/{id}` | owner | soft-delete; segments using it keep their content with `characterId = null`; avatar removed |
| POST | `segment` | owner | `chapterHashId, characterId?, title?, imageHashId?, narration?, audioHashId?, order?, isEnding`; image or narration required; `isEnding` true rejected while the segment has choices; `characterId` must be a character of the same post; `order` null → appended |
| PUT | `segment/{id}` | owner | same fields; `imageHashId` / `audioHashId`: same = keep, new = replace, null = remove |
| DELETE | `segment/{id}` | owner | also removes choices pointing to it |
| PUT | `segment/{id}/choices` | owner | `{ choices: [{ label, targetSegmentId }] }` replaces all; order = array index; max 8; targets must be live segments of the same chapter, distinct, not itself; non-empty → segment becomes Choice (`isEnding` reset); empty → plain Next |
| GET/POST/PUT/DELETE | `comment/...` | as Game | `comment/post/{hashId}`, `comment/{id}/replies`, `comment`, `comment/{id}` |
| GET/POST/DELETE | `reaction/post/...`, `reaction/comment/...` | as Game | upsert per user, types 0..6 |

## Reader flow (frontend)
1. `GET tapshow/{hashId}` → `GET chapter/post/{hashId}` → pick a chapter.
2. `GET chapter/{chapterHashId}` → start at `startSegmentId`, show `imageUrl` + `narration` (with `characterName` / `characterAvatarUrl` as the speaker when set, play `audioUrl` if present), then by `kind`: `Next` → button Next → `nextSegmentId` (null = chapter over); `Choice` → render `choices` as buttons, jump to `targetSegmentId`; `Ending` → stop. Whole graph is one response, navigation is local. A "14 / 17" counter = index of the current segment in `segments` (ordered by `order`).
3. Optional: `GET character/post/{hashId}` for a cast list.

## Permission rules
- `Private`: never listed, post / chapters return 404 to anyone but the owner.
- `Premium`: post and chapter list visible; chapter detail is locked (no segments) unless the viewer is premium (JWT `isPremium`) or the owner.
- Draft chapters: owner only (list and detail).

## Storage
- Minio instance `Default`, bucket `{bucket}-public`, keys `tapshow/{userFolder}/images/{hash}.jpg` and `tapshow/{userFolder}/audios/{hash}.{ext}` (audio stored as uploaded with its audio content type).
- A post / segment may only reference resources with `AuthorId` = current user, `PostId` null, `IsDelete = true`; anything else → 400.
- Orphan uploads stay as temp rows; cleanup query like the Game area (`tapshow."TapShowResources"` where `PostId IS NULL AND IsDelete`).

## Deploy checklist
1. Run the SQL script on the target DB (creates the tables and the `TapShowAudioSize` SystemSettings row; `GET config` exposes it as `tapShowAudioSize`).
2. Add the `/api/tapshow/` prefix in `ntada-azure-infra/helm_local/values-{env}.yaml` (copy the `/api/game/` rule, keep the upload body-size annotation).
3. Bucket `{bucket}-public` must exist.

## Code map
- Domain: `Common/Mcsg.Common.Domain/Entities/TapShows/*` (+ `Configurations`, `Extensions` with Create/Update/Delete helpers).
- API: `Mcsg.Api/Areas/TapShow/{Controllers,Services,Interfaces,Requests,Validators,Models}`. `TapShowResourceService` is the shared upload-attach/release helper used by post and segment services. `TapShowChapterService.ProjectSegments` is the single segment projection.
- Wiring: `Program.cs` (`AddTapShowServices`, `TapShowConfig.MediaExtensionAllow`), `DbSchema.TapShow`, `MinioFolder.TapShow`.
