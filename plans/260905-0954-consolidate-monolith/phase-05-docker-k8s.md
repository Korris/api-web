# Phase 5: Docker & K8s Infrastructure Updates

## Priority: P2 | Status: pending

## Overview

Replace 6 Dockerfiles and 6 K8s deployments with 1 unified Dockerfile and 1 K8s deployment. Update CI/CD pipeline (Jenkinsfile) and ArgoCD GitOps repo.

## Current State

### Dockerfiles (all identical pattern)
- Base: `mcr.microsoft.com/dotnet/aspnet:8.0-bookworm-slim`
- SDK: `mcr.microsoft.com/dotnet/sdk:8.0-bookworm-slim`
- Each copies its own csproj + 4 Common csproj files
- Ports: 8080, 8081

### CI/CD Pipeline (Jenkinsfile)
- Jenkins parses JOB_NAME to extract service suffix and type
- Builds Docker image per service
- Pushes to Harbor registry: `harbor.local/focfoc/{service}_{env}:{build}`
- Updates ArgoCD GitOps repo kustomization.yaml

### K8s (ArgoCD GitOps)
- Separate deployment per service at `apps/{env}/{service}/kustomization.yaml`
- Service names: `api-web-comic`, `api-web-story`, `api-web-document`, `api-web-social`, `api-web-identity`, `api-web-realtime`

## New Dockerfile

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0-bookworm-slim AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0-bookworm-slim AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy all csproj files for restore
COPY ["Mcsg.Api/Mcsg.Api.csproj", "Mcsg.Api/"]
COPY ["Common/Mcsg.Common.Domain/Mcsg.Common.Domain.csproj", "Common/Mcsg.Common.Domain/"]
COPY ["Common/Mcsg.Common/Mcsg.Common.csproj", "Common/Mcsg.Common/"]
COPY ["Common/Mcsg.Common.Core/Mcsg.Common.Core.csproj", "Common/Mcsg.Common.Core/"]
COPY ["Common/Mcsg.Common.SeedWork/Mcsg.Common.SeedWork.csproj", "Common/Mcsg.Common.SeedWork/"]
RUN dotnet restore "./Mcsg.Api/Mcsg.Api.csproj"

COPY . .
WORKDIR "/src/Mcsg.Api"
RUN dotnet build -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
RUN dotnet publish -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Copy Firebase credential files for Realtime domain
COPY ["Mcsg.Api/Areas/Realtime/firebase*.json", "./"]

ENTRYPOINT ["dotnet", "Mcsg.Api.dll"]
```

### Key differences from old Dockerfiles
- References `Mcsg.Api.csproj` instead of individual service csproj
- Must copy firebase JSON files for Realtime's notification service
- Single image serves all API traffic

## K8s Changes

### Remove old deployments
Delete from ArgoCD GitOps repo:
- `apps/{env}/api-web-comic/`
- `apps/{env}/api-web-story/`
- `apps/{env}/api-web-document/`
- `apps/{env}/api-web-social/`
- `apps/{env}/api-web-identity/`
- `apps/{env}/api-web-realtime/`

### Create new unified deployment
Create `apps/{env}/api-web/` with:

```yaml
# deployment.yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: api-web
spec:
  replicas: 2  # Can scale as needed
  selector:
    matchLabels:
      app: api-web
  template:
    spec:
      containers:
      - name: api-web
        image: harbor.local/focfoc/api-web_{env}:latest
        ports:
        - containerPort: 8080  # HTTP/1 (REST + SignalR)
          name: http
        - containerPort: 8081  # HTTP/2 (gRPC)
          name: grpc
        env:
          # Merge all env vars from 6 services
          # Use single prefix (Api) or keep all prefixes
        livenessProbe:
          httpGet:
            path: /health
            port: 8080
        readinessProbe:
          httpGet:
            path: /health
            port: 8080
        resources:
          requests:
            memory: "512Mi"  # Higher than individual services
            cpu: "250m"
          limits:
            memory: "1Gi"
            cpu: "1000m"
```

### Ingress update

Old Ingress rules (6 separate backends):
```yaml
- path: /api/comic/
  backend: api-web-comic:8080
- path: /api/story/
  backend: api-web-story:8080
- path: /api/identity/
  backend: api-web-identity:8080
# ... etc
```

New Ingress (single backend):
```yaml
- path: /api/
  backend: api-web:8080
- path: /commentHub
  backend: api-web:8080   # WebSocket upgrade needed
- path: /notificationHub
  backend: api-web:8080
- path: /followHub
  backend: api-web:8080
```

**SignalR WebSocket support:** Ingress must allow WebSocket upgrade for hub paths. Add annotation:
```yaml
nginx.ingress.kubernetes.io/proxy-read-timeout: "3600"
nginx.ingress.kubernetes.io/proxy-send-timeout: "3600"
nginx.org/websocket-services: "api-web"
```

### Service definition

```yaml
apiVersion: v1
kind: Service
metadata:
  name: api-web
spec:
  ports:
  - name: http
    port: 8080
    targetPort: 8080
  - name: grpc
    port: 8081
    targetPort: 8081
  selector:
    app: api-web
```

### ConfigMap / Secrets

Merge env vars from all 6 services' ConfigMaps into one. If using Option A (single prefix `Api`), rename all env vars. If using Option B (keep all prefixes), concatenate all vars.

**Recommended:** Option A. One-time rename, cleaner long-term.

## CI/CD Updates

### Jenkinsfile changes

The current Jenkinsfile auto-detects service from JOB_NAME. For the merged API:

**Option 1:** Create a new Jenkins job `Web-Focfoc.Api` that builds `Mcsg.Api/Dockerfile`.

**Option 2:** Modify Jenkinsfile to handle the merged service case.

Simplest approach -- new Jenkins job with hardcoded values:

```groovy
pipeline {
    agent { label 'agent01u' }
    environment {
        HARBOR_REGISTRY = "http://harbor.local/v2/"
        GITOPS_REPO = "https://repo.ntadamedia.com/focfoc/argo.git"
        SERVICE = "api-web"
    }
    stages {
        stage('Build Docker Image') {
            steps {
                script {
                    def BRANCH = env.GIT_BRANCH.tokenize('/').last()
                    def ENV = /* same switch as before */
                    def DOCKER_IMAGE = "harbor.local/focfoc/${SERVICE}_${ENV}:${BUILD_NUMBER}"

                    docker.build(DOCKER_IMAGE, "-f Mcsg.Api/Dockerfile .")
                }
            }
        }
        // Push + GitOps update same as before
    }
}
```

### Deprecate old Jenkins jobs

After migration verified:
1. Disable old per-service Jenkins jobs
2. Remove old per-service ArgoCD applications
3. Clean up Harbor -- old service images can be retained for rollback

## Migration Strategy (Zero-Downtime)

### Phase 5a: Deploy merged API alongside old services
1. Deploy `api-web` as a new K8s deployment (not replacing old ones)
2. Add a new Ingress rule pointing to it on a test path (e.g., `/api-v2/`)
3. Run smoke tests against the new API

### Phase 5b: Switch traffic
1. Update Ingress to point `/api/*` to the merged `api-web` service
2. Keep old services running but receiving no traffic (canary rollback)
3. Monitor for 24-48h

### Phase 5c: Decommission old services
1. Scale old deployments to 0
2. After 1 week with no issues, delete old K8s resources
3. Remove old Dockerfiles, old Jenkins jobs

## Environment Variables Consolidation

List of env var prefixes to merge:
- `Cmc_*` (Comic) -> `Api_*`
- `Sto_*` (Story) -> `Api_*`
- `Doc_*` (Document) -> `Api_*`
- `Soc_*` (Social) -> `Api_*`
- `Ide_*` (Identity) -> `Api_*`
- `Rea_*` (Realtime) -> `Api_*`

Common vars (same value across services): JWT config, DB connection, origins, etc. -- set once.
Domain-specific vars: notification queue names, Firebase path, Redis config -- prefix with domain or use nested config sections.

## Implementation Steps

1. [ ] Create `Mcsg.Api/Dockerfile`
2. [ ] Build and test Docker image locally
3. [ ] Create K8s deployment manifest for `api-web`
4. [ ] Create K8s service manifest
5. [ ] Merge ConfigMaps/Secrets
6. [ ] Update Ingress (add WebSocket support)
7. [ ] Create new Jenkins job or update Jenkinsfile
8. [ ] Update ArgoCD kustomization.yaml
9. [ ] Deploy to dev environment
10. [ ] Run smoke tests
11. [ ] Deploy to staging
12. [ ] Cutover production traffic
13. [ ] Decommission old services

## Success Criteria

- [ ] Single Docker image builds successfully
- [ ] K8s deployment starts and passes health checks
- [ ] All API routes accessible through Ingress
- [ ] SignalR hubs connect via WebSocket
- [ ] gRPC services accessible on port 8081
- [ ] CI/CD pipeline builds and deploys automatically

## Risk

- **Memory increase** -- single pod needs more RAM than any individual service. Size appropriately (start with 1Gi limit).
- **Blast radius** -- one bug takes down ALL APIs, not just one service. Mitigate with health checks, readiness probes, and rolling deployments.
- **WebSocket Ingress config** -- SignalR needs sticky sessions or Redis backplane for multi-replica. If currently single-replica Realtime, this may need attention.
- **gRPC + HTTP/1 on same port** -- Kestrel handles this with protocol negotiation, but verify Ingress doesn't interfere. Use separate port (8081) for gRPC.
