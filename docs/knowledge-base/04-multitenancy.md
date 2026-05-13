# 04 — Multi-Tenancy

## Purpose

This file defines the multi-tenancy rules for PCMS.

PCMS is a healthcare SaaS platform. Tenant isolation is a clinical safety, privacy, and compliance requirement.

A tenant represents one clinic or medical center.

The system must prevent one clinic from accessing another clinic’s patients, staff, appointments, visit records, invoices, payments, or clinical data.

---

## Design Decision

PCMS uses a separate PostgreSQL database per tenant.

Each clinic has its own isolated tenant database.

This decision favors:

- Stronger tenant isolation
- Lower risk of cross-clinic data leaks
- Smaller blast radius if one tenant database is damaged
- Easier tenant-level backup and restore
- Stronger compliance and privacy posture

Trade-off:

- More operational complexity
- More complex migrations
- More complex tenant provisioning
- Harder cross-tenant reporting

This trade-off is acceptable because PCMS handles healthcare and financial data.

---

## Database Types

PCMS uses two database categories:

```text
Platform Database
Tenant Databases
```

The Platform Database stores SaaS/control-plane metadata.

Tenant Databases store clinic-specific operational, clinical, and financial data.

---

## Platform Database

The platform database is named conceptually as:

```text
pcms_platform
```

It stores only platform-level data:

- Tenants
- Tenant subdomains
- Tenant status
- Tenant database connection metadata
- Encrypted tenant connection string or secret reference
- Platform-level configuration
- Platform admin accounts where applicable
- Tenant provisioning status
- Tenant migration status

The platform database must never store:

- Patients
- Doctors
- Appointments
- Visit records
- Prescriptions
- Invoices
- Invoice line items
- Payments
- Tenant staff data
- Tenant clinical notes
- Tenant financial transaction details

## Platform Database Safety Rules

- Do not store plain production connection strings.
- Use encrypted connection strings or secret references.
- Tenant connection metadata must never be exposed through normal tenant APIs.
- Platform APIs must never return tenant secrets.
- Platform database access must be restricted to platform-level services only.

---

## Tenant Database

Each tenant database stores clinic-specific data.

Example tenant database:

```text
pcms_al_shifa
```

Each tenant database stores:

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
- Reports data

## Tenant Database Safety Rules

- Tenant clinical and financial data must live only in the tenant database.
- Tenant staff data must live in the tenant database unless explicitly moved to a central identity model.
- Tenant databases must not query each other.
- Cross-tenant data access is forbidden in MVP.
- Tenant database names must not be accepted from user input.
- Tenant database connection resolution must happen only on the server.

---

## Tenant Resolution Strategy

PCMS uses subdomain-based tenant resolution.

Example request:

```text
https://al-shifa.pcms.com/api/appointments
```

The tenant subdomain is:

```text
al-shifa
```

The backend resolves this subdomain through the Platform Database tenant registry.

The frontend must not decide the tenant.

---

## Tenant Resolution Flow

```text
Request arrives
→ TenantMiddleware extracts subdomain
→ TenantMiddleware validates subdomain format
→ Platform Database lookup finds tenant record
→ Backend verifies tenant is active
→ Backend resolves tenant database connection metadata
→ Backend stores safe tenant data in ITenantContext
→ TenantDbContext uses infrastructure resolver to connect to tenant database
→ All tenant queries run only against that tenant database
```

---

## Authenticated Tenant Validation Flow

Tenant resolution from subdomain is not enough.

After authentication, the backend must verify that the authenticated staff user belongs to the resolved tenant.

Example unsafe case:

```text
A staff user from clinic-a sends a valid JWT to clinic-b.pcms.com.
```

The request must be rejected.

Validation flow:

```text
Resolve tenant from subdomain
→ Authenticate JWT
→ Read tenant/user claims
→ Verify user belongs to resolved tenant
→ Reject if token tenant does not match resolved tenant
→ Continue request only if tenant and user access are valid
```

A JWT from one tenant must never access another tenant’s database.

---

## Required Interfaces

`ITenantContext` should expose safe tenant metadata only.

```csharp
public interface ITenantContext
{
    Guid TenantId { get; }
    string Subdomain { get; }
    string DatabaseName { get; }
    bool IsActive { get; }
}
```

Connection string resolution should be hidden inside Infrastructure.

```csharp
public interface ITenantConnectionResolver
{
    string GetConnectionString(Guid tenantId);
}
```

Application code should not directly receive or pass raw tenant connection strings.

---

## Lifetime Rules

`ITenantContext` must be registered as scoped.

It is resolved once per HTTP request.

`TenantDbContext` must be scoped.

`TenantDbContext` must use the resolved tenant context for the current request.

Do not cache tenant context in singleton services.

Do not inject scoped tenant context into singleton services.

---

## Tenant Middleware Rules

`TenantMiddleware` is responsible for:

- Extracting the subdomain
- Validating subdomain format
- Looking up the tenant in the Platform Database
- Validating that the tenant exists
- Validating that the tenant is active
- Setting scoped `ITenantContext`
- Rejecting invalid tenant requests

`TenantMiddleware` must not:

- Accept tenant ID from request body
- Accept tenant database name from request body
- Accept raw connection string from request body
- Trust frontend-selected tenant state
- Query tenant clinical data directly

---

## Tenant-Scoped Endpoint Rules

Tenant resolution is required for all tenant-scoped endpoints.

Examples:

- Tenant staff login
- Patients
- Doctors
- Appointments
- Visit records
- Invoices
- Payments
- Reports
- Staff management

Platform-level endpoints must be explicitly marked and must use the Platform Database, not the Tenant Database.

Examples:

- Platform health check
- Tenant provisioning
- Platform admin tenant management
- Platform-level diagnostics

---

## Tenant Provisioning

Creating a tenant requires a controlled provisioning workflow.

Tenant provisioning steps:

1. Validate clinic information.
2. Reserve and validate a unique subdomain.
3. Create platform tenant record with status `Provisioning`.
4. Create tenant database.
5. Apply tenant database migrations.
6. Seed required tenant data.
7. Create the first ClinicAdmin staff account.
8. Verify the tenant database is reachable.
9. Mark tenant as `Active`.

If any critical step fails, the tenant must not be marked as active.

Provisioning failures must be logged.

Partial provisioning must be recoverable or safely rolled back.

---

## Tenant Statuses

Recommended tenant statuses:

```csharp
public enum TenantStatus
{
    Provisioning,
    Active,
    Suspended,
    Disabled
}
```

## Tenant Status Rules

- `Provisioning` tenants cannot receive normal tenant traffic.
- `Active` tenants can use the system.
- `Suspended` tenants cannot access normal operations but may be restorable.
- `Disabled` tenants are not allowed to authenticate or access tenant APIs.
- Tenant status changes must be audit logged at platform level.

---

## Migration Rules

EF Core migrations must be applied per tenant database.

Tenant migrations must be applied in a controlled process.

The system must track migration status per tenant.

A failed migration for one tenant must not corrupt or block unrelated tenant databases.

Migration rules:

- Platform migrations apply only to the Platform Database.
- Tenant migrations apply to each Tenant Database.
- Do not mix platform and tenant migrations.
- Do not run destructive migrations without backup and review.
- Migration scripts must be reviewed before production execution.

For MVP, tenant migrations may be applied manually or through an internal admin script.

Later, use a controlled `TenantMigrationRunner`.

---

## Backup and Restore Rules

Separate database per tenant allows tenant-level backup and restore.

Rules:

- Each tenant database must be backed up independently.
- Backups must be encrypted.
- Restore operations must be authorized and audit logged.
- Restore operations must not overwrite the wrong tenant.
- Production backups must never be restored into development without anonymization.

---

## Reporting Rules

In MVP, reports are tenant-scoped only.

Allowed:

- Clinic daily revenue
- Clinic monthly revenue
- Doctor revenue inside the same tenant
- Unpaid invoices inside the same tenant
- Appointment reports inside the same tenant

Out of MVP:

- Cross-tenant analytics
- Platform-wide clinic performance reports
- Shared reporting database
- Data warehouse

Cross-tenant reporting must not be implemented unless explicitly approved.

---

## Tenant Safety Rule

If a user tries to access a resource outside their tenant, return either:

- `404 Not Found`
- `403 Forbidden`

Choose based on whether revealing the resource existence could leak information.

Do not leak whether a resource exists in another tenant.

Examples:

- For patient records, prefer `404 Not Found`.
- For authenticated but unauthorized staff actions inside the same tenant, `403 Forbidden` may be appropriate.

---

## Hard Rules

- No service may accept a raw connection string from user input.
- No service may accept tenant database name from user input.
- No service may accept tenant ID from normal request bodies for tenant-scoped operations.
- Tenant resolution must happen server-side.
- Tenant data must use `TenantDbContext`.
- Platform data must use `PlatformDbContext`.
- Cross-tenant data access is forbidden in MVP.
- No patient or clinical data may be stored in the Platform Database.
- EF Core migrations must be applied per tenant database.
- A JWT from one tenant must never access another tenant’s database.
- Tenant context must not be stored in singleton services.
- Frontend tenant state must never be trusted as the security boundary.

---

## Non-Negotiable Multi-Tenancy Rules

- Tenant isolation is a clinical safety requirement.
- Tenant resolution must be validated server-side.
- Tenant database connection metadata must be protected.
- Tenant-specific clinical and financial data must live in the tenant database.
- Platform database must not store tenant clinical data.
- Tenant users must be validated against the resolved tenant.
- Tenant data must never leak across clinics.
- Cross-tenant reporting is out of MVP.
