## Quick orientation for AI coding agents

This repository is a multi-service .NET solution (SICCARV3) that runs locally via Docker Compose + Dapr or in Kubernetes (manifests in `k8s/` and `deploy/`). Use these notes to make edits, tests, and PRs consistently.

- Big picture
  - Monorepo .sln at the repo root. Service code lives under `src/Services/<Service>/<ServiceName>/` (e.g. `src/Services/Wallet/WalletService`). Each service has its own Dockerfile referenced from `docker-compose.yml`.
  - Dapr is used for sidecars and cross-service concerns. Dapr components live in `components/` (e.g. `localsecretstore.yaml`, `component-state-*.yaml`, `config.yaml`). The docker-compose file runs explicit daprd containers named `<service>-dapr` that use `network_mode: "service:<service>"`.
  - Infrastructure manifests and infra-as-code are under `deploy/` (Bicep) and Kubernetes manifests are under `k8s/`.

- Developer workflows (how to build/run/debug)
  - Local quick run: Docker Desktop + select the Docker Compose project in Visual Studio. The compose file is `docker-compose.yml` and contains service and dapr sidecar images.
  - Build a single service: use existing VS Code tasks or `dotnet build` on the service csproj. Example task labels present in the workspace: `build-wallet`, `build-tenant`, `build-register`, `build-peer`, `build-validator`.
  - Run a service with Dapr for local debugging: tasks like `daprd-debug-wallet`, `daprd-debug-tenant`, etc. These start daprd with ports (http/grpc/metrics) set in the task and depend on the corresponding build task.
  - Tests: run `dotnet test` at solution or project level.

- Secrets and package feeds
  - Dapr secret store: you must create `components/secretsFile.json` locally containing keys: `keyVaultConnectionString`, `siccarV3ClientId`, `siccarV3ClientSecret`, `walletEncryptionKey` (see root `README.md`). Kubernetes alternative: `kubectl create secret generic local-secret-store ...`.
  - Private NuGet feed: the repo uses a private feed. Ensure `FEED_ACCESSTOKEN` is set in your `.env` (solution root) for Docker-Compose builds or authenticate to the NuGet feed in Visual Studio. Builds will fail if the PAT expires.

- Important files to reference when making changes
  - `docker-compose.yml` — defines service images, dependencies, and dapr sidecars. Use when changing how services are composed locally.
  - `components/` — all Dapr components (state stores, secret stores, pubsub). Edit or add here when introducing new Dapr bindings/state stores.
  - `src/Services/<Service>/<ServiceName>/Dockerfile` — where service container builds are defined; keep paths in sync with `docker-compose.yml` and k8s manifests.
  - `k8s/` and `deploy/` — Kubernetes manifests and Bicep templates; update these when changing deployment contracts.
  - `resources/SiccarV3.postman_collection.json` — Postman scripts for generating tokens and manual endpoint testing.

- Code & naming conventions observed (do not invent conventions)
  - Service folder names commonly end with `Service` (e.g. `WalletService`, `TenantService`). The csproj is typically the service name (e.g. `Wallet.csproj` under Wallet.API or WalletService depending on service).
  - Dapr sidecars follow `<service>-dapr` naming in `docker-compose.yml`. When adding a new microservice, add both the service and a `<service>-dapr` entry mirroring existing services.

- Integration points to be careful with
  - Cross-service messaging uses RabbitMQ (see `docker-compose.yml`) and Dapr pub/sub components in `components/`. When adding events, update the corresponding pubsub component and all consumers.
  - State stores: MongoDB / MySQL (see compose) — altering state schema may require clearing volumes (data persisted under `~/.docker-conf` for local runs). Remove those volumes when testing schema changes.

- Small contract for edits
  - Inputs: modified service code under `src/Services/...`, optional container/Dapr config changes in `components/` or `docker-compose.yml`, optional k8s/deploy updates.
  - Outputs: keep project/solution buildable (`dotnet build`), compose-up runnable (no missing secrets), and tests passing (`dotnet test`). If you change an API surface, update Postman collection and k8s manifests as needed.

- Common quick fixes and gotchas
  - If `dotnet build` fails due to NuGet, check `FEED_ACCESSTOKEN` in `.env` and PAT expiry. See root `README.md` for feed URL.
  - Missing local Dapr secrets -> ensure `components/secretsFile.json` exists. The README shows the required JSON keys.
  - When adding a new service, remember to add: Dockerfile, `docker-compose.yml` service entry + `<service>-dapr` entry, and k8s manifests if deploying.

If any of the above is unclear or you'd like me to include more examples (e.g. a checklist for adding a new service or a short PR template), tell me which area to expand. 
### Checklist — adding a new microservice

- Create service scaffold under `src/Services/<Service>/<ServiceName>/` following existing services (see `src/Services/Wallet/WalletService` for an example).
- Add a `Dockerfile` at the same path and a service `csproj` (project name usually matches the service folder). Keep the dockerfile path consistent with `docker-compose.yml`.
- Add `docker-compose.yml` entries: a service entry plus a `<service>-dapr` entry that uses `network_mode: "service:<service>"` (copy pattern from `tenant`/`tenant-dapr`).
- If the service uses state or pubsub, add or update Dapr components in `components/` (e.g., `component-state-<service>.yaml`) and confirm pubsub topics/consumers are registered.
- Add or update Kubernetes manifests in `k8s/` (deployment, service, ingress) using the existing `deployment-microservice-*.yaml` templates.
- Update `deploy/bicep/*` if the service will be deployed to Azure (follow existing bicep conventions in `deploy/bicep/`).
- Add a VS Code task (pattern: `build-<service>` and `daprd-debug-<service>`) matching the other tasks in the workspace to simplify local debug.
- Update `resources/SiccarV3.postman_collection.json` if exposing new endpoints that require auth scopes.
- Verify locally: `dotnet build <csproj>`, `dotnet test`, and `docker-compose up` (ensure `components/secretsFile.json` and `.env` are present).

### Minimal `components/secretsFile.json` (local dev)

{
  "keyVaultConnectionString": "",
  "siccarV3ClientId": "",
  "siccarV3ClientSecret": "",
  "walletEncryptionKey": ""
}

### Suggested PR template snippet

- Short summary of change (what/why)
- Services touched (list folders under `src/Services/...`)
- How to run locally (build tasks / docker-compose services to start)
- Secrets or env required (e.g., `components/secretsFile.json`, `.env` FEED_ACCESSTOKEN)
- Migration steps or schema changes (DB migrations, state clears)
- Checklist for reviewer:
  - Builds clean: `dotnet build` for modified projects
  - Tests: `dotnet test` for affected test projects
  - Docker compose: `docker-compose up` for changed services
  - Dapr components: confirm any new `components/` are added and referenced

---

If you want this checklist converted into a PR checklist file or a reviewer GitHub Action, I can add that next.
