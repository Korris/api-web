---
name: verify-ef-translation-without-db
description: How to empirically check whether an EF Core projection/query in this repo translates, without a database (scratch console + ToQueryString)
metadata:
  type: reference
---

To verify an EF Core query translates (instead of guessing), build a scratch console app in the session scratchpad that references
`C:\Project\FocFoc\api-web\Common\Mcsg.Common.Domain\Mcsg.Common.Domain.csproj` (net8.0), construct
`new McsgContext(new DbContextOptionsBuilder<McsgContext>().UseNpgsql("Host=localhost;Database=x;Username=x;Password=x").Options)`
(namespace `Mcsg.Common.Domain`), replicate the query on `ctx.Set<T>()`, and call `.ToQueryString()`.
Compilation errors (`ArgumentException`, "could not be translated") surface without any DB connection. ~1-2 min build.

**Why:** 2026-09-22 TapShow review — a helper reused inside a projection (`ProjectSummary(new[]{c}.AsQueryable()).First()`) compiled in C# but threw at EF query compile time; static reasoning was ambiguous, `ToQueryString()` settled it in one run.
**How to apply:** any time a review flags "is this projection translatable?" (static helper called inside `Select`, conditional subqueries, nested `ToList`), run this instead of speculating. Also handy to show reviewers the exact SQL for N+1 / index arguments.
