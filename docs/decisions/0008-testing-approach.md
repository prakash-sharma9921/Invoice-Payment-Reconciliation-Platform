# 8. Testing approach

**Status:** Accepted

## Decision

- Test framework: xUnit.
- Two test projects:
  - Recon.UnitTests — tests pieces in isolation (services, validators).
    Uses the EF Core InMemory provider as a fake database, injected through
    IApplicationDbContext. No web server, no real database.
  - Recon.IntegrationTests — tests the full HTTP request pipeline
    (endpoint -> validation -> service -> DbContext -> response) using
    WebApplicationFactory to boot the real API in memory. The SQL Server
    provider is swapped for the InMemory provider inside the test factory, so
    the suite needs no Docker/SQL Server running.
- Test method style: Arrange / Act / Assert.
- Each unit test gets its own uniquely-named in-memory database for isolation.

## Alternatives considered

- Mocking libraries (e.g. Moq) instead of InMemory — rejected for now: more
  setup and more brittle for EF Core queries; InMemory runs real queries.
- Integration tests against real SQL Server (Testcontainers) — deferred: adds
  Docker as a hard dependency for the test suite. May revisit for tests that
  need true relational behaviour.

## Why

Fast, dependency-free tests that run anywhere (no external services). The two
layers complement each other: unit tests check logic; integration tests check
the real wiring.

## Known limitation (important)

The EF Core InMemory provider is NOT a real relational database. It does not
enforce unique indexes, foreign keys, or SQL-specific behaviour. It is suitable
for testing our own logic, not the database engine. Rules that depend on real
relational behaviour are covered by the actual SQL Server database at runtime
(and could later be covered by Testcontainers-based tests if needed).
