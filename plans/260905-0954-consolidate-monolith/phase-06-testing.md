# Phase 6: Testing Strategy

## Priority: P1 | Status: pending

## Overview

Verify the merged API is functionally equivalent to the 6 separate services. Focus on route compatibility, DI correctness, and integration testing.

## Testing Layers

### 1. Compilation Verification

First gate: the merged project must compile without errors.

```bash
cd Mcsg.Api
dotnet build
```

Expected issues to fix:
- Missing `using` statements after namespace changes
- Ambiguous type references (same class name from different domains)
- Missing NuGet packages
- Proto compilation issues

### 2. Route Compatibility Testing

**Critical:** Mobile apps depend on exact API paths. Create a route inventory test.

#### Generate route inventory from old services

Before migration, capture all routes from each service's Swagger:
```bash
# For each running service
curl http://localhost:{port}/swagger/v1/swagger.json > routes-{service}.json
```

#### Verify routes in merged API

After migration, start merged API and compare:
```bash
curl http://localhost:8080/swagger/v1/swagger.json > routes-merged.json
# Compare that all old routes exist in merged output
```

#### Automated route check script

```bash
# Test every known endpoint returns non-404
endpoints=(
  "api/comic/comic"
  "api/comic/post"
  "api/comic/comment"
  "api/comic/feed"
  "api/story/story"
  "api/story/post"
  "api/document/document"
  "api/social/feed"
  "api/social/chart"
  "api/identity/authentication"
  "api/identity/user"
  "api/realtime/notification"
  "api/realtime/home"
)
for ep in "${endpoints[@]}"; do
  status=$(curl -s -o /dev/null -w "%{http_code}" "http://localhost:8080/$ep")
  if [ "$status" == "404" ]; then
    echo "FAIL: $ep returned 404"
  fi
done
```

### 3. DI Smoke Testing

Verify all services resolve at startup. Add a startup health check that resolves critical services:

```csharp
// In Program.cs after Build()
using (var scope = app.Services.CreateScope())
{
    // Verify each domain's key services resolve
    scope.ServiceProvider.GetRequiredService<Comic.Interfaces.IPostService>();
    scope.ServiceProvider.GetRequiredService<Story.Interfaces.IPostService>();
    scope.ServiceProvider.GetRequiredService<Document.Interfaces.IPostService>();
    scope.ServiceProvider.GetRequiredService<Social.Interfaces.IFeedService>();
    scope.ServiceProvider.GetRequiredService<Identity.Interfaces.IAuthenticationService>();
    scope.ServiceProvider.GetRequiredService<Realtime.Interfaces.INotificationService>();
}
```

### 4. Integration Tests

#### API endpoint tests (per domain)

Test key endpoints for each domain with actual HTTP calls:

**Comic:**
- `GET /api/comic/feed` -- returns feed
- `POST /api/comic/post` -- creates post (auth required)
- `GET /api/comic/comment?postId={id}` -- returns comments

**Identity:**
- `POST /api/identity/authentication/login` -- login flow
- `GET /api/identity/user/profile` -- user profile (auth required)

**Realtime:**
- `GET /api/realtime/notification` -- notifications (auth required)
- WebSocket connect to `/commentHub` -- SignalR handshake

**gRPC:**
- Call `UserService.GetUser()` on port 8081

#### Use existing Test project

Check `D:/NTada/FocFocNew/api-web/Test/` for existing tests. Update project references to point to `Mcsg.Api` instead of individual service projects.

### 5. Regression Testing

#### Database operations
- Verify CRUD operations work for each domain
- Verify cross-domain queries work (e.g., Social feed pulling from Comic/Story/Document)
- Verify MediatR handlers fire correctly

#### Authentication
- JWT token validation works
- OpenIddict token endpoint responds
- Protected endpoints reject unauthenticated requests

#### File operations
- Upload flow works for each domain (Comic, Story, Document, Social)
- MinIO storage integration intact

#### Real-time features
- SignalR comment hub receives messages
- Notification hub pushes notifications
- Follow hub works

### 6. Performance Baseline

Before migration, capture baseline metrics from production:
- Response times per endpoint (P50, P95, P99)
- Memory usage per pod
- CPU usage per pod

After migration, compare merged API metrics. Expected:
- Slightly higher memory (all code loaded in one process)
- Similar or better response times (no inter-service HTTP calls)
- Lower total resource usage (1 pod vs 6 pods)

## Test Execution Plan

| Step | Test | Env | Blocking? |
|------|------|-----|-----------|
| 1 | Compilation | Local | Yes |
| 2 | DI smoke test | Local | Yes |
| 3 | Route inventory comparison | Local | Yes |
| 4 | API endpoint smoke tests | Dev | Yes |
| 5 | SignalR connection test | Dev | Yes |
| 6 | gRPC service test | Dev | Yes |
| 7 | Auth flow test | Dev | Yes |
| 8 | Existing unit/integration tests | CI | Yes |
| 9 | Full regression (manual) | Staging | Yes |
| 10 | Performance baseline comparison | Staging | No |
| 11 | 24h soak test | Staging | Recommended |

## Rollback Plan

If critical issues found after production deployment:
1. Revert Ingress rules to point back to old 6 services (still running, scaled to 0)
2. Scale old services back to original replica count
3. Investigate and fix issues in merged API
4. Re-deploy merged API

Keep old service deployments at 0 replicas for at least 2 weeks after successful migration.

## Implementation Steps

1. [ ] Capture route inventory from all 6 running services
2. [ ] Write route compatibility test script
3. [ ] Add DI smoke test to Program.cs (dev mode only)
4. [ ] Update existing Test project references
5. [ ] Run compilation + fix errors iteratively
6. [ ] Run DI smoke test locally
7. [ ] Deploy to dev, run endpoint tests
8. [ ] Deploy to staging, run full regression
9. [ ] Capture performance baseline comparison
10. [ ] Sign off for production deployment

## Success Criteria

- [ ] All routes from old services accessible in merged API
- [ ] All DI services resolve without error
- [ ] SignalR hubs connect successfully
- [ ] gRPC services respond correctly
- [ ] Auth flows work end-to-end
- [ ] No performance regression >10%
- [ ] Existing tests pass

## Unresolved Questions

1. Are there any mobile app endpoints that use non-standard paths (not `/api/{service}/...`)? Need to verify with mobile team.
2. Does the Realtime service require sticky sessions for SignalR with multiple replicas? If yes, need Redis backplane.
3. Are there any inter-service HTTP calls (e.g., Comic calling Identity API via HTTP)? These would become unnecessary in-process calls.
