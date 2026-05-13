# AGENTS.md

This file is the entry point for AI agents working on PCMS.

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
- Follow Clean Architecture.
- Use manual CQRS.
- Enforce tenant isolation and backend authorization.
- Do not trust tenant IDs, database names, or connection strings from the frontend.
- Do not expose secrets, tokens, private keys, connection strings, or real patient data.
- Do not rewrite documentation unnecessarily.

## AI Guideline Files

Use these files when relevant:

- `docs/ai-guidelines/readme-maintenance.md` for README updates.
- `docs/ai-guidelines/feature-generation.md` for feature implementation.
- `docs/ai-guidelines/code-review.md` for code review.
