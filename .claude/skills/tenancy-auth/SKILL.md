---
name: tenancy-auth
description: Use for multi-tenancy or authentication/authorization work — tenant resolution, tenant provisioning, tenant database access, platform/tenant boundaries, login, JWT, refresh tokens, roles, permissions, staff access.
---

# Tenancy & Auth Workflow

Scope: anything touching tenant isolation, tenant lifecycle, authentication, or authorization. This is the highest-risk area in PCMS — cross-tenant access is a patient-safety bug, not a defect.

## Before Writing Code

Read these KB files (skip any already in context):

1. `docs/knowledge-base/01-project-context.md` — tenancy model overview.
2. `docs/knowledge-base/02-architecture-rules.md` — where tenant/auth concerns live per layer.
3. `docs/knowledge-base/04-multitenancy.md` — subdomain resolution, DB-per-tenant, platform vs tenant DB.
4. `docs/knowledge-base/06-api-conventions.md` — auth-related API behavior and error responses.
5. `docs/knowledge-base/07-database-efcore-rules.md` — platform vs tenant DbContext rules.
6. `docs/knowledge-base/08-auth-authorization.md` — JWT, roles, permissions, staff access.
7. `docs/knowledge-base/13-testing-quality-rules.md` — required isolation/authorization tests.

## Current State Warning

None of this exists yet — no auth packages, no middleware, no DbContexts. The KB is the spec; you are likely creating the first instance. Check what exists before assuming structure.

## Non-Negotiables (extra emphasis)

- Tenant context is resolved server-side from the subdomain — never from request bodies, headers the client controls, or query strings.
- Never accept tenant IDs, database names, or connection strings from the client.
- JWT must carry `tenantId` and `subdomain`; validate them on every request.
- Platform DB holds tenant metadata only; clinic data lives only in that tenant's DB.
- Authorization is enforced in handlers on the backend; frontend checks are UX only.
- Sensitive actions (role changes, staff management, permission changes) are audit logged.

## Verify

- `dotnet build backend/PCMS.slnx`
- `dotnet test backend/PCMS.slnx` — tenant-isolation and authorization tests are mandatory for this area per KB 13, not optional.
