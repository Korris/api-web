# Code Review: TapShow area (post type, chapters, segments, choices)

Scope: `Common/Mcsg.Common.Domain/Entities/TapShows/**`, `Mcsg.Api/Areas/TapShow/**`, wiring diffs (PostType, DbSchema, Setting, McsgContext, IMcsgContext, User, Program.cs), `Database/Scripts/20260922-add-tapshow-area.sql`, InitData/Designer/snapshot merge, `docs/tapshow-area.md`. ~3.6k LOC new. Reference: `Mcsg.Api/Areas/Game/**`.
Method: full read + `git diff`, Python diff of SQL vs migration, scratch console app (`Mcsg.Common.Domain` + Npgsql, `ToQueryString()`, no DB) to verify EF translation of the suspicious projection.

## Critical

### C1. `GET chapter/{hashId}` always 500 — projection not compilable
`Mcsg.Api/Areas/TapShow/Services/TapShowChapterService.Query.cs:193-203`
`Summary = ProjectSummary(new[] { c }.AsQueryable()).First()` inside `Select`. Verified with EF Core 8.0.14 / Npgsql 8.0.11: query compilation throws
`ArgumentException: Expression of type 'TapShowChapter[]' cannot be used for parameter of type 'IQueryable<TapShowChapter>' of method ProjectSummary(...)`.
EF rewrites the lambda parameter `c` to a shaper and can no longer bind `new[]{c}` to the user method; there is no translation and no client-eval fallback. Even if it client-evaluated, `c.Post` would be null (no Include, no-tracking) → NRE, and both counts would be 0.

Fix (verified translatable, one SQL round-trip):
```csharp
var chapter = await _context.Available<TapShowChapter>(false)
    .Where(c => c.HashId == request.HashId && !c.Post.IsDelete)
    .Select(c => new
    {
        Summary = new ChapterResponse
        {
            Id = c.Id, HashId = c.HashId!, PostId = c.PostId, PostHashId = c.Post.HashId, Title = c.Title, Order = c.Order,
            Status = c.Status, PublishDate = c.PublishDate, CreatedOn = c.CreatedOn, ModifiedOn = c.ModifiedOn,
            SegmentCount = c.TapShowSegments.Count(s => !s.IsDelete),
            EndingCount = c.TapShowSegments.Count(s => !s.IsDelete && !s.Choices.Any(x => !x.IsDelete))
        },
        c.Status, PostUserId = c.Post.UserId, PostStatus = c.Post.Status, PostPermission = c.Post.Permission
    })
    .FirstOrDefaultAsync();
```
To keep one projection source (DRY with `ProjectSummary`), turn the body into a static `Expression<Func<TapShowChapter, ChapterResponse>>` and use `.Select(SummaryExpr)` in `ProjectSummary`, but the inline copy is fine (KISS). Alternative: 2 queries (visibility fields first, then `GetSummaryAsync(id)`).

## High

### H1. `Private` permission not enforced on comment / reaction paths
- `Services/TapShowCommentService.cs:163` (create), `Services/TapShowCommentService.Query.cs:30` (list root) — check `Status == Public || UserId == userId` only; `Permission == Private` ignored.
- `Services/TapShowCommentService.Query.cs:25` (list replies by ParentId) — no post visibility check at all.
- `Services/TapShowReactService.cs:135` (react to post), `:151` (react to comment) — same.
Post detail returns 404 for Private posts (`TapShowPostService.Query.cs:226`) but anyone with the hashId can list/create comments and react → existence leak + interaction on a Private post. Inherited from Game (same lines there), but `docs/tapshow-area.md` promises "Private: never listed, post / chapters return 404 to anyone but the owner".
Fix: add `&& (p.Permission != PostPermission.Private || p.UserId == userId)` to the 3 post lookups; for replies filter `c.Post.Status == Public && (c.Post.Permission != Private || c.Post.UserId == request.UserId)`; for comment reactions add the same via `c.Post`. Consider one shared `Expression<Func<TapShowPost,bool>> VisibleTo(Guid? userId)` used by post/chapter/comment/react services (4 hand-written copies today).

## Medium

### M1. Chapter lookups by `HashId` alone have no usable index
`Common/.../TapShows/Configurations/TapShowChapterConfiguration.cs:17` — only index is `(PostId, HashId, AuthorId)`; every `c.HashId == x` lookup (`GetOwnedChapterAsync`, `GetAsync`, segment `CreateAsync`) is a seq scan on `TapShowChapters`. Posts are fine (`HashId` leads). Fix: `builder.HasIndex(x => x.HashId)` + matching line in the SQL script + InitData/Designer/snapshot. Also: `SubHashLength` collisions across posts return an arbitrary chapter (`FirstOrDefault`); a unique index on `HashId` would make it explicit — or include `PostHashId` in the route as Comic does.

### M2. `SegmentChoicesSetV` NREs on `"choices": null`
`Validators/SegmentChoicesSetV.cs:19-23` — `NotNull().Must(c => c.Count <= ...)`: FluentValidation default cascade is Continue, `Must` runs with null → `NullReferenceException` → 500 instead of 400. Fix: `RuleFor(p => p.Choices).Cascade(CascadeMode.Stop).NotNull()...`.

### M3. DRY vs Game
- `RequireUser` copy-pasted in 5 services (`TapShowPostService.cs:124`, `TapShowChapterService.cs:104`, `TapShowSegmentService.cs:129`, `TapShowCommentService.cs:262`, `TapShowReactService.cs:178`). One `internal static class TapShowGuard { static Guid RequireUser(Guid?) }` is enough.
- `TapShowReactService`, `TapShowCommentService(.Query)`, `MediaOnlyAttribute`, `UploadFileDto`, `ReactionSummaryResponse`, `CommentResponse` are near byte-for-byte Game copies (3rd generation: Comic → Game → TapShow). Not blocking (team convention is per-area clones), but the react service is already generic over `DbSet<T> where T : BaseReaction` — it could live in a shared `Areas/Shared` and be reused by Game with zero behaviour change.
- Positive: `TapShowResourceService` is a cleaner extraction than `GamePostService.Resources.cs`; Game could adopt it later.

### M4. Dead constant
`Common/Mcsg.Common.Core/Constants/DbSchema.cs:74` `TapShowTables` — no reader anywhere (`GameTables` is dead too). Drop it or wire it where it was meant to go.

## Low

- `Services/TapShowPostService.cs:91` unused `var now`.
- `Services/TapShowFileService.cs:48` `file.OpenReadStream().IsImage()` stream never disposed (copied from Game).
- `Services/TapShowChapterService.Query.cs:226` `request.IsPremium` → `BaseR.cs:211` does `Payload.RootElement.GetProperty("isPremium")` which throws `KeyNotFoundException` if a logged-in user's JWT lacks the claim. Comic feed already relies on it so tokens presumably carry it; just be aware this is the first anonymous-reachable endpoint outside Comic that touches it.
- `SetChoicesAsync` / `NextOrderAsync`: no unique constraint on `(SegmentId, TargetSegmentId, IsDelete=false)`; two concurrent PUTs can leave duplicate live choices. Acceptable for owner-only authoring; note only.
- `ReactionController.GetPostReacts` / `GetCommentReacts`: no visibility check by id (Game same). Ids of Private posts are only returned to the owner, so low.
- Docs `docs/tapshow-area.md:48`: "Private ... 404 to anyone but the owner" is not true for `comment/*` and `reaction/*` until H1 is fixed.

## OK — verified

- **Image swap (segment)** `TapShowSegmentService.cs:74-95`: keep (same hash → no release, `Attach` early-returns), replace (temp fetched *before* release, so a 400 leaves nothing mutated), remove (null → release, ImageUrl null; validator forces narration). `image != current` reference compare is safe (same DbContext identity map; equal-hash case is excluded by the branch).
- **Thumbnail swap (post)** `TapShowPostService.cs:64-80`: same pattern; `SegmentId == null` filter keeps segment images out; passing a segment image's hash as thumbnail → 400 (PostId not null).
- **No double attach**: `Attach` sets `resource.Post` → `PostId` persisted; `GetTempResourceAsync` requires `PostId == null && IsDelete && AuthorId == userId && Type == Image` → a hash can be consumed once, only by its uploader; released rows keep `PostId` so they cannot be re-attached.
- **Visibility**: post detail/list, chapter list (Draft filtered for non-owner), chapter detail (post status/permission + chapter status + Premium lock), all segment endpoints owner-only via `GetOwnedSegmentAsync` (checks chapter and post not deleted). `ImageHashId` only when `isOwner` (`ProjectSegments`, `GetByIdAsync` passes `true` only from owner paths).
- **Choices rules**: max 8 + distinct (validator), not self, all targets live and same chapter (`CountAsync` vs distinct count), order = index, empty = ending. Segment delete removes incoming + outgoing choices; chapter/post delete cover all choices of their segments.
- **Soft-delete cascade** consistent at every level; released Minio objects removed only after `SaveChanges`, best-effort, logged.
- **EF translation** (except C1): `ProjectSegments` (conditional subquery + nested `ToList`), `ProjectSummary`, post `Project` with `ChapterCount` owner branch, comment `ReplyCount`, grouped reaction summary — all server-side; reactions batched per page (no N+1).
- **SQL script vs EF model**: all 8 tables — column names, types, nullability, PK defaults, 15 FKs with identical `ON DELETE`, 15 indexes incl. 4 unique — match the migration exactly (scripted diff, 0 mismatches). Schema `tapshow` ensured in both.
- **Migration merge**: `git diff` on `InitData.cs` has 0 removed lines (GameResources drop intact). `Down()` order is dependency-safe (CommentReactions → PostReactions → Resources → Choices → Comments → Segments → Chapters → Posts → Game...). `Up()` places TapShow tables after `Users` (l.689) and in FK order (Segments 4463 < CommentReactions 4495 < Resources 4533 < Choices 4591). Designer and snapshot additions are identical (29 entity refs each, diff empty).
- `PostType.TapShow` = 7 as documented; Program.cs wiring and DI registration complete; all files < 200 lines.

## Recommended actions (in order)
1. C1 — fix `GetAsync` projection (blocking; endpoint is dead).
2. H1 — enforce `Private` on comment/react lookups (and replies).
3. M1 — `HashId` index on `TapShowChapters` (+ SQL script + InitData/Designer/snapshot).
4. M2 — cascade stop on `Choices` rule.
5. M3/M4/Low — cleanup in the same PR if cheap.

## Unresolved questions
- Should `Premium` also lock the chapter *list* counts (`SegmentCount`/`EndingCount`)? Currently exposed to non-premium viewers; design doc silent.
- Is the Game area expected to inherit the H1 fix (same bug there)?

**Status:** DONE_WITH_CONCERNS
**Summary:** One blocking bug (chapter detail endpoint throws at query compile, empirically verified) and one authz gap (Private posts reachable via comment/reaction routes); everything else — image swap/attach logic, soft-delete cascades, SQL script, migration merge — verified correct.
