# 08 — Authentication and Authorization

## Purpose

Defines authentication, authorization, token, and role rules for PCMS.

PCMS protects healthcare and financial data. Authorization failures are critical safety bugs.

---

## Authentication Strategy

PCMS uses ASP.NET Core Identity with JWT.

Only internal clinic staff users can log in during MVP.

Patients do not have login accounts in MVP.

---

## Account Types

PCMS has two account scopes:

- PlatformAdmin: platform-level account
- StaffUser: tenant-level clinic account

Patient records are not user accounts.

---

## Access Token

Access token lifetime: 15 minutes.

Access tokens must include:

```json
{
  "sub": "UserId",
  "email": "doctor@al-shifa.ps",
  "tenantId": "TenantId",
  "subdomain": "al-shifa",
  "roles": ["Doctor"],
  "permissions": ["appointments.read"],
  "lang": "ar",
  "exp": "unix timestamp"
}
```

## Access Token Rules

- Access tokens must be signed and short-lived.
- Staff tokens must include tenant context.
- A token from one tenant must never access another tenant.
- Do not store secrets, connection strings, or clinical data in JWT claims.
- Match JWT tenant claims against the resolved subdomain tenant.

---

## Refresh Token

Refresh token lifetime: 7 days.

Rules:

- Store refresh tokens as hashed values.
- Store staff refresh tokens in the tenant database.
- Store platform admin refresh tokens in the platform database.
- Rotate refresh token on every refresh.
- Invalidate the old refresh token after refresh.
- Delete refresh token on logout.
- Invalidate all refresh tokens after password change.
- Refresh tokens must not be exposed in URLs.

---

## Password Rules

Use ASP.NET Core Identity password hashing.

Minimum rules:

- Minimum length: 8 characters
- Require non-trivial passwords
- Lock account after repeated failed login attempts
- Use a secure password reset flow

Do not store plain-text passwords.

---

## Roles

### PlatformAdmin

Scope: Entire platform.

Stored in the Platform Database.

Permissions:

- Manage tenants
- Manage platform configuration
- View tenant provisioning status
- Run platform-level diagnostics

PlatformAdmin must not directly access tenant clinical data through normal workflows.

### ClinicAdmin

Scope: Own clinic only.

Permissions:

- Manage staff users
- Manage clinic settings
- Manage doctors
- Manage patients
- Manage appointments
- Manage invoices
- View reports
- Assign roles inside own clinic

### Doctor

Scope: Own clinic.

Permissions:

- View assigned appointments
- View assigned patients
- Create and update visit records
- Move appointments to InProgress / Completed where allowed
- View relevant patient history inside own clinic

Out of MVP:

- Prescription module
- Advanced EMR
- Clinical decision support

### Receptionist

Scope: Own clinic.

Permissions:

- Create and update patients
- Book appointments
- Check in patients
- Manage queue/check-in workflow
- Create invoices
- Record payments if allowed by clinic policy

Forbidden:

- Cannot write visit records
- Cannot access sensitive clinical notes unless explicitly allowed

### Accountant

Scope: Own clinic.

Permissions:

- View invoices
- Create or record payments
- View financial reports
- View unpaid invoices

Forbidden:

- Cannot write visit records
- Cannot manage clinical notes
- Cannot manage tenant security settings

### Patient

Patient login is out of MVP.

Do not expose:

- Patient login UI
- Patient portal authentication
- Patient JWT flow
- Patient self-service account management

Patient records exist as clinic data only.

---

## Permission Rules

Authorization should be permission-based, not only role-name based.

Roles group permissions.

Examples:

```txt
patients.read
patients.create
patients.update
appointments.read
appointments.create
appointments.check_in
appointments.cancel
visit_records.read
visit_records.create
invoices.read
invoices.create
invoices.pay
reports.read
staff.manage
roles.manage
```

Backend must enforce permissions.

Frontend permission checks are for UX only.

---

## Tenant Security Rules

JWT must include:

- `tenantId`
- `subdomain`

For tenant-scoped requests, backend must verify:

1. Tenant resolved from subdomain.
2. JWT is valid.
3. JWT tenant matches resolved tenant.
4. Staff user exists in tenant database.
5. Staff user is active.
6. Staff user has required permission.

A JWT from one tenant must never access another tenant database.

---

## Authorization Rules

- Clinic users can access only their own tenant.
- Receptionist cannot write visit records.
- Doctor can create/update visit records.
- Accountant can access financial workflows only.
- ClinicAdmin can manage staff and settings inside own clinic.
- PlatformAdmin manages platform data only.
- Staff role changes must be audit logged.
- Staff deactivation must invalidate active refresh tokens.
- Authorization failures are critical safety bugs.

---

## Audit Rules

Audit these actions:

- Login success/failure where practical
- Logout
- Refresh token rotation
- Password change/reset
- Staff creation/deactivation
- Role or permission change
- Failed forbidden access attempt where practical
- Tenant mismatch attempt

Audit logs must include:

- User ID
- Tenant ID where applicable
- Action
- Timestamp UTC
- Safe metadata

Do not store passwords, tokens, or clinical text in audit metadata.

---

## Forbidden

Do not:

- Trust tenant ID from request body.
- Trust connection string from request body.
- Allow tenant switching by request parameter.
- Expose another tenant's data.
- Reveal whether a resource exists in another tenant.
- Store passwords in plain text.
- Store refresh tokens in plain text.
- Store clinical data in JWT claims.
- Put authorization only in the frontend.
- Allow inactive staff users to authenticate.
