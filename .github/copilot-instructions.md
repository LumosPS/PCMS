# GitHub Copilot Instructions

Follow the repository instructions in `AGENTS.md`.

Before generating, reviewing, or modifying code, follow the relevant PCMS documentation:

- `docs/knowledge-base/01-project-context.md`
- `docs/knowledge-base/11-hard-constraints.md`
- Relevant feature-specific files in `docs/knowledge-base/`
- Relevant workflow files in `docs/ai-guidelines/`

Core rules:

- Keep changes small and feature-focused.
- Do not implement out-of-MVP features unless explicitly requested.
- Follow Clean Architecture.
- Use CQRS with MediatR 12.x (do not upgrade past 12.x — licensing).
- Enforce tenant isolation and backend authorization.
- Do not trust tenant IDs, database names, or connection strings from the frontend.
- Do not expose secrets, tokens, private keys, connection strings, or real patient data.
- Do not rewrite documentation unnecessarily.
