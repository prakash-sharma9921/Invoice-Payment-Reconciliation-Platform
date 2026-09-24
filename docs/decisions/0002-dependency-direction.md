# 2. Dependency direction (clean architecture)

**Status:** Accepted

## Decision

Project references point inward only:

- Domain → references nothing
- Application → Domain
- Infrastructure → Application
- Api → Application, Infrastructure

## Why

Keeps business logic (Domain, Application) free of database and web concerns.
The database and API are treated as replaceable details, not the core.
This is the main architecture signal for the project.
