# 4. Primary key strategy

**Status:** Accepted

## Decision

All primary keys are `Guid` (uniqueidentifier in SQL Server).

## Alternatives considered

- Integer keys — rejected: guessable (tenant 2 implies tenant 3 exists) and
  can clash if databases are ever merged.

## Why

GUIDs are not guessable and never clash — important when many tenants' data
shares one database, and useful for the parked cloud plan.
