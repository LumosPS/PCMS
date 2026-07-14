# PCMS Documentation

Map of the `docs/` folder and how to use it.

## Folders

| Folder | Contents | Start with |
|---|---|---|
| `knowledge-base/` | **Source of truth** for architecture, domain, and safety rules — 14 numbered files | Its README (reading order and per-task file sets) |
| `design/` | Brand and design system — visual identity for marketing/social media plus the UI design system spec | Its README |
| `ai-guidelines/` | Agent-agnostic workflow guides (feature generation, code review, README maintenance) | The file matching your task |

## How to Use

- Writing or reviewing code: follow `knowledge-base/` — attach the file set for your task type per its README.
- Building UI or producing marketing material: follow `design/`.
- If sources conflict, `knowledge-base/` wins; escalate and fix the conflict.

AI agents enter through the root `AGENTS.md`, which routes here.
