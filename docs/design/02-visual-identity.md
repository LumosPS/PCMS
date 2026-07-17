# 02 — Visual Identity

The visual identity serves both product UI and marketing/social media. Marketing may use the palette more expressively; the product UI uses it conservatively through the tokens in `03-ui-design-system.md`.

## Color

### Brand palette — PCMS Teal

| Step | Hex | Use |
|---|---|---|
| Teal 50 | `#F0FDFA` | Tinted backgrounds, selected rows |
| Teal 100 | `#CCFBF1` | Tinted surfaces, badges |
| Teal 200 | `#99F6E4` | Decorative accents (marketing) |
| Teal 400 | `#2DD4BF` | Decorative accents (marketing), charts |
| Teal 600 | `#0D9488` | Large graphics, icons, hover fills |
| **Teal 700** | **`#0F766E`** | **Primary brand color** — buttons, links, key accents |
| Teal 800 | `#115E59` | Hover/active on primary |
| Teal 900 | `#134E4A` | Deep ink — marketing headlines, dark blocks |

### Neutrals — Slate

| Step | Hex | Use |
|---|---|---|
| Slate 50 | `#F8FAFC` | App background |
| Slate 100 | `#F1F5F9` | Subtle fills, table header |
| Slate 200 | `#E2E8F0` | Borders, dividers |
| Slate 400 | `#94A3B8` | Placeholders, disabled text |
| Slate 500 | `#64748B` | Secondary/muted text |
| Slate 700 | `#334155` | Body text alternative |
| Slate 900 | `#0F172A` | Primary text, headings |

### Semantic colors

| Meaning | Base | Hex | Tint background |
|---|---|---|---|
| Success | Green 700 | `#15803D` | `#F0FDF4` |
| Warning | Amber 700 | `#B45309` | `#FFFBEB` |
| Danger | Red 700 | `#B91C1C` | `#FEF2F2` |
| Info | Blue 700 | `#1D4ED8` | `#EFF6FF` |

### Color rules

- **60/30/10**: roughly 60% neutral background, 30% neutral content, 10% teal accent. Teal is an accent, not a wallpaper — calm & clinical means mostly white/light surfaces.
- Text must meet WCAG AA: ≥ 4.5:1 for normal text, ≥ 3:1 for large text. The 700-level values above pass AA on white; lighter steps (400–600) are for large graphics and decoration only, never body text.
- Meaning is never carried by color alone — pair status colors with an icon or label (see hard constraints on safety).
- Danger red is reserved for errors and destructive actions. Never decorative.

## Typography

### Typefaces

| Script | Family | Source |
|---|---|---|
| Arabic | **IBM Plex Sans Arabic** | Google Fonts, free (OFL) |
| Latin | **IBM Plex Sans** | Google Fonts, free (OFL) |

They are one designed family — weights and proportions harmonize, so mixed AR/EN lines look intentional.

### Weights

| Weight | Use |
|---|---|
| 400 Regular | Body text |
| 500 Medium | Emphasis, labels, table headers |
| 600 SemiBold | Headings, buttons |
| 700 Bold | Marketing headlines, large numbers only |

Never use weights below 400 for UI text.

### Rules

- Arabic renders visually smaller than Latin at equal size — use line-height ≥ 1.6 for Arabic body text (1.5 is fine for English) and verify Arabic legibility at every size you ship.
- Western digits (0–9) in both languages, always — clinical and financial data must never be ambiguous.
- Fallback stack: system UI fonts (`Segoe UI`, `Tahoma` render Arabic acceptably) — never a serif fallback.
- No condensed, italic-as-style, or decorative faces anywhere in the brand.

## Logo — Planned

The logo does not exist yet. Requirements for when it is designed:

- A **wordmark-first** identity: "PCMS" set in IBM Plex Sans SemiBold, optionally with a simple mark (abstract, geometric; medical-adjacent without a literal cross or caduceus).
- Must work: in teal 700 on white, in white on teal 900, and in single-color black — no gradient-dependent design.
- Must sit comfortably beside Arabic text; provide a lockup with the Arabic descriptor «نظام إدارة العيادات».
- Clear space: the height of the "P" on all sides. Minimum size: 24 px height on screen.
- Don'ts: no stretching, recoloring outside the palette, drop shadows, or busy photographic backgrounds.

Until the logo exists, marketing uses the wordmark "PCMS" in IBM Plex Sans SemiBold, teal 700 or white.

## Iconography

- Outline style, consistent 1.5px stroke, rounded joins — [Lucide](https://lucide.dev) matches this spec and is the default set.
- Icons inherit text color; teal only when the icon itself is the accent.
- Directional icons (arrows, chevrons, "back") must mirror in RTL. Universal symbols (clock, search, user) do not mirror.

## Imagery

- Photography: real, calm clinic scenes — natural light, uncluttered, respectful. No sterile blue-tinted stock clichés, no staged thumbs-up doctors.
- **Never use real patient data, real documents, or identifiable patient photos in any material.** Screenshots in marketing must use obviously fictional demo data.
- Illustration (if used): flat, geometric, palette-only colors, generous whitespace. Decoration stays behind content, never competing with it.
