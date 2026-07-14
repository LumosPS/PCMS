# 05 — Screen Map

Derived from `docs/srs/clinic_spec_v4.md` filtered through the MVP scope in `docs/knowledge-base/01-project-context.md`. Every screen below is a planned unit of frontend work; none are implemented yet.

Rules of this document:

- The KB defines what is MVP. SRS features outside KB MVP scope are listed under Post-MVP and get no screens now.
- MVP roles are the KB's: **ClinicAdmin, Doctor, Receptionist, Accountant** (staff only — no patient login). SRS "Owner/Manager" maps to ClinicAdmin. SRS Lab Tech, Support, and Investor roles are post-MVP.
- Every screen supports Arabic/English with RTL/LTR, and defines loading, empty, error, and no-permission states. All lists are paginated.

## App Shell

Dashboard layout per `03-ui-design-system.md`: sidebar (Dashboard, Patients, Appointments, Queue, Invoices, Reports, Settings — filtered by role), top bar (clinic name, language switcher, user menu).

## MVP Screens

### Auth

| # | Screen | Route | Roles | Content & behavior |
|---|---|---|---|---|
| A1 | Login | `/login` | all staff | Email + password, validation errors, localized; no self-registration, no MFA (post-MVP) |

### Dashboard

| # | Screen | Route | Roles | Content & behavior |
|---|---|---|---|---|
| D1 | Dashboard | `/` | all | Role-based widgets. ClinicAdmin: today's appointments, revenue summary, outstanding balances, staff count. Doctor: own appointments today, pending visit records. Receptionist: today's calendar + queue snapshot. Accountant: financial KPIs |

### Patients

| # | Screen | Route | Roles | Content & behavior |
|---|---|---|---|---|
| P1 | Patients list | `/patients` | all (Accountant read-only) | Search by name / phone / national ID / file number; paginated table; register button |
| P2 | Patient profile | `/patients/[id]` | all, tab-scoped | Header (name AR/EN, file number, phone, age). Tabs: Overview, Appointments, Visit records (Doctor only), Invoices (Accountant/ClinicAdmin/Doctor-own). Receptionist sees no clinical tabs |
| P3 | Patient form | `/patients/new`, edit | Receptionist, ClinicAdmin, Doctor | Demographics AR/EN names, contacts (+970/+972 prefix), emergency contact; archive (soft delete) from edit |

### Appointments & Queue

| # | Screen | Route | Roles | Content & behavior |
|---|---|---|---|---|
| Q1 | Appointments calendar | `/appointments` | Receptionist, ClinicAdmin (all doctors); Doctor (own) | Day / week / month views; status colors per KB lifecycle (Scheduled, Confirmed, Arrived, InProgress, Completed, NoShow, Cancelled) + status icon/label; book/walk-in buttons |
| Q2 | Book appointment | dialog over Q1 | Receptionist, ClinicAdmin | Patient search-select, doctor, date/time slot, type (Booked/WalkIn), notes; conflict validation server-side |
| Q3 | Queue / check-in | `/queue` | Receptionist, Doctor, ClinicAdmin | Today's board: Arrived → Waiting → With Doctor → Done columns or ordered list; one-tap status advance; live-ish refresh |

### Visit Records

| # | Screen | Route | Roles | Content & behavior |
|---|---|---|---|---|
| V1 | Visit record form/view | `/patients/[id]/visits/[visitId]`, new from appointment | Doctor only (write); ClinicAdmin read | Structured notes, diagnosis text, follow-up instructions — nothing more in MVP (KB clinical safety rule). Amendment history visible, no silent overwrite |

### Invoices & Payments

| # | Screen | Route | Roles | Content & behavior |
|---|---|---|---|---|
| F1 | Invoices list | `/invoices` | Accountant, ClinicAdmin; Doctor (own patients); Receptionist (view + record payment) | Filters: status, date range, doctor, patient; status badges; paginated |
| F2 | Invoice detail / create | `/invoices/[id]`, `/invoices/new` | create/edit: Accountant, ClinicAdmin | Line items (service, qty, unit price, discount), VAT rate applied at creation (configurable per clinic, default 16%), totals in ILS ₪; Paid/Cancelled invoices not editable (hard constraint 15) |
| F3 | Record payment | dialog over F1/F2 | Accountant, ClinicAdmin, Receptionist (record only) | Amount, method (cash/card/transfer), date; partial payments update balance; receipt view |

### Reports

| # | Screen | Route | Roles | Content & behavior |
|---|---|---|---|---|
| R1 | Financial reports | `/reports` | Accountant, ClinicAdmin | Basic MVP set: daily/monthly income, revenue per doctor, outstanding balances list; charts follow the dataviz rules; export post-MVP |

### Settings

| # | Screen | Route | Roles | Content & behavior |
|---|---|---|---|---|
| S1 | Staff management | `/settings/staff` | ClinicAdmin | Staff list, add/edit, role assignment, activate/deactivate; role changes audit logged |
| S2 | Doctors & schedules | `/settings/doctors` | ClinicAdmin | Doctor profiles (AR/EN names, specialty), weekly working hours + slot duration for booking |
| S3 | Clinic settings | `/settings/clinic` | ClinicAdmin | Clinic profile AR/EN, working hours, VAT rate, services catalog with prices (ILS) |

## Post-MVP (from SRS — no screens now)

Patient portal & online booking · WhatsApp/SMS notifications & templates · Lab module (requests, results, lab services) · Pharmacy & e-prescriptions (QR) · Treatment plans, dental chart, dermatology body map · Expenses & debtor aging · Multi-currency (USD/JOD) · QR-coded VAT invoices & exports · Installment plan management UI · MFA enrollment · Permission overrides UI · Platform admin console (tenants, subscription plans, platform billing) · Support tickets · Investor dashboards · Drag-and-drop rescheduling (calendar enhancement)

## SRS ↔ KB Conflicts (KB wins; recorded, not resolved here)

| SRS says | KB says | Screens follow |
|---|---|---|
| SQL Server | PostgreSQL 16 | n/a (backend) |
| Angular frontend | Next.js App Router | Next.js |
| Automatic 16% VAT | VAT configurable per clinic, default 16% (hard constraint 6) | S3 exposes the setting; F2 stores applied rate |
| 8 roles incl. Lab Tech, Support, Investor | 4 tenant staff roles + PlatformAdmin | MVP screens use KB roles |
| Multi-currency ILS/USD/JOD | ILS only in MVP | All money in ₪ |
| Multi-center + center selector at login | KB has Branch entity; no center-selector workflow defined for MVP | Single-branch UX assumed; flagged as open question |

## Open Questions

1. Multi-branch: does MVP need a branch selector (SRS §6.4) or is one branch per clinic assumed? Screens assume single branch until decided.
2. Invoice numbering format and receipt layout — needed before F2/F3 are built, not before mockups.
