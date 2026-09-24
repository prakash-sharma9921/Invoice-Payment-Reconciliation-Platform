# 5. API style, validation, and mapping

**Status:** Accepted

## Decision

- API style: Minimal APIs, organised into per-feature endpoint classes
  (never dumped in Program.cs).
- Validation of incoming requests: FluentValidation (separate validator
  classes in the Application layer).
- Object mapping (entity <-> DTO): hand-written, no library.
- Entity-to-table configuration: EF Core Fluent API (separate config classes
  in Infrastructure), not data annotations.

## Alternatives considered

- Controllers — valid, more familiar, but Minimal APIs are the modern idiom
  and pair well with one-slice-at-a-time.
- Data annotations for validation — rejected: weak for complex rules, clutters
  models.
- AutoMapper — rejected: hides behaviour, harder to debug for learning, and
  moved to a commercial license in 2025.

## Why

These keep the Domain clean, the rules testable, and the behaviour explicit —
matching the learning and portfolio goals.
