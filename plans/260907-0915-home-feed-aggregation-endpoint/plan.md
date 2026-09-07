# Home feed aggregation endpoint

**Status:** Done (server side) | **Created:** 2026-09-07 | **Branch:** release

## Problem
Client home page: `social/post/latest-posts-by-type` returns ids only, then client fires 3-4 follow-up calls
(`story/post/get-post-by-list-id`, `comic/post/get-post-by-list-id`, `document/...`, `social/feed/get-feed-by-list-id`).
4 sequential round-trips -> slow first paint. All 4 areas now live in one process (`Mcsg.Api`), so hydration can be server-side.

## Solution
New endpoint `GET api/social/post/latest-posts-by-type/detail` (old endpoint kept for old clients).
1. `Social.IPostService.GetLatestPostsByType` -> ordered ids (Analytic gRPC).
2. Group hashIds by `PostType`.
3. Call in-process services: Story/Comic/Document `IPostService.GetPostDetails`, Social `IFeedService.GetFeedsByIds`.
4. Merge in original order -> `{ totalItems, items: [{ hashId, type, postId, point, data }] }`.

## Constraints
- Sequential calls (shared scoped `IDbConnection` + DbContext not thread-safe). Measure before adding per-branch scopes.
- Per-type hydration failure logged, `data` = null for that type; endpoint still returns.
- `data` is `object` (4 different box models per area); client switches on `type` as today.

## Files
- Create `Mcsg.Api/Areas/Social/Models/LatestPostsDetailResponse.cs`
- Create `Mcsg.Api/Areas/Social/Interfaces/IHomeFeedAggregationService.cs`
- Create `Mcsg.Api/Areas/Social/Services/HomeFeedAggregationService.cs`
- Modify `Mcsg.Api/Areas/Social/Controllers/PostController.cs` (new action)
- Modify `Mcsg.Api/Program.cs` (DI)

## Todo
- [x] Plan
- [x] Implement
- [x] Build `Mcsg.Api` (0 errors)
- [x] Code review (report: plans/reports/code-reviewer-260907-0915-home-feed-aggregation.md; applied M1, M2, L1 + cancellation rethrow; M3 partial-flag and M4 cache deferred until measured)
- [ ] Client switch to new endpoint (frontend repo, out of scope here)
