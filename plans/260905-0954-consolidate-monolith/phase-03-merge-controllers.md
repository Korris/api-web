# Phase 3: Controller Consolidation with Routing

## Priority: P1 | Status: pending

## Overview

Move all controllers into domain folders and update route attributes to preserve backward-compatible API paths.

## Key Insight: How Routing Works Today

```
Mobile app calls:  GET https://api.focfoc.com/api/comic/comment/123
K8s Ingress:       routes /api/comic/* -> comic-service:8080
Comic service:     UseApiPathRewrite("comic") strips "/api/comic/" prefix
Controller:        [Route("[controller]")] matches "/comment/123"
```

After merge, there's no Ingress-level service routing. The single API must handle all paths directly.

## Routing Strategy

### Option chosen: Explicit route prefix per controller

Change every controller's `[Route("[controller]")]` to include its domain prefix:

```csharp
// Before (in Mcsg.Comic.Api)
namespace Mcsg.Comic.Api.Controllers;
[Route("[controller]")]
public class CommentController : ControllerBase { }

// After (in Mcsg.Api)
namespace Mcsg.Api.Areas.Comic.Controllers;
[Route("api/comic/[controller]")]
public class CommentController : ControllerBase { }
```

This means:
- `GET /api/comic/comment/123` -> `Mcsg.Api.Areas.Comic.Controllers.CommentController`
- `GET /api/story/comment/123` -> `Mcsg.Api.Areas.Story.Controllers.CommentController`
- `GET /api/identity/authentication/login` -> `Mcsg.Api.Areas.Identity.Controllers.AuthenticationController`

### Remove UseApiPathRewrite middleware

No longer needed. The route prefix on each controller directly matches the full path.

## Implementation Steps

### Step 1: Create domain controller folders

```
Mcsg.Api/Areas/Comic/Controllers/
Mcsg.Api/Areas/Story/Controllers/
Mcsg.Api/Areas/Document/Controllers/
Mcsg.Api/Areas/Social/Controllers/
Mcsg.Api/Areas/Identity/Controllers/
Mcsg.Api/Areas/Realtime/Controllers/
```

### Step 2: Copy controllers per domain

For each of the 6 services, copy all controller files into the corresponding `Areas/{domain}/Controllers/` folder.

### Step 3: Update namespaces

Find and replace in each domain's controllers:

| Old Namespace | New Namespace |
|---------------|---------------|
| `Mcsg.Comic.Api.Controllers` | `Mcsg.Api.Areas.Comic.Controllers` |
| `Mcsg.Story.Api.Controllers` | `Mcsg.Api.Areas.Story.Controllers` |
| `Mcsg.Document.Api.Controllers` | `Mcsg.Api.Areas.Document.Controllers` |
| `Mcsg.Social.Api.Controllers` | `Mcsg.Api.Areas.Social.Controllers` |
| `Mcsg.Identity.Api.Controllers` | `Mcsg.Api.Areas.Identity.Controllers` |
| `Mcsg.Realtime.Api.Controllers` | `Mcsg.Api.Areas.Realtime.Controllers` |

Also update `using` statements for services, DTOs, commands, queries, etc.:
- `using Mcsg.Comic.Api.Services;` -> `using Mcsg.Api.Areas.Comic.Services;`
- `using Interfaces;` -> `using Mcsg.Api.Areas.Comic.Interfaces;` (implicit usings need to become explicit)

### Step 4: Update route attributes

For every controller, change:

```csharp
[Route("[controller]")]
// to
[Route("api/comic/[controller]")]  // or story, document, social, identity, realtime
```

For controllers with `[Route("[controller]"), Authorize]`, change to:
```csharp
[Route("api/comic/[controller]"), Authorize]
```

### Step 5: Handle edge cases

**Identity controllers that have non-standard routes:**
- Check if `AuthenticationController` has action-level routes like `[HttpPost("login")]` -- these remain unchanged since they're relative to the controller route.
- OpenIddict endpoints (`/connect/token`, `/connect/authorize`, `/connect/userinfo`) are mapped separately in middleware, NOT via controllers. These stay as-is.

**Realtime HomeController:**
- Currently at `/api/realtime/home`. Verify if any client depends on this path.

**Social NotificationController vs Realtime NotificationController:**
- These are in different namespaces now, routed to `/api/social/notification` and `/api/realtime/notification` respectively. No conflict.

### Step 6: Verify no action-level route conflicts

Within each domain, all controllers have unique names (CommentController, FavoriteController, etc.), so action routes can't conflict within a domain. Cross-domain, the prefix differentiates them.

## Controller Migration Checklist

### Comic (13 controllers)
- [ ] ComicController -> `[Route("api/comic/[controller]")]`
- [ ] CommentController -> `[Route("api/comic/[controller]")]`
- [ ] FavoriteController -> `[Route("api/comic/[controller]")]`
- [ ] FeedController -> `[Route("api/comic/[controller]")]`
- [ ] FileController -> `[Route("api/comic/[controller]")]`, keep `Authorize`
- [ ] LinkPreviewController -> `[Route("api/comic/[controller]")]`
- [ ] PostController -> `[Route("api/comic/[controller]")]`
- [ ] ReactionController -> `[Route("api/comic/[controller]")]`
- [ ] ReportController -> `[Route("api/comic/[controller]")]`
- [ ] SmartLookupController -> `[Route("api/comic/[controller]")]`
- [ ] SoundController -> `[Route("api/comic/[controller]")]`
- [ ] SubPostController -> `[Route("api/comic/[controller]")]`
- [ ] TagController -> `[Route("api/comic/[controller]")]`

### Story (13 controllers)
- [ ] StoryController -> `[Route("api/story/[controller]")]`
- [ ] CommentController -> `[Route("api/story/[controller]")]`
- [ ] FavoriteController, FeedController, FileController, LinkPreviewController, PostController, ReactionController, ReportController, SmartLookupController, SoundController, SubPostController, TagController -- same pattern

### Document (14 controllers)
- [ ] DocumentController -> `[Route("api/document/[controller]")]`
- [ ] ResourceController -> `[Route("api/document/[controller]")]`
- [ ] All shared controllers -- same pattern

### Social (14 controllers)
- [ ] ChartController -> `[Route("api/social/[controller]")]`
- [ ] NotificationController -> `[Route("api/social/[controller]")]`
- [ ] All shared controllers -- same pattern

### Identity (9 controllers)
- [ ] AuthenticationController -> `[Route("api/identity/[controller]")]`
- [ ] ConfigController -> `[Route("api/identity/[controller]")]`
- [ ] FeedbackController -> `[Route("api/identity/[controller]")]`
- [ ] RatingController -> `[Route("api/identity/[controller]")]`
- [ ] UserController -> `[Route("api/identity/[controller]")]`
- [ ] UserAuthenticatorController -> `[Route("api/identity/[controller]")]`
- [ ] UserRecoveryController -> `[Route("api/identity/[controller]")]`
- [ ] UserReferralController -> `[Route("api/identity/[controller]")]`
- [ ] VerificationController -> `[Route("api/identity/[controller]")]`

### Realtime (3 controllers)
- [ ] AuthenticationController -> `[Route("api/realtime/[controller]")]`
- [ ] HomeController -> `[Route("api/realtime/[controller]")]`
- [ ] NotificationController -> `[Route("api/realtime/[controller]")]`

## Supporting Code Migration

Controllers depend on services, commands, queries, DTOs. For each domain, ALL supporting code must also be moved and have namespaces updated:

| Folder | Namespace Pattern |
|--------|-------------------|
| Commands/ | `Mcsg.Api.Areas.{Domain}.Commands` |
| Queries/ | `Mcsg.Api.Areas.{Domain}.Queries` |
| Services/ | `Mcsg.Api.Areas.{Domain}.Services` |
| Interfaces/ | `Mcsg.Api.Areas.{Domain}.Interfaces` |
| Dtos/ | `Mcsg.Api.Areas.{Domain}.Dtos` |
| Models/ | `Mcsg.Api.Areas.{Domain}.Models` |
| Requests/ | `Mcsg.Api.Areas.{Domain}.Requests` |
| Validators/ | `Mcsg.Api.Areas.{Domain}.Validators` |
| Mappings/ | `Mcsg.Api.Areas.{Domain}.Mappings` |
| Filters/ | `Mcsg.Api.Areas.{Domain}.Filters` |
| Attributes/ | `Mcsg.Api.Areas.{Domain}.Attributes` |
| Constants/ | `Mcsg.Api.Areas.{Domain}.Constants` |
| Extensions/ | `Mcsg.Api.Areas.{Domain}.Extensions` |

## Automation Script

Given ~66 controllers and hundreds of supporting files, manual migration is error-prone. Create a PowerShell/bash script:

```bash
# For each domain:
# 1. Create target directories
# 2. Copy files from old service folder
# 3. sed/replace namespace: Mcsg.{Domain}.Api -> Mcsg.Api.Areas.{Domain}
# 4. Update [Route("[controller]")] -> [Route("api/{domain}/[controller]")]
```

## Success Criteria

- [ ] All 66 controllers compile in single project
- [ ] No route collisions (verified via Swagger)
- [ ] All existing API paths still work (test with curl/Postman)
- [ ] No namespace conflicts

## Risk

- **Implicit using statements** -- old projects may rely on `<ImplicitUsings>enable</ImplicitUsings>` and project-level usings. Moving files may break implicit resolution. Fix: add explicit `using` statements where needed.
- **Internal references** -- if Comic's PostService calls Story's API internally (via HTTP), those calls now become direct method calls. Review and simplify.
