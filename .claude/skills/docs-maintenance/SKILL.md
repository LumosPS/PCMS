---
name: docs-maintenance
description: Use when updating PCMS documentation — README files (root, backend, frontend, docs), the knowledge base, AGENTS.md, or this skills layer. Examples - update the README, document the new setup step, fix the KB.
---

# Documentation Maintenance Workflow

Scope: changes to README files, `docs/`, `AGENTS.md`, `CLAUDE.md`, or `.claude/`.

## Before Writing

Read `docs/ai-guidelines/readme-maintenance.md` — it defines what each README must contain and the incremental update rules. Follow it exactly for README work.

## Core Rules

- Do not rewrite documentation unnecessarily; update only the relevant file or section.
- Never document a feature as completed unless it is implemented — use "Planned" / "In progress" / "Implemented".
- Add setup commands only after they are tested or confirmed to work.
- Never include secrets, connection strings, tokens, credentials, or real patient data.
- Do not duplicate `docs/` content into READMEs; link to it.

## Layering Contract (for AI-docs changes)

- Rules live in `docs/knowledge-base/` and nowhere else.
- `AGENTS.md` is the single agent entry point; `CLAUDE.md` and `.claude/` only route to it and the KB.
- Skills contain workflow and pointers, never rules. If a change adds a rule to a router file, move it to the KB instead.
- Keep `CLAUDE.md` tiny; keep skill bodies under ~80 lines.

## After Changing the KB

If a KB file is renamed, added, or removed, update in the same change:

1. The Reading Order and file sets in `docs/knowledge-base/README.md`.
2. Any skill under `.claude/skills/` that references it.
3. `AGENTS.md` required reading if affected.

## Verify

- Every path referenced in the changed files resolves.
- No rule is now stated in two places.
