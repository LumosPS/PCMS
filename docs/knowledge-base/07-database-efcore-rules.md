# 07 — Database and EF Core Rules

## Purpose

Define database and EF Core rules for PCMS with focus on tenant isolation, data integrity, financial correctness, and predictable performance.

---

## Database Setup

PCMS uses PostgreSQL 16.

PCMS has:

- One Platform Database
- One Tenant Database per clinic

Use `PlatformDbContext` for platform/control-plane data.

Use `TenantDbContext` for clinic-specific data.

---

## PlatformDbContext

Stores only:

- Tenants
- Tenant subdomains
- Tenant status
- Tenant database connection metadata or secret references
- Platform admin accounts where applicable
- Platform-level configuration
- Tenant provisioning status
- Tenant migration status

Must not store:

- Patients
- Doctors
- Appointments
- Visit records
- Invoices
- Payments
- Tenant staff users
- Tenant clinical data

---

## TenantDbContext

Stores tenant-specific data:

- Clinic profile
- Branches
- Patients
- Doctors
- Staff users
- Roles and permissions
- Appointments
- Queue/check-in data
- Visit records
- Invoices
- Invoice line items
- Payments
- Audit logs
- Tenant reports data

Prescription tables are out of MVP unless explicitly enabled.

---

## Soft Delete Rules

Clinical, financial, and staff records must not be hard-deleted by normal workflows.

Use soft delete or archive behavior for:

- Patient
- Doctor
- Appointment
- VisitRecord
- Invoice
- InvoiceLineItem where applicable
- Payment where applicable
- StaffUser
- Branch

Do not use this in normal application code for sensitive records:

```csharp
DbContext.Remove(entity);
```

Hard delete is allowed only for:

- Temporary records
- Internal cleanup records
- Test data
- Legally approved retention workflows

Hard delete must not be exposed as a normal UI action.

---

## Global Query Filter

Tenant domain entities inheriting from `BaseEntity` should use a global query filter for soft delete.

Example concept:

```csharp
modelBuilder.Entity<Patient>()
    .HasQueryFilter(x => !x.IsDeleted);
```

If using dynamic filters, test them carefully.

Soft-delete filters must not block explicit admin-safe workflows such as audit, restore, or legal retention review.

---

## EF Core Rules

Use:

- Fluent API
- `IEntityTypeConfiguration<T>`
- Explicit `.Include()` / `.ThenInclude()` only when needed
- UTC timestamps
- `decimal(18,2)` for money
- Guid primary keys
- Projection to DTOs
- Pagination for list queries
- Database constraints for critical rules

Do not use:

- Lazy loading
- EF Core data annotations on domain entities
- Raw SQL except approved reporting queries
- `float` or `double` for money
- Local time storage
- Unbounded list queries
- Direct domain entity exposure from API responses

---

## Money Rules

Money must use:

```csharp
decimal
```

Database columns must use:

```csharp
.HasColumnType("decimal(18,2)")
```

Do not use:

```csharp
float
double
```

Invoice totals must be calculated from line items.

Do not trust totals sent from the frontend.

---

## VAT Rules

VAT must be stored in clinic configuration.

Do not hardcode VAT in application logic.

Bad:

```csharp
var vat = subtotal * 0.16m;
```

Good:

```csharp
var vatRate = clinicProfile.VatRate;
var vat = subtotal * vatRate;
```

Each invoice must store the VAT rate used at issue or creation time.

Old invoices must not recalculate using a new VAT rate after configuration changes.

---

## DateTime Rules

Persist all timestamps in UTC.

Use UTC field names where practical:

- `CreatedAtUtc`
- `UpdatedAtUtc`
- `DeletedAtUtc`
- `StartTimeUtc`
- `EndTimeUtc`
- `PaidAtUtc`

Do not call `DateTime.UtcNow` directly inside domain entities.

Use an injected clock abstraction:

```csharp
IClock.UtcNow
```

Convert to local/display format only in the UI or presentation layer.

Default clinic timezone: `Asia/Hebron`.

---

## Raw SQL Rules

Raw SQL is forbidden except for approved complex reporting queries.

If raw SQL is needed, use an approved abstraction:

```csharp
IReportRepository
```

Raw SQL must:

- Use parameters
- Avoid string concatenation
- Be reviewed
- Never include user-provided column/table names directly
- Respect tenant isolation

---

## Performance Rules

- Do not use lazy loading.
- Avoid N+1 queries.
- Use projection to DTOs for reads.
- Add pagination to all list endpoints.
- Add indexes for common filters.
- Add unique constraints for business identifiers.
- Avoid loading large object graphs for list pages.
- Use `AsNoTracking()` for read-only queries where appropriate.

Common indexes:

- Patient phone number
- Patient national ID where used
- Appointment doctor/date
- Appointment patient/date
- Invoice number
- Invoice status/date
- Staff email
- Queue branch/date/number

---

## Concurrency Rules

Use database constraints, transactions, or concurrency tokens for sensitive operations:

- Appointment booking
- Queue number generation
- Invoice number generation
- Payment updates
- Invoice status changes
- Appointment status transitions

For appointment booking, prevent double booking using both:

1. Application-level validation
2. Transaction-safe database logic

Do not rely only on frontend checks.

---

## Transaction Rules

Use transactions for commands that modify multiple records.

Required examples:

- Create invoice with line items
- Add payment and update invoice totals
- Cancel appointment and write audit log
- Create visit record and update appointment status
- Create staff user and assign roles

A command must not partially complete if one required step fails.

---

## Migration Rules

Platform migrations and tenant migrations must be separate.

- `PlatformDbContext` migrations apply only to the Platform Database.
- `TenantDbContext` migrations apply to each Tenant Database.
- Do not mix platform and tenant migrations.
- Do not edit old shared migrations after they are applied.
- Create a new migration for new changes.
- Review destructive migrations before production.

Tenant migration status should be tracked per tenant.

---

## Constraint Rules

Use database constraints for critical invariants where practical.

Examples:

- Required fields
- Unique invoice number per tenant
- Unique queue number per branch/day where practical
- Positive payment amount
- Non-negative money values
- Valid foreign keys
- Required appointment patient/doctor links

Application validation improves UX.

Database constraints protect data integrity.

Use both for critical rules.

---

## Audit Persistence Rules

Sensitive changes must create audit logs.

Audit logs should include:

- Tenant ID
- User ID
- Action
- Entity type
- Entity ID
- Timestamp UTC
- Safe metadata

Do not store unnecessary clinical text in audit metadata.

Audit logs should not be soft-deleted through normal workflows.

---

## Non-Negotiable Database Rules

- Do not store tenant clinical data in the Platform Database.
- Do not trust tenant IDs from the frontend.
- Do not hard-delete clinical or financial records through normal workflows.
- Do not use lazy loading.
- Do not use floating point types for money.
- Do not store local time.
- Do not return unbounded lists.
- Do not bypass database constraints for critical rules.
- Do not run destructive migrations without review.
