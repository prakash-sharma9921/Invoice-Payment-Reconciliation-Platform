# 1. Repository and solution structure

**Status:** Accepted

## Decision

- Single mono-repo containing both `backend/` and `frontend/`.
- Backend is a layered solution with four projects: Domain, Application,
  Infrastructure, Api — plus UnitTests and IntegrationTests.
- Projects live under `backend/src/` and `backend/tests/`.
- Project name prefix: `Recon.*`.
- Solution file format: `.slnx`.

## Alternatives considered

- Two separate repos (backend and frontend) — rejected: harder to clone,
  version, and later run one CI pipeline for a solo project.
- Flat project layout (no src/tests folders) — rejected: less conventional,
  noisier repo root.
- `.sln` format — rejected: `.slnx` is the .NET 10 default, cleaner and
  Git-friendly; convertible later if ever needed.

## Why

Mono-repo is simpler for a solo portfolio project. The layered split enforces
clean dependencies (Domain depends on nothing; dependencies point inward).
