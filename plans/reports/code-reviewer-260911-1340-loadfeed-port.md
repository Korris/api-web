# Code Review: LoadFeed port (Comic / Story / Document / Social)

## Scope
- New: `Common/Mcsg.Common.Domain/Dtos/CommentLateDto.cs`; `Areas/{Comic,Story,Document}/{Dtos/PostFeedDto.cs, Extensions/PostFeedExtension.cs, Requests/Posts/PostLoadFeedR.cs, Queries/Post(s)/PostLoadFeedH.cs, PostLoadFeedH.Stats.cs}`; `Areas/Social/{Requests/Posts/PostLoadFeedR.cs, Queries/Posts/PostLoadFeedH.cs}`
- Modified: 4x `PostController.cs`, 4x `DiPostExtension.cs`, 3x `PostFilter.cs`
- LOC: ~1,150 new (3 identical clones x ~330 + Social ~180)
- Focus: static review vs api-mobile sources; no DB available
- Story/Document verified byte-identical to Comic after prefix normalization (only the doc-comment differs)

## Overall Assessment
Comic/Story/Document port is a faithful transcription of mobile `PostSearchH` (IsMine=false) with sensible improvements (batched reactions, no-tracking, shared projection). EF translation risks (nested `Take(1)`, conditional-null `Resource`, `Max` in `OrderByDescending`) are the same shapes mobile already runs on the same EF/Npgsql 8.0.11 stack, so they are low risk. The Social port has one very likely runtime failure: it feeds the **mobile** DB function result set into the **web** `FeedsListQueryDbDto`/`MappingFeedInListRespone`, whose column names differ. Several security gaps are inherited 1:1 from mobile and are called out so the owner can decide whether the monolith should keep them.

## Critical
### C1. Social: `fm_posts_by_username` columns do not match web `FeedsListQueryDbDto` -> `ArgumentNullException` on every non-empty feed
- `Mcsg.Api/Areas/Social/Queries/Posts/PostLoadFeedH.cs:50,93,104` -> `Services/FeedService.cs:1080`
- Mobile consumer of this function (`api-mobile .../PostService.cs:82+`, mobile `FeedsListQueryDbDto`) reads columns `TotalResources`, `SubPostResourceString`, `SubPostString`, `LatestComment`, `TotalComments`, `TotalReactions`, `IsLongText`. Web DTO (populated by web `fw_*` functions, e.g. `fw_get_posts_by_postid_trackings`) expects `TotalResource`, `SubPostResourceStr`, `SubPostStr`. Dapper maps by column name -> web fields stay null/0.
- `FeedService.MappingFeedInListRespone` line 1080: `JsonConvert.DeserializeObject<List<ResourceDto>>(item.SubPostStr)` is unguarded; Newtonsoft throws `ArgumentNullException` for null input.
- Scenario: `PATCH api/social/Post/v1/LoadFeed {Filter:{UserName:"alice"}}`, alice has 1+ post -> handler catches -> `SetError("Value cannot be null. (Parameter 'value')")`. If the column happened to exist, `TotalResource`=0 would still drop all media.
- Fix (verify first with `\sf social.fm_posts_by_username`): alias in the SELECT, e.g. `SELECT t.*, t."SubPostString" AS "SubPostStr", t."SubPostResourceString" AS "SubPostResourceStr", t."TotalResources" AS "TotalResource" FROM social.fm_posts_by_username(...) t`; or create/use a `fw_posts_by_username` returning web names; and null-guard line 1080 (`item.SubPostStr == null ? [] : ...`).

## High
### H1. Social: rows come from SQL with hard-coded `IsMyseft = true`, no viewer/premium input; EF permission filter only affects `TotalRecords`
- `PostLoadFeedH.cs:69-78` (count) vs `:84-93` (rows)
- The permission/hide/premium `Where` chain is only used for the count. The function receives `IsMyseft=true` (line 90) and no viewer id. If the function uses that flag to include private/hidden/inactive posts (its name strongly suggests "viewer is the author"), any anonymous caller gets another user's Private/Premium posts, and `TotalRecords` disagrees with the row set (broken paging).
- Inherited from mobile; but the port's comment on line 68 asserts "visible to the caller", which the row query does not enforce.
- Fix: `IsMyseft = request.UserId != null && request.UserId == authorId`; additionally post-filter `rows` by `Permission`/`Hide` with the same predicate as `q`, or verify the function body.

### H2. Comic/Story/Document: author-feed branch returns Private/Premium/Inactive posts to any caller
- `PostLoadFeedH.cs:91-100` (all three areas)
- When `Filter.UserId`/`UserName` resolves, the query becomes `p.UserId == userId` with no `Permission` check and no relation to `request.UserId`/`IsPremium`. `StatusUtils.PostStatuses` = Inactive + Public, so Inactive posts are also returned with full `Body`, `Title`, `ThumbnailUrl`; `IsCensored` (line 136) is only a flag, nothing is redacted.
- Scenario: anonymous `PATCH .../v1/LoadFeed {Filter:{UserName:"alice"}}` -> alice's Private comics with body/thumbnail/latest comment.
- Inherited 1:1 from mobile `PostSearchH` (IsMine=false). Fix if the monolith should be tighter: in the `else` branch add `&& (p.Permission == PostPermission.Public || (request.IsPremium && p.Permission == PostPermission.Premium) || p.UserId == request.UserId)`; when `IsCensored`, blank `Body`/`Title`/`ThumbnailUrl`/`LatestComment`.

## Medium
### M1. `keyword.ToLower()` evaluated client-side by EF before SQL -> throws when `Keyword` omitted
- `PostLoadFeedH.cs:74-75` (all three areas)
- `keyword.ToLower()` is an evaluatable closure subtree; EF Core 8's parameter extractor compiles and invokes it eagerly to produce `@__ToLower_0`, regardless of the `string.IsNullOrWhiteSpace(keyword) ||` short-circuit. With `Filter` omitted or `{UserName:"x"}` only, `keyword` is null -> `NullReferenceException` wrapped in `InvalidOperationException("An exception was thrown while attempting to evaluate a LINQ query parameter expression")` -> error response.
- Same pattern exists in `MyComicSearchH.cs:79` and mobile, so it may be masked by clients always sending `Keyword`; the by-username LoadFeed call is the one most likely to omit it. Cheap to make safe.
- Fix: `var kw = (keyword ?? "").Trim().ToLower(); q = q.WhereIf(kw != "", p => (p.Title + "").ToLower().Contains(kw) || (p.Body + "").ToLower().Contains(kw));`

### M2. Ordering: DB sorts by all sub-posts (incl. deleted/unpublished), then page is re-sorted client-side
- `PostLoadFeedH.cs:105-107` (DB order = `Max(ComicSubPosts.CreatedOn)` over every chapter) vs `:127` (page re-sorted by `SortCreatedOn` = latest **published** `PublishDate`)
- Scenario: a comic with a newly created but scheduled (future `PublishDate`) or soft-deleted chapter is pulled to page 1 by the DB order, but shows an old `SortCreatedOn`/`IsNewChapter=false`; cross-page order is not monotonic. Inherited from mobile.
- Fix: `OrderByDescending(p => p.ComicSubPosts.Where(sp => !sp.IsDelete && sp.PublishDate <= DateTime.UtcNow).Max(sp => (DateTime?)sp.PublishDate) ?? p.CreatedOn)` and drop the client re-sort.

### M3. `TotalRecords` in Social is computed against a different predicate than the rows (see H1); in Comic it uses sync `q.Count()` (`:119`) while everything else is async
- Minor, mirrors `MyComicSearchH`; prefer `await q.CountAsync(cancellationToken)` for consistency with the Social port.

## Low
- L1. `IsFavorite` filter materialises every favorite id then `Contains` (`PostLoadFeedH.cs:112-114`): unbounded `IN (...)` parameter list for heavy users. Use a correlated `Any` instead. Inherited.
- L2. `_businessText.Process` called once per item (`:142`); `FeedService.GetSharePosts` shows the batched pattern (`GetProfiles(allBodies)` then `Process(body, profiles)`). At most PageSize calls, so acceptable.
- L3. Contract drift vs mobile: `TotalComments` is `0` instead of `null` for posts without comments (`:138` `GetValueOrDefault`); `CommentLateDto` adds `HashId`, `AuthorName`, `ResourceId` (always null here). Additive/benign, note for mobile client.
- L4. Social `favoritePostIds` query (`:81`) runs even when `request.UserId` is null (`WHERE "UserId" IS NULL`); skip when anonymous.
- L5. DRY: 3 byte-identical clones x 5 files. Acceptable per web convention, but any fix above must be applied three times; consider a shared generic over `BasePost` later.

## EF translation review (no findings)
- Nested `Select(...).OrderByDescending().Take(1).ToList()` two levels deep, `Resource == null ? null : new ComicResource{...}`, `new User { Avatar = ... }` and `Max` inside `OrderByDescending`: identical shapes are in the mobile handler running on the same EF Core 8 / Npgsql 8.0.11; `Take(1)` becomes a `ROW_NUMBER` window, empty `Max` yields NULL -> CASE falls to `CreatedOn`. Changing post-comment `Resource` from always-new to conditional-null is output-equivalent (`resource?.` on both sides).
- `ToFeedDto` `Concat` vs mobile `Union`: equivalent for reference-type DTOs without equality overrides.
- Reactions batch (`Stats.cs:23-62`) is semantically equal to mobile's per-post `MappingReactionsOfPost` (soft-deleted excluded, first user reaction wins, tie-break on `MostReactionType` is SQL-order dependent in both).
- Dapper params: `int[]` -> `integer[]`, null `@UserName` -> text NULL, `ushort` -> `int` casts fine; connection not opened explicitly matches `FeedService.GetSharePosts`.

## Positive
- Batched reaction/comment queries remove the mobile 2N+2 pattern.
- `Available<T>(false)` everywhere; projection shared via `Expression<Func<>>` field; `Resource` conditional is cleaner than mobile's always-new.
- DI/controller/filter changes are consistent with existing area conventions; Story keeps its `Queries/Post` folder.

## Recommended Actions
1. C1: confirm function column names against DB and alias/guard before shipping Social.
2. H1/H2: decide whether the monolith keeps mobile's open author-feed semantics; if not, add the permission predicate and redact censored items.
3. M1: hoist `kw` outside the lambda (3 files) and in `MyComicSearchH`.
4. M2/M3/L*: optional cleanups.

## Metrics
- Build: 0 errors / 0 warnings (reported by caller)
- Tests: none cover the new handlers
- Lint: n/a

## Unresolved Questions
1. Exact output columns and `IsMyseft` semantics of `social.fm_posts_by_username` (no SQL source in either repo).
2. Is the mobile client guaranteed to send `Filter.Keyword` (even empty)? Determines whether M1 is latent or live.
3. Should web LoadFeed keep returning another author's Private/Premium/Inactive posts (H2), as mobile does?
