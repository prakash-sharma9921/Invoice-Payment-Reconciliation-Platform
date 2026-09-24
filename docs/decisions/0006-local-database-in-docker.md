# 6. Local database in Docker

**Status:** Accepted

## Decision

- SQL Server 2022 runs in a Docker container via docker-compose.yml.
- Exposed on host port 14330 (mapped to the container's 1433).
- Data persisted in a named Docker volume so it survives container recreation.

## Alternatives considered

- Use the existing local SQL Server on port 1433 — rejected: the developer's
  local instance is used by other work; a separate port avoids any clash.

## Why

Containerised DB is reproducible (config lives in the repo), disposable, and
isolated from the developer's existing SQL Server. Rehearses the parked
cloud-deployment path.
