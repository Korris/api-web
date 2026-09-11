---
name: mobile-port-dapper-column-naming
description: Porting api-mobile endpoints into api-web — mobile `fm_*` Postgres functions return column names matching the MOBILE DTOs, which differ from web DTO/mapping names; reusing web FeedService mappers on fm_ rows breaks
metadata:
  type: project
---

When a web port calls a mobile DB function (`social.fm_*`) and reuses web `FeedsListQueryDbDto` + `FeedService.MappingFeedInListRespone`, check column names first.

**Why:** Mobile DTO/function use `TotalResources`, `SubPostResourceString`, `SubPostString`, `LatestComment`; web DTO/functions (`fw_*`) use `TotalResource`, `SubPostResourceStr`, `SubPostStr`. Dapper maps by name, and web `MappingFeedInListRespone` deserialises `SubPostStr` unguarded (throws on null). Found in the 2026-09-11 LoadFeed port review. No `.sql` source for these functions exists in either repo — must be verified via `psql \sf`.

**How to apply:** In any review of a mobile->web port that uses Dapper, diff the mobile DTO vs web DTO property names and confirm the function prefix (`fm_` mobile vs `fw_` web). Also recheck inherited mobile security gaps (author-feed branch returns Private/Premium posts; `keyword.ToLower()` inside EF lambdas with null keyword) rather than assuming parity is safe.
