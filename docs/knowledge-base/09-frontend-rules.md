# 09 — Frontend Rules

## Frontend Stack

- React
- Next.js 16 App Router
- TypeScript strict mode
- Zustand for client state only when needed
- Typed API clients
- Arabic / English support
- RTL / LTR layout support

---

## Frontend Responsibility

Next.js handles:

- UI rendering
- Routing
- Forms
- Tables
- Localization
- RTL / LTR layout
- Calling ASP.NET Core APIs

Next.js must not own:

- Authentication rules
- Authorization rules
- Tenant isolation
- Clinical business rules
- Invoice calculations
- Audit logging
- Database writes

ASP.NET Core is the source of truth.

---

## Folder Structure

```txt
src/
├── app/
│   ├── (auth)/
│   └── (dashboard)/
│       ├── patients/
│       ├── appointments/
│       ├── doctors/
│       ├── visit-records/
│       ├── invoices/
│       └── reports/
├── components/
│   ├── ui/
│   └── features/
├── lib/
│   ├── api/
│   ├── auth/
│   ├── i18n/
│   └── utils/
├── hooks/
├── store/
└── types/
```

---

## Component Rules

Separate UI primitives from feature/domain components.

UI primitives:

- Button
- Input
- Modal
- Table
- Select
- Badge

Feature components:

- PatientCard
- AppointmentCalendar
- DoctorSchedule
- VisitRecordForm
- InvoiceSummary

Components must not contain backend business rules.

---

## API Client Rules

- Use typed API clients per domain.
- Types should mirror backend DTOs.
- Handle the standard response envelope.
- Handle validation errors by field.
- Do not expose domain entities directly.
- Do not duplicate business rules in the frontend.
- Do not send `tenantId`, database name, or connection string from the frontend.
- Backend is the source of truth for permissions and validation.

---

## Localization Rules

- Support Arabic and English.
- Arabic layout must be RTL.
- English layout must be LTR.
- Do not hardcode UI strings inside components.
- Use centralized translation files.
- User language is `ar` or `en`.
- Do not transliterate Arabic automatically.

---

## Date and Currency Formatting

Use shared utilities for:

- Date formatting
- Currency formatting
- Phone number formatting

Rules:

- Display dates as `DD/MM/YYYY`.
- Display ILS with `₪`.
- Do not format money manually inside components.
- Do not calculate trusted invoice totals in the frontend.

---

## Auth Frontend Rules

- Only staff users can log in during MVP.
- Patient login is out of MVP.
- Hide routes/actions based on permissions for UX.
- Backend authorization is the real security boundary.
- Refresh token handling must follow backend rules.
- Do not store clinical data in browser storage.
- Do not store tenant secrets in browser storage.

Access token storage strategy must be finalized with the auth architecture.

Preferred options:

- Secure HttpOnly cookie, or
- In-memory access token with refresh flow

Avoid localStorage for sensitive tokens unless explicitly approved.

---

## State Management Rules

Use Zustand only for client UI state or short-lived app state.

Good uses:

- Sidebar state
- Language preference cache
- Lightweight filters
- Modal state
- Current UI view state

Avoid storing:

- Clinical records
- Large patient datasets
- Access secrets
- Tenant secrets
- Authorization source of truth

Server data should come from API calls and query/cache tools if added later.

---

## Role / Permission UI Rules

Frontend may hide UI based on permissions.

Examples:

- Hide invoice payment button if user lacks permission.
- Hide visit record form for receptionist.
- Hide staff management from non-admin users.

Hidden UI is not security.

Backend must reject unauthorized requests.

---

## Patient Portal Rule

Patient portal/login is out of MVP.

Do not build:

- Patient login screen
- Patient dashboard
- Patient self-service profile
- Patient JWT flow

Patient is a record, not an authenticated frontend user in MVP.

---

## Error Handling Rules

Frontend must handle:

- Validation errors
- Unauthorized errors
- Forbidden errors
- Not found errors
- Tenant mismatch errors
- Server errors

Do not show raw internal errors to users.

Show safe, localized messages.

---

## Non-Negotiable Frontend Rules

- Do not put clinical or financial business rules only in the frontend.
- Do not trust frontend permissions for security.
- Do not send tenant identifiers as authority.
- Do not store clinical data in browser storage.
- Do not build patient login in MVP.
- Do not hardcode UI strings.
- Do not manually calculate trusted invoice totals.
