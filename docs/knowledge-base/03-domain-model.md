# 03 — Domain Model

## Purpose

This file defines the core domain entities and business rules for the PCMS MVP.

PCMS is a healthcare clinic management system. Domain modeling must prioritize:

- Tenant isolation
- Clinical data integrity
- Financial correctness
- Auditability
- Simple and maintainable workflows

The MVP must avoid advanced clinical automation. Basic visit records are allowed, but advanced EMR, prescription safety modules, insurance claims, lab integrations, and pharmacy integrations are out of MVP.

---

## Core Domain Entities

The MVP includes these main entities:

- Patient
- Doctor
- Appointment
- VisitRecord
- Invoice
- InvoiceLineItem
- Payment
- StaffUser
- Role / Permission
- Clinic / Tenant
- Branch

Out of MVP:

- Advanced EMR
- Advanced prescription safety module
- Insurance claims
- Lab orders
- Pharmacy integrations
- Patient portal account
- Multi-currency billing

---

## Base Entity

All domain entities should inherit from `BaseEntity`.

Domain entities must not directly call `DateTime.UtcNow`.

Timestamps must be passed from the Application layer using an `IClock` abstraction.

```csharp
public abstract class BaseEntity
{
    public Guid Id { get; protected set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }
    public bool IsDeleted { get; private set; }
    public Guid? DeletedByUserId { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }

    protected BaseEntity(Guid id, DateTime createdAtUtc)
    {
        Id = id;
        CreatedAtUtc = createdAtUtc;
        IsDeleted = false;
    }

    public void MarkUpdated(DateTime updatedAtUtc)
    {
        UpdatedAtUtc = updatedAtUtc;
    }

    public void SoftDelete(Guid deletedByUserId, DateTime deletedAtUtc)
    {
        if (IsDeleted)
            return;

        IsDeleted = true;
        DeletedByUserId = deletedByUserId;
        DeletedAtUtc = deletedAtUtc;
    }
}
```

## Base Entity Rules

- Use `CreatedAtUtc`, `UpdatedAtUtc`, and `DeletedAtUtc` for persisted timestamps.
- Do not use local time in domain entities.
- Do not hard-delete clinical or financial entities through normal application workflows.
- Soft delete / archive actions must be audit logged for sensitive entities.
- Domain entities should expose behavior through methods instead of uncontrolled property setters.

Good:

```csharp
appointment.Cancel(cancelledByUserId, reason, utcNow);
invoice.MarkAsPaid(paymentInfo, paidByUserId, utcNow);
patient.UpdatePhoneNumber(phoneNumber, updatedByUserId, utcNow);
```

Bad:

```csharp
appointment.Status = AppointmentStatus.Cancelled;
invoice.Status = InvoiceStatus.Paid;
patient.PhoneNumber = phoneNumber;
```

---

## Patient

Represents a clinic patient record.

Patients do not have login accounts in MVP.

Expected fields:

- Id
- NameAr
- NameEn
- DateOfBirth
- Gender
- NationalId
- PhoneNumber
- Address
- EmergencyContactName
- EmergencyContactPhone
- Notes
- IsDeleted
- CreatedAtUtc
- UpdatedAtUtc

Out of MVP or optional:

- InsuranceInfo
- Full MedicalHistory
- Patient portal account

## Patient Rules

- Arabic name is required.
- English name is optional.
- Phone number must include `+970` or `+972`.
- Patient records must not be hard-deleted by normal users.
- Patient archive/delete must be audit logged.
- Patient data must never be shared across tenants.
- Patient search endpoints must be paginated.

---

## Doctor

Represents a doctor working in a clinic.

Expected fields:

- Id
- NameAr
- NameEn
- SpecializationAr
- SpecializationEn
- LicenseNumber
- PhoneNumber
- Email
- BranchId
- IsActive
- CreatedAtUtc
- UpdatedAtUtc

Optional later:

- WorkingHours
- Commission settings
- Online booking availability
- Doctor-specific service pricing

## Doctor Rules

- Arabic name is required.
- English name is optional.
- Doctor must belong to a branch.
- Inactive doctors cannot receive new appointments.
- Existing historical appointments must remain linked to the doctor.

---

## Branch

Represents a clinic branch.

Expected fields:

- Id
- NameAr
- NameEn
- Address
- PhoneNumber
- IsActive
- CreatedAtUtc
- UpdatedAtUtc

## Branch Rules

- Each tenant must have at least one branch.
- Staff, doctors, appointments, and invoices may be linked to a branch.
- Inactive branches cannot receive new appointments.

---

## Appointment

Represents a scheduled, walk-in, or urgent visit.

Expected fields:

- Id
- PatientId
- DoctorId
- BranchId
- StartTimeUtc
- EndTimeUtc
- Status
- Type
- Notes
- CheckedInAtUtc
- QueueNumber
- StartedAtUtc
- CompletedAtUtc
- CancelledByUserId
- CancelledAtUtc
- CancellationReason
- CreatedAtUtc
- UpdatedAtUtc

## Appointment Types

```csharp
public enum AppointmentType
{
    Scheduled,
    WalkIn,
    Urgent
}
```

## Appointment Statuses

```csharp
public enum AppointmentStatus
{
    Scheduled,
    Confirmed,
    CheckedIn,
    InProgress,
    Completed,
    Cancelled,
    NoShow
}
```

## Appointment Status Flow

```txt
Scheduled → Confirmed → CheckedIn → InProgress → Completed
Scheduled → CheckedIn → InProgress → Completed
CheckedIn → InProgress → Completed

Scheduled → Cancelled
Confirmed → Cancelled
CheckedIn → Cancelled

Scheduled → NoShow
Confirmed → NoShow

WalkIn/Urgent appointments usually start at CheckedIn.
```

## Terminal Appointment States

These states are terminal:

- Completed
- Cancelled
- NoShow

Terminal appointments cannot return to active workflow states.

## Appointment Business Rules

- A doctor cannot have overlapping active appointments.
- Appointment time must be inside doctor working hours when working hours are configured.
- Scheduled appointments cannot be booked in the past.
- Walk-in and urgent appointments may be created at the current time.
- Staff users with permission may cancel appointments with a required reason.
- Patient/self-service cancellation window is out of MVP.
- Cancellation window must be configurable later in clinic settings.
- Completed appointments cannot return to InProgress.
- Cancelled appointments cannot be edited like active appointments.
- NoShow can only be set after appointment time passes and the patient did not arrive.
- Status changes must be audit logged for sensitive transitions.
- Appointment list endpoints must be paginated.

---

## Basic Queue / Check-In

The MVP supports basic queue/check-in behavior only.

Expected behavior:

- When a patient arrives, staff can check in the appointment.
- A queue number can be assigned.
- Staff can view waiting patients.
- Doctor can move an appointment to InProgress.
- Doctor or staff can complete the appointment.

Out of MVP:

- Advanced smart queue optimization
- Public digital display screen
- Automated delay prediction
- WhatsApp queue notifications

## Queue Rules

- Queue number should be unique per branch per day where practical.
- Queue order should be based on check-in time.
- Urgent appointments may be prioritized manually by authorized staff.
- Queue changes must not bypass appointment status rules.

---

## VisitRecord

Represents basic clinical notes linked to an appointment.

This is not a full EMR.

Expected fields:

- Id
- AppointmentId
- PatientId
- DoctorId
- DiagnosisText
- DoctorNotes
- FollowUpInstructions
- CreatedByDoctorId
- CreatedAtUtc
- UpdatedAtUtc

Out of MVP:

- Advanced EMR
- Medication interaction checks
- Allergy safety engine
- Clinical decision support
- Lab result interpretation
- Complex treatment protocols

## VisitRecord Rules

- A visit record must belong to one appointment.
- A visit record must belong to one patient.
- A visit record must be created or approved by a doctor role.
- Visit records must not be silently overwritten.
- Edits to visit records should be audit logged.
- If finalization is implemented, finalized visit records require amendment history instead of direct overwrite.
- Visit records must never be exposed across tenants.

---

## Prescription

Prescription functionality is out of MVP unless explicitly enabled.

If enabled later, prescriptions must support:

- Draft state
- Finalized state
- Cancelled state
- Doctor signature
- Audit trail
- Medication name
- Dosage
- Frequency
- Duration
- Instructions

## Prescription Safety Rules

- Finalized prescriptions must not be silently edited.
- Any change after finalization must create an amendment or cancellation record.
- Prescription printing/export must be audit logged.
- Prescription safety checks are not part of MVP unless explicitly approved.

---

## Invoice

Represents billing for an appointment.

Expected fields:

- Id
- AppointmentId
- PatientId
- BranchId
- Number
- Currency
- LineItems
- SubTotal
- VatRate
- VatAmount
- DiscountAmount
- Total
- PaidAmount
- RemainingAmount
- Status
- IssuedAtUtc
- CancelledAtUtc
- CancellationReason
- CreatedAtUtc
- UpdatedAtUtc

## Invoice Statuses

```csharp
public enum InvoiceStatus
{
    Draft,
    Issued,
    PartiallyPaid,
    Paid,
    Cancelled
}
```

## Invoice Status Flow

```txt
Draft → Issued → PartiallyPaid → Paid

Draft → Cancelled
Issued → Cancelled

PartiallyPaid → Cancelled only through authorized adjustment workflow
```

## Invoice Business Rules

- Currency is ILS only in MVP.
- Money must use `decimal`, never `float` or `double`.
- Database money columns must use `decimal(18,2)`.
- VAT is configurable from clinic config.
- VAT must not be hardcoded in application logic.
- Each invoice must store the VAT rate applied at creation or issue time.
- Invoice number format is `INV-YYYY-XXXXXX`.
- Invoice numbers are sequential per clinic.
- Invoice number should be assigned when the invoice is issued.
- Every invoice must reference an appointment.
- Standalone invoices are not allowed in MVP.
- Invoice totals are calculated from line items.
- Invoice totals must not be manually overwritten.
- Paid or Cancelled invoices cannot be edited.
- Cancelled invoices cannot receive payments.
- Use a credit note or adjustment workflow instead of editing Paid or Cancelled invoices.
- Invoice creation, payment, and cancellation must be audit logged.

---

## InvoiceLineItem

Represents an item or service on an invoice.

Expected fields:

- Id
- InvoiceId
- DescriptionAr
- DescriptionEn
- Quantity
- UnitPrice
- LineTotal
- CreatedAtUtc

## InvoiceLineItem Rules

- Quantity must be greater than zero.
- Unit price must not be negative.
- Line total must be calculated from quantity and unit price.
- Invoice line items cannot be edited after invoice is Paid or Cancelled.

---

## Payment

Represents a payment made against an invoice.

Expected fields:

- Id
- InvoiceId
- Amount
- Method
- PaidAtUtc
- ReceivedByStaffId
- Notes

## Payment Methods

```csharp
public enum PaymentMethod
{
    Cash,
    Card,
    BankTransfer,
    Other
}
```

## Payment Rules

- Payment amount must be greater than zero.
- Payment amount cannot exceed remaining invoice amount unless overpayment handling is explicitly implemented.
- Payment must update invoice paid and remaining amounts.
- Payment must be audit logged.
- Payments cannot be added to Cancelled invoices.
- Payments cannot be added to Paid invoices unless adjustment workflows are implemented.

---

## StaffUser

Represents an internal tenant staff account.

Patients are not staff users and cannot log in during MVP.

Expected fields:

- Id
- IdentityUserId
- NameAr
- NameEn
- Email
- PhoneNumber
- DefaultLanguage
- IsActive
- CreatedAtUtc
- UpdatedAtUtc

## StaffUser Rules

- Staff users are internal clinic users only.
- Patient login is out of MVP.
- A staff user may have one or more roles.
- Inactive staff users cannot log in.
- Staff account creation, deactivation, and role changes must be audit logged.
- Staff users must never access another tenant database.

---

## Role / Permission

Represents RBAC rules for staff users.

Initial MVP roles:

- ClinicAdmin
- Doctor
- Receptionist
- Accountant

Optional platform role:

- PlatformAdmin

## RBAC Rules

- Roles define what staff users can do.
- Permissions should be checked on the backend.
- Frontend permission checks are for user experience only.
- Authorization failures are critical safety bugs.
- Role changes must be audit logged.

---

## Clinic / Tenant

Represents a clinic tenant in the SaaS platform.

Tenant metadata belongs in the Platform Database.

Expected platform fields:

- Id
- Subdomain
- DisplayNameAr
- DisplayNameEn
- DatabaseConnectionReference
- IsActive
- CreatedAtUtc

## Tenant Rules

- Each clinic is an independent tenant.
- Each tenant has a separate database.
- Tenant is resolved from the subdomain.
- Subdomain resolution must be validated against the platform tenant registry.
- Tenant database connection resolution must happen only on the server.
- Tenant connection metadata must not be exposed through normal API responses.
- A JWT from one tenant must never access another tenant’s database.
- Clinical and financial data must live in the tenant-specific database.

---

## ClinicProfile

Represents clinic profile information inside the tenant database.

Expected fields:

- Id
- DisplayNameAr
- DisplayNameEn
- Address
- PhoneNumber
- VatRate
- DefaultCurrency
- Timezone
- CreatedAtUtc
- UpdatedAtUtc

## ClinicProfile Rules

- VAT rate is configurable.
- Default currency is ILS in MVP.
- Default timezone is `Asia/Hebron`.
- Clinic profile changes must be audit logged.

---

## Dual-Language Naming Pattern

All displayable names should use:

```csharp
public string NameAr { get; private set; }
public string? NameEn { get; private set; }
```

Arabic is required. English is optional.

Use this pattern for business display names such as:

- Patient names
- Doctor names
- Specialization names
- Branch names
- Service names
- Invoice item descriptions where needed

Do not use this pattern for UI labels such as:

- Save
- Cancel
- Patients
- Appointments

UI labels belong in localization files.

---

## DTO Localization Rule

For normal reads, do not return both `NameAr` and `NameEn` unless the UI specifically needs both for editing.

Return a localized `Name` property:

```csharp
public class SpecializationDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}
```

The query handler should resolve the correct name using the current user language.

For create/edit screens, APIs may return both Arabic and English fields when the user is allowed to edit them.

---

## Non-Negotiable Domain Rules

- Domain entities must not expose uncontrolled public setters.
- Domain entities must not directly call `DateTime.UtcNow`.
- Domain entities must not depend on EF Core, ASP.NET Core, JWT, or HTTP context.
- Clinical records must not be silently overwritten.
- Financial totals must be calculated, not manually trusted.
- Paid or Cancelled invoices must not be edited directly.
- Tenant data must never leak across tenants.
- Patient records must not be hard-deleted by normal users.
- Authorization must be enforced on the backend, not only in the frontend.
