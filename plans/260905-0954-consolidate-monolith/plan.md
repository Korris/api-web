---
title: "Consolidate 6 ASP.NET Core Services into 1 Monolithic API"
description: "Merge Comic, Story, Document, Social, Identity, Realtime APIs into single Mcsg.Api project while preserving backward-compatible routes"
status: pending
priority: P1
effort: 40h
branch: feat/consolidate-monolith
tags: [architecture, monolith, consolidation, breaking-change]
created: 2026-09-05
---

# Consolidate 6 Services into 1 Monolithic API

## Problem

6 ASP.NET Core services (Comic, Story, Document, Social, Identity, Realtime) are deployed separately but share 1 PostgreSQL database. This is a **distributed monolith** -- all downsides of microservices (operational complexity, 6 Dockerfiles, 6 K8s deployments, inter-service gRPC calls) with none of the benefits (they share a DB, so can't scale independently).

## Solution

Merge all 6 into a single `Mcsg.Api` project. Keep Function.Job, Media.Tool, and OpenId.Mvc as separate deployables.

## Key Insight: Path-Rewrite Routing

Each service currently uses `UseApiPathRewrite(serviceName)` middleware to strip `/api/{serviceName}/` prefix before routing. This means:
- Comic API: `/api/comic/comment` -> routes to `CommentController`
- Story API: `/api/story/comment` -> routes to `CommentController`
- Identity API: `/api/identity/authentication` -> routes to `AuthenticationController`

**The merged API must handle ALL prefixes** and route to the correct controller. The simplest approach: use route prefixes per domain area.

## Phases

| # | Phase | Status | Est. |
|---|-------|--------|------|
| 1 | [Analysis](phase-01-analysis.md) | pending | 2h |
| 2 | [Project Structure](phase-02-project-structure.md) | pending | 4h |
| 3 | [Merge Controllers](phase-03-merge-controllers.md) | pending | 16h |
| 4 | [Merge DI & Middleware](phase-04-merge-di-middleware.md) | pending | 8h |
| 5 | [Docker & K8s](phase-05-docker-k8s.md) | pending | 4h |
| 6 | [Testing](phase-06-testing.md) | pending | 6h |

## Architecture Decision: Route Prefixes (Not Areas)

**Chosen: Route prefix per domain** -- change `[Route("[controller]")]` to `[Route("api/comic/[controller]")]` etc.

Why not ASP.NET Areas: Areas add folder structure complexity + require `[Area("comic")]` attribute + convention-based routing config. Route prefixes are simpler, more explicit, and achieve the same result.

## Dependencies

- Function.Job, Media.Tool, OpenId.Mvc continue communicating with merged API via same DB + message queues (RabbitMQ)
- gRPC: Identity.Api serves gRPC (user, authenticator, recovery). These must remain exposed from merged API
- SignalR: Realtime.Api hubs (CommentHub, NotificationHub, FollowHub) must be mapped in merged API
- OpenIddict: Identity.Api configures OpenIddict server -- must carry over

## Risk Assessment

1. **Route collisions** -- 10 duplicate controller names across services. Mitigated by domain-prefixed routes.
2. **DI conflicts** -- Same interface names (IFeedService, ICommentService etc.) in different namespaces. Must use fully-qualified registrations.
3. **Setting class conflicts** -- Each service has its own `Setting.cs` with same class name. Must merge into single unified Setting.
4. **Mobile app breakage** -- Clients depend on existing routes like `/api/comic/post`, `/api/identity/authentication`. Path-rewrite middleware must be replaced by direct route prefixes that match existing paths.
5. **gRPC port sharing** -- Identity gRPC services need HTTP/2 port alongside HTTP/1 API. Kestrel can handle both.
