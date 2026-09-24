# 3. Multi-tenancy approach

**Status:** Accepted

## Decision

- Shared database, shared schema. Every tenant-owned table carries a
  `TenantId` column.
- Tenant isolation enforced via an EF Core global query filter (added when
  the first tenant-owned table, Client, is built).
- The Tenant table itself has NO TenantId (it is the tenant).
- Current tenant is resolved by a service. Until authentication exists, that
  service reads a temporary request header (`X-Tenant-Id`). When auth is
  added, only that service changes — it reads the tenant from the token
  instead.

## Alternatives considered

- Database-per-tenant — rejected: heavier to run and deploy for this scope.
- Hardcoded single tenant for dev — rejected: cannot prove isolation works.

## Why

Shared database is the finishable, common SaaS pattern. Resolving the tenant
behind a service means the temporary header can be swapped for real login
with no wider changes.
