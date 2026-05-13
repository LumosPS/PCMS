# 10 — Feature Generation Template

Use this file when asking an LLM to generate PCMS code.

## Base Prompt

```txt
You are a senior .NET/Next.js developer.

Generate the [FeatureName] feature for PCMS.

Follow the PCMS Knowledge Base strictly.

Core rules:
- Clean Architecture.
- Manual CQRS.
- No MediatR.
- Thin controllers.
- No business logic in controllers.
- No direct DbContext in controllers.
- Use TenantDbContext for tenant data.
- Use PlatformDbContext for platform data.
- Enforce backend authorization.
- Enforce tenant isolation.
- Do not trust tenantId from frontend.
- Use Fluent API only.
- No lazy loading.
- Use soft delete/archive for sensitive records.
- Store timestamps in UTC.
- Use decimal(18,2) for money.
- Use standard API response envelope.
- Paginate all list endpoints.
- Add audit logs for sensitive actions.
```

---

## Required Output

Ask the LLM to return only what the feature needs:

1. Domain changes
2. Enums/value objects if needed
3. Command + handler
4. Query + handler
5. DTOs
6. Validator
7. EF Core configuration
8. Controller endpoint
9. Error codes
10. Indexes/constraints
11. Tests for critical rules

Do not generate unrelated files.

---

## Backend Feature Prompt

```txt
Generate the [FeatureName] backend feature for PCMS.

Feature requirements:
- [Requirement 1]
- [Requirement 2]
- [Requirement 3]

Technical rules:
- Use manual CQRS.
- Command may return Result, Result<Guid>, or small result DTO.
- Query returns DTO or PagedResult<T>.
- Use TenantDbContext unless feature is platform-scoped.
- Enforce authorization in backend.
- Enforce tenant isolation.
- Do not expose domain entities.
- Use Fluent API.
- Use UTC timestamps.
- Add audit log if sensitive.
- Add tests for critical rules.

Return:
- Domain changes if needed
- Command/handler
- Query/handler if needed
- DTOs
- Validator
- EF configuration
- Controller endpoint
- Error codes
- Tests
```

---

## Example: Create Appointment

```txt
Generate the CreateAppointment backend feature for PCMS.

Business rules:
- Doctor cannot have overlapping active appointments.
- Scheduled appointment cannot be in the past.
- Appointment must be inside doctor working hours if configured.
- Initial status is Scheduled for scheduled appointments.
- Type can be Scheduled, WalkIn, or Urgent.
- WalkIn/Urgent may start at current time.
- Tenant isolation and authorization are required.
- Sensitive status changes must be audit logged.

Technical rules:
- Use manual CQRS.
- Use TenantDbContext.
- Command returns Result<CreateAppointmentResult>.
- Controller must be thin.
- No MediatR.
- No lazy loading.
- Use UTC timestamps.
- Return standard API response envelope.

Include:
- CreateAppointmentCommand
- CreateAppointmentResult
- CreateAppointmentHandler
- Validator
- Appointment domain method if needed
- EF configuration changes if needed
- Controller endpoint
- Error codes
- Unit/integration tests
```

---

## Example: Get Patients

```txt
Generate the GetPatients backend feature for PCMS.

Requirements:
- Return paginated patients.
- Support search by Arabic/English name, national ID, and phone number.
- Support sorting by name and created date.
- Return localized Name based on current user language.
- Do not return deleted/archived patients by default.

Technical rules:
- Use manual CQRS.
- Query must not change state.
- Use TenantDbContext.
- Project directly to DTO.
- Use AsNoTracking().
- Apply pagination.
- Enforce authorization.
- Return standard API response envelope.
```

---

## Frontend Page Prompt

```txt
Generate the [PageName] frontend page for PCMS.

Stack:
- Next.js 15 App Router
- React
- TypeScript strict mode

Rules:
- Follow frontend KB.
- Use typed API client.
- Handle standard API response envelope.
- Support Arabic and English.
- Support RTL and LTR.
- Include loading, empty, error, and validation states.
- Use permission-based UI hiding for UX only.
- Do not duplicate backend business rules.
- Do not send tenantId from frontend.

Page requirements:
- [Requirement 1]
- [Requirement 2]
- [Requirement 3]
```

---

## Review Prompt

```txt
Review this generated PCMS feature.

Check for:
- Clean Architecture violations
- CQRS violations
- Tenant isolation bugs
- Authorization gaps
- Missing validation
- Missing audit logs
- EF Core mistakes
- Missing pagination
- Unsafe error responses
- Missing tests
- Token-heavy or unnecessary code
```

---

## Token Usage Rule

Ask for one feature at a time.

Do not ask an LLM to generate a full module unless explicitly needed.

Prefer short prompts that reference the KB instead of repeating all rules.
