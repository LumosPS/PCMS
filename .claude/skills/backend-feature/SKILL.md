---
name: backend-feature
description: Use when adding or modifying PCMS backend features — entities, commands, queries, handlers, endpoints, controllers, DTOs, validators. Examples - patients, doctors, appointments, invoices, payments, visit records, reports.
---

# Backend Feature Workflow

Scope: any change to the .NET backend that adds or modifies a business feature.

## Before Writing Code

Read these KB files (skip any already in context):

1. `docs/knowledge-base/01-project-context.md` — confirm the feature is in MVP scope.
2. `docs/knowledge-base/02-architecture-rules.md` — layer responsibilities and dependency rules.
3. `docs/knowledge-base/03-domain-model.md` — ONLY the sections for the entities involved (the file is ~700 lines; do not read all of it).
4. `docs/knowledge-base/05-cqrs-rules.md` — MediatR 12.x markers, handler responsibilities, controller pattern.
5. `docs/knowledge-base/06-api-conventions.md` — routes, response envelope, error codes, pagination.
6. `docs/knowledge-base/07-database-efcore-rules.md` — if the feature touches persistence.
7. `docs/knowledge-base/13-testing-quality-rules.md` — which tests are required.

Templates: `docs/knowledge-base/10-feature-generation-template.md` (per-feature checklist) and `docs/knowledge-base/14-implementation-planning.md` (phase sequencing).

## Current State Warning

The KB describes the TARGET architecture. Check what actually exists (Glob/Read) before assuming any structure exists — you may be creating the first instance of a pattern, and the KB is its spec. Remove placeholder template code (WeatherForecast*, Class1.cs) when superseding it; never imitate it.

## Workflow

1. Plan: list the files to create/change per layer before writing any code.
2. Domain: entities, value objects, domain methods. State changes go through domain methods only.
3. Application: command/query + handler + validator + DTOs under `Features/<Feature>/`.
4. Infrastructure: EF Core configuration (Fluent API only) and persistence pieces.
5. API: thin controller endpoint dispatching via `ISender`.
6. Tests: per KB 13 — critical rules first (tenant isolation, authorization, business invariants).

One feature at a time. No unrelated files. Include validation, authorization, tenant isolation, and audit logging where relevant.

## Verify

- `dotnet build backend/PCMS.slnx`
- `dotnet test backend/PCMS.slnx`
