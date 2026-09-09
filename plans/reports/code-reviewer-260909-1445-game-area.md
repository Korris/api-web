# Code review: Game area (2026-09-09)

Scope: Mcsg.Api/Areas/Game/**, Common/Mcsg.Common.Domain/Entities/Games/**, InitData + snapshot merge, IStorageStrategy PutObject(contentType) overload, Program.cs wiring. Build passes; static review only (no DB / Minio locally).

## Findings and resolution
| Sev | Finding | Status |
|---|---|---|
| High | H1 game .html directly navigable on media origin; needs CSP sandbox header + separate hostname at ingress/Minio | Infra. Documented in docs/game-area.md |
| High | H2 Permission stored but never enforced (Private listed publicly, Premium GameUrl exposed) | Fixed: list excludes Private; detail 404 for non-owner; GameUrl nulled for Premium unless premium/owner |
| High | H3 URL ownership check bucket-wide, not user-wide | Fixed: prefix is game/{UserFolder}/thumbnails or games; GameUrl must end with .html |
| Med | M1 deleted / replaced files stay public forever | Fixed: RemoveObject on delete and on URL change (best effort, logged) |
| Med | M2 no per-user upload quota | Deferred (YAGNI), noted in docs |
| Med | M3 DisableRequestSizeLimit on a 5 MB endpoint | Fixed: RequestSizeLimit / RequestFormLimits 6 MB |
| Med | M4 PageSize uncapped | Fixed: max 50 |
| Med | M5 GameUrl no max length | Fixed: validator Url.Max (column stays text) |
| Low | L1 MediaOnlyAttribute reads Files[0] not the bound File field | Fixed |
| Low | L2 thumbnail key keeps client extension, no content type | Fixed: {hash}.jpg + image/jpeg, re-encode mandatory |
| Low | L3 Permission NotNull is a no-op | Fixed: IsInEnum |
| Low | L4 AuthorName optional when not current user | Fixed: required When(!IsCurrentUserAuthor) |
| Low | L5 Hide filter not applied | Deferred (nothing sets Hide for games) |
| Low | L6 SQL script BOM | Fixed (rewritten) |
| Low | L7 UserNameHistories seed GUID churn (DataSeeder Guid.NewGuid) | Pre-existing, untouched |
| Low | L8 / L9 DRY nits, ILIKE escaping | Partially (BucketNamePublic, redundant Type filter removed); ILIKE escaping skipped |

## Unresolved questions
- Does the CORS Origins setting include the Minio public host in any env?
- Is the media PublicUrl host on the same registrable domain as the app / API cookie?
- Does the -public bucket allow anonymous ListBucket?
