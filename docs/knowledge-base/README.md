# PCMS LLM Knowledge Base

This folder contains Markdown rules for generating and reviewing code for the Palestinian Clinic Management System — PCMS.

PCMS is a multi-tenant healthcare SaaS. The KB keeps generated code consistent, safe, maintainable, tenant-isolated, and aligned with MVP scope.

---

## Source of Truth

Use these files before generating backend or frontend code.

If a request conflicts with the KB, stop and explain the conflict before generating code.

---

## Reading Order

1. `01-project-context.md`
2. `11-hard-constraints.md`
3. `02-architecture-rules.md`
4. `03-domain-model.md`
5. `04-multitenancy.md`
6. `05-cqrs-rules.md`
7. `06-api-conventions.md`
8. `07-database-efcore-rules.md`
9. `08-auth-authorization.md`
10. `09-frontend-rules.md`
11. `10-feature-generation-template.md`
12. `12-version-control-rules.md`
13. `13-testing-quality-rules.md`

---

## Token Usage Rule

Do not attach every KB file for every task.

Always include:

- `01-project-context.md`
- `11-hard-constraints.md`

Then attach only the feature-specific files needed for the task.

---

## Base LLM Instruction

```txt
Follow the attached PCMS Knowledge Base files strictly.

If my request conflicts with the KB or hard constraints, tell me before generating code.

Keep the solution compact, safe, and MVP-focused.
Do not generate unrelated files.
```

---

## Common File Sets

### Backend Feature

Use for patients, doctors, appointments, invoices, payments, visit records, and reports.

Attach:

- `01-project-context.md`
- `11-hard-constraints.md`
- `02-architecture-rules.md`
- `03-domain-model.md`
- `05-cqrs-rules.md`
- `06-api-conventions.md`
- `07-database-efcore-rules.md`
- `13-testing-quality-rules.md`

### Multi-Tenancy Feature

Use for tenant resolution, tenant provisioning, tenant DB access, and platform/tenant boundaries.

Attach:

- `01-project-context.md`
- `11-hard-constraints.md`
- `02-architecture-rules.md`
- `04-multitenancy.md`
- `07-database-efcore-rules.md`
- `08-auth-authorization.md`
- `13-testing-quality-rules.md`

### Authentication / Authorization Feature

Use for login, JWT, refresh tokens, roles, permissions, and staff access.

Attach:

- `01-project-context.md`
- `11-hard-constraints.md`
- `02-architecture-rules.md`
- `04-multitenancy.md`
- `06-api-conventions.md`
- `08-auth-authorization.md`
- `13-testing-quality-rules.md`

### Frontend Feature

Use for dashboard pages, forms, tables, localization, and typed API clients.

Attach:

- `01-project-context.md`
- `11-hard-constraints.md`
- `06-api-conventions.md`
- `09-frontend-rules.md`

### Database / EF Core Feature

Use for migrations, entity configuration, constraints, indexes, and persistence rules.

Attach:

- `01-project-context.md`
- `11-hard-constraints.md`
- `03-domain-model.md`
- `04-multitenancy.md`
- `07-database-efcore-rules.md`
- `13-testing-quality-rules.md`

### Code Review

Attach:

- `01-project-context.md`
- `11-hard-constraints.md`
- Relevant feature KB files
- `13-testing-quality-rules.md`

Prompt:

```txt
Review this PCMS code against the attached KB files.

Check for:
- MVP scope violations
- Clean Architecture violations
- Tenant isolation bugs
- Authorization gaps
- EF Core mistakes
- API convention issues
- Missing validation
- Missing audit logs
- Missing or weak tests
- Unnecessary complexity
```

---

## Short Feature Prompt

```txt
Generate the [FeatureName] feature for PCMS.

Follow the attached PCMS KB files strictly.
Do not violate hard constraints.
Do not generate unrelated files.
Keep the solution compact, safe, and MVP-focused.
```

---

## Non-Negotiable Reminder

PCMS handles healthcare and financial data.

Generated code must prioritize:

- Tenant isolation
- Backend authorization
- Data integrity
- Auditability
- UTC timestamps
- Safe error responses
- Financial correctness
- Maintainability
