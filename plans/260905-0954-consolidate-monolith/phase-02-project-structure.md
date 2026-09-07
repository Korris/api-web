# Phase 2: New Project Structure Design

## Priority: P1 | Status: pending

## Overview

Design the folder structure for the unified `Mcsg.Api` project that houses all 6 merged services.

## Key Insights

- Comic/Story/Document/Social share 12 identical controller names -- must be separated by folder and namespace
- Identity and Realtime are architecturally distinct with unique concerns (OpenIddict, SignalR, Firebase)
- Each domain's services/interfaces/DTOs/commands/queries have same class names but different implementations

## Architecture: Domain Folders with Route Prefixes

Organize by **domain area** (not by layer). Each domain gets its own folder subtree with its own namespace, preventing class name collisions naturally via C# namespaces.

```
Mcsg.Api/
├── Program.cs                          # Single unified entry point
├── Mcsg.Api.csproj                     # Merged csproj
├── Setting.cs                          # Unified setting class
├── Dockerfile
├── appsettings.json                    # Merged config
├── appsettings.Dev.json
├── appsettings.Stg.json
│
├── Areas/
│   ├── Comic/
│   │   ├── Controllers/
│   │   │   ├── ComicController.cs      # [Route("api/comic/[controller]")]
│   │   │   ├── CommentController.cs    # [Route("api/comic/[controller]")]
│   │   │   ├── FavoriteController.cs
│   │   │   ├── FeedController.cs
│   │   │   ├── FileController.cs
│   │   │   ├── LinkPreviewController.cs
│   │   │   ├── PostController.cs
│   │   │   ├── ReactionController.cs
│   │   │   ├── ReportController.cs
│   │   │   ├── SmartLookupController.cs
│   │   │   ├── SoundController.cs
│   │   │   ├── SubPostController.cs
│   │   │   └── TagController.cs
│   │   ├── Commands/
│   │   ├── Queries/
│   │   ├── Services/
│   │   ├── Interfaces/
│   │   ├── Dtos/
│   │   ├── Models/
│   │   ├── Mappings/
│   │   ├── Validators/
│   │   ├── Requests/
│   │   ├── Filters/
│   │   ├── Attributes/
│   │   ├── Constants/
│   │   ├── Extensions/
│   │   │   └── ComicServiceCollectionExtensions.cs  # DI registration
│   │   └── Protos/
│   │       └── Clients/
│   │
│   ├── Story/                          # Same structure as Comic
│   │   ├── Controllers/
│   │   ├── Commands/
│   │   ├── ...
│   │
│   ├── Document/                       # Same structure as Comic
│   │   ├── Controllers/
│   │   ├── ...
│   │   └── (includes ResourceController)
│   │
│   ├── Social/                         # Same structure + ChartController, NotificationController
│   │   ├── Controllers/
│   │   ├── ...
│   │
│   ├── Identity/                       # Distinct structure
│   │   ├── Controllers/
│   │   │   ├── AuthenticationController.cs  # [Route("api/identity/[controller]")]
│   │   │   ├── ConfigController.cs
│   │   │   ├── FeedbackController.cs
│   │   │   ├── RatingController.cs
│   │   │   ├── UserController.cs
│   │   │   ├── UserAuthenticatorController.cs
│   │   │   ├── UserRecoveryController.cs
│   │   │   ├── UserReferralController.cs
│   │   │   └── VerificationController.cs
│   │   ├── Checkers/
│   │   ├── Commands/
│   │   ├── Queries/
│   │   ├── Services/
│   │   ├── Interfaces/
│   │   ├── Helpers/
│   │   ├── Extensions/
│   │   │   └── IdentityServiceCollectionExtensions.cs
│   │   └── Protos/
│   │       ├── Clients/
│   │       ├── Servers/
│   │       └── Services/
│   │
│   └── Realtime/                       # Distinct structure
│       ├── Controllers/
│       │   ├── AuthenticationController.cs  # [Route("api/realtime/[controller]")]
│       │   ├── HomeController.cs
│       │   └── NotificationController.cs
│       ├── Hubs/
│       │   ├── CommentHub.cs
│       │   ├── NotificationHub.cs
│       │   └── FollowHub.cs
│       ├── Commands/
│       ├── Services/
│       ├── Interfaces/
│       ├── Dtos/
│       ├── Mappings/
│       ├── Responses/
│       ├── Translation/
│       ├── Validators/
│       ├── Extensions/
│       │   └── RealtimeServiceCollectionExtensions.cs
│       └── firebase.*.json
│
├── Shared/                             # Cross-cutting concerns
│   ├── Extensions/                     # Shared DI extension methods
│   └── Middleware/
│
└── Properties/
    └── launchSettings.json
```

## Namespace Convention

```
Mcsg.Api.Areas.Comic.Controllers
Mcsg.Api.Areas.Comic.Services
Mcsg.Api.Areas.Story.Controllers
Mcsg.Api.Areas.Identity.Controllers
...
```

This ensures `Mcsg.Api.Areas.Comic.Controllers.CommentController` and `Mcsg.Api.Areas.Story.Controllers.CommentController` are distinct types -- no collision.

## csproj Design

Single `Mcsg.Api.csproj` that references all 4 Common libraries:

```xml
<ItemGroup>
    <ProjectReference Include="..\Common\Mcsg.Common\Mcsg.Common.csproj" />
    <ProjectReference Include="..\Common\Mcsg.Common.Core\Mcsg.Common.Core.csproj" />
    <ProjectReference Include="..\Common\Mcsg.Common.Domain\Mcsg.Common.Domain.csproj" />
    <ProjectReference Include="..\Common\Mcsg.Common.SeedWork\Mcsg.Common.SeedWork.csproj" />
</ItemGroup>
```

Must include all NuGet packages from all 6 service csproj files (merge PackageReferences, deduplicate).

## Unified Setting.cs

Merge all 6 Setting classes into one. Fields from content services are largely identical. Add Identity-specific and Realtime-specific fields:

```csharp
namespace Mcsg.Api;

public class Setting : SettingBase, ISetting
{
    // --- Shared (content services) ---
    public string NotificationExchange { get; set; }
    public string NotificationQueuePostReact { get; set; }
    public string NotificationQueueSmartLookup { get; set; }
    public string NotificationQueueSyncData { get; set; }
    public string NotificationQueueViewHistory { get; set; }

    // --- Identity-specific ---
    public string NotificationQueueEmail { get; set; }
    public string NotificationQueueSms { get; set; }
    public string ReCaptchaSecretKey { get; set; }
    public string EncryptKey { get; set; }
    // OTP settings...

    // --- Realtime-specific ---
    public string NotificationQueuePostComment { get; set; }
    public RedisDto Redis { get; set; }

    // --- Social-specific ---
    public double PercentFeed { get; set; }
    public double PercentComic { get; set; }
    public double PercentDocument { get; set; }
    public double PercentStory { get; set; }
    public int NumberOfPosts { get; set; }

    // --- Media extensions per domain ---
    public string ComicMediaExtensionAllow => Minio.ComicMediaExtensionAllow;
    public string StoryMediaExtensionAllow => Minio.StoryMediaExtensionAllow;
    public string DocumentMediaExtensionAllow => Minio.DocumentMediaExtensionAllow;
    public string SocialMediaExtensionAllow => Minio.SocialMediaExtensionAllow;
}
```

## Environment Variable Strategy

Current: each service loads env vars with its own prefix (Cmc, Sto, Doc, Soc, Ide, Rea).

**Option A (recommended):** Use a single new prefix (e.g., `Api`) and consolidate all env vars under it. Requires updating K8s ConfigMaps/Secrets once.

**Option B:** Load settings for all 6 prefixes in Program.cs and merge. More complex, keeps backward compat with existing K8s configs.

Recommend **Option A** for simplicity. The K8s config update is a one-time task.

## Files to Create

- [ ] `Mcsg.Api/Mcsg.Api.csproj`
- [ ] `Mcsg.Api/Program.cs`
- [ ] `Mcsg.Api/Setting.cs`
- [ ] `Mcsg.Api/Dockerfile`
- [ ] `Mcsg.Api/appsettings.json` (merged)
- [ ] `Mcsg.Api/Areas/{domain}/Extensions/{Domain}ServiceCollectionExtensions.cs` (6 files)

## Files to Move (from existing services)

For each domain (Comic, Story, Document, Social, Identity, Realtime):
- Move `Controllers/` -> `Areas/{domain}/Controllers/`
- Move `Commands/` -> `Areas/{domain}/Commands/`
- Move `Queries/` -> `Areas/{domain}/Queries/`
- Move `Services/` -> `Areas/{domain}/Services/`
- Move `Interfaces/` -> `Areas/{domain}/Interfaces/`
- Move `Dtos/` -> `Areas/{domain}/Dtos/`
- Move `Models/` -> `Areas/{domain}/Models/`
- Move `Mappings/` -> `Areas/{domain}/Mappings/`
- Move `Validators/` -> `Areas/{domain}/Validators/`
- Move `Requests/` -> `Areas/{domain}/Requests/`
- Move `Filters/` -> `Areas/{domain}/Filters/`
- Move `Attributes/` -> `Areas/{domain}/Attributes/`
- Move `Constants/` -> `Areas/{domain}/Constants/`
- Move `Extensions/` -> `Areas/{domain}/Extensions/`
- Move `Protos/` -> `Areas/{domain}/Protos/`

## Namespace Updates

All moved files need namespace updates:
- `Mcsg.Comic.Api.Controllers` -> `Mcsg.Api.Areas.Comic.Controllers`
- `Mcsg.Comic.Api.Services` -> `Mcsg.Api.Areas.Comic.Services`
- etc.

This is a bulk find-and-replace operation per domain.

## Solution File Update

Add `Mcsg.Api` project to `FocfocWeb.sln`. Old service projects can remain temporarily for reference but should be excluded from build.

## Success Criteria

- [ ] Single csproj compiles with all domain code
- [ ] No class name collisions
- [ ] Namespaces are clean and consistent
- [ ] All NuGet packages consolidated

## Risk

- **Large namespace refactor** -- every `using` statement in moved files needs updating. Use IDE refactoring tools or `dotnet format`.
- **Circular dependencies** -- if Comic.Services reference Story.Services, they'll now be in the same assembly. This is fine (no circular ref possible within one assembly), but review for tight coupling.
