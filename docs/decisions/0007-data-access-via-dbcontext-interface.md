# 7. Data access via a DbContext interface

**Status:** Accepted

## Decision

Application defines IApplicationDbContext (exposing DbSets + SaveChangesAsync).
Infrastructure's ReconDbContext implements it. Services live in Application and
depend on the interface, not on the concrete DbContext.

## Alternatives considered

- Repository per entity — rejected: extra boilerplate per table, against the
  "plain service classes" decision.
- Service using the concrete DbContext directly — rejected: would break the
  inward dependency rule or push logic into Infrastructure.

## Why

Least boilerplate, keeps logic in Application, stays testable, and keeps the
dependency pointing inward. Trade-off: Application references EF Core
abstractions (not the SQL Server provider), which is an accepted compromise.
