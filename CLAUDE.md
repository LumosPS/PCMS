# CLAUDE.md — PCMS

`AGENTS.md` is the source of truth for agent instructions; `docs/knowledge-base/` is the source of truth for rules. This file only wires them into Claude Code.

@AGENTS.md

@docs/knowledge-base/11-hard-constraints.md

## Claude Code Specifics

- Skills in `.claude/skills/` map each task type to the KB files it needs. Read the KB files a skill names before writing code.
- Keep this file tiny. New agent guidance goes in `AGENTS.md`; new rules go in `docs/knowledge-base/`; task workflows go in skills.
