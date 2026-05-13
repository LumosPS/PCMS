# 12 — Version Control Rules

## Branching Strategy

Use GitHub Flow.

The `main` branch must always remain stable and deployable.

All work must be done in short-lived branches.

Allowed branch prefixes:

- `feature/`
- `fix/`
- `hotfix/`
- `refactor/`
- `docs/`
- `test/`
- `chore/`

Examples:

- `feature/patient-search`
- `fix/invoice-payment-total`
- `docs/update-multitenancy-rules`

---

## Main Branch Rules

The `main` branch must be protected.

Do not push directly to `main`.

Merge only through pull requests.

Before merge:

- Code builds successfully.
- Tests pass.
- Migration impact is reviewed if applicable.
- High-risk changes receive extra review.

---

## Pull Request Rules

Every change must go through a pull request before merging to `main`.

A pull request must include:

- Clear summary
- Related requirement or ticket
- Testing notes
- Screenshots for UI changes when applicable
- Migration notes if database changes are included
- Security/tenant impact notes if relevant

Keep pull requests small and focused.

Avoid mixing unrelated backend, frontend, database, and refactor changes in one PR.

---

## Commit Rules

Use clear commit messages.

Follow Conventional Commits where practical.

Recommended format:

- `feat: add patient search`
- `fix: prevent invoice edit after payment`
- `docs: update multitenancy rules`
- `refactor: simplify appointment status logic`
- `test: add invoice calculation tests`
- `chore: update dependencies`

---

## Safety Rules

Never commit:

- Production secrets
- Real patient data
- Database backups with real data
- Private keys
- Access tokens
- Refresh tokens
- `.env.production`
- Connection strings
- Tenant secrets

Use `.gitignore` for local environment files.

Use secret management or environment variables for sensitive configuration.

---

## Database Migration Rules

Database migrations must be committed with the feature that requires them.

Migration files must be reviewed carefully.

Do not edit old migrations after they have been shared or applied.

Create a new migration for new changes.

Platform database migrations and tenant database migrations must stay separate.

Migration notes are required when a PR changes:

- PlatformDbContext
- TenantDbContext
- Entity configuration
- Indexes
- Constraints
- Seed data

Destructive migrations require explicit review before merge.

---

## High-Risk Change Rule

Any change affecting the following areas requires extra review before merge:

- Authentication
- Authorization
- Tenant isolation
- Staff permissions
- Clinical records / visit records
- Invoices
- Payments
- Audit logs
- Database migrations
- Token handling
- Tenant connection resolution

These changes must include testing notes.

---

## Testing Rule

Before merging, run relevant tests.

Required for high-risk changes:

- Unit tests
- Authorization tests where applicable
- Tenant isolation tests where applicable
- Invoice/payment tests where applicable
- Migration verification where applicable

Do not merge known failing critical tests.

---

## Release Tag Rule

When creating a stable MVP release, use tags.

Example:

```txt
v0.1.0-mvp
v0.2.0-beta
v1.0.0
```

Use semantic versioning when the project becomes production-ready.

---

## Non-Negotiable Version Control Rules

- Do not push directly to `main`.
- Do not commit secrets.
- Do not commit real patient data.
- Do not edit shared/applied migrations.
- Do not merge high-risk changes without review.
- Do not mix unrelated changes in one PR.
