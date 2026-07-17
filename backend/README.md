# PCMS Backend

ASP.NET Core Web API (.NET 10) following Clean Architecture with feature-based CQRS (MediatR 12.x, planned).

Architecture and coding rules live in [`docs/knowledge-base/`](../docs/knowledge-base/) — start with [`02-architecture-rules.md`](../docs/knowledge-base/02-architecture-rules.md).

## Solution Structure

Solution: `PCMS.slnx`

| Project | Role | References |
|---|---|---|
| `src/PCMS.Domain` | Entities, value objects, domain rules | — |
| `src/PCMS.Application` | Use cases, CQRS handlers, DTOs, validation | Domain |
| `src/PCMS.Infrastructure` | Persistence, external services | Application, Domain |
| `src/PCMS.API` | Controllers, middleware, DI, HTTP | Application, Infrastructure |

Tests (xUnit): `tests/PCMS.Domain.Tests`, `tests/PCMS.Application.Tests`, `tests/PCMS.API.Tests`.

## Commands

From the repository root:

```bash
dotnet build backend/PCMS.slnx
dotnet test backend/PCMS.slnx
dotnet run --project backend/src/PCMS.API
```

In development, interactive API docs (Scalar) are available at `/scalar`.

## Status

The solution contains scaffolding and placeholder template code only. Planned (see the knowledge base for the target design):

- EF Core + PostgreSQL 16 (platform and per-tenant databases)
- JWT staff authentication and role-based authorization
- Subdomain-based multi-tenancy
- Business features (patients, appointments, invoicing, reports)

Database setup and migration commands will be documented here once they exist and are verified.
