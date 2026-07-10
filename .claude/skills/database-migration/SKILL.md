---
name: database-migration
description: Use for EF Core and database work — migrations, entity configuration, Fluent API mappings, constraints, indexes, seed data, DbContext changes. Examples - add an index, configure a new entity, create or apply a migration.
---

# Database & EF Core Workflow

Scope: schema, entity configuration, and migration work against PostgreSQL 16 via EF Core.

## Before Writing Code

Read these KB files (skip any already in context):

1. `docs/knowledge-base/03-domain-model.md` — ONLY the sections for the entities involved.
2. `docs/knowledge-base/04-multitenancy.md` — decide which database this belongs to: platform or tenant.
3. `docs/knowledge-base/07-database-efcore-rules.md` — the persistence rulebook for this task.
4. `docs/knowledge-base/13-testing-quality-rules.md` — required persistence tests.

## Current State Warning

EF Core is NOT installed yet — no packages, no DbContexts, no migrations exist. Installing packages and creating `PlatformDbContext`/`TenantDbContext` may be part of your task; package installs need user approval. The KB is the spec for what to create.

## Rules (extra emphasis)

- Fluent API only — no data annotations on entities.
- Platform and tenant migrations stay strictly separate.
- Never edit a migration that has been shared or applied; create a new one.
- Soft delete/archive for sensitive records — no hard deletes.
- `decimal(18,2)` for money; UTC timestamps; no lazy loading.

## Safety Gates (always ask the user first)

- `dotnet ef database update` — never run without explicit confirmation.
- Destructive migrations (dropping columns/tables, data loss) — surface the impact and wait for approval.
- `dotnet add package` — package installs need approval.

## Verify

- `dotnet build backend/PCMS.slnx`
- `dotnet test backend/PCMS.slnx`
- Note the migration impact in the PR description per KB 12.
