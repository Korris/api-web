# Phase 4: DI and Middleware Consolidation

## Priority: P1 | Status: pending

## Overview

Merge 6 Program.cs files into 1, consolidating DI registrations, middleware pipeline, and configuration loading.

## Key Insight

All 6 services follow the same Program.cs template with ~90% identical code. The merge is mostly additive -- register all services from all domains in one builder.

## Unified Program.cs Design

### Structure

```csharp
namespace Mcsg.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // 1. Common setup (same across all)
        ConfigureCommon(builder);

        // 2. Domain-specific DI (extracted to extension methods)
        builder.Services.AddComicDomain(config, setting);
        builder.Services.AddStoryDomain(config, setting);
        builder.Services.AddDocumentDomain(config, setting);
        builder.Services.AddSocialDomain(config, setting);
        builder.Services.AddIdentityDomain(config, setting);
        builder.Services.AddRealtimeDomain(config, setting);

        // 3. Build and configure middleware
        var app = builder.Build();
        ConfigureMiddleware(app, setting);
        app.Run();
    }
}
```

### Common Setup (do once, not 6 times)

These are identical across all services -- register once:

```csharp
// Npgsql timestamp fix
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Serilog
builder.Host.UseSerilog();

// DbContext (shared DB)
builder.Services.AddDataLibrary(connectionString);

// Authentication
builder.Services.AddBearerAuthentication(setting.Jwt);
builder.Services.AddResponseCaching();

// Routing
builder.Services.AddRouting(options => options.LowercaseUrls = true);

// Controllers with JSON options
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new IsoDateTimeConverter());
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerDocumentation();

// Infrastructure
builder.Services.AddHttpContextAccessor();
builder.Services.AddEmailSender();
builder.Services.AddHealthChecks();
```

### Domain Extension Methods

Each domain registers its specific services in an extension method:

#### `AddComicDomain()`
```csharp
public static class ComicServiceCollectionExtensions
{
    public static IServiceCollection AddComicDomain(
        this IServiceCollection services, IConfiguration config, Setting setting)
    {
        services.AddScoped<Comic.Interfaces.IFileService, Comic.Services.FileService>();
        services.AddScoped<Comic.Interfaces.IJobService, Comic.Services.JobService>();
        services.AddScoped<Comic.Interfaces.IPostService, Comic.Services.PostService>();
        // ... all Comic-specific registrations

        services.AddAutoMapper(typeof(ComicServiceCollectionExtensions).Assembly);

        services.AddMediatR(p =>
        {
            p.RegisterServicesFromAssemblyContaining<ComicServiceCollectionExtensions>();
            p.AddDiPost();
            p.AddDiReport();
            p.AddDiSubPost();
        });

        return services;
    }
}
```

**Repeat pattern for Story, Document, Social.**

#### `AddIdentityDomain()` -- unique registrations
```csharp
public static class IdentityServiceCollectionExtensions
{
    public static IServiceCollection AddIdentityDomain(
        this IServiceCollection services, IConfiguration config, Setting setting)
    {
        // OpenIddict
        services.AddOpenIddict()
            .AddCore(p => p.UseEntityFrameworkCore().UseDbContext<McsgContext>())
            .AddServer(p => { /* token/authorize endpoints */ });

        // ASP.NET Identity
        services.AddIdentity<LocalizeIdentityErrorDescriber>();

        // Security
        services.AddSingleton<ISecurityAes>(p => new SecurityAes(setting.EncryptKey));
        services.AddScoped<IUserNameUniquenessChecker, UserNameUniquenessChecker>();

        // gRPC server
        services.AddGrpc();

        // Services
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddSSOService();
        services.AddScoped<IOtpService, OtpService>();

        // MediatR
        services.AddMediatR(p =>
        {
            p.RegisterServicesFromAssemblyContaining<IdentityServiceCollectionExtensions>();
            p.AddDiUserAuthenticator();
            p.AddDiUserRecovery();
            // ...
        });

        return services;
    }
}
```

#### `AddRealtimeDomain()` -- unique registrations
```csharp
public static class RealtimeServiceCollectionExtensions
{
    public static IServiceCollection AddRealtimeDomain(
        this IServiceCollection services, IConfiguration config, Setting setting)
    {
        // Firebase
        services.AddNotification(p => { p.CredentialPath = credentialPath; });

        // Redis
        services.AddSingleton<IRedisStore>(p => new RedisStore(setting.Redis));

        // SignalR
        services.AddSignalR();

        // Domain comment/reply services
        services.AddScoped<IComicCommentService, ComicCommentService>();
        services.AddScoped<IDocumentCommentService, DocumentCommentService>();
        services.AddScoped<ISocialCommentService, SocialCommentService>();
        services.AddScoped<IStoryCommentService, StoryCommentService>();
        // ... reply services, notification services, follow services

        return services;
    }
}
```

## DI Conflict Resolution

### Problem: Same interface names in different namespaces

Comic, Story, Document, Social all have:
- `IFileService` / `FileService`
- `IPostService` / `PostService`
- `ICommentService` / `CommentService`
- `IFeedService` / `FeedService`
- etc.

These are in different namespaces (`Mcsg.Api.Areas.Comic.Interfaces.IFileService` vs `Mcsg.Api.Areas.Story.Interfaces.IFileService`), so they're different types in C#. DI registers them all -- no conflict.

**But controllers must inject the correct one.** Since each controller is in its own namespace and uses its domain's interfaces, the `using` statements resolve correctly:

```csharp
// In Mcsg.Api.Areas.Comic.Controllers.FileController
using Mcsg.Api.Areas.Comic.Interfaces; // IFileService resolves to Comic's

public class FileController(IFileService fileService) : ControllerBase { }
```

### Problem: Singleton ISetting

Currently each service registers `ISetting` -> its own `Setting`. In merged API, there's one unified `Setting` registered once as `ISetting`.

All domain code that injects `ISetting` will get the unified setting. This works because the unified Setting has all fields from all domains.

### Problem: MediatR handler scanning

MediatR must scan ALL domain assemblies. Since everything is in one assembly now:

```csharp
services.AddMediatR(p =>
{
    p.RegisterServicesFromAssembly(typeof(Program).Assembly);
    // All Di extensions from all domains
    p.AddDiPost();       // Comic + Story + Document + Social
    p.AddDiReport();
    p.AddDiSubPost();
    p.AddDiPostFavorite();  // Social
    p.AddDiTag();           // Social
    p.AddDiUserAuthenticator();  // Identity
    p.AddDiUserRecovery();
    p.AddDiUserReferral();
    p.AddDiUser();
    p.AddDiFeedback();
    p.AddDiRating();
    p.AddDiVerification();
    p.AddDiAuthentication();  // Realtime
});
```

**Risk:** If `AddDiPost()` registers a pipeline behavior by type name and Comic/Story/Document/Social all call it, verify these don't collide. Likely they register different handler types per namespace, so no issue.

### Problem: AutoMapper profiles

Multiple domains register AutoMapper. Since all profiles are in one assembly:

```csharp
services.AddAutoMapper(typeof(Program).Assembly);
```

Scans all `Profile` subclasses in the assembly. If two domains define a mapping for the same source->dest types, there'll be a conflict. Review AutoMapper profiles for overlapping mappings.

## Middleware Pipeline

### Unified pipeline (do once)

```csharp
// Swagger
if (app.Environment.IsDevelopment() || setting.SwaggerEnabled)
{
    app.UseSwagger(/* conditional RouteTemplate */);
    app.UseSwaggerUI();
}

// CORS
app.UseCors(p => p.AllowAnyHeader().AllowAnyMethod()
    .WithOrigins(origins).AllowCredentials());

// HTTPS
if (!setting.IsLocal) app.UseHttpsRedirection();

// NO MORE UseApiPathRewrite -- routes are explicit now

app.UseRouting();
app.UseMiddleware<ResponseExceptionWrapperMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");
app.UseResponseCaching();

// SignalR hubs
app.MapHub<CommentHub>("/commentHub");
app.MapHub<NotificationHub>("/notificationHub");
app.MapHub<FollowHub>("/followHub");

// gRPC services (Identity)
app.MapGrpcService<Protos.Services.UserService>();
app.MapGrpcService<Protos.Services.UserAuthenticatorService>();
app.MapGrpcService<Protos.Services.UserRecoveryService>();
```

### Kestrel HTTP protocol config

Must support both HTTP/1 (REST API) and HTTP/2 (gRPC) on appropriate ports. Combine the protocol config from all services.

## Configuration Merging

### appsettings.json

Merge all 6 services' `appsettings.json`. Sections to combine:
- `ConnectionStrings:McsgConnectionString` -- same across all, keep one
- `Queue:Notification` -- merge notification queue configs
- `FeedDisplayConfigs` -- from content services
- `OtpSetting` -- from Identity

### Environment-specific configs

Merge `appsettings.Dev.json`, `appsettings.Stg.json` from all 6 services.

### Setting loading from database

Currently each service loads `SystemSettings` and `SystemConfigs` from DB. Do this once in merged Program.cs, loading all domain-specific settings:

```csharp
// Load all system settings once
if (set.TryGetValue("XApiKey", out var ett)) Setting.XApiKey = ...;
// Identity settings
if (set.TryGetValue(nameof(st.AccountDeletedAfter), out ett)) ...;
// Social settings
if (set.TryGetValue(nameof(st.PercentFeed), out ett)) ...;
```

## Implementation Steps

1. [ ] Create unified `Setting.cs` merging all 6 Setting classes
2. [ ] Create unified `ISetting` interface (if not already in Common)
3. [ ] Create `ComicServiceCollectionExtensions.cs`
4. [ ] Create `StoryServiceCollectionExtensions.cs`
5. [ ] Create `DocumentServiceCollectionExtensions.cs`
6. [ ] Create `SocialServiceCollectionExtensions.cs`
7. [ ] Create `IdentityServiceCollectionExtensions.cs`
8. [ ] Create `RealtimeServiceCollectionExtensions.cs`
9. [ ] Create unified `Program.cs`
10. [ ] Merge `appsettings.json` files
11. [ ] Merge `appsettings.Dev.json` files
12. [ ] Merge `appsettings.Stg.json` files
13. [ ] Remove `UseApiPathRewrite` from middleware pipeline
14. [ ] Verify MediatR handler registration (no duplicates)
15. [ ] Verify AutoMapper profile compatibility
16. [ ] Build and fix compilation errors

## Success Criteria

- [ ] Single Program.cs builds and starts
- [ ] All DI services resolve correctly
- [ ] Middleware pipeline handles all routes
- [ ] gRPC services accessible
- [ ] SignalR hubs connectable
- [ ] Swagger shows all endpoints grouped by domain

## Risk

- **MediatR handler naming conflicts** -- if two domains define `PostCreateH` (handler), they must be in different namespaces. Verify.
- **AutoMapper profile overlap** -- two domains mapping `Post -> PostDto` with different profiles. Review.
- **Static field `_mediaExtensionAllow`** -- each service sets this differently. Replace with domain-specific properties on Setting.
- **Static `Setting.DevelopmentMode`** -- set once, fine.
- **`BuildServiceProvider()` call in each service** -- this is an anti-pattern (builds a temporary container). In merged code, do it once.
