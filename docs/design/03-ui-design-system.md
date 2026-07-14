# 03 — UI Design System

Design tokens and UI rules for the PCMS frontend. This is the layer developers and AI agents consume; it derives entirely from `02-visual-identity.md`.

Status: **specification** — the frontend is still template code. Implement these tokens with the first real UI work, before the first page.

Frontend *code* rules (structure, localization mechanics, state, security) live in `docs/knowledge-base/09-frontend-rules.md` and are not repeated here.

## Design Tokens

Tokens follow a three-level hierarchy:

```
Brand values (02-visual-identity.md hex)  →  Semantic tokens (this file)  →  Component usage
        #0F766E                           →      primary                  →   bg-primary
```

Components must reference **semantic tokens**, never raw hex values or palette steps. This is what lets the brand evolve without repainting every page. Every fill token has a defined content color to place on it (`surface` carries `text`, `primary` carries `on-primary`, tints carry their 700-level semantic color) — never guess a text color for a background.

### Color tokens

| Token | Value | Use |
|---|---|---|
| `background` | Slate 50 `#F8FAFC` | App/page background |
| `surface` | White `#FFFFFF` | Cards, tables, dialogs, inputs |
| `surface-muted` | Slate 100 `#F1F5F9` | Table headers, subtle fills |
| `border` | Slate 200 `#E2E8F0` | Borders, dividers |
| `text` | Slate 900 `#0F172A` | Primary text |
| `text-muted` | Slate 500 `#64748B` | Secondary text, captions |
| `text-disabled` | Slate 400 `#94A3B8` | Disabled, placeholders |
| `primary` | Teal 700 `#0F766E` | Primary actions, links, active nav |
| `primary-hover` | Teal 800 `#115E59` | Hover/active on primary |
| `primary-tint` | Teal 50 `#F0FDFA` | Selected rows, active backgrounds |
| `on-primary` | White `#FFFFFF` | Text/icons on primary |
| `success` / `success-tint` | `#15803D` / `#F0FDF4` | Positive status |
| `warning` / `warning-tint` | `#B45309` / `#FFFBEB` | Caution status |
| `danger` / `danger-tint` | `#B91C1C` / `#FEF2F2` | Errors, destructive actions |
| `info` / `info-tint` | `#1D4ED8` / `#EFF6FF` | Neutral information |
| `focus-ring` | Teal 600 `#0D9488` | Keyboard focus indicator |

### Typography tokens

Font: IBM Plex Sans Arabic + IBM Plex Sans (see `02-visual-identity.md`).

| Token | Size / line-height | Weight | Use |
|---|---|---|---|
| `text-xs` | 12 / 16 | 400–500 | Captions, table meta |
| `text-sm` | 14 / 20 | 400 | Secondary text, dense tables |
| `text-base` | 16 / 24 | 400 | Body, inputs |
| `text-lg` | 18 / 28 | 600 | Card/section titles |
| `text-xl` | 20 / 28 | 600 | Page section headings |
| `text-2xl` | 24 / 32 | 600 | Page titles |
| `text-3xl` | 30 / 36 | 600–700 | Dashboard numbers, rare |

For Arabic body text prefer the next larger line-height when text wraps to multiple lines.

### Spacing, radius, elevation, motion

- **Spacing**: 4 px base scale (4, 8, 12, 16, 24, 32, 48, 64). Default gap inside components 8–12, between form fields 16, between page sections 24–32.
- **Radius**: `sm` 4 px (checkboxes, tags), `md` 8 px (buttons, inputs, cards — the default), `lg` 12 px (dialogs, large cards), `full` (pills, avatars). One radius per component; never mix on siblings.
- **Elevation**: two levels only — `shadow-sm` for cards, a single stronger shadow for overlays (dropdowns, dialogs). Calm & clinical means flat by default; borders separate surfaces, shadows indicate floating.
- **Motion**: two tiers, defined below. Define animations as `--animate-*` / `--motion-*` tokens with their `@keyframes` inside `@theme`, never as ad-hoc transitions per component.

### Motion tokens

Shared rules for all motion, both tiers:

- Easing: `--motion-ease: cubic-bezier(0.4, 0, 0.2, 1)`.
- Animate `transform` and `opacity` only — never layout-affecting properties.
- Scroll-triggered motion starts only when the element is visible (IntersectionObserver).
- `prefers-reduced-motion: reduce` zeroes all durations and shows final states (counts at full value, paths fully drawn). The guard is never removed.
- No bounces, no pulses, no infinite loops.

**Product tier** — the only motion allowed inside the app:

| Token | Value | Use |
|---|---|---|
| `--animate-fade-in` / `--animate-fade-out` | 150 ms | Hovers, reveals, dismissals |
| `--animate-slide-in` | 200 ms, 8 px rise | Panels, dialogs |
| `--motion-lift` | −2 px translateY, 150 ms | Hover on clickable cards |

Product UI never uses entrance staggers, count-ups, or draw-on-view: clinical and financial values must be instantly readable — a number mid-animation is briefly a wrong number.

**Expressive tier** — marketing site and landing pages only (usage rules in `04-application-guides.md`):

| Token | Value | Use |
|---|---|---|
| `--motion-base-delay` | 0.15 s | Delay before the first staggered element |
| `--motion-word-stagger` | 0.08 s | Per-word delay in kinetic headlines |
| `--animate-headline-in` | 0.4 s, 12 px rise + fade | Hero headline word entrance |
| `--motion-countup` | 900 ms, ease-out cubic | Landing-page stat count-ups |
| `--motion-draw` | 2 s | SVG line/sparkline draw-on-view |

### Tailwind 4 reference implementation — Planned

Tokens are defined once in `frontend/src/app/globals.css` via `@theme`:

```css
@theme {
  --font-sans: "IBM Plex Sans", "IBM Plex Sans Arabic", system-ui, sans-serif;
  --color-background: #f8fafc;
  --color-surface: #ffffff;
  --color-border: #e2e8f0;
  --color-text: #0f172a;
  --color-text-muted: #64748b;
  --color-primary: #0f766e;
  --color-primary-hover: #115e59;
  --color-primary-tint: #f0fdfa;
  --color-on-primary: #ffffff;
  --color-danger: #b91c1c;
  /* … remaining tokens from the table above */
}
```

Components then use `bg-surface`, `text-text-muted`, `bg-primary hover:bg-primary-hover`, etc. Raw palette classes (`bg-teal-700`) and arbitrary values (`bg-[#0F766E]`) are forbidden in components.

Dark mode is **out of MVP scope**. Because all colors flow through semantic tokens, a future dark theme is a `@custom-variant dark` block overriding token values — build nothing for it now, but never bypass tokens in a way that would block it.

### Component implementation pattern — Planned

UI primitives in `components/ui/` are built as variant-driven components using **CVA** (`class-variance-authority`) with the `cn()` helper (`clsx` + `tailwind-merge`):

- One `cva()` definition per primitive: base classes, then `variant` and `size` maps — e.g. Button variants `primary | secondary | ghost | danger`, sizes `sm | md`. Variant names must match the vocabulary in Component Rules below.
- Shared state classes in the base: `focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-focus-ring focus-visible:ring-offset-2` and `disabled:pointer-events-none`.
- Disabled styling uses `text-disabled` on `surface-muted` — **not** `disabled:opacity-50`; opacity-only disabled states fail legibility for clinical data.
- React 19: `ref` is a normal prop — no `forwardRef`.
- Inputs surface errors accessibly: `aria-invalid`, error message linked via `aria-describedby`, `role="alert"`.

## RTL / LTR

- Use CSS logical properties exclusively: Tailwind `ms-*`/`me-*`, `ps-*`/`pe-*`, `start-*`/`end-*`, `text-start`/`text-end`. Physical classes (`ml-*`, `pl-*`, `left-*`, `text-left`) are forbidden.
- Set `dir` on `<html>` from the active language; never per-component.
- Directional icons mirror in RTL; see iconography rules in `02-visual-identity.md`.
- Localization mechanics (translation files, language switching) follow `docs/knowledge-base/09-frontend-rules.md`.

## Layout

- Dashboard shell: fixed sidebar (start side — right in Arabic), top bar with clinic/tenant context and user menu, content area max-width ~1280 px on `background` with `surface` cards.
- Density: clinical staff scan lists all day — favor comfortable-but-dense tables (row height ~48 px, `text-sm`), not airy marketing spacing.
- Tables: `surface-muted` header, `border` row dividers, numeric columns aligned to the end side, actions at the row end. All lists are paginated (hard constraint 11).
- Forms: single column by default, labels above inputs, 16 px between fields; group with section titles, not boxes-in-boxes.

## Component Rules

UI primitives (Button, Input, Select, Table, Modal, Badge — per KB 09) are built once in `components/ui/` against tokens.

Every interactive component defines all states:

| State | Treatment |
|---|---|
| Hover | One step darker fill or `surface-muted` background |
| Focus (keyboard) | 2 px `focus-ring` outline with 2 px offset — always visible, never removed |
| Disabled | `text-disabled`, no pointer events; never rely on opacity alone |
| Loading | Inline spinner, keep dimensions stable; disable submit while pending |
| Error | `danger` border + message under the field, calm wording per `01-brand-foundation.md` |
| Empty | Short message + primary action where relevant, no illustration required |

Buttons: primary (filled teal) — at most one per view; secondary (outline); ghost (text) for low-emphasis; danger (filled red) only for destructive confirmation.

## Accessibility Baseline

- WCAG 2.1 AA contrast for all text and meaningful icons.
- Minimum interactive target 40×40 px.
- Status conveyed by icon/label + color, never color alone.
- Full keyboard operability; dialogs trap focus and restore it on close.
- Both `lang` and `dir` correct on `<html>` so screen readers pick the right voice.
