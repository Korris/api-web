# Code Review: startup warm-up + single-encode JPEG

Scope: uncommitted changes on `release` (Mcsg.Api monolith)
- `Mcsg.Api/Services/StartupWarmupHostedService.cs` (new, 136 LOC)
- `Mcsg.Api/Services/WarmupReadinessHealthCheck.cs` (new, 20 LOC)
- `Mcsg.Api/Program.cs` (+15/-2)
- `Common/Mcsg.Common.Core/Extensions/IFormFileExtension.cs` (+11/-19)

Build: 0 errors. Design decisions (never wedge forever, sequential, keep both endpoints) respected.

## Critical
None.

## High

### H1. Image warm-up runs synchronously inside `StartAsync` and delays Kestrel binding
`StartupWarmupHostedService.cs:35,64-79`. `WarmImagePipeline` does all its CPU work then returns `Task.CompletedTask`; `await action(ct)` on an already-completed task never yields, so `ExecuteAsync` does not return control to `BackgroundService.StartAsync` until the whole Magick.NET / System.Drawing / JIT warm-up finishes. With `WebApplicationBuilder` the `GenericWebHostService` (Kestrel) is registered last and hosted services start sequentially (`HostOptions.ServicesStartConcurrently` defaults to false), so the pod is connection-refused on `/health` AND `/health/ready` for the entire image step - exactly the cold-start seconds the change was meant to keep liveness alive for. With a tight liveness `initialDelaySeconds` this makes restarts more likely than before.

Fix: first line of `ExecuteAsync`: `await Task.Yield();` (or run the image step via `Task.Run(..., ct)`). Kestrel then binds immediately and `/health/ready` answers "warming up" instead of refusing connections.

### H2. No deadline on warm-up steps; readiness can stay unhealthy for minutes on a bad MinIO/DB
- `StorageMinio.cs:243` `StatObject` takes no `CancellationToken`; the MinIO client uses a default `HttpClient` (100 s timeout). A black-holed endpoint = up to 100 s per instance x 2 instances (`StartupWarmupHostedService.cs:100-104`).
- `WarmDatabase` line 90 `GetSettingDouble` has no ct, and `AddDataLibrary` enables `EnableRetryOnFailure()` (6 retries, up to ~2 min) (`Common/Mcsg.Common.Domain/Extensions/IServiceCollectionExtension.cs:82`).

Not "forever", but `kubectl rollout status` in `.gitlab-ci.yml:214` waits on readiness; a 3-5 min unready window can fail the deploy job and stalls the rollout even though the old code path would have served (degraded) traffic.

Fix: per-step `CancellationTokenSource.CreateLinkedTokenSource(stoppingToken)` with ~15 s `CancelAfter`; for `StatObject` (no ct) wrap with `Task.WhenAny(stat, Task.Delay(timeout, ct))`. Or one overall deadline: `await Task.WhenAny(RunSteps(), Task.Delay(30s))` before `MarkReady`.

## Medium

### M1. Readiness probe wiring cannot be verified from this repo
Chart lives in `repo.ntada.vn/devops/ntada-azure-infra` -> `helm_local/values-$ENV.yaml` (`.gitlab-ci.yml:192-212`). If `readinessProbe.path` there is still `/health`, the gate is inert (and H1 still applies). Action: set readiness to `/health/ready`, keep liveness on `/health`, and make sure `initialDelaySeconds` / `startupProbe` covers the sync window until H1 is fixed.

### M2. Single-encode semantics (`IFormFileExtension.cs:174-187`) - mostly parity, differences listed
- EXIF/ICC: preserved before (ImageSharp keeps Exif+Icc on save) and after. Magick additionally keeps IPTC/8BIM/XMP, so output may carry a few more KB of profiles. GPS EXIF was already passed through pre-change (pre-existing PII pass-through; optional hardening: `AutoOrient()` then `Strip()`, which would also make Width/Height the display dims).
- Orientation: neither path applies it; `Width/Height` are the stored (pre-rotation) dims in both. Parity.
- CMYK: ImageSharp 3.1 re-encodes using `JpegMetadata.ColorType` of the decoded image, so CMYK stayed CMYK before too. Parity (one manual check with a CMYK sample if ever reported).
- Alpha flatten to white unchanged.
- Chroma subsampling / progressive: ImageMagick reuses the input JPEG's sampling factor and interlace on JPEG->JPEG; ImageSharp previously normalised to baseline + 4:2:0 (<91) / 4:4:4. Output bytes now vary with input. Visually fine, size +/- a few %.
- Stream ownership: `output` MemoryStream not disposed, wrapped by `FormFile`; `OpenReadStream()` returns a per-call `ReferenceReadStream` which callers dispose (`FileService.cs:161`). Same as before; MemoryStream dispose is a no-op. OK.
- Lost second-decoder validation: ImageSharp `Load` used to prove the produced bytes were a decodable JPEG. Negligible.
- Same helper is used by Document/Social/Story areas (`Areas/*/Services/FileService.cs`) with identical `Width/Height` usage - no divergent caller assumptions.

### M3. Thumbnail path still double-encodes
`Common/Mcsg.Common.Core/Extensions/StreamExtension.cs:34-76` `ResizeImage` still does Magick JPEG -> ImageSharp decode -> ImageSharp JPEG. Only hit for `Type=Thumb`, but it is the same cost the change removed and thumbnails now have different encoder characteristics than the main image. Follow-up: apply the same single-encode there.

### M4. `WarmMinio` "ok" log is misleading
`StorageMinio.cs:255-263` swallows every exception and returns null (logs via `Console.WriteLine`), so `[WARMUP] minio ok` prints even when creds/endpoint are wrong; the inner catch at `StartupWarmupHostedService.cs:106` can only ever see `GetStrategy` `ArgumentOutOfRangeException` (fewer `Storages` entries than enum values - correctly tolerated). Cosmetic, but do not rely on that log to diagnose MinIO.

## Low
- `StartupWarmupHostedService.cs:64` `ct` parameter unused (goes away with H1's `Task.Run`).
- `IFormFileExtension.cs:4` `using SixLabors.ImageSharp.Formats.Jpeg;` now unused in this file (warning only; `GetRatio` still uses ImageSharp via full name).
- `IsGifAnimated` (`IFormFileExtension.cs:230`) uses `System.Drawing.Common`, which throws `PlatformNotSupportedException` on Linux in .NET 8 -> caught -> `false`. The warm-up "warms" an exception path; pre-existing, but it means animated-GIF detection is dead in Linux pods (see Unresolved).
- `StorageMinio.cs:395-420` lazy `Mc` build is check-then-set without a lock. Warm-up building it before traffic narrows the race; if readiness is not wired (M1), request threads and warm-up can double-build (extra HttpClient, benign). Pre-existing.

## Verified OK
- `try/finally` guarantees `MarkReady()`; step exceptions are caught; `OperationCanceledException` on shutdown propagates and is tolerated by `BackgroundService`/Host (no `StopHost` crash).
- `IStorageClient` is a singleton (`Mcsg.Common.Core/Extensions/IServiceCollectionExtension.cs:64`), so injecting into a singleton hosted service is valid; `IMcsgContext` is resolved through an `IServiceScopeFactory` scope - correct.
- `WarmupState` volatile bool: single writer, monotonic, fine.
- `HealthCheckRegistration.Tags` is `ISet<string>`; `Contains` is O(1), case-sensitive, fine. `/health` had no registered checks before; with the predicate it still evaluates zero checks and returns Healthy - no change for existing probes (apart from H1).
- `(int)magickImage.Width` cast from `uint` (Magick.NET 14.5) is safe for real image sizes.
- Warm-up 512x512 PNG exceeds the 288x432 thumb geometry, so `ResizeImage` actually resizes (path exercised, not short-circuited).

## Recommended actions (ordered)
1. Add `await Task.Yield();` (or `Task.Run`) so Kestrel binds before the image warm-up (H1).
2. Add a bounded deadline per step / overall before `MarkReady` (H2).
3. Confirm/update `readinessProbe` to `/health/ready` in `ntada-azure-infra/helm_local/values-*.yaml` (M1).
4. Follow-up: single-encode in `ResizeImage` (M3); drop unused ImageSharp using.

## Unresolved questions
- Is the container image Linux? If yes, `IsGifAnimated` has been returning `false` for every upload (System.Drawing.Common unsupported), so animated GIFs are being flattened to JPEG - intended?
- Does `values-$ENV.yaml` currently define a readiness probe at all, and what are the liveness `initialDelaySeconds` / `failureThreshold`?

**Status:** DONE_WITH_CONCERNS
**Summary:** Single-encode change is semantically safe (parity on EXIF/ICC/orientation/CMYK/Width/Height; stream ownership unchanged). Warm-up service is crash-safe and never wedges forever, but its synchronous image step delays Kestrel binding (liveness/readiness are connection-refused during the cold window) and the MinIO/DB steps have no deadline, which can hold readiness for minutes on a bad dependency.
