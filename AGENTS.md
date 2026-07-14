# AGENTS.md

This file is the entry point for AI agents working on PCMS.

PCMS (Palestinian Clinic Management System) is a multi-tenant healthcare SaaS for Palestinian clinics. It handles clinical and financial data, is strictly MVP-scoped, and must support Arabic and English with RTL/LTR layouts. Tenant isolation is a patient-safety requirement, not a feature.

## Repo Map

- `backend/` — ASP.NET Core Web API, .NET 10, Clean Architecture. Solution: `backend/PCMS.slnx`. Projects: `PCMS.Domain` → `PCMS.Application` → `PCMS.Infrastructure` / `PCMS.API`, plus three xUnit test projects under `backend/tests/`.
- `frontend/` — Next.js 16 App Router, React 19, TypeScript 5 (strict), Tailwind CSS 4.
- `docs/knowledge-base/` — **the source of truth** for all architecture, domain, and safety rules. 14 numbered files; see its README for reading order and per-task file sets.
- `docs/design/` — brand and design system: visual identity (marketing/social media) and the UI design system spec the frontend must follow.
- `docs/ai-guidelines/` — agent-agnostic workflow guides.

## Current State — Read This First

The backend and frontend contain ONLY placeholder template code (WeatherForecastController, Class1.cs, default create-next-app pages). EF Core, auth, tenancy, and all features are documented but NOT built yet.

- The knowledge base describes the TARGET architecture. Treat it as the spec.
- Check what actually exists before assuming any structure exists.
- You may be creating the first instance of a pattern — generate it from the KB rules.
- Remove placeholder code when superseding it. Never imitate it.

## Required Reading

Before generating or modifying code, read:

1. `README.md`
2. `docs/knowledge-base/01-project-context.md`
3. `docs/knowledge-base/11-hard-constraints.md`
4. The relevant feature-specific files from `docs/knowledge-base/`
5. The relevant workflow files from `docs/ai-guidelines/`

## Core Rules

- Keep changes small and feature-focused.
- Do not implement out-of-MVP features unless explicitly requested.
- Follow Clean Architecture; dependencies point inward, and `PCMS.Domain` has none.
- Use CQRS with MediatR 12.x (do not upgrade past 12.x — licensing).
- Enforce tenant isolation and backend authorization.
- Do not trust tenant IDs, database names, or connection strings from the frontend.
- Do not expose secrets, tokens, private keys, connection strings, or real patient data.
- Do not rewrite documentation unnecessarily.

## Commands

Backend (from repo root):

- Build: `dotnet build backend/PCMS.slnx`
- Test: `dotnet test backend/PCMS.slnx`

Frontend (from `frontend/`):

- Lint: `npm run lint`
- Type check: `npx tsc --noEmit`
- Build: `npm run build`

## AI Guideline Files

Use these files when relevant:

- `docs/ai-guidelines/readme-maintenance.md` for README updates.
- `docs/ai-guidelines/feature-generation.md` for feature implementation.
- `docs/ai-guidelines/code-review.md` for code review.

## Tool-Specific Layers

- Claude Code additionally loads `CLAUDE.md` and `.claude/` (skills, agents, settings). These are routers to this file and `docs/knowledge-base/` — never alternate sources of rules. If they conflict with the knowledge base, the knowledge base wins.
- Task-type workflows live in `.claude/skills/*/SKILL.md`. They are written for Claude Code but are plain markdown — any agent may read and follow the one matching the current task.
- GitHub Copilot uses `.github/copilot-instructions.md`.
