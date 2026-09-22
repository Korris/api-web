# TapShow post type — implementation report (2026-09-22)

Plan: `plans/260922-1413-tapshow-post-type/plan.md` · Review: `plans/reports/code-reviewer-260922-1432-tapshow-area.md` · Docs: `docs/tapshow-area.md`

## Delivered
- `PostType.TapShow = 7`, schema `tapshow`, 9 tables (posts, chapters, characters, segments, segment choices, resources, comments, reactions, comment reactions).
- Domain entities + EF configs + Create/Update/Delete helpers in `Common/Mcsg.Common.Domain/Entities/TapShows/`.
- Migration merged into `20250327042022_InitData.cs` + Designer + snapshot (team convention). Verified: temp `dotnet ef migrations add` → empty `Up()`. Idempotent SQL: `Database/Scripts/20260922-add-tapshow-area.sql` (verified column-by-column vs model by reviewer).
- API area `Mcsg.Api/Areas/TapShow` (routes `api/tapshow/...`): image upload, post CRUD/list/my, chapter CRUD + list + detail (full segment graph), segment CRUD + `PUT segment/{id}/choices` (branching), comments, reactions. DI + Program.cs wired.
- Characters (added after first review): `TapShowCharacters` (name + avatar per post), `api/tapshow/character` list/create/update/delete, segment `characterId` (speaker, validated same post), chapter detail returns `characterName` / `characterAvatarUrl` per segment. Migration regenerated from clean snapshot and re-verified (empty `Up()`); SQL script updated.
- Segment voice-over audio (per screenshot review): `file/upload-audio` (mp3/m4a/aac/wav/ogg, SystemSettings `TapShowAudioSize`, default 10 MB, ceiling 30), `TapShowSegments.AudioUrl`, segment `audioHashId` keep/replace/remove like the image, `audioUrl` in chapter detail. `ConfigController` size keys + SQL setting row added. Migration regenerated and re-verified.
- `dotnet build Mcsg.Api` succeeds.

## Review fixes applied
- C1 chapter detail projection not EF-translatable → split into visibility query + summary query.
- H1 Private posts reachable via comment/reaction routes → same visibility rule as post detail on comment create/list/replies and post/comment reactions.
- M1 added index `IX_TapShowChapters_HashId` (config + InitData + Designer + snapshot + SQL, re-verified empty migration).
- M2 `SegmentChoicesSetV` cascade stop (null `choices` no longer NREs). Low: unused var removed.

## Not done / notes
- No automated tests: no test project references `Mcsg.Api` (same as Game area). Manual smoke test on dev after running the SQL script + adding `/api/tapshow/` ingress prefix.
- M3 (shared code between Game/TapShow areas: RequireUser, react service, DTOs) left as-is to avoid touching Game; candidate for `Areas/Shared` later.
- Not committed (branch `release`, 60+ files changed).

## Unresolved questions
1. Game area has the same H1 gap (Private posts reachable via `api/game/comment` / `api/game/reaction`). Apply the same fix there?
2. Should Premium hide `segmentCount` / `endingCount` in the chapter list for non-premium viewers? (Currently visible, segments locked.)
3. Cross-chapter branching (choice → segment of another chapter) intentionally not supported. OK?
