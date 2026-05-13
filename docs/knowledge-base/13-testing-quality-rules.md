# 13 — Testing and Quality Rules

## Purpose

PCMS is a healthcare clinic system. Tests protect patient privacy, tenant isolation, appointment reliability, and financial correctness.

Testing is required for features affecting:

- Authentication
- Authorization
- Tenant isolation
- Staff permissions
- Patients
- Appointments
- Visit records
- Invoices
- Payments
- Audit logs

---

## Testing Philosophy

Prefer high-value tests over fake coverage.

Focus on:

- Tenant isolation
- Authorization correctness
- Appointment lifecycle
- Invoice/payment correctness
- Visit record safety
- Validation rules
- Audit logging
- Critical API behavior

---

## Backend Testing Stack

Use:

- xUnit
- FluentAssertions
- Moq or NSubstitute
- Microsoft.AspNetCore.Mvc.Testing
- PostgreSQL test database for integration tests where practical
- Test data builders/factories

Mock external services only.

Good mocks:

- `IClock`
- `ICurrentUser`
- Email/WhatsApp service later
- File storage later

Do not mock:

- Domain rules
- Invoice calculations
- Authorization rules being tested
- Appointment lifecycle rules

---

## Frontend Testing Stack

Use:

- Vitest
- React Testing Library
- Playwright later for E2E

Frontend tests should cover critical UI behavior only:

- Login form
- Permission-based UI hiding
- Appointment forms
- Invoice form display
- RTL/LTR switching
- API error handling

Frontend tests are not a security boundary.

Backend authorization must be tested separately.

---

## Unit Test Rules

Unit tests must be:

- Fast
- Deterministic
- Independent
- No real database
- No real network
- No direct system time
- No uncontrolled randomness

Use:

- `IClock` instead of `DateTime.UtcNow`
- `ICurrentUser` instead of direct claims access

---

## Required Unit Tests

### Appointment Logic

Test:

- Create valid appointment
- Reject past scheduled appointment
- Reject overlapping active appointment
- Check-in appointment
- Start appointment
- Complete appointment
- Cancel appointment with reason
- Reject invalid status transitions

Valid examples:

```text
Scheduled -> CheckedIn -> InProgress -> Completed
Scheduled -> Cancelled
Scheduled -> NoShow
```

Invalid examples:

```text
Completed -> Scheduled
Cancelled -> InProgress
NoShow -> Completed
```

---

### Invoice and Payment Logic

Test:

- Subtotal calculation
- VAT calculation
- Discount calculation
- Total calculation
- Paid amount calculation
- Remaining amount calculation
- Prevent negative totals
- Prevent payment above remaining amount
- Prevent editing Paid or Cancelled invoice
- Store VAT rate at invoice issue/creation time

Money must use `decimal`.

Never use `float` or `double`.

---

### Visit Record Logic

Test:

- Create visit record for valid appointment
- Ensure visit record belongs to correct patient
- Ensure visit record belongs to correct doctor
- Prevent unauthorized staff from writing visit records
- Prevent silent overwrite where amendment history is required

---

### RBAC Logic

Test:

- ClinicAdmin permissions
- Doctor permissions
- Receptionist permissions
- Accountant permissions
- Forbidden actions
- Role changes

Authorization failures are critical safety bugs.

---

## Required Integration Tests

Integration tests are required for workflows involving:

- ASP.NET Core API pipeline
- JWT authentication
- Authorization policies
- Tenant resolution
- EF Core
- PostgreSQL constraints
- Transactions

Required areas:

### Authentication

Test:

- Valid staff login
- Invalid login
- Disabled staff cannot log in
- Invalid/expired token rejected
- JWT contains required tenant claims

### Tenant Isolation

Test:

- Tenant A staff cannot access Tenant B patients
- Tenant A staff cannot access Tenant B appointments
- Tenant A staff cannot access Tenant B invoices
- JWT tenant must match resolved subdomain tenant
- Tenant DB resolution happens server-side

### Patient APIs

Test:

- Create patient
- Update patient
- Search patients with pagination
- Archive patient
- Reject hard delete from normal workflow
- Validate phone number
- Validate required Arabic name

### Appointment APIs

Test:

- Create appointment
- Check in appointment
- Start appointment
- Complete appointment
- Cancel appointment
- Reject invalid status transition
- Reject cross-tenant access

### Invoice APIs

Test:

- Create invoice
- Issue invoice
- Add payment
- Cancel invoice
- Prevent editing Paid/Cancelled invoice
- Validate VAT and totals
- Reject cross-tenant access

### Audit Logging

Test audit logs for:

- Staff login where practical
- Patient create/update/archive
- Appointment cancel/status changes
- Visit record create/update
- Invoice issue/payment/cancel
- Staff and role changes

Audit metadata must not store passwords, tokens, or unnecessary clinical text.

---

## Database Test Rules

Test important constraints:

- Required fields
- Foreign keys
- Unique invoice number
- Unique queue number per branch/day where practical
- Decimal precision for money
- Positive payment amount
- Non-negative invoice totals
- UTC timestamp usage
- Soft delete filters

Do not rely only on application validation.

Database constraints protect data integrity.

---

## Validation Test Rules

Test invalid input:

- Empty required fields
- Invalid phone number
- Invalid language value
- Invalid appointment date/time
- Negative money values
- Invalid VAT value
- Invalid role/permission
- Invalid tenant access

Validation errors must be safe and not expose internals.

---

## Test Naming Rule

Use clear names:

```text
Method_State_ExpectedResult
```

Examples:

```text
CreateInvoice_WithValidItems_CalculatesTotal
CreateAppointment_WithPastDate_ReturnsValidationError
CheckInAppointment_WhenCompleted_ReturnsConflict
GetPatient_FromDifferentTenant_ReturnsNotFound
PayInvoice_WhenAlreadyPaid_ReturnsConflict
```

---

## Test Data Rules

Use builders/factories:

- PatientBuilder
- DoctorBuilder
- AppointmentBuilder
- InvoiceBuilder
- TenantBuilder
- StaffUserBuilder

Never use real patient data.

Never use production backups in tests.

---

## Pull Request Testing Rules

PR testing notes must mention:

- What was tested
- Tests added/updated
- Migration impact
- Authorization impact
- Tenant isolation impact
- Financial or clinical impact

High-risk changes require tests before merge.

---

## CI Rule

When CI is added, run:

- Backend tests
- Frontend tests
- Build checks
- TypeScript checks
- Linting
- Integration tests where practical

`main` must not accept failing critical tests.

---

## Non-Negotiable Testing Rules

- Do not skip tests for authentication changes.
- Do not skip tests for authorization changes.
- Do not skip tests for tenant isolation changes.
- Do not skip tests for invoice/payment logic.
- Do not skip tests for appointment lifecycle logic.
- Do not skip tests for visit record changes.
- Do not rely on frontend tests for security.
- Do not use real patient data in tests.
- Do not merge broken critical tests.
