# TapShow post type (interactive branching visual story)

**Status:** In progress | **Created:** 2026-09-22 | **Branch:** release

## Goal
New post type `TapShow` = post → chapters → segments (image + narration) → choices (branching graph, multiple endings).
Cloned from the Game area (`Mcsg.Api/Areas/Game`, trimmed Comic copy). Schema `tapshow`, routes `api/tapshow/...`.

## Data model (schema `tapshow`)
| Table | Base | Extra |
|---|---|---|
| `TapShowPosts` | BasePost | `PostType.TapShow = 7` |
| `TapShowChapters` | BaseSubPost | Title, HashId, Order (float), Status (Draft/Public) |
| `TapShowCharacters` | AuditableEntity | PostId, Name, AvatarUrl?, Order (added 2026-09-22 pm) |
| `TapShowSegments` | AuditableEntity | ChapterId, CharacterId?, Title?, ImageUrl?, Narration, Order (float) |
| `TapShowSegmentChoices` | AuditableEntity | SegmentId (from), TargetSegmentId (to, same chapter), Label, Order |
| `TapShowResources` | BaseResource | PostId?, SegmentId? (temp row IsDelete=true until attached) |
| `TapShowPostComments` / `TapShowPostReactions` / `TapShowPostCommentReactions` | Game counterparts | post-level only |

Rules: start segment = lowest Order in chapter; ending = segment with 0 choices; choices replaced as a whole per segment.

## Phases
| # | Phase | Status |
|---|---|---|
| 1 | Domain: entities, configs, DbContext, User navs, enums, MinioFolder | done |
| 2 | Migration: `dotnet ef migrations add` → merge into InitData + Designer + snapshot, idempotent SQL script | done |
| 3 | API area `Mcsg.Api/Areas/TapShow`: file upload, post CRUD/list, chapter CRUD, segment CRUD + choices, comments, reactions | done |
| 4 | Wiring: Program.cs, ConfigController size keys (none new), docs/tapshow-area.md | done |
| 5 | Build, code review, fix | done |

## Endpoints (`api/tapshow/...`)
- `file/upload-media` POST (image) · `file/upload-audio` POST (segment voice-over)
- `tapshow` POST / PUT {hashId} / DELETE {hashId} / GET {hashId} / GET list / GET my
- `chapter` GET post/{postHashId} / GET {hashId} (segments + choices graph) / POST / PUT {hashId} / DELETE {hashId}
- `segment` POST / PUT {id} / DELETE {id} / PUT {id}/choices (replace outgoing choices)
- `character` GET post/{postHashId} / POST / PUT {id} / DELETE {id}
- `comment`, `reaction`: same as Game

## Out of scope (YAGNI)
Chapter-level comments/reactions, favorites, tags, reports, home feed aggregation, view counting, cross-chapter branching.
