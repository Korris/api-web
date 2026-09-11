# LoadFeed port: api-mobile -> api-web (Mcsg.Api)

Date: 2026-09-11 | Branch: release | Build: `dotnet build Mcsg.Api/Mcsg.Api.csproj` OK, 0 errors, 0 new warnings

## Routes

| Mobile (old)                                  | Web (new, same PATCH body)            |
|-----------------------------------------------|---------------------------------------|
| PATCH /api/social/post/v1/Post/LoadFeed       | PATCH /api/social/post/v1/LoadFeed    |
| PATCH /api/Comic/v1/Post/LoadFeed             | PATCH /api/comic/post/v1/LoadFeed     |
| PATCH /api/Story/v1/Post/LoadFeed             | PATCH /api/story/post/v1/LoadFeed     |
| PATCH /api/Document/v1/Post/LoadFeed          | PATCH /api/document/post/v1/LoadFeed  |

Follows existing convention (`v1/MyComicSearch`, `v1/PostHideUpdate`). Anonymous, like mobile.

## Files

New
- `Common/Mcsg.Common.Domain/Dtos/CommentLateDto.cs` (shared latest-comment DTO)
- Comic/Story/Document (each): `Dtos/PostFeedDto.cs`, `Extensions/PostFeedExtension.cs` (EF projection + mapping), `Requests/Posts/PostLoadFeedR.cs`, `Queries/Post(s)/PostLoadFeedH.cs`, `Queries/Post(s)/PostLoadFeedH.Stats.cs` (batched reactions + comment counts). Story/Document are prefix-renamed clones of Comic.
- Social: `Requests/Posts/PostLoadFeedR.cs`, `Queries/Posts/PostLoadFeedH.cs` (Dapper `social.fm_posts_by_username` + `IFeedService` mapping)

Modified
- 4x `Controllers/PostController.cs` (action), 4x `Extensions/DiPostExtension.cs` (MediatR registration)
- Comic/Story/Document `Filters/PostFilter.cs`: `+Hashtag`
- Social `Dtos/FeedsListQueryDbDto.cs`: alias props `TotalResources`, `SubPostString`, `SubPostResourceString` (mobile fm_* column names)
- Social `Services/FeedService.cs:1084`: null-guard `SubPostStr` deserialize

## Behaviour vs mobile
- Comic/Story/Document: same filters (UserId/UserName, Keyword, Hashtag, Hides, Permission, PostHides, IsFavorite), same DTO shape as mobile `ComicPost.SearchDto`.
  Changes: reactions/comment counts batched per page; keyword hoisted (mobile evaluates `keyword.ToLower()` client-side -> NRE when Keyword omitted); `ex.Message` instead of stack trace.
- Social: response uses web `FeedDto` shape (same as other web ports): no `LatestComment`, `TotalComments`, `TotalReactions`; `TotalResource` (singular) instead of `TotalResources`.

## Code review (code-reviewer-260911-1340-loadfeed-port.md)
Fixed: C1 (column names/null deserialize), M1 (null keyword), M3, L1, L3, L4.
Kept for parity, decision needed:
- H1 Social: SQL called with `IsMyseft = true` (as mobile); `TotalRecords` counts only public/premium.
- H2 Comic/Story/Document: author filter returns author's Private/Premium/Inactive posts to anyone (mobile behaviour); `IsCensored` only flags.
- M2: DB order uses all chapters' `CreatedOn`, page re-sorted by `PublishDate` (mobile behaviour).

## Not done
- Runtime test: local Postgres (localhost:5432) down. Need one call per route against a DB that has `social.fm_posts_by_username`.
- No git commit (not requested).

## Unresolved questions
1. Output columns of `social.fm_posts_by_username` (verify with `\sf`); alias props cover both naming variants but not unknown ones.
2. Keep mobile's open author-feed semantics (H2) and hard-coded `IsMyseft` (H1) in the monolith?
3. Should Social keep `LatestComment` in the response for the mobile client?
