# 06 — API Conventions

## Purpose

Defines REST API rules for PCMS.

APIs must be predictable, tenant-safe, secure, and easy for the frontend to consume.

---

## URL Rules

Use plural nouns:

```txt
GET /api/patients
GET /api/appointments
GET /api/doctors
GET /api/invoices
```

Use kebab-case:

```txt
GET /api/visit-records
GET /api/staff-users
```

Use nested resources only when the relationship is clear:

```txt
GET /api/patients/{id}/appointments
GET /api/doctors/{id}/appointments
```

Use action endpoints only for business workflows:

```txt
POST /api/appointments/{id}/check-in
POST /api/appointments/{id}/start
POST /api/appointments/{id}/complete
POST /api/appointments/{id}/cancel
POST /api/invoices/{id}/issue
POST /api/invoices/{id}/payments
POST /api/invoices/{id}/cancel
```

Do not hide workflow transitions inside generic update endpoints.

Avoid:

```txt
PUT /api/appointments/{id}
{
  "status": "Completed"
}
```

---

## Versioning

For MVP, API versioning is optional while the API is internal only.

When external integrations or breaking changes are expected, use header versioning:

```txt
X-Api-Version: 1
```

Do not put API version in the URL path unless the architecture decision changes.

---

## Response Envelope

Most JSON responses should use this envelope.

### Success

```json
{
  "success": true,
  "data": {},
  "error": null,
  "meta": null
}
```

### Paged Success

```json
{
  "success": true,
  "data": [],
  "error": null,
  "meta": {
    "page": 1,
    "pageSize": 20,
    "total": 140
  }
}
```

### Error

```json
{
  "success": false,
  "data": null,
  "error": {
    "code": "APPOINTMENT_SLOT_TAKEN",
    "message": "This time slot is already booked.",
    "fields": {
      "startTime": "Slot unavailable"
    }
  },
  "meta": null
}
```

`204 No Content` responses do not return an envelope.

File/download endpoints may use a different response format when needed.

---

## HTTP Status Codes

| Status | Use Case |
|---|---|
| 200 OK | Successful GET, PUT, PATCH |
| 201 Created | Resource created |
| 204 No Content | Successful delete/archive/cancel with no body |
| 400 Bad Request | Invalid request format |
| 401 Unauthorized | Missing or invalid JWT |
| 403 Forbidden | Authenticated but not allowed |
| 404 Not Found | Resource missing or hidden for tenant safety |
| 409 Conflict | Duplicate data or state conflict |
| 422 Unprocessable Entity | Validation or business rule failure |
| 500 Internal Server Error | Unexpected server error |

For tenant-sensitive resources, prefer `404 Not Found` when revealing existence would leak data.

---

## Pagination Rules

All list endpoints must support pagination.

Example:

```txt
GET /api/patients?page=1&pageSize=20&sortBy=createdAt&sortDir=desc&search=ahmad
```

Rules:

- Default page size: 20
- Maximum page size: 100
- Never return unbounded lists
- Always return pagination metadata
- Search and sorting are handled in query handlers
- Only allow sorting by approved fields

---

## Filtering Rules

Use query parameters for filtering.

Examples:

```txt
GET /api/appointments?doctorId={id}&date=2026-05-08
GET /api/invoices?status=paid&fromDate=2026-05-01&toDate=2026-05-31
GET /api/patients?search=ahmad
```

Do not pass raw database column names directly into queries.

---

## DTO Rules

Do not expose EF Core entities or domain entities directly.

Use request and response DTOs.

Good:

```txt
CreatePatientRequest
UpdatePatientRequest
PatientDto
PatientListItemDto
```

Bad:

```txt
ActionResult<Patient>
ActionResult<Invoice>
```

Responses must expose only fields the current staff user is allowed to see.

---

## Tenant Safety Rules

Tenant ID, database name, and connection string must not be accepted from request bodies or query strings for tenant-scoped endpoints.

Tenant context must come from:

```txt
Subdomain resolution
Authenticated staff context
Backend tenant validation
```

The backend must verify that the authenticated staff user belongs to the resolved tenant.

A JWT from one tenant must never access another tenant’s API/data.

---

## Authorization Rules

All protected endpoints must require authentication.

Authorization must be enforced on the backend.

Frontend permission checks are only for user experience.

High-risk endpoints require explicit permission checks:

- Patient access
- Visit record access
- Appointment status changes
- Invoice creation/payment/cancellation
- Staff management
- Role changes
- Reports

---

## Error Code Naming

Use uppercase snake case.

Examples:

```txt
PATIENT_NOT_FOUND
PATIENT_NATIONAL_ID_EXISTS
DOCTOR_NOT_FOUND
APPOINTMENT_SLOT_TAKEN
APPOINTMENT_OUTSIDE_WORKING_HOURS
APPOINTMENT_IN_PAST
APPOINTMENT_INVALID_STATUS_TRANSITION
INVOICE_NOT_FOUND
INVOICE_ALREADY_PAID
UNAUTHORIZED_TENANT_ACCESS
VALIDATION_ERROR
FORBIDDEN
```

Rules:

- Error codes must be stable.
- Do not expose internal exception messages.
- Field validation errors should include field names.
- Production errors must not expose stack traces, SQL errors, connection strings, or tenant secrets.

---

## Audit Rules

Sensitive API actions must create audit logs.

Examples:

```txt
POST /api/patients
PATCH /api/patients/{id}
POST /api/appointments/{id}/cancel
POST /api/invoices/{id}/payments
POST /api/staff-users
PATCH /api/staff-users/{id}/roles
```

Do not store unnecessary clinical text inside audit metadata.

---

## Non-Negotiable API Rules

- Use resource-based REST routes.
- Use explicit workflow endpoints for clinical and financial state changes.
- Do not expose domain entities.
- Do not trust tenant data from the frontend.
- Do not return unpaginated lists.
- Do not expose internal errors.
- Enforce authorization on the backend.
- Keep controllers thin.
