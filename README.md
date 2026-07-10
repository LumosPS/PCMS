# PCMS

PCMS is a multi-tenant healthcare management SaaS platform for clinics and medical centers in Palestine.

> This project is under active development.

## Overview

PCMS aims to help healthcare providers manage daily clinic operations such as patients, staff, appointments, queues, visit records, invoices, payments, and reports through a secure Arabic/English web platform.

## Tech Stack

- Backend: ASP.NET Core Web API
- Frontend: Next.js with TypeScript
- Database: PostgreSQL
- Architecture: Clean Architecture
- Authentication: JWT-based staff authentication

## Current Status

Project setup and documentation phase.

## Planned MVP Features

- Multi-tenant clinic management
- Staff accounts and role-based access
- Patient management
- Doctor management
- Appointment booking
- Basic queue/check-in workflow
- Basic visit records
- Invoices and payments
- Basic financial reports
- Arabic/English support

## Repository Structure

```txt
backend/     ASP.NET Core Web API (.NET 10, Clean Architecture)
frontend/    Next.js 16 App Router (TypeScript, Tailwind CSS)
docs/        Knowledge base and AI guidelines (source of truth for rules)
AGENTS.md    Entry point for AI coding agents
CLAUDE.md    Claude Code shim (imports AGENTS.md and hard constraints)
.claude/     Claude Code skills, reviewer agent, and permissions
README.md
.gitignore
```

## Documentation

- `docs/knowledge-base/` — architecture, domain, and safety rules; start with its README for reading order.
- `AGENTS.md` — where AI coding tools (Claude Code, Copilot, others) get their instructions.
- `backend/README.md` and `frontend/README.md` — per-part setup and commands.
