# 01 — Project Context

## Project Name

Palestinian Clinic Management System — PCMS

## Product Type

Multi-tenant SaaS web platform for small to mid-sized healthcare clinics in Palestine.

## MVP Goal

The MVP helps clinics manage core daily operations:

- Staff accounts
- Staff authentication
- Clinic admin login
- Doctor login
- Receptionist login
- Accountant login
- JWT authentication
- Role-based access
- Patients
- Doctors
- Appointments
- Basic appointment queue/check-in status
- Basic visit records
- Invoices
- Basic financial reports
- Arabic / English user interface

The MVP must prioritize tenant isolation, data integrity, auditability, and low-latency clinic workflows.

## Technology Stack

| Area              | Technology                                     |
| ----------------- | ---------------------------------------------- |
| Backend           | ASP.NET Core Web API — .NET 10, EF Core, JWT   |
| Frontend          | React / Next.js App Router                     |
| Frontend Language | TypeScript strict mode                         |
| Database          | PostgreSQL 16                                  |
| Architecture      | Clean Architecture + feature-based CQRS (MediatR 12.x) |
| Authentication    | ASP.NET Core Identity + JWT                    |
| Multi-tenancy     | Subdomain-based tenant resolution              |
| Tenant Isolation  | Separate database per clinic                   |

## Product Context

PCMS is designed for Palestinian healthcare clinics. Each clinic is an independent tenant with its own isolated database.

The platform must support Arabic and English, including RTL and LTR layouts.

Tenant isolation is a clinical safety and privacy requirement. The system must prevent any clinic from accessing another clinic’s patients, staff, appointments, invoices, or clinical records.

## MVP Scope

Included in MVP:

- Tenant management
- Staff management
- Role-based access control
- Patient management
- Doctor management
- Appointment booking
- Appointment status lifecycle
- Basic queue/check-in workflow
- Basic visit records
- Invoices
- Basic financial reports
- Arabic / English support
- JWT authentication
- Soft delete / archive behavior
- Audit logging for sensitive actions
- Clean Architecture
- Feature-based CQRS using MediatR 12.x

Out of MVP:

- WhatsApp Business API integration
- Advanced smart queue optimization
- Public digital queue display
- Offline-first mode
- Patient portal login
- Insurance claims integration
- Lab integrations
- Pharmacy integrations
- Multi-currency billing
- QR-coded VAT invoices
- SMS / push notifications
- Advanced EMR
- Advanced prescription safety module

## Important MVP Rule

Do not implement out-of-MVP features unless explicitly requested.

However, design the code in a way that does not block these features later.

The MVP should favor simple, reliable workflows over complex automation.

## Clinical Safety Rules

- Patient data must never leak between tenants.
- Clinical records must not be silently overwritten.
- Sensitive actions must be audit logged.
- Normal users must not hard-delete clinical or financial records.
- Visit records should support amendment history where applicable.
- Prescription functionality, if enabled, must clearly separate draft and finalized states.
- Authorization failures are critical safety bugs.
- Basic visit records in MVP are limited to structured notes, diagnosis text, and follow-up instructions.
- The MVP must not implement advanced EMR features such as medication interaction checks, allergy safety engines, clinical decision support, lab result interpretation, or complex treatment protocols.

## Multi-Tenancy Rules

- Tenant is resolved from the subdomain.
- Subdomain resolution must be validated against a central tenant registry.
- The backend must verify that the authenticated user belongs to the resolved tenant.
- A JWT from one tenant must never access another tenant’s database.
- Each tenant has a separate database.
- Tenant database connection resolution must happen only on the server.

## Localization Rules

- Arabic is required for display names.
- English is optional for display names.
- UI must support RTL and LTR.
- User language is stored per user.
- Valid language values are `ar` and `en`.
- Never transliterate Arabic automatically.
- Store Arabic text as UTF-8.
- Do not store translated UI labels in business tables.

## Locale and Formatting

| Item                    | Rule                                     |
| ----------------------- | ---------------------------------------- |
| Currency                | ILS only in MVP                          |
| Currency symbol         | ₪                                        |
| Money storage           | `decimal(18,2)`                          |
| Dates in database       | UTC                                      |
| Dates in UI             | `DD/MM/YYYY`                             |
| Default clinic timezone | `Asia/Hebron`                            |
| Phone numbers           | Include country prefix: `+970` or `+972` |
| VAT                     | Configurable, default 16%                |

## Financial Safety Rules

- Money calculations must use decimal types only.
- Do not use floating point types for money.
- Each invoice must store the VAT rate applied at creation time.
- Paid invoices should not be edited directly.
- Corrections should be handled through cancellation, adjustment, or credit-note style workflows.

## User Account Scope

- In MVP, only internal clinic staff users can log in.
- Patient records exist in the system, but patients do not have user accounts and cannot authenticate into the platform.
- The term `user` means an internal staff account unless explicitly stated otherwise.

## Frontend / Backend Responsibility Boundary

- Next.js is responsible for UI rendering, routing, forms, localization, RTL/LTR layout, and calling backend APIs.

- ASP.NET Core is responsible for authentication, authorization, tenant isolation, business rules, clinical validation, audit logging, and all database writes.

- Do not place critical authorization, tenant isolation, financial calculations, or clinical safety rules only in the frontend.

## Platform Database

The system uses a central platform database for non-clinical SaaS metadata:

- Tenant registry
- Tenant subdomains
- Tenant database connection metadata
- Tenant status
- Platform admin accounts where applicable

Clinical and financial tenant data must live in the tenant-specific database.
