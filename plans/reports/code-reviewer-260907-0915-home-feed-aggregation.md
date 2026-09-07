# Code Review: Home feed aggregation endpoint

**Date:** 2026-09-07 | **Branch:** release | **Reviewer:** code-reviewer
**Endpoint:** `GET api/social/post/latest-posts-by-type/detail`

## Scope
- New: `Mcsg.Api/Areas/Social/Services/HomeFeedAggregationService.cs` (119 LOC)
- New: `Mcsg.Api/Areas/Social/Interfaces/IHomeFeedAggregationService.cs`
- New: `Mcsg.Api/Areas/Social/Models/LatestPostsDetailResponse.cs`
- Modified: `Mcsg.Api/Areas/Social/Controllers/PostController.cs`, `Mcsg.Api/Program.cs`
- Build: 0 errors (confirmed by caller). Design decisions (sequential calls, per-type swallow, `object` Data, old endpoint kept) not re-litigated.

## Overall
Small, readable, correct for the happy path. Semantics match the four per-area controllers exactly: same `new PaginatedR(HttpContext, hashIds)` construction (so `UserId`, `Hides`, `IsPremium`, `IsAdministrator` come from the same claims/headers), none of the replaced actions had `[Authorize]`, hide-lists and status/permission filters are applied inside the area SQL (`Story/PostService.Query.cs:879-880`, `FeedService.Query.cs`), and hashIds are bound as `ANY(@HashIds)` (no injection). Ordering preserved via `rankedItems.Select`; per-type `Distinct()` dedups the query while duplicate ranked rows are still echoed (identical to old flow). Null paths covered: `LatestPostsResponses ?? new`, blank hashIds skipped, all four area methods return empty lists (never null). DI lifetimes consistent (scoped -> scoped, no captive dependency). No `Task.WhenAll`, so the shared scoped `NpgsqlConnection` is never used concurrently.

No Critical findings. No High findings.

## Medium

### M1. Header dump (incl. Authorization/Cookie) now logged up to 5x per request
`HomeFeedAggregationService.cs:37` (`new BaseR(hc)`) and `:72` (`new PaginatedR(hc, hashIds)` inside `HydrateType`). Every `BaseR`/`PaginatedR` ctor calls `LogHeader()` (`Common/Mcsg.Common.Core/Requests/BaseR.cs:153-164`), which serialises all request headers (Authorization bearer, Cookie) to Info log. Pre-existing behaviour, but the old flow logged once per HTTP request; this endpoint emits 1 + N(types) dumps for a single hit. Fix without touching Common: build one `PaginatedR` before the loop and assign `req.HashIds` per type (settable property) -> 2 dumps instead of 5. Longer-term: redact `Authorization`/`Cookie` in `LogHeader` (out of scope, flagged).

### M2. Eager DI graph on every `api/social/post/*` request
`PostController.cs:28` injects `IHomeFeedAggregationService` into the controller ctor. That service's ctor (`HomeFeedAggregationService.cs:19-25`) resolves Story, Comic, Document `PostService` (10-11 ctor deps each) plus `FeedService` (8 deps). All ~10 other actions on this controller (tag lists, `SyncToAna`, `PostHideUpdate`, ...) now pay that construction cost for nothing. No lifetime bug, only overhead/coupling. Fix: `GetLatestPostsByTypeWithDetail([FromServices] IHomeFeedAggregationService svc)` and drop the ctor field.

### M3. Silent degradation: broken shared connection or client cancel yield HTTP 200 with all-null Data
`HomeFeedAggregationService.cs:93-96`. Two consequences of the accepted per-type swallow:
1. All four areas share one scoped `IDbConnection`. If area #1 fails mid-read (timeout, broken pipe) the connection may be unusable, so areas #2-#4 also throw, get swallowed, and the client receives 200 with every `Data = null`. Client cannot distinguish "post hidden/deleted" from "backend down"; monitoring sees only error logs, not error rates on this route.
2. `catch (Exception)` also swallows `OperationCanceledException` when the client disconnects, producing `LogError` noise.
Recommendation (keeps the design): add `catch (OperationCanceledException) { throw; }` before the generic catch, and expose `List<PostType> FailedTypes` (or `bool Partial`) on `LatestPostsDetailResponse`. Cheap, does not change the null-Data contract.

### M4. Cost amplification without cache/limit
One anonymous GET now triggers 1 gRPC call + ~4 x (box query + reactions + comments + mentions/profiles) ~= 15-20 DB round trips. Total work equals the old 5-call flow, but it now sits behind a single cheap request, so any per-endpoint rate limiting/caching that applied to the old calls no longer bounds it. Ranking window is per-UTC-day (`Social/PostService.cs:157-158`) but personalised by `UserId`/`IsPremium`/`Hides`, so a response cache keyed on those (or a short anonymous-only cache) would be safe. Not blocking; measure first.

## Low

### L1. Dictionary keyed by hashId only, not (type, hashId)
`HomeFeedAggregationService.cs:41, 58, 103`. HashIds are 12-char random strings generated independently per area (`Story/PostService.cs:117`, `PostConfig.HashLength = 12`), so a Story and a Comic post can in principle share a hashId; the later-hydrated type would overwrite the earlier box and the wrong `Data` shape would be returned under the wrong `Type`. Probability negligible today; fix is one line: `Dictionary<(PostType, string), object>` + `details.GetValueOrDefault((p.Type, p.HashId))`.

### L2. `TotalItems` vs actual hydrated count
`LatestPostsDetailResponse.TotalItems` is Analytic's `TotalRecords`; `Items` may be shorter and any item may have `Data == null` (hidden, deleted, not public, or area failure). Same as old flow; add a one-line XML doc note so the client filters `data == null` rather than reserving slots.

### L3. Analytic outage surfaces as HTTP 400 with internal message
`Social/PostService.cs:190` wraps every failure in `BadRequestException(QuerySyntaxWrong, ex.Message)`; middleware maps it to 400 and the gRPC error text reaches the client. Pre-existing on the old endpoint, inherited here. Flag only.

### L4. `PostType.None` / `PostType.All`
Fall to `default:` and log a warning (`HomeFeedAggregationService.cs:88-90`). Acceptable; loud if Analytic ever emits these.

## Positive
- Sequential awaits respect the shared scoped `NpgsqlConnection`/`McsgContext`; concurrency risk correctly avoided.
- Zero user-controlled input beyond headers; hashIds originate from Analytic and are parameterised.
- Per-area `Hides`/status/permission filtering unchanged (runs inside existing area SQL).
- Type aliases (`StoryPost`, `ComicPost`, `DocumentPost`) keep the three identically named `IPostService` interfaces readable.
- Old endpoint retained; response additive, no breaking change. File sizes within the 200-line guideline.

## Recommended actions (priority order)
1. M2: move `IHomeFeedAggregationService` to `[FromServices]` on the action.
2. M3: rethrow `OperationCanceledException`; add `FailedTypes`/`Partial` to the response.
3. M1: build `PaginatedR` once, set `HashIds` per type.
4. L1: key details by `(PostType, HashId)`.
5. M4: measure p95 and query count in staging before client switch; consider anonymous output cache.

## Unresolved questions
- Is there an existing rate-limit or output-cache policy on `api/social/post/*` that the new route should opt into?
- Does the frontend need `FailedTypes`, or is "null Data == skip" sufficient for the first release?

**Status:** DONE_WITH_CONCERNS
**Summary:** Implementation is correct and preserves the per-area auth/header/hide semantics; no blocking issues. Concerns are operational: 5x header (incl. Authorization) log dumps per request, eager construction of four heavy area services on every Social post action, and silent 200-with-null-Data when the shared connection breaks.
