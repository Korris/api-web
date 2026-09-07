# Phase 1: Current State Analysis

## Priority: P1 | Status: pending

## Overview

Document the exact current state of all 6 services to be merged, identifying commonalities, differences, and conflicts.

## Service Inventory

### Prefixes & Service Names

| Service | Prefix | MicroService Enum | Path Rewrite Target |
|---------|--------|-------------------|---------------------|
| Mcsg.Comic.Api | `Cmc` | `Comic` | `/api/comic/` |
| Mcsg.Story.Api | `Sto` | `Story` | `/api/story/` |
| Mcsg.Document.Api | `Doc` | `Document` | `/api/document/` |
| Mcsg.Social.Api | `Soc` | `Social` | `/api/social/` |
| Mcsg.Identity.Api | `Ide` | `Identity` | `/api/identity/` |
| Mcsg.Realtime.Api | `Rea` | `Realtime` | `/api/realtime/` |

### Controller Duplication Matrix

Controllers that exist in multiple services (the core merge challenge):

| Controller | Comic | Story | Document | Social | Identity | Realtime |
|------------|-------|-------|----------|--------|----------|----------|
| CommentController | x | x | x | x | | |
| FavoriteController | x | x | x | x | | |
| FeedController | x | x | x | x | | |
| FileController | x | x | x | x | | |
| PostController | x | x | x | x | | |
| ReactionController | x | x | x | x | | |
| ReportController | x | x | x | x | | |
| SmartLookupController | x | x | x | x | | |
| SoundController | x | x | x | x | | |
| SubPostController | x | x | x | x | | |
| TagController | x | x | x | x | | |
| LinkPreviewController | x | x | x | x | | |
| AuthenticationController | | | | | x | x |
| NotificationController | | | | x | | x |

### Unique Controllers (no conflicts)

| Controller | Service |
|------------|---------|
| ComicController | Comic |
| StoryController | Story |
| DocumentController | Document |
| ResourceController | Document |
| ChartController | Social |
| ConfigController | Identity |
| FeedbackController | Identity |
| RatingController | Identity |
| UserController | Identity |
| UserAuthenticatorController | Identity |
| UserRecoveryController | Identity |
| UserReferralController | Identity |
| VerificationController | Identity |
| HomeController | Realtime |

### Routing Mechanism

All controllers use `[Route("[controller]")]`. The `UseApiPathRewrite` middleware strips `/api/{serviceName}/` prefix:

```
Client request: GET /api/comic/comment/123
Middleware strips: /api/comic/ -> /comment/123
Controller route: [Route("[controller]")] matches /comment
```

In merged API, we replace this with explicit route prefixes per domain.

### DI Registration Patterns

**Shared across Comic/Story/Document/Social (content services):**
- `ISetting` -> `Setting` (singleton, but each service has own Setting class!)
- `IBusinessText` -> `BusinessText`
- `AddDataLibrary(cs)` -- shared DbContext
- `MediaOnlyAttribute` (scoped)
- `IFileService`, `IJobService`, `IPostService`, `IMetaDataService`, `ITagService`, `ISoundService`, `IPostLinkService`, `ISmartLookupService`
- MediatR: `AddDiPost()`, `AddDiReport()`, `AddDiSubPost()`
- `IFeedService`, `ILinkPreviewService`, `ICommentService`
- React services: `IPostReactService`, `ISubPostReactService`, `IPostCommentReactService`, `ISubPostCommentReactService`, `IReactService<>`
- `ISmartCountService`
- `IFavoriteService` with validators

**Identity-specific DI:**
- `ISecurityAes` -> `SecurityAes` (with EncryptKey)
- `IUserNameUniquenessChecker`
- `ISecurityService`, `IAuthenticationService`, `IUserService`, `ITokenService`
- SSO services
- `IOtpService`
- OpenIddict (core + server)
- ASP.NET Identity with `LocalizeIdentityErrorDescriber`
- MediatR: `AddDiUserAuthenticator()`, `AddDiUserRecovery()`, `AddDiUserReferral()`, `AddDiUser()`, `AddDiFeedback()`, `AddDiRating()`, `AddDiVerification()`
- No AutoMapper, no JSON options, no file upload limits

**Realtime-specific DI:**
- Firebase notification service with env-specific credential file
- Redis store
- SignalR
- Domain-specific comment/reply services: `IComicCommentService`, `IDocumentCommentService`, `ISocialCommentService`, `IStoryCommentService` (and reply equivalents)
- `INotificationService`, `IStoryNotificationService`, `IComicNotificationService`
- `IMentionService`, `IFollowService`, `IFollowPostService`
- MediatR: `AddDiAuthentication()`

**Social-specific extras:**
- `AddGrpc()` -- serves as gRPC server? No, it only uses gRPC clients to Analytic service
- `IUserService`, `IChartService`, `INotificationService`
- `AddDiPostFavorite()`, `AddDiTag()` -- extra MediatR pipelines
- Extra system settings: PercentFeed, PercentComic, PercentDocument, PercentStory, NumberOfPosts

### gRPC Usage

**As Server (Identity only):**
- `UserService`, `UserAuthenticatorService`, `UserRecoveryService`
- Proto files in `Protos/Servers/`

**As Client (all content services + Identity):**
- Call external Analytic service via `GrpcChannel.ForAddress(_setting.Rpc.Analytic.Analytic!)`
- Proto files in `Protos/Clients/` (comic.proto, user.proto, wallet.proto, analytic.proto, etc.)

### SignalR Hubs (Realtime only)

- `CommentHub` at `/commentHub`
- `NotificationHub` at `/notificationHub`
- `FollowHub` at `/followHub`

### Middleware Pipeline (identical across all 6)

1. Swagger (conditional)
2. CORS
3. HTTPS Redirect (non-local)
4. `UseApiPathRewrite` (service-specific)
5. `UseRouting`
6. `ResponseExceptionWrapperMiddleware`
7. `UseAuthentication`
8. `UseAuthorization`
9. `MapControllers`
10. `MapHealthChecks("/health")`
11. `UseResponseCaching`

### Setting Class Conflicts

Each service defines `Setting : SettingBase, ISetting` in its own namespace. Content services (Comic/Story/Document/Social) are nearly identical with queue settings. Identity has unique fields (ReCaptchaSecretKey, EncryptKey, OTP settings). Realtime has Redis and Firebase configs.

**Resolution needed: Single unified Setting class.**

### Environment Variable Loading

Each service loads settings via `_prefix.ConvertEnvironmentVariable<Setting>(CommonPrefix)` where prefix differs (Cmc, Sto, Doc, Soc, Ide, Rea). The merged service needs to load ALL prefixed env vars.

## Key Findings

1. **Comic/Story/Document/Social are ~95% identical** in Program.cs structure, DI, and middleware. The only differences: the `_prefix`, the media extension allow config, the domain-specific service (IComicService vs IStoryService etc.), and the favorite validator generic type.
2. **Identity is architecturally distinct** -- OpenIddict, ASP.NET Identity, gRPC server, no AutoMapper/file upload.
3. **Realtime is architecturally distinct** -- SignalR, Firebase, Redis, no file upload, domain-aware comment/reply services.
4. **All 6 share the same DB** via `AddDataLibrary(cs)` with same connection string.
5. **gRPC clients** are used ad-hoc via `GrpcChannel.ForAddress()` -- not DI-registered. No change needed.
6. **gRPC server** in Identity must continue working in merged API.

## Todo

- [x] Inventory all controllers
- [x] Map DI registrations
- [x] Identify routing mechanism
- [x] Catalog gRPC usage
- [x] Catalog SignalR hubs
- [x] Identify Setting conflicts
- [x] Document middleware pipeline
