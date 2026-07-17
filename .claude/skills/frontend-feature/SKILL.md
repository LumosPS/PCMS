---
name: frontend-feature
description: Use for PCMS frontend work — dashboard pages, forms, tables, typed API clients, Arabic/English localization, RTL/LTR layout, loading and error states. Examples - patients list page, appointment booking form, invoice table.
---

# Frontend Feature Workflow

Scope: any change to the Next.js frontend — pages, components, API clients, localization.

## Before Writing Code

Read these KB files (skip any already in context):

1. `docs/knowledge-base/01-project-context.md` — confirm the feature is in MVP scope.
2. `docs/knowledge-base/06-api-conventions.md` — response envelope, error codes, pagination contracts the client must handle.
3. `docs/knowledge-base/09-frontend-rules.md` — stack, structure, localization, and state rules.
4. `docs/design/03-ui-design-system.md` — design tokens, layout, component states, and accessibility rules for anything visual.

## Current State Warning

The frontend is bare create-next-app output (default `layout.tsx`/`page.tsx`); there are no components, API clients, or i18n setup yet. The KB describes the TARGET structure — check what exists first; you may be creating the first instance of a pattern. Replace default template pages; never imitate them.

## Workflow

1. Check for existing components/utilities to reuse before creating new ones.
2. Use Next.js 16 App Router conventions and TypeScript strict mode.
3. Go through the typed API client; handle the standard response envelope and error codes.
4. Support Arabic and English, RTL and LTR, for all user-facing text and layout.
5. Include loading, empty, error, and validation states.
6. Permission-based UI hiding is UX only — the backend is the security boundary.
7. Never send `tenantId`, database names, or connection info from the frontend.

## Verify

From `frontend/`:

- `npm run lint`
- `npx tsc --noEmit`
- `npm run build`
