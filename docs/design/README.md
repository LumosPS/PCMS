# PCMS Design System

This folder is the source of truth for the PCMS brand and design system. It covers the full brand — product UI, marketing, and social media — not only the frontend.

Brand direction (decided 2026-07): **calm & clinical** personality, **IBM Plex Sans Arabic + IBM Plex Sans** typography, **teal / medical green** primary color.

## Files

| File | Contents | Audience |
|---|---|---|
| `01-brand-foundation.md` | Mission, audience, personality, voice & tone (Arabic/English) | Everyone |
| `02-visual-identity.md` | Color palette, typography, logo, iconography, imagery | Designers, marketers, developers |
| `03-ui-design-system.md` | Design tokens, layout, component and accessibility rules | Developers, AI agents |
| `04-application-guides.md` | Applying the identity to product UI and to marketing/social media | Developers, marketers |
| `05-screen-map.md` | MVP screen inventory derived from the SRS: routes, roles, states, post-MVP list | Developers, AI agents |

## Reading Order

Read `01` and `02` first — everything else derives from them. Read `03` before building any frontend UI. Read `04` before producing marketing or social media material.

## Relationship to the Knowledge Base

- `docs/knowledge-base/09-frontend-rules.md` stays authoritative for frontend *code* rules (structure, localization mechanics, state, security). This folder defines what the UI *looks and feels* like.
- If a rule here conflicts with the knowledge base, the knowledge base wins — escalate and fix the conflict instead of working around it.

## Status

This design system is a specification. Nothing is implemented in `frontend/` yet; the logo does not exist yet. Files mark implementation-dependent items as Planned.
