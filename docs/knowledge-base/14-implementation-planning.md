# 14 — Implementation Planning

## Purpose

This file defines the implementation plan an LLM or developer must follow when building PCMS.

PCMS must be built incrementally. Do not generate the whole system at once.

Each implementation step must protect:

- Tenant isolation
- Backend authorization
- Data integrity
- Auditability
- Financial correctness
- MVP scope

---

## Planning Principles

- Build one vertical slice at a time.
- Keep pull requests small and reviewable.
- Start with backend foundation before frontend screens.
- Implement core infrastructure before business modules.
- Do not implement out-of-MVP features unless explicitly approved.
- Do not add abstractions without clear value.
- Add tests for high-risk workflows.

---

## MVP Implementation Order

### Phase 0 — Repository and KB Setup

Goal: Prepare the project workspace.

Tasks:

- Create repository.
- Add `/docs/llm-kb/` folder.
- Add all KB markdown files.
- Configure `.gitignore`.
- Protect `main` branch when using remote Git.
- Use GitHub Flow.

Done when:

- KB files are committed.
- Repository has clean initial structure.
- No secrets or real patient data are committed.

---

### Phase 1 — Backend Solution Foundation

Goal: Create the backend project structure.

Projects:

```txt
src/
├── PCMS.Domain/
├── PCMS.Application/
├── PCMS.Infrastructure/
└── PCMS.API/

tests/
├── PCMS.Domain.Tests/
├── PCMS.Application.Tests/
└── PCMS.IntegrationTests/
```

Tasks:

- Create ASP.NET Core Web API solution.
- Add project references following Clean Architecture.
- Add common `Result` and error model.
- Add standard API response envelope.
- Add global exception handling.
- Add `IClock` abstraction.
- Add `ICurrentUser` abstraction.
- Add basic validation structure.
- Add base test projects.

Done when:

- Solution builds.
- Tests run.
- API returns safe error responses.
- No business logic exists in controllers.

---

### Phase 2 — Database and Multi-Tenancy Foundation

Goal: Implement platform and tenant database boundaries.

Tasks:

- Create `PlatformDbContext`.
- Create `TenantDbContext`.
- Add tenant entity in platform database.
- Add tenant status.
- Add `ITenantContext`.
- Add tenant resolution middleware.
- Add tenant connection resolver.
- Add separate migrations for platform and tenant databases.
- Add tenant-safe configuration structure.

Done when:

- Tenant is resolved from subdomain.
- Platform data uses `PlatformDbContext`.
- Tenant data uses `TenantDbContext`.
- Tenant connection resolution happens server-side only.
- Tenant clinical data is not stored in platform database.

---

### Phase 3 — Authentication and Authorization Foundation

Goal: Enable secure staff login and role/permission checks.

Tasks:

- Add ASP.NET Core Identity.
- Add staff login.
- Add JWT generation.
- Add refresh token rotation.
- Add roles and permissions.
- Add backend permission checks.
- Validate JWT tenant claims against resolved tenant.
- Add audit logs for authentication and staff security events.

MVP roles:

- ClinicAdmin
- Doctor
- Receptionist
- Accountant

Done when:

- Only staff users can log in.
- Patient login does not exist.
- JWT includes `tenantId` and `subdomain`.
- Token tenant must match resolved subdomain tenant.
- Inactive staff cannot authenticate.
- Authorization is enforced on backend.

---

### Phase 4 — Core Reference Data

Goal: Build simple tenant-scoped administrative data.

Recommended order:

1. Branches
2. Doctors
3. Staff users
4. Roles/permissions management

Done when:

- ClinicAdmin can manage branches, doctors, and staff.
- Inactive doctors cannot receive new appointments.
- Staff role changes are audit logged.
- All list endpoints are paginated.

---

### Phase 5 — Patient Management

Goal: Build the first full business vertical slice.

Features:

- Create patient
- Update patient
- Get patient by ID
- Search/list patients
- Archive patient

Required rules:

- Arabic name is required.
- Phone must support `+970` or `+972`.
- Patient records are tenant-scoped.
- Patient records are not user accounts.
- Patient records must not be hard-deleted.
- Patient access must be authorized.

Done when:

- Backend feature follows CQRS.
- DTOs are used.
- List endpoint is paginated.
- Tenant isolation is tested.
- Archive is audit logged.

---

### Phase 6 — Appointments and Basic Queue

Goal: Support clinic scheduling and check-in workflow.

Features:

- Create appointment
- Confirm appointment
- Check in appointment
- Start appointment
- Complete appointment
- Cancel appointment
- Mark no-show
- View appointment list
- View basic queue

Required rules:

- Scheduled appointments cannot be booked in the past.
- Doctor cannot have overlapping active appointments.
- Terminal states cannot return to active states.
- Cancellation requires reason.
- Queue number is assigned at check-in where applicable.
- Status changes must be audit logged.

Done when:

- Appointment lifecycle is enforced by domain/application logic.
- Invalid status transitions are rejected.
- Queue/check-in workflow works.
- Appointment tests cover lifecycle and overlap rules.

---

### Phase 7 — Visit Records

Goal: Add basic clinical notes linked to appointments.

Features:

- Create visit record
- Update visit record where allowed
- Get visit record by appointment or patient

Required rules:

- Visit record belongs to one appointment.
- Visit record belongs to one patient.
- Only authorized doctor roles can create/update visit records.
- Visit records must not be silently overwritten.
- Edits must be audit logged.

Out of MVP:

- Advanced EMR
- Prescription safety module
- Lab integration
- Clinical decision support

Done when:

- Receptionist cannot write visit records.
- Visit records are tenant-isolated.
- Update behavior is safe and audit logged.

---

### Phase 8 — Invoices and Payments

Goal: Implement safe billing workflows.

Features:

- Create draft invoice
- Add invoice line items
- Issue invoice
- Add payment
- Cancel invoice
- List invoices
- Get invoice by ID

Required rules:

- Currency is ILS only in MVP.
- Use `decimal(18,2)` for money.
- VAT comes from clinic configuration.
- Invoice stores VAT rate at issue/creation time.
- Totals are calculated from line items.
- Frontend totals are not trusted.
- Paid or Cancelled invoices cannot be edited directly.
- Payments cannot exceed remaining amount unless explicitly approved.

Done when:

- Invoice totals are correct.
- Payment updates invoice status safely.
- Financial changes are audit logged.
- Invoice/payment tests pass.

---

### Phase 9 — Basic Financial Reports

Goal: Provide MVP financial visibility.

Reports:

- Daily revenue
- Monthly revenue
- Revenue by doctor
- Paid invoices
- Unpaid invoices
- Outstanding balances

Required rules:

- Reports are tenant-scoped only.
- Cross-tenant reporting is out of MVP.
- Reports must not use unbounded queries.
- Raw SQL requires approved reporting abstraction.

Done when:

- Reports are paginated or bounded by date range.
- Accountant/ClinicAdmin permissions are enforced.
- Reports do not leak cross-tenant data.

---

### Phase 10 — Frontend Foundation

Goal: Build the staff-facing UI shell.

Tasks:

- Create Next.js 16 App Router project.
- Add TypeScript strict mode.
- Add dashboard layout.
- Add auth pages for staff only.
- Add localization setup.
- Add RTL/LTR support.
- Add typed API client.
- Add standard response/error handling.
- Add permission-based UI hiding.

Done when:

- Staff can log in.
- Dashboard layout works in Arabic and English.
- API errors are handled safely.
- Patient portal screens do not exist.

---

### Phase 11 — Frontend Business Pages

Recommended order:

1. Patients
2. Doctors
3. Appointments
4. Basic queue
5. Visit records
6. Invoices/payments
7. Reports
8. Staff/settings

Rules:

- Do not duplicate backend business rules.
- Do not send tenant ID as authority.
- Do not store clinical data in browser storage.
- Use backend permissions as source of truth.
- Show loading, empty, validation, and error states.

Done when:

- Core MVP workflows can be completed from UI.
- RTL/LTR works for key pages.
- Unauthorized UI actions are hidden but still rejected by backend.

---

## Vertical Slice Rule

For each feature, implement in this order:

1. Domain model/rules
2. EF Core configuration
3. Command/query contracts
4. Validators
5. Handlers
6. Controller endpoint
7. Error codes
8. Tests
9. Frontend API client
10. Frontend page/component

Do not build frontend screens before the backend contract is stable.

---

## LLM Task Rule

Ask the LLM for one feature or one layer at a time.

Good:

```txt
Generate CreatePatient backend feature.
```

Bad:

```txt
Generate the full clinic system.
```

Each LLM request must include:

- Feature name
- Scope
- Required KB files
- Expected output files
- Tests required
- What not to generate

---

## Planning Prompt Template

```txt
Plan the [FeatureName] feature for PCMS before coding.

Follow the attached PCMS KB files strictly.

Return:
1. Scope
2. Out of scope
3. Domain changes
4. Commands
5. Queries
6. API endpoints
7. Database changes
8. Authorization rules
9. Audit requirements
10. Tests required
11. Implementation order
12. Risks or open questions

Do not generate code yet.
```

---

## Coding Prompt Template

```txt
Generate the [FeatureName] feature for PCMS.

Follow the attached PCMS KB files strictly.
Follow the approved feature plan.
Do not violate hard constraints.
Do not generate unrelated files.
Keep the solution compact, safe, and MVP-focused.
```

---

## Review Prompt Template

```txt
Review this PCMS implementation against the attached KB files and approved plan.

Check for:
- MVP scope violations
- Clean Architecture violations
- CQRS violations
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

## Done Definition for Any Feature

A feature is done only when:

- It is inside MVP scope.
- Backend authorization is enforced.
- Tenant isolation is enforced.
- Controllers are thin.
- Domain entities are not exposed from APIs.
- List endpoints are paginated.
- Sensitive actions are audit logged.
- UTC timestamps are used.
- Money uses `decimal` where applicable.
- Tests cover critical rules.
- No hard constraints are violated.

---

## Token Usage Rules

- Do not attach all KB files for every task.
- Always attach `01-project-context.md` and `11-hard-constraints.md`.
- Add only feature-specific KB files.
- Ask for plans before large code generation.
- Ask for one feature at a time.
- Prefer compact output.
- Do not ask for unrelated layers.

---

## Recommended First Implementation

Start with:

```txt
Phase 1: Backend Solution Foundation
```

Then implement:

```txt
Patients backend vertical slice
```

Do not start with appointments, invoices, or frontend dashboards before the backend foundation is stable.
