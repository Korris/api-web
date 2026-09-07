# Planner Report: Consolidate 6 Services into 1 Monolithic API

**Date:** 2026-09-05
**Plan:** `plans/260905-0954-consolidate-monolith/`

## Summary

Created comprehensive 6-phase implementation plan to merge Comic, Story, Document, Social, Identity, and Realtime APIs into a single `Mcsg.Api` project. Function.Job, Media.Tool, and OpenId.Mvc remain separate.

## Key Findings

1. **Comic/Story/Document/Social are ~95% identical** in structure, DI, and middleware. Only differ by `_prefix`, domain-specific service (IComicService vs IStoryService), media extension config, and favorite validator generic type.

2. **Routing mechanism understood:** `UseApiPathRewrite` middleware strips `/api/{serviceName}/` prefix. In merged API, replace with explicit `[Route("api/{domain}/[controller]")]` on each controller. This preserves all existing API paths.

3. **10 controller names duplicated** across 4 content services (CommentController, FavoriteController, FeedController, etc.). Resolved via C# namespaces: `Mcsg.Api.Areas.Comic.Controllers.CommentController` vs `Mcsg.Api.Areas.Story.Controllers.CommentController`.

4. **Identity is architecturally distinct** -- has OpenIddict server, ASP.NET Identity, gRPC server (3 services), cookie auth.

5. **Realtime is architecturally distinct** -- has SignalR (3 hubs), Firebase push notifications, Redis store.

6. **All 6 share same PostgreSQL DB** via `AddDataLibrary(cs)`. gRPC is used as client to external Analytic service, and as server only in Identity.

## Architecture Decision

**Domain folders with route prefixes** (not ASP.NET Areas convention). Each domain gets `Areas/{Domain}/` folder with its own Controllers, Services, Commands, Queries, etc. Route prefixes (`api/comic/`, `api/story/`, etc.) on controller attributes replace the path-rewrite middleware.

## Phases

| Phase | Description | Effort |
|-------|-------------|--------|
| 1. Analysis | Current state documented | 2h |
| 2. Project Structure | New folder/namespace design | 4h |
| 3. Merge Controllers | Move 66 controllers + supporting code, update routes/namespaces | 16h |
| 4. Merge DI & Middleware | Unified Program.cs, Setting.cs, domain extension methods | 8h |
| 5. Docker & K8s | Single Dockerfile, K8s deployment, Jenkinsfile, Ingress | 4h |
| 6. Testing | Route compat, DI smoke, integration, performance | 6h |
| **Total** | | **40h** |

## Risks

- Mobile app route breakage (mitigated by exact route prefix matching)
- DI conflicts from same interface names (mitigated by distinct namespaces)
- MediatR/AutoMapper profile collisions (needs verification during implementation)
- Blast radius increase (1 pod failure = all APIs down, mitigated by health checks + rolling deploys)
- SignalR multi-replica needs Redis backplane

## Unresolved Questions

1. Do mobile apps use any non-standard routes beyond `/api/{service}/...`?
2. Does Realtime.Api require sticky sessions for SignalR with multiple replicas?
3. Are there inter-service HTTP calls that would become in-process calls?
4. What is the current replica count per service? (sizing for merged pod)
