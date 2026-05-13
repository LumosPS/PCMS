# 02 — Architecture Rules

## Architecture Style

PCMS uses Clean Architecture with feature-based organization.

The backend solution must be separated into these layers:

```txt
PCMS.Domain
PCMS.Application
PCMS.Infrastructure
PCMS.API
```

Clean Architecture is used to protect business rules, tenant isolation, clinical safety, testability, and long-term maintainability.

It must not be used to add unnecessary abstractions or boilerplate.

---

## Dependency Rules

Allowed dependency direction:

```txt
PCMS.API → PCMS.Application → PCMS.Domain
PCMS.Infrastructure → PCMS.Application → PCMS.Domain
```

Forbidden dependency direction:

```txt
PCMS.Domain → PCMS.Application
PCMS.Domain → PCMS.Infrastructure
PCMS.Domain → PCMS.API
PCMS.Application → PCMS.Infrastructure
PCMS.Application → PCMS.API
```

The Domain layer must remain the most stable and least dependent layer.

---

## Layer Responsibilities

| Layer | Responsibility | Allowed Dependencies |
|---|---|---|
| Domain | Entities, value objects, enums, domain exceptions, core business behavior | None |
| Application | Use cases, CQRS handlers, DTOs, interfaces, validation, authorization orchestration, result models | Domain |
| Infrastructure | EF Core, database contexts, repositories, migrations, token services, tenant database resolution, external service implementations | Application + Domain |
| API | Controllers, middleware, dependency injection, HTTP request/response mapping, global exception handling | Application |

---

## Pragmatic Architecture Rule

Clean Architecture must protect business rules, testability, and safety.

Do not add abstractions only for style.

Create interfaces only when they provide one of these benefits:

- Protect the Application layer from infrastructure details
- Enable testing of critical business workflows
- Support multiple implementations
- Isolate external services
- Hide tenant database resolution
- Hide current user/time/context access

Avoid generic architecture patterns that make the system harder to understand.

Do not create repositories, services, or interfaces automatically for every entity unless they serve a clear purpose.

---

## Domain Layer Rules

The Domain layer must be pure C#.

Do not add:

- EF Core attributes
- ASP.NET attributes
- Database-specific logic
- HTTP-specific logic
- DTOs
- API response models
- Infrastructure services
- JWT logic
- Current user service access
- Tenant resolution services

Domain entities should expose behavior through methods instead of allowing uncontrolled state changes.

Good:

```csharp
appointment.Cancel(cancelledByUserId, reason, utcNow);
appointment.CheckIn(checkedInByUserId, utcNow);
invoice.MarkAsPaid(paymentInfo, paidByUserId, utcNow);
patient.UpdatePhoneNumber(phoneNumber, updatedByUserId, utcNow);
```

Bad:

```csharp
appointment.Status = AppointmentStatus.Cancelled;
invoice.Status = InvoiceStatus.Paid;
patient.PhoneNumber = phoneNumber;
```

Domain entities may receive actor IDs, tenant IDs, and timestamps as values.

Domain entities must not access authentication services, HTTP context, JWT claims, system clock, or current user services directly.

---

## Domain Responsibilities

The Domain layer should contain rules such as:

- Appointment status transition rules
- Invoice total calculation
- Invoice payment state rules
- Patient entity invariants
- Visit record amendment behavior
- Value object validation
- Domain exceptions for invalid business operations

Examples:

```txt
Completed appointments cannot be moved back to Scheduled.
Cancelled appointments cannot be moved to InProgress.
Paid invoices cannot be edited directly.
Invoice totals cannot be negative.
Visit records must not be silently overwritten where amendment history is required.
```

---

## Application Layer Rules

The Application layer contains use cases.

It should include:

- Commands
- Command handlers
- Queries
- Query handlers
- DTOs
- Validators
- Interfaces
- Result models
- Paged result models
- Application services
- Authorization orchestration
- Transaction orchestration

The Application layer should not know about EF Core directly unless through abstractions.

The Application layer must not reference:

- EF Core `DbContext` directly unless explicitly approved
- ASP.NET controllers
- HTTP context directly
- Infrastructure implementations
- Database provider-specific APIs

---

## Application Handler Rules

Application handlers are responsible for orchestrating use cases safely.

A command handler should usually:

1. Validate input
2. Verify tenant context
3. Verify authorization
4. Load required data
5. Call domain behavior
6. Save changes in a transaction when needed
7. Create audit logs for sensitive actions
8. Return a safe result model

A query handler should usually:

1. Verify tenant context
2. Verify authorization
3. Apply tenant-safe filtering
4. Apply pagination where needed
5. Return only the data allowed for the current user

Handlers must not bypass tenant isolation or authorization checks.

---

## Infrastructure Layer Rules

The Infrastructure layer implements technical details:

- `MasterDbContext`
- `TenantDbContext`
- EF Core configurations
- Repositories
- Migrations
- Token services
- Password/email/identity infrastructure
- Tenant provisioning service
- Tenant database connection resolution
- External service implementations
- File storage implementation
- Audit log persistence

Infrastructure must implement technical details only.

Infrastructure must not decide:

- Whether a user is allowed to access a patient
- Whether an appointment status transition is valid
- Whether a paid invoice can be edited
- Whether a clinical record can be amended
- Whether one tenant can access another tenant's data

Those decisions belong in Application and Domain.

---

## API Layer Rules

The API layer is only an HTTP adapter.

Controllers must be thin:

```txt
Receive request → Dispatch command/query → Return HTTP response
```

Controllers must not contain:

- Business logic
- EF Core queries
- Validation rules
- Tenant resolution logic
- Direct `DbContext` usage
- Complex mapping logic
- Invoice calculations
- Appointment workflow decisions
- Clinical safety decisions

The API layer is responsible for:

- HTTP routing
- Authentication middleware
- Authorization middleware
- Request binding
- Response formatting
- Global exception handling
- Correlation ID handling
- Returning safe error responses
- Dependency injection configuration

The API must not expose:

- Stack traces
- SQL errors
- Connection strings
- Internal exception details
- Sensitive clinical text
- Sensitive tenant configuration

---

## Cross-Cutting Concerns

The following concerns must be handled consistently across features:

- Tenant resolution
- Authorization
- Validation
- Transactions
- Audit logging
- Error handling
- UTC time handling
- Pagination
- Soft delete / archive behavior
- Localization-ready messages
- Safe API response formatting

These concerns should not be reimplemented inconsistently inside every controller.

---

## Where Code Belongs

| Concern | Correct Layer |
|---|---|
| Appointment status transition rules | Domain |
| Invoice total calculation | Domain |
| VAT application rule | Domain / Application |
| Checking current user role/permission | Application / API policy |
| JWT generation | Infrastructure |
| Password hashing | Infrastructure / ASP.NET Identity |
| EF Core configuration | Infrastructure |
| Tenant database resolution | Infrastructure |
| Request DTO binding | API |
| Response DTOs | Application or API |
| Audit log creation decision | Application |
| Audit log persistence | Infrastructure |
| Global exception handling | API |
| PostgreSQL migrations | Infrastructure |
| External service integration | Infrastructure |
| Feature use case orchestration | Application |

---

## Transaction Rules

Commands that modify more than one aggregate or require audit logs must use an explicit transaction.

Examples requiring transaction safety:

- Creating invoice with invoice items
- Paying invoice and creating payment record
- Cancelling appointment and writing audit log
- Creating visit record and updating appointment status
- Creating staff user and assigning roles

A command must not partially complete if one required step fails.

---

## Time Rules

Business logic must not call `DateTime.UtcNow` directly.

Use an application abstraction such as:

```csharp
IClock.UtcNow
```

This improves testing and consistency.

All persisted timestamps must be UTC.

Clinic-local display time belongs in the presentation/UI layer or formatting layer.

---

## Current User Rules

Business logic must not read user identity directly from HTTP context.

Use an application abstraction such as:

```csharp
ICurrentUser
```

This abstraction may expose:

```csharp
UserId
TenantId
Roles
Permissions
Language
```

The implementation belongs in Infrastructure or API.

---

## Result and Error Rules

Application handlers should return safe result models.

Do not throw exceptions for normal validation errors.

Use predictable result types for:

- Validation failure
- Not found
- Forbidden
- Conflict
- Business rule violation

Exceptions should be reserved for unexpected system failures.

Error messages must be safe for API responses and localization-ready where practical.

---

## Recommended Backend Folder Structure

```txt
src/
├── PCMS.Domain/
│   ├── Entities/
│   ├── ValueObjects/
│   ├── Enums/
│   ├── Events/
│   └── Exceptions/
│
├── PCMS.Application/
│   ├── Common/
│   │   ├── Interfaces/
│   │   ├── Models/
│   │   ├── Results/
│   │   ├── Behaviors/
│   │   └── Security/
│   └── Features/
│       ├── Appointments/
│       │   ├── Commands/
│       │   ├── Queries/
│       │   ├── DTOs/
│       │   └── Validators/
│       ├── Patients/
│       ├── Doctors/
│       ├── Invoices/
│       ├── VisitRecords/
│       └── Auth/
│
├── PCMS.Infrastructure/
│   ├── Persistence/
│   │   ├── MasterDbContext.cs
│   │   ├── TenantDbContext.cs
│   │   ├── Configurations/
│   │   └── Migrations/
│   ├── Repositories/
│   ├── Services/
│   ├── Identity/
│   ├── Auditing/
│   └── MultiTenancy/
│
└── PCMS.API/
    ├── Controllers/
    ├── Middleware/
    ├── Filters/
    └── Program.cs
```

---

## Non-Negotiable Architecture Rules

- Domain must not depend on Application, Infrastructure, or API.
- Application must not depend on Infrastructure or API.
- API controllers must stay thin.
- Business rules must not live in controllers.
- Tenant isolation must never depend on frontend logic.
- Authorization must be enforced on the backend.
- Clinical and financial workflows must be auditable.
- Infrastructure must not own business decisions.
- Critical workflows must be testable without real external services.

## REST API Responsibility Rule

- The API layer must expose resources using consistent REST conventions.
- REST API design rules belong in `06-api-conventions.md`.
- Controllers must not expose domain entities directly. API responses must use safe DTOs/result models.
- The API must provide predictable HTTP status codes, safe error responses, pagination for list endpoints, and consistent route naming.
