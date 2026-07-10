# PCMS Frontend

Staff-facing web app for PCMS: Next.js 16 (App Router), React 19, TypeScript 5 (strict), Tailwind CSS 4.

Frontend rules live in [`docs/knowledge-base/09-frontend-rules.md`](../docs/knowledge-base/09-frontend-rules.md).

## Commands

From this directory:

```bash
npm install        # install dependencies
npm run dev        # start dev server at http://localhost:3000
npm run lint       # ESLint
npx tsc --noEmit   # type check
npm run build      # production build
```

## Status

Bare scaffold (default create-next-app output). Planned, per the knowledge base:

- Arabic/English localization with RTL/LTR layout support (MVP requirement)
- Typed API client handling the standard PCMS response envelope
- Staff dashboard: patients, appointments, queue, visit records, invoices, reports
- Permission-based UI (display only — the backend is the security boundary)

Environment variables will be documented here once the API integration exists.
