# 04 — Application Guides

How the identity in `01`–`02` is applied in each medium. Product UI additionally follows `03-ui-design-system.md`.

## Product UI

- Calm & clinical in practice: light neutral surfaces, teal only on the primary action and active navigation, generous whitespace, no decoration.
- One primary action per screen. Everything else is secondary or ghost.
- Copy follows the voice rules in `01-brand-foundation.md` — short, calm, blame-free, localized in both languages from day one.
- Demo/seed data shown in screenshots or demos must be obviously fictional.

## Marketing & Social Media

### Layout and color

- Follow 60/30/10: light background (white or teal 50), slate 900 text, teal accent. For high-impact posts, invert: teal 900 background with white text and teal 200/400 accents.
- One message per post. A post is a headline, at most one supporting line, and the wordmark — not a paragraph.
- Keep a consistent margin (≥ 8% of canvas) on all sides; the wordmark sits in a fixed corner across all posts.

### Typography in posts

- Headlines: IBM Plex Sans (Arabic) SemiBold/Bold, large. Body: Regular.
- Arabic-first: lead with Arabic; English version is a separate post or secondary line, not mixed mid-sentence.
- Western digits in both languages.

### Post types (templates to standardize)

| Type | Structure |
|---|---|
| Feature highlight | Headline + one UI screenshot (fictional data) on `surface` card + wordmark |
| Announcement | Teal 900 background, white headline, wordmark |
| Health-awareness / tip | Teal 50 background, slate 900 text, small accent icon |
| Milestone / thank-you | Photo or flat illustration, short caption |

### Motion on the marketing site

The expressive motion tier (tokens in `03-ui-design-system.md`) is allowed here and only here:

- Kinetic headline: staggered word entrance (0.08 s stagger after a 0.15 s base delay, 0.4 s per word, 12 px rise). One per page, in the hero.
- Stat count-ups (900 ms) and SVG/sparkline draw-on-view (2 s) — start when scrolled into view.
- Hover lift (−2 px) on clickable cards.

Still forbidden even in marketing: tickers and any continuously moving content, pulsing elements, scroll-jacking, and anything that ignores `prefers-reduced-motion`. The motion should read as "the product is fast and calm" — never as decoration competing with the message. Product UI uses the product motion tier only.

### Hard rules for marketing

- **Never real patient data, names, photos, or documents — in any form, including blurred.** Screenshots use fictional demo data only.
- No fear-based or urgency-based messaging ("don't lose your patients!") — it contradicts the calm & clinical personality and healthcare ethics.
- No claims about features that are not implemented; describe planned features as coming, not existing.
- Danger red is never used decoratively in marketing.
- Palette colors only; no off-brand gradients or neon accents.

## Consistency Checklist

Before shipping any UI screen or publishing any marketing asset:

1. Colors come from the palette (`02`) / tokens (`03`) — no ad-hoc values.
2. Typography is IBM Plex Sans (Arabic) at approved weights.
3. Arabic and English are both correct, and layout direction matches the language.
4. Numbers use Western digits.
5. Tone is calm, specific, and blame-free.
6. No real patient or tenant data appears anywhere.
7. Status/meaning is never color-only.
